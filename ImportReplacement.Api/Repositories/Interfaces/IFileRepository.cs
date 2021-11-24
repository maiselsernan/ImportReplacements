using System.Threading.Tasks;
using ImportReplacement.Models;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Repositories.Interfaces
{
    public interface IFileRepository
    {
        Task<Response> SaveFileAsync(IFormFile file, int siteID);
        Task DeleteSitesConsumersAsync(int siteID);
    }
}
