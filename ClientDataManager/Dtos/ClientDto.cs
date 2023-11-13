using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace ClientDataManager.Dtos
{
    public class ClientDto
    {
        public int ClientId { get; set; }
        public string? Name { get; set; }
        public string? Address { get; set; }
        public string? PostCode { get; set; }
        public string? Street { get; set; }
        public string? Number { get; set; }
        public string? City { get; set; }
        public string? Uri { get; set; }
    }
}