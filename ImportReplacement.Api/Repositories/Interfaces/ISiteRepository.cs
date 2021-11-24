using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface ISiteRepository
    {
        Task<IEnumerable<Site>> GetSitesAsync();
    }
}
