using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
    public interface IReplacementService
    {
        Task<int> ImportReplacements();
        Task ChangeRowAsync(IDetailsProvider rowToApprove);
        Task<byte[]> SaveApprovedRowsAsync(RequestTypes type, int siteId);
        Task<IEnumerable<RowToApprove>> GetMetersToApproveAsync(RequestTypes type);
        Task<IEnumerable<UndefinedRow>> GetUndefinedRowsAsync(RequestTypes type);
        Task SetChannelsAsync();
        Task PerformedManuallyAsync(long commandId);
        Task MoveRowsToSiteAsync(IEnumerable<long> commandIds, int? siteId);
        Task<HttpResponseMessage> CreateNewConsumersAsync(IEnumerable<long> commandIds, int? siteId);
        Task<IEnumerable<RowToCharge>> GetRowsToChargeAsync(int siteId,  DateTime from, DateTime to);
        Task IgnoreMeterAsync(long commandId);
        Task ChangeRowChargedAsync(RowToCharge rowToCharge);
        Task RemoveFromBillingAsync(long commandId);
        Task RestoreMeterAsync(long commandId);
        Task<IEnumerable<RowToApprove>> GetHandledRowsAsync(DateTime from, DateTime to);
        Task<HttpResponseMessage> UpdateDuplicatesAsync(IEnumerable<ConsumerNumbers> consumerNumbers);
    }
}
