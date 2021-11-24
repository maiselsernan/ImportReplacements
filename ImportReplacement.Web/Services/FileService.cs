using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using ImportReplacement.Models;
using ImportReplacement.Web.Services.Interfaces;
using Tewr.Blazor.FileReader;

namespace ImportReplacement.Web.Services
{
    class FileService : IFileService
    {
        private readonly HttpClient _httpClient;
        Stream _fileStream;

        public FileService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<Response> FileUploadAsync(IFileReference file, int siteID)
        {
            var fileInfo = await file.ReadFileInfoAsync();

            using (var memoryStream = await file.CreateMemoryStreamAsync((int)fileInfo.Size))
            {
                _fileStream = new MemoryStream(memoryStream.ToArray());
            }

            var content = new MultipartFormDataContent();
            content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
            content.Add(new StreamContent(_fileStream, (int)_fileStream.Length), "file", fileInfo.Name);

            var response = await _httpClient.PostAsync($"Files/{siteID}", content);
            return response.Content.ReadAsAsync<Response>().Result;
        }

        public async Task DeleteNonAmrConsumersBySiteId(int siteID)
        {
            await _httpClient.DeleteAsync($"Files/{siteID}");
        }
    }
}