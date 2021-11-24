using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;


namespace ImportReplacement.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilesController : ControllerBase
    {
        private readonly IFileRepository _fileRepository;
        public FilesController(IFileRepository fileRepository)
        {
            _fileRepository = fileRepository;
        }

        [HttpPost("{siteID}")]
        public async Task<ActionResult> Post([FromForm] IFormFile file, int siteID)
        {
            try
            {
                if (file.Length == 0)
                {
                    return BadRequest("Upload a file");
                }

                var response = await _fileRepository.SaveFileAsync(file, siteID);
                return Ok(response);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error uploading file to server");
            }
        }

        [HttpDelete("{siteID}")]
        public async Task<ActionResult> Delete(int siteID)
        {
            try
            {
                await _fileRepository.DeleteSitesConsumersAsync(siteID);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error uploading file to server");
            }
        }

    }
}
