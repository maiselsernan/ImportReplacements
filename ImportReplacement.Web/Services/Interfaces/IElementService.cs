using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
   public interface IElementService
    {
        Task<IEnumerable<Element>> GetElementsAsync();
        Task<IEnumerable<ConsumerElement>> GetConsumerElementsAsync(long commandId);
        Task UpdateConsumerElements(IEnumerable<ConsumerElement> consumerElement, long commandId);
        Task<JsonElement[]> GetElementsToCharge(List<long> commands);
        Task AddNewElementAsync(string newElement);
    }
}
