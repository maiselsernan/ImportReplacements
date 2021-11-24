using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]"))]
    [ApiController]
    public class ReasonController : ControllerBase
    {
        private readonly IReasonRepository _reasonRepository;

        public ReasonController(IReasonRepository reasonRepository)
        {
            _reasonRepository = reasonRepository;
        }
        [HttpGet]
        public async Task<ActionResult> Reasons()
        {
            try
            {
                return Ok(await _reasonRepository.GetReasonsAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
    }
}
