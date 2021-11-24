using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Web.Services.Interfaces
{
    public  interface IReasonService
    {
        public Task<IEnumerable<Reason>> GetReasonsAsync();

    }
}
