using ClientDataManager.Data;
using ClientDataManager.Dtos;
using ClientDataManager.Infrastructure;
using ClientDataManager.Models;
using Microsoft.EntityFrameworkCore;

namespace ClientDataManager.Repository
{
    public class ClientRepository : IClientRepository
    {
        private readonly ApplicationDbContext _context;

        public ClientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<ClientDto>> GetAllClients()
        {
            try
            {
                var clients =  await _context.Clients.ToListAsync();

                _context.SaveChanges();
                var clientDtos = clients.Select(client => new ClientDto
                {
                    ClientId = client.ClientID,
                    Name = client.Name,
                    Address = client.Address,
                    PostCode = client.PostCode,
                    City = ClientHelper.GetCity(client.Address),
                    Number = ClientHelper.GetNumber(client.Address),
                    Street = ClientHelper.GetStreet(client.Address),
                }).ToList();

                return clientDtos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }


        public async Task<List<Client>?> AddClientAsync(List<ClientDto> clientDtos)
        {
            var clientEntities = clientDtos.Select(clientDto => new Client
            {
                ClientID = clientDto.ClientId,
                Name = clientDto.Name,
                Address = clientDto.Address,
                PostCode = clientDto.PostCode,

            }).ToList();

            _context.Clients.AddRange(clientEntities);
            await _context.SaveChangesAsync();

            return await _context.Clients.ToListAsync();
        }

        public async Task UpdateClientAsync(List<ClientDto> clientDtos)
        {
            foreach (var client in clientDtos)
            {
                var clientToUpdate = _context.Clients.Find(client.ClientId);
                clientToUpdate.PostCode = client.PostCode;

            }
            _context.SaveChangesAsync();
        }

    }
}
