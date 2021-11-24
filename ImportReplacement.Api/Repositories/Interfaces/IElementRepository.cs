using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface IElementRepository
    {
        Task<IEnumerable<Element>> GetElementsAsync();
        Task<IEnumerable<ConsumerElement>> GetConsumerElementsAsync(long commandId);
        Task UpdateConsumerElementsAsync(ElementsRequest request);
        Task<IEnumerable<dynamic>> GetElementsToChargeAsync(IEnumerable<long> commands);
        Task AddNewElementAsync(string newElement);
    }

   
}
