using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
    public interface IConsumerService
    {
        public Task<IEnumerable<Consumer>> GetConsumersAsync(int siteID, long? meterNumber, long? billingID, long? propertyID);
        public Task MoveConsumerToTypeAsync(long commandId, long consumerId, RequestTypes newType, int siteId);
    }
}
