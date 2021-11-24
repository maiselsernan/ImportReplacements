using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]/[action]"))]
    [ApiController]
    public class ConsumerController : ControllerBase
    {
        private readonly IConsumerRepository _consumerRepository;

        public ConsumerController(IConsumerRepository consumerRepository)
        {
            _consumerRepository = consumerRepository;
        }

        [HttpGet]
        public async Task<ActionResult> Consumers(int siteID, long? meterNumber, long? billingID, long? propertyID)
        {
            try
            {
                return Ok(await _consumerRepository.GetConsumerAsync(siteID, meterNumber, billingID, propertyID));
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }
        [HttpPost("{commandId}")]
        public async Task<ActionResult> MoveConsumerAsync(long commandId,  MoveConsumerObject moveConsumerObject)
        {
            try
            {
                await _consumerRepository.MoveConsumerToTypeAsync(commandId,  moveConsumerObject.ConsumerId, moveConsumerObject.NewType, moveConsumerObject.SiteId);
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    "Error retrieving data from the database");
            }
        }

    }

    public class MoveConsumerObject
    {
        public long ConsumerId { get; set; }
        public RequestTypes NewType { get; set; }
        public int SiteId { get; set; }
    }
}
