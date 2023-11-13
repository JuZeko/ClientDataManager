using ClientDataManager.Dtos;
using ClientDataManager.Models;

namespace ClientDataManager.Repository
{
    public interface IClientRepository
    {
        Task<List<ClientDto>> GetAllClients();
        Task<List<Client>?> AddClientAsync(List<ClientDto> clientDtos);
        Task UpdateClientAsync(List<ClientDto> clientDtos);
    }
}
