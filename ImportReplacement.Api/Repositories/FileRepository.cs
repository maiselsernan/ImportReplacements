using System;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using ExcelDataReader;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;

namespace ImportReplacement.Api.Repositories
{
    class FileRepository : IFileRepository
    {
        private readonly SqlConnectionConfiguration _sqlConfiguration;
        public FileRepository(SqlConnectionConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public async Task<Response> SaveFileAsync(IFormFile file, int siteID)
        {
            var response = new Response(false,"");
            try
            {
                await using var stream = file.OpenReadStream();
                using IExcelDataReader reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                {
                    FallbackEncoding = Encoding.GetEncoding(1255)
                });
                var result = reader.AsDataSet(new ExcelDataSetConfiguration
                {
                    UseColumnDataType = true,
                    FilterSheet = (_, _) => true,
                    ConfigureDataTable = (_) => new ExcelDataTableConfiguration
                    {
                        UseHeaderRow = true,
                        FilterRow = (_) => true,
                        FilterColumn = (_, _) => true
                    }
                });
                using var bulkCopy = new SqlBulkCopy(_sqlConfiguration.Value)
                {
                    DestinationTableName = "replacement.NonAmrConsumers"
                };
                var table = result.Tables[0];
                table.Columns.Add(new DataColumn("SiteID") { DefaultValue = siteID });
                MapColumns(table, bulkCopy);
                using var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled);
                await DeleteSitesConsumersAsync(siteID);
                await bulkCopy.WriteToServerAsync(table);
                transaction.Complete();
                response.IsSuccess = true;
            }
            catch (Exception e)
            {
                while (e.InnerException != null) e = e.InnerException;
                response.Message = e.Message;
            }

            return response;
        }

        public async Task DeleteSitesConsumersAsync(int siteID)
        {
            await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);
            await sqlConn.ExecuteAsync(Queries.DeleteSitesConsumersQuery, new { SiteID = siteID });
        }

        private static void MapColumns(DataTable table, SqlBulkCopy bulkCopy)
        {
            foreach (DataColumn column in table.Columns)
            {
                bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
            }
        }
    }
}