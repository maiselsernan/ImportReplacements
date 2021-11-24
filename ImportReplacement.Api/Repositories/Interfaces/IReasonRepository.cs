using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Models;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface IReasonRepository
    {
        Task<IEnumerable<Reason>> GetReasonsAsync();
    }
    
}
