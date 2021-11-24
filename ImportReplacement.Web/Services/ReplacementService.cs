using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;
using Newtonsoft.Json;

namespace ImportReplacement.Web.Services
{
    public class ReplacementService : IReplacementService
    {
        private readonly HttpClient _httpClient;

        public ReplacementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<int> ImportReplacements()
        {
            var result = await _httpClient.PostAsync("Replacement/ImportReplacements", null!);
            return await result.Content.ReadFromJsonAsync<int>();
        }
        public async Task ChangeRowAsync(IDetailsProvider rowToApprove)
        {
            var rowToApproveInString = JsonConvert.SerializeObject(rowToApprove);
            var result = await _httpClient.PostAsync("Replacement/ChangeRow/", new StringContent(rowToApproveInString, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
        }
        public async Task<IEnumerable<RowToApprove>> GetMetersToApproveAsync(RequestTypes type)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RowToApprove>>($"Replacement/MetersToApprove/{type}");
        }

        public async Task<IEnumerable<UndefinedRow>> GetUndefinedRowsAsync(RequestTypes type)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<UndefinedRow>>($"Replacement/UndefinedRows/{type}");
        }

        public async Task<IEnumerable<RowToApprove>> GetHandledRowsAsync(DateTime from, DateTime to)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RowToApprove>>($"Replacement/HandledRows?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
        }

        public async Task<byte[]> SaveApprovedRowsAsync(RequestTypes type, int siteId)
        {
            var requestString = JsonConvert.SerializeObject(new Request { Type = type, SiteId = siteId });
            var result = await _httpClient.PostAsync("Replacement/SaveApprovedRows", new StringContent(requestString, Encoding.UTF8, "application/json"));
            if (result.StatusCode == HttpStatusCode.NoContent)
            {
                return null;
            }
            result.EnsureSuccessStatusCode();
            return await result.Content.ReadAsByteArrayAsync();
        }
        public async Task SetChannelsAsync()
        {
            var result = await _httpClient.PostAsync("Replacement/SetChannels", null!);
            result.EnsureSuccessStatusCode();
        }

        public async Task PerformedManuallyAsync(long commandId)
        {
            var result = await _httpClient.PostAsync($"Replacement/PerformedManually/{commandId}", null!);
            result.EnsureSuccessStatusCode();
        }

        public async Task MoveRowsToSiteAsync(IEnumerable<long> commandIds, int? siteId)
        {
            var request = JsonConvert.SerializeObject(new MoveRowsToSiteRequest { CommandIds = commandIds, SiteId = siteId });
            var result = await _httpClient.PostAsync("Replacement/MoveRowsToSite", new StringContent(request, Encoding.UTF8, "application/json")!);
            result.EnsureSuccessStatusCode();

        }

        public async Task<HttpResponseMessage> CreateNewConsumersAsync(IEnumerable<long> commandIds, int? siteId)
        {
            var request = JsonConvert.SerializeObject(new MoveRowsToSiteRequest { CommandIds = commandIds, SiteId = siteId });
            var result = await _httpClient.PostAsync("Replacement/CreateNewConsumers", new StringContent(request, Encoding.UTF8, "application/json")!);
            return result.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<RowToCharge>> GetRowsToChargeAsync(int siteId,  DateTime from, DateTime to)
        {
            return await _httpClient.GetFromJsonAsync<IEnumerable<RowToCharge>>($"Replacement/GetRowsToCharge/{siteId}?from={from:yyyy-MM-dd}&to={to:yyyy-MM-dd}");
        }

        public async Task IgnoreMeterAsync(long commandId)
        {
            var result = await _httpClient.PostAsync($"Replacement/IgnoreMeter/{commandId}", null!);
            result.EnsureSuccessStatusCode();
        }
        public async Task RestoreMeterAsync(long commandId)
        {
            var result = await _httpClient.PostAsync($"Replacement/RestoreMeter/{commandId}", null!);
            result.EnsureSuccessStatusCode();
        }

        public async Task ChangeRowChargedAsync(RowToCharge rowToCharge)
        {
            
            var rowToApproveInString = JsonConvert.SerializeObject(rowToCharge);
            var result = await _httpClient.PostAsync("Replacement/ChangeChargeRow/", new StringContent(rowToApproveInString, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
        }

        public async Task RemoveFromBillingAsync(long commandId)
        {
            var result = await _httpClient.PostAsync($"Replacement/RemoveFromBilling/{commandId}", null!);
            result.EnsureSuccessStatusCode();
        }

        public async Task<HttpResponseMessage> UpdateDuplicatesAsync(IEnumerable<ConsumerNumbers> consumerNumbers)
        {
            var request = JsonConvert.SerializeObject(consumerNumbers);
            var result = await _httpClient.PostAsync("Replacement/UpdateDuplicates", new StringContent(request, Encoding.UTF8, "application/json")!);
            return result.EnsureSuccessStatusCode();
        }

      
    }
}
