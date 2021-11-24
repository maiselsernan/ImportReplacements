using System.Threading.Tasks;
using ImportReplacement.Models;
using Tewr.Blazor.FileReader;

namespace ImportReplacement.Web.Services.Interfaces
{
    public interface IFileService
    {
        Task<Response> FileUploadAsync(IFileReference file, int siteID);
        Task DeleteNonAmrConsumersBySiteId(int siteID);

    }
}
