using LapsAI.Shared.Models;
using LapsAIDAO.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Net.Http.Json;
using System.Text;

namespace LapsAI.Shared.Services
{
    public class APIGateway : IAPIGateway
    {
        private readonly HttpClient _httpClient;

        private readonly IConfiguration _configuration;

        private readonly StorageService _storageService;



        public APIGateway(HttpClient httpClient, IConfiguration configuration, StorageService storageService)
        {
            _httpClient = httpClient;
            //_httpClient.BaseAddress = new Uri("https://localhost:7226");
            _configuration = configuration;
            _storageService = storageService;
        }

        public async Task<string> GetAServiceValue()
        {
            //var Token = await _storageService.Get("Token");
            var endpoint = "api/APIGateway";
            //_httpClient.DefaultRequestHeaders.Authorization=new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer" ,Token);
            var response = await _httpClient.GetAsync(endpoint);

            return await response.Content.ReadAsStringAsync();
        }

        public async Task<string> GetToken(UserInfo userInfo)
        {
            var endpoint = "api/Token";
            _httpClient.DefaultRequestHeaders.Add("X-API-KEY", "Bala");
            var response = await _httpClient.PostAsJsonAsync(endpoint, userInfo);

            TokenResponse tokenResponse = JsonConvert.DeserializeObject<TokenResponse>(await response.Content.ReadAsStringAsync());

            return tokenResponse.Token;
        }

        public async Task<bool> postloginData(UserInfo userInfo)
        {
            var endpoint = "api/Login";
            var response = await _httpClient.PostAsJsonAsync(endpoint, userInfo);

            Boolean.TryParse(await response.Content.ReadAsStringAsync(), out bool result);

            return result;
        }


    }
}
