using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Configurations;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories
{
    public class SiteRepository : BaseRepository, ISiteRepository
    {
        public SiteRepository(SqlConnectionConfiguration sqlConfiguration) : base(sqlConfiguration)
        {
        }
        
        public async Task<IEnumerable<Site>> GetSitesAsync()
        {
            return await QueryAsync<Site>(Queries.GetSitesQuery);
        }
    }
}