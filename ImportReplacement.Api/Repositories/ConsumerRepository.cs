using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories
{
    class ConsumerRepository  : BaseRepository, IConsumerRepository
    {
        public ConsumerRepository(SqlConnectionConfiguration sqlConfiguration) : base(sqlConfiguration)
        {
        }
        public async Task<IEnumerable<Consumer>> GetConsumerAsync(int siteID, long? meterNumber, long? billingID, long? propertyID)
        {
            return await QueryAsync<Consumer>(Queries.GetConsumerQuery,
                                            new{ SiteID = siteID, MeterNumber = meterNumber,BilingID = billingID,PropertyID = propertyID} );
        }

        public async Task MoveConsumerToTypeAsync(long commandId, long consumerId, RequestTypes newType, int siteId)
        {
            await ExecuteAsync(Queries.ChangeTypeQuery,new
                                                            {
                                                                CommandId = commandId,
                                                                ConsumerId = consumerId,
                                                                NewType = newType,
                                                                SiteId = siteId
                                                            });
        }
    }
}