using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]/[action]"))]
    [ApiController]
    public class ElementController : ControllerBase
    {
        private readonly IElementRepository _elementRepository;

        public ElementController(IElementRepository elementRepository)
        {
            _elementRepository = elementRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Elements()
        {
            try
            {
                return Ok(await _elementRepository.GetElementsAsync());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        [HttpGet("{commandId}")]
        public async Task<ActionResult> ReplacementModelElements(long commandId)
        {
            try
            {
                return Ok(await _elementRepository.GetConsumerElementsAsync(commandId));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

        [HttpPost]
        public async Task<ActionResult> UpdateConsumerElements([FromBody] ElementsRequest request)
        {
            try
            {
                await _elementRepository.UpdateConsumerElementsAsync(request);
                return Ok();
            }
            catch (Exception e) when(e is not BadHttpRequestException)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
       
        [HttpPost]
        public async  Task<ActionResult> ElementsToChargeAsync(List<long> commands)
        {
            try
            {
                return Ok(await _elementRepository.GetElementsToChargeAsync(commands));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

    
        [HttpPost]
        public async Task<ActionResult> AddNewElement([FromBody]string newElement)
        {
            try
            {
                if (string.IsNullOrEmpty(newElement))
                {
                    return StatusCode(StatusCodes.Status500InternalServerError,
                        "Empty element string");
                }

                await _elementRepository.AddNewElementAsync(newElement);
                return Ok();
            }
            catch (Exception exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    exception.Message);
            }
        }
    }
}
