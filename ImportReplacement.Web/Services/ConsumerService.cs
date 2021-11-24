using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;
using Microsoft.AspNetCore.WebUtilities;

namespace ImportReplacement.Web.Services
{
    class ConsumerService : IConsumerService
    {
        private readonly HttpClient _httpClient;

        public ConsumerService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
      
        public async Task<IEnumerable<Consumer>> GetConsumersAsync(int siteID, long? meterNumber, long? billingID, long? propertyID)
        {
            var uri = QueryHelpers.AddQueryString("Consumer/Consumers", new Dictionary<string, string>
            {
                ["siteID"] = siteID.ToString(),
                ["meterNumber"] = meterNumber.ToString(),
                ["billingID"] = billingID.ToString(),
                ["propertyID"] = propertyID.ToString()
            });
            return await _httpClient.GetFromJsonAsync<IEnumerable<Consumer>>(uri);
        }

        public async Task MoveConsumerToTypeAsync(long commandId, long consumerId,RequestTypes newType, int siteId)
        {
            var result = await _httpClient.PostAsJsonAsync($"Consumer/MoveConsumer/{commandId}",
                                                                        new { consumerId, newType, siteId});
            result.EnsureSuccessStatusCode();
        }
     
    }
}   