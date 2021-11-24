using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;

namespace ImportReplacement.Web.Services
{
    class TypesService : ITypesService
    {
        private readonly HttpClient _httpClient;

        public TypesService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<MeterType>> GetMeterTypesAsync()
        {
            var fromJsonAsync = await _httpClient.GetFromJsonAsync<MeterType[]>("Types/MeterTypes");
            return fromJsonAsync;
        }

        public async Task<IEnumerable<MeterManufacturer>> GetMeterManufacturersAsync()
        {
            return await _httpClient.GetFromJsonAsync<MeterManufacturer[]>("Types/MeterManufacturers");
        }

        public async Task<IEnumerable<MeterModel>> GetMeterModelsAsync()
        {
            return await _httpClient.GetFromJsonAsync<MeterModel[]>("Types/MeterModels");

        }
    }
}