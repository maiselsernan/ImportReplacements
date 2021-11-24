using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]/[action]"))]
    [ApiController]
    public class TypesController : ControllerBase
    {
        private readonly ITypesRepository _typesRepository;
        public TypesController(ITypesRepository typesRepository)
        {
            _typesRepository = typesRepository;
        }

        [HttpGet]
        public async Task<ActionResult> MeterTypes()
        {
            try
            {
                return Ok(await _typesRepository.GetMeterTypesAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        [HttpGet]
        public async Task<ActionResult> MeterModels()
        {
            try
            {
                return Ok(await _typesRepository.GetMeterModelsAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        [HttpGet]
        public async Task<ActionResult> MeterManufacturers()
        {
            try
            {
                return Ok(await _typesRepository.GetMeterManufacturersAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
    }
}
