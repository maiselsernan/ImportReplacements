using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;
using Newtonsoft.Json;

namespace ImportReplacement.Web.Services
{
    public class ElementService : IElementService
    {
        private readonly HttpClient _httpClient;

        public ElementService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<IEnumerable<Element>> GetElementsAsync()
        {
            var fromJsonAsync = await _httpClient.GetFromJsonAsync<Element[]>("Element/Elements");
            return fromJsonAsync;
        }
        public async Task<IEnumerable<ConsumerElement>> GetConsumerElementsAsync(long commandId)
        {
            var fromJsonAsync = await _httpClient.GetFromJsonAsync<ConsumerElement[]>
                                                                        ($"Element/ReplacementModelElements/{commandId}");
            return fromJsonAsync;
        }

        public async Task UpdateConsumerElements(IEnumerable<ConsumerElement> consumerElement,long commandId)
        {
            
            var elementsString = JsonConvert.SerializeObject(new ElementsRequest{ConsumerElements = consumerElement, CommandId = commandId});
            var result = await _httpClient.PostAsync("Element/UpdateConsumerElements/", new StringContent(elementsString, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
        }

        public  async Task<JsonElement[]> GetElementsToCharge(List<long> commands)
        {
            var fromJsonAsync = await _httpClient.PostAsJsonAsync
                ($"Element/ElementsToCharge/", commands);
            fromJsonAsync.EnsureSuccessStatusCode();

            return await fromJsonAsync.Content.ReadFromJsonAsync<JsonElement[]>();

        }

        public async Task AddNewElementAsync(string newElement)
        {
            var requestString = JsonConvert.SerializeObject(newElement);
            var result = await _httpClient.PostAsync("Element/AddNewElement/",
                new StringContent(requestString, Encoding.UTF8, "application/json"));
            result.EnsureSuccessStatusCode();
             await result.Content.ReadAsAsync<bool>();
        }
    }
}
