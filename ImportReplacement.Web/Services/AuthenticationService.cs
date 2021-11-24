using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;
using Newtonsoft.Json;

namespace ImportReplacement.Web.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly HttpClient _httpClient;

        public AuthenticationService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> Authenticate(string password)
        {

            var requestString = JsonConvert.SerializeObject(password);
            var result = await _httpClient.PostAsync("Authentication/Authenticate/",
                               new StringContent(requestString, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsAsync<bool>();
        }
    }
}
