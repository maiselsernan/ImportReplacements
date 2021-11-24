using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;

namespace ImportReplacement.Web.Services
{
    class SiteService : ISiteService
    {
        private readonly HttpClient _httpClient;

        public SiteService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Site>> GetSitesAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Site>>("Sites");
        }
    }
}