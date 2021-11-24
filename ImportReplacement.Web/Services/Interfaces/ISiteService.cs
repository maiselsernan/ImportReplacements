using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
   public interface ISiteService
   {
       public Task<IEnumerable<Site>> GetSitesAsync();
   }
}
