using ClientDataManager.Dtos;
using ClientDataManager.Models;
using ClientDataManager.Repository;
using ClientDataManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace ClientDataManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ClientDataController : ControllerBase
    {
        private readonly IClientRepository _clientRepository;

        private readonly IPostCodeService _postCodeService;

        public ClientDataController(IClientRepository clientRepository, IPostCodeService postCodeService)
        {
            _clientRepository = clientRepository;
            _postCodeService = postCodeService;
        }

        [HttpPost("import")]
        public async Task<List<Client>?> AddClientsAsync(List<ClientDto> clientDtos) => await _clientRepository.AddClientAsync(clientDtos);

        [HttpGet("getClientData")]
        public async Task<ActionResult<List<ClientDto>>> GetAllClientsAsync() => new OkObjectResult(await _clientRepository.GetAllClients());

        [HttpGet("updatePostCode")]
        public async Task UpdatePostCode() => await _postCodeService.GetClientPostCodes();


    }
}

