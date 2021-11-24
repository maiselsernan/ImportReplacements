using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;

namespace ImportReplacement.Web.Services
{
    public class ReasonService : IReasonService
    {
        private readonly HttpClient _httpClient;

        public ReasonService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Reason>> GetReasonsAsync()
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<Reason>>("Reason");
        }
    }
}
