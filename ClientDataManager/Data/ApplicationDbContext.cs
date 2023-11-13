using ClientDataManager.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;

namespace ClientDataManager.Data
{
        public class ApplicationDbContext : DbContext
        {

            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
            public DbSet<Client> Clients { get; set; }
            public DbSet<Audit> Audits { get; set; }

            public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default(CancellationToken))
            {
                var auditEntries = OnBeforeSaveChanges();
                var result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
                await OnAfterSaveChanges(auditEntries);
                return result;
            }

            private List<AuditEntry> OnBeforeSaveChanges()
            {
                ChangeTracker.DetectChanges();
                var auditEntries = new List<AuditEntry>();
                foreach (var entry in ChangeTracker.Entries())
                {
                    if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                        continue;

                    var auditEntry = new AuditEntry(entry);
                    auditEntry.TableName = entry.Metadata.GetTableName();

                    auditEntries.Add(auditEntry);

                    foreach (var property in entry.Properties)
                    {
                        if (property.IsTemporary)
                        {
                            auditEntry.TemporaryProperties.Add(property);
                            continue;
                        }

                        string propertyName = property.Metadata.Name;
                        if (property.Metadata.IsPrimaryKey())
                        {
                            auditEntry.KeyValues[propertyName] = property.CurrentValue;
                            continue;
                        }

                        switch (entry.State)
                        {
                            case EntityState.Added:
                                auditEntry.NewValues[propertyName] = property.CurrentValue;
                                break;

                            case EntityState.Deleted:
                                auditEntry.OldValues[propertyName] = property.OriginalValue;
                                break;

                            case EntityState.Modified:
                                if (property.IsModified)
                                {
                                    auditEntry.OldValues[propertyName] = property.OriginalValue;
                                    auditEntry.NewValues[propertyName] = property.CurrentValue;
                                }
                                break;
                        }
                    }
                }

                foreach (var auditEntry in auditEntries.Where(_ => !_.HasTemporaryProperties))
                {
                    Audits.Add(auditEntry.ToAudit());
                }

                return auditEntries.Where(_ => _.HasTemporaryProperties).ToList();
            }

            private Task OnAfterSaveChanges(List<AuditEntry> auditEntries)
            {
                if (auditEntries == null || auditEntries.Count == 0)
                    return Task.CompletedTask;

                foreach (var auditEntry in auditEntries)
                {
                    foreach (var prop in auditEntry.TemporaryProperties)
                    {
                        if (prop.Metadata.IsPrimaryKey())
                        {
                            auditEntry.KeyValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                        else
                        {
                            auditEntry.NewValues[prop.Metadata.Name] = prop.CurrentValue;
                        }
                    }

                    Audits.Add(auditEntry.ToAudit());
                }

                return SaveChangesAsync();
            }

            public class AuditEntry
            {
                public AuditEntry(EntityEntry entry)
                {
                    Entry = entry;
                }

                public EntityEntry Entry { get; }
                public string TableName { get; set; }
                public Dictionary<string, object> KeyValues { get; } = new Dictionary<string, object>();
                public Dictionary<string, object> OldValues { get; } = new Dictionary<string, object>();
                public Dictionary<string, object> NewValues { get; } = new Dictionary<string, object>();
                public List<PropertyEntry> TemporaryProperties { get; } = new List<PropertyEntry>();

                public bool HasTemporaryProperties => TemporaryProperties.Any();

                public Audit ToAudit()
                {
                    var audit = new Audit();
                    audit.TableName = TableName;
                    audit.DateTime = DateTime.UtcNow;
                    audit.KeyValues = JsonConvert.SerializeObject(KeyValues);
                    audit.OldValues = OldValues.Count == 0 ? null : JsonConvert.SerializeObject(OldValues);
                    audit.NewValues = NewValues.Count == 0 ? null : JsonConvert.SerializeObject(NewValues);
                    return audit;
                }
            }
        }
    }





