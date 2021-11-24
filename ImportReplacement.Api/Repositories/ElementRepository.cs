using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Dapper;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;
using Microsoft.Data.SqlClient;

namespace ImportReplacement.Api.Repositories
{
    class ElementRepository : BaseRepository, IElementRepository
    {
        private readonly SqlConnectionConfiguration _sqlConfiguration;

        public ElementRepository(SqlConnectionConfiguration sqlConfiguration) : base(sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }

        public async Task<IEnumerable<Element>> GetElementsAsync() => await QueryAsync<Element>(Queries.GetElementsQuery);
        public async Task<IEnumerable<ConsumerElement>> GetConsumerElementsAsync(long commandId) =>
                                await QueryAsync<ConsumerElement>(Queries.GetConsumerElementsQuery, new { CommandId = commandId });

        public async Task UpdateConsumerElementsAsync(ElementsRequest request)
        {
            try
            {
                await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);

                var commandId = request.CommandId;
                var table = new DataTable();
                table.Columns.Add("ElementId", typeof(int));
                table.Columns.Add("Quantity", typeof(int));
                foreach (var row in request.ConsumerElements) table.Rows.Add(row.ID, row.Quantity);
                await sqlConn.ExecuteAsync(Queries.UpdateConsumerElementsQuery, new
                {
                    CommandId = commandId,
                    Data = table.AsTableValuedParameter("[replacement].ReplacementModelElementsType")
                });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
          
        }

        public Task<IEnumerable<dynamic>> GetElementsToChargeAsync(IEnumerable<long> commands)
        {
            StringBuilder pivotList = new StringBuilder();
            StringBuilder commandsList = new StringBuilder();

            foreach (var command in commands)
            {
                pivotList.Append("p.[" + command + "],");
                commandsList.Append("[" + command + "],");
            }

            pivotList.Remove(pivotList.Length - 1, 1);
            commandsList.Remove(commandsList.Length - 1, 1);
            var query = @$"SELECT Elements.Name, {pivotList}
                           FROM [replacement].[ReplacementModelElements] PIVOT (SUM([Quantity]) FOR [command_id] IN ({commandsList})) p
	                       JOIN [replacement].Elements ON Elements.ID = p.ElementId";
           return QueryAsync<dynamic>(query);
        }

        public async Task AddNewElementAsync(string newElement) => 
                            await QueryAsync<Element>(Queries.AddNewElementQuery, new {Name = newElement});
    }
}
