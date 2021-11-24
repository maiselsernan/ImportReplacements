using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using ImportReplacement.Api.Repositories.Configurations;
using Microsoft.Data.SqlClient;

namespace ImportReplacement.Api.Repositories
{
    public class BaseRepository
    {
        private readonly SqlConnectionConfiguration _sqlConfiguration;

        public BaseRepository(SqlConnectionConfiguration sqlConfiguration)
        {
            _sqlConfiguration = sqlConfiguration;
        }
        protected async Task<IEnumerable<T>> QueryAsync<T>(string sql, object? param = null)
        {
            await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);
            return await sqlConn.QueryAsync<T>(sql, param);
        }
        protected async Task<int> ExecuteAsync(string sql, object? param = null)
        {
            await using var sqlConn = new SqlConnection(_sqlConfiguration.Value);
            return await sqlConn.ExecuteAsync(sql, param);
        }

    }
}