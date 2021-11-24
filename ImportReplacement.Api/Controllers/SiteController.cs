using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]"))]
    [ApiController]
    public class SitesController : ControllerBase
    {

        private readonly ISiteRepository _siteRepository;
        public SitesController(ISiteRepository siteRepository)
        {
            _siteRepository = siteRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Sites()
        {
            try
            {
                return Ok(await _siteRepository.GetSitesAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        
        
    }
}
