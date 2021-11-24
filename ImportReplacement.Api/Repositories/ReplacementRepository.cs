using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;
using Microsoft.Data.SqlClient;
using Npgsql;

namespace ImportReplacement.Api.Repositories
{
    public class ReplacementRepository : IReplacementRepository
    {
        private readonly NpgsqlConnectionConfiguration _npgsqlConfiguration;
        private readonly SqlConnectionConfiguration _sqlConfiguration;

        public ReplacementRepository(NpgsqlConnectionConfiguration npgsConfiguration, SqlConnectionConfiguration sqlConfiguration)
        {
            _npgsqlConfiguration = npgsConfiguration;
            _sqlConfiguration = sqlConfiguration;
        }

        public async Task<int> ImportReplacements()
        {
            int effectedRows = await RemoveReplacementsFromPostgreSqlAsync();
            effectedRows += await SetReplacementsAsync();
            await SetChannelsAsync();
            return effectedRows;
        }

        private async Task<int> RemoveReplacementsFromPostgreSqlAsync()
        {
            int rowsCopied;
            using var scope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await using (var npgsqlConn = new NpgsqlConnection(_npgsqlConfiguration.Value))
            {
                //TODO: IMPORTANT - change max_prepared_transactions property on postgresql.conf file to 1, and uncomment!! -> (change requires restart)

                await using var resultNpgsqlDataReader = await npgsqlConn.ExecuteReaderAsync(Queries.ReplacesQuery);
                using var bulkCopy = new SqlBulkCopy(_sqlConfiguration.Value)
                {
                    DestinationTableName = "replacement.ReplacementModel"
                };
                foreach (var column in await resultNpgsqlDataReader.GetColumnSchemaAsync())
                {
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                }

                await bulkCopy.WriteToServerAsync(resultNpgsqlDataReader);
                rowsCopied = bulkCopy.RowsCopied;
            }
            scope.Complete();
            return rowsCopied;
        }
        private Task<int> SetReplacementsAsync() => ExecuteAsync(Queries.FindAndSetType);
        public Task SetChannelsAsync() => ExecuteAsync(Queries.SetChannelNotAssociated);
      

        public Task<IEnumerable<RowToApprove>> GetMetersToMaintainAsync() => QueryAsync<RowToApprove>(Queries.RowsToMaintainQuery,new{ ReplacementTypes = new List<int> {1}});
        public Task<IEnumerable<RowToApprove>> GetMetersToReplaceAsync() => QueryAsync<RowToApprove>(Queries.RowsToReplaceQuery);
        public Task<IEnumerable<RowToApprove>> GetMetersToInstallAsync() => QueryAsync<RowToApprove>(Queries.RowsToInstallQuery);
        public Task<IEnumerable<RowToApprove>> GetIgnoresMetersAsync() => QueryAsync<RowToApprove>(Queries.RowsToIgnoreQuery);
        public Task<IEnumerable<RowToApprove>> GetHandledRowsAsync(DateTime from, DateTime to) => QueryAsync<RowToApprove>(Queries.GetHandledRows,
            new {FromDate = from, ToDate = to });
        public Task<IEnumerable<UndefinedRow>> GetUndefinedRows(RequestTypes type) => QueryAsync<UndefinedRow>(Queries.UndefinedRowsQuery, new { Type = type });
        public Task<int> PerformedManuallyAsync(long commandId) => ExecuteAsync(Queries.PerformedManuallyQuery, new { CommandId = commandId });
        public Task MoveRowsToSiteAsync(MoveRowsToSiteRequest request) => ExecuteAsync(Queries.RemoveRowsToSiteQuery, request);
        public Task CreateNewConsumersAsync(MoveRowsToSiteRequest request) => ExecuteAsync(Queries.SetAsNewInstallation, request);
        public Task ChangeRowAsync(RowToApprove rowToApprove) => ExecuteAsync(Queries.UpdateRowQuery, rowToApprove);
        public Task<List<ResponseToFile>> SaveApprovedMaintainsAsync(int? siteId) => QueryInTransaction<ResponseToFile>(Queries.SaveApprovedMaintainsQuery, new { SiteId = siteId });
        public Task<List<ResponseToFile>> SaveApprovedReplacementsAsync(int? siteId) => QueryInTransaction<ResponseToFile>(Queries.CreateConsumersQuery, new { SiteId = siteId, Type = 2 });
        public Task<List<ResponseToFile>> SaveApprovedInstallationsAsync(int? siteId) => QueryInTransaction<ResponseToFile>(Queries.CreateConsumersQuery, new { SiteId = siteId, Type = 3 });
        public Task<List<ResponseToFile>> SaveMissingChannelsAsync(int? siteId) => QueryInTransaction<ResponseToFile>(Queries.SaveMissingChannelsQuery, new {SiteId = siteId});
        public Task<IEnumerable<RowToCharge>> GetRowsToChargeAsync(int siteId, DateTime from, DateTime to) => QueryAsync<RowToCharge>(Queries.RowsToChargeQuery,
                                                    new { SiteId = siteId, FromDate = from, ToDate = to });
        public Task<int> IgnoreMeterAsync(long commandId) => ExecuteAsync(Queries.IgnoreMeterQuery, new { CommandId = commandId });
        public Task<int> RestoreMeterAsync(long commandId) => ExecuteAsync(Queries.RestoreMeterAsyncQuery, new { CommandId = commandId });
        public Task ChangeChargeRowAsync(RowToCharge rowToCharge) => ExecuteAsync(Queries.UpdateChargeRowQuery, rowToCharge);
        public Task<int> RemoveFromBillingAsync(long commandId) => ExecuteAsync(Queries.RemoveFromBillingQuery, new { CommandId = commandId });
        public Task<IEnumerable<RowToApprove>> GetRequiredChannelAsync() => QueryAsync<RowToApprove>(Queries.RowsToMaintainQuery,new{ ReplacementTypes = new List<int> {6, 7, 8}});


        private async Task<List<T>> QueryInTransaction<T>(string query, object? param = null)
        {
            using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
            await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);
            var result = (await sqlConn.QueryAsync<T>(query, param)).ToList();
            transaction.Complete();
            return result;
        }
        public Task<IEnumerable<Channel>> GetChannelsAsync(long meterNumber) => QueryAsync<Channel>(Queries.GetChannelsQuery, new { MeterNumber = meterNumber });
        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);
            return await sqlConn.QueryAsync<T>(sql, param);
        }
        public async Task UpdateDuplicatesAsync(IEnumerable<ConsumerNumbers> request)
        {
            try
            {
                await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);

                var table = new DataTable();
                table.Columns.Add("CommandId", typeof(long));
                table.Columns.Add("OldNumber", typeof(long));
                table.Columns.Add("NewNumber", typeof(long));
                foreach (var row in request) table.Rows.Add(row.CommandId, row.OldNumber , row.NewNumber);
                await sqlConn.ExecuteAsync(Queries.UpdateDuplicatesAsyncQuery, new
                {
                    Data = table.AsTableValuedParameter("[replacement].ReplacementModelConsumerNumbers")
                });
                await SetReplacementsAsync();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }

      

        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);
            return await sqlConn.ExecuteAsync(sql, param);
        }
    }
}
