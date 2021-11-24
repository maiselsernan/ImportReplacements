using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface IConsumerRepository
    {
        Task<IEnumerable<Consumer>> GetConsumerAsync(int siteID, long? meterNumber, long? billingID, long? propertyID);
        Task MoveConsumerToTypeAsync(long commandId, long consumerId, RequestTypes newType, int siteId);
    }
}
