using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;

namespace ImportReplacement.Web.Services
{
    class ChannelService : IChannelService
    {
        private readonly HttpClient _httpClient;

        public ChannelService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<IEnumerable<Channel>> GetChannelsAsync(long meterNumber)
        {
            return await _httpClient.GetFromJsonAsync<Channel[]>($"Replacement/Channels/{meterNumber}");
        }
    }
}