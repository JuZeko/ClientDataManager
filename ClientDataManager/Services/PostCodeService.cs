using ClientDataManager.Dtos;
using ClientDataManager.Infrastructure;
using ClientDataManager.Repository;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ClientDataManager.Services
{
    public class PostCodeService : IPostCodeService
    {
        private readonly IClientRepository _clientRepository;
        private readonly PostItSettings _postItSettings;
        private readonly HttpClient _client;

        public PostCodeService(IOptions<PostItSettings> postItSettings, IClientRepository clientRepository)
        {
            _postItSettings = postItSettings.Value;
            _clientRepository = clientRepository;
            _client = new HttpClient();
        }

        public async Task GetClientPostCodes()
        {
            var clientDtos = await _clientRepository.GetAllClients();

            List<Task> tasks = clientDtos.Select(async client =>
            {
                client.Uri = $"{_postItSettings.BasePostitUrl}" +
                              $"?city={client.City}" +
                              $"&address={client.Street}+{client.Number}" +
                              $"&key={_postItSettings.ApiKey}";

                client.PostCode = await PerformGetRequest(client.Uri, client);

            }).ToList();

            await Task.WhenAll(tasks);

            await _clientRepository.UpdateClientAsync(clientDtos);
        }

        static async Task <string> PerformGetRequest(string uri, ClientDto modifiedCLientDto)
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {

                    using (HttpResponseMessage response = await client.GetAsync(uri))
                    {
                        if (response.IsSuccessStatusCode)
                        {

                            string responseData = await response.Content.ReadAsStringAsync();


                            PostItResponseDto result = JsonConvert.DeserializeObject<PostItResponseDto>(responseData);

                            return  result.Data.FirstOrDefault()?.PostCode;


                        }
                        else
                        {
                            Console.WriteLine($"GET request failed with status code: {response.StatusCode}");
                            return null;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }
            return null;
        }

    }
}
