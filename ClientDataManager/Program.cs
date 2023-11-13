using ClientDataManager;
using ClientDataManager.Data;
using ClientDataManager.Infrastructure;
using ClientDataManager.Repository;
using ClientDataManager.Services;
using Microsoft.EntityFrameworkCore;


var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{   
    options.UseSqlServer(connectionString);
}, ServiceLifetime.Transient);

builder.Services.Configure<PostItSettings>(builder.Configuration.GetSection("PostItSettings"));
builder.Services.AddTransient<IPostCodeService, PostCodeService>();
builder.Services.AddTransient<IClientRepository, ClientRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(builder => builder
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();