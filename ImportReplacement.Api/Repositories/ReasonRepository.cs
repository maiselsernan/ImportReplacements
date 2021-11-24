using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories
{
    public class ReasonRepository :  BaseRepository, IReasonRepository
    {
        public ReasonRepository(SqlConnectionConfiguration sqlConfiguration) : base(sqlConfiguration)
        {
        }
        public async Task<IEnumerable<Reason>> GetReasonsAsync()
        {
            return await QueryAsync<Reason>(Queries.GetReasonsQuery);
        }
    }
}
