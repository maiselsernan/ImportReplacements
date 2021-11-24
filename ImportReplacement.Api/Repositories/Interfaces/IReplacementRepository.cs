using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface IReplacementRepository
    {
        Task<int> ImportReplacements();
        Task ChangeRowAsync(RowToApprove rowToApprove);
        Task<List<ResponseToFile>> SaveApprovedMaintainsAsync(int? siteId);
        Task<List<ResponseToFile>> SaveApprovedReplacementsAsync(int? siteId);
        Task<List<ResponseToFile>> SaveApprovedInstallationsAsync(int? siteId);
        Task<List<ResponseToFile>> SaveMissingChannelsAsync(int? siteId);
        Task<IEnumerable<RowToApprove>> GetMetersToMaintainAsync();
        Task<IEnumerable<RowToApprove>> GetMetersToReplaceAsync();
        Task SetChannelsAsync();
        Task<IEnumerable<Channel>> GetChannelsAsync(long meterNumber);
        Task<IEnumerable<RowToApprove>> GetMetersToInstallAsync();
        Task<IEnumerable<RowToApprove>> GetIgnoresMetersAsync();
        Task<IEnumerable<UndefinedRow>> GetUndefinedRows(RequestTypes type);
        Task<int> PerformedManuallyAsync(long commandId);
        Task MoveRowsToSiteAsync(MoveRowsToSiteRequest request);
        Task CreateNewConsumersAsync(MoveRowsToSiteRequest request);
        Task<IEnumerable<RowToCharge>> GetRowsToChargeAsync(int siteId, DateTime from, DateTime to);
        Task<int> IgnoreMeterAsync(long commandId);
        Task<int> RestoreMeterAsync(long commandId);
        Task ChangeChargeRowAsync(RowToCharge rowToCharge);
        Task<int> RemoveFromBillingAsync(long commandId);
        Task<IEnumerable<RowToApprove>> GetHandledRowsAsync(DateTime from, DateTime to);
        Task UpdateDuplicatesAsync(IEnumerable<ConsumerNumbers> request);

        Task<IEnumerable<RowToApprove>> GetRequiredChannelAsync();
    }
}
