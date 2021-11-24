using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.TypeConversion;
using ImportReplacement.Api.Repositories.Interfaces;
using ImportReplacement.Models;
using Microsoft.AspNetCore.Http;

namespace ImportReplacement.Api.Controllers
{
    [Route(("api/[controller]/[action]"))]
    [ApiController]
    public class ReplacementController : ControllerBase
    {
        private readonly IReplacementRepository _replacementRepository;

        public ReplacementController(IReplacementRepository replacementRepository)
        {
            _replacementRepository = replacementRepository;
        }
        [HttpGet("{meterNumber}")]
        public  Task<ActionResult> Channels(int meterNumber)
            => ExecuteAsync(() => _replacementRepository.GetChannelsAsync(meterNumber));

        [HttpGet("{type}")]
        public Task<ActionResult> MetersToApprove(RequestTypes type)
            => ExecuteAsync(() => type switch
            {
                RequestTypes.Maintain => _replacementRepository.GetMetersToMaintainAsync(),
                RequestTypes.Replace => _replacementRepository.GetMetersToReplaceAsync(),
                RequestTypes.Installation => _replacementRepository.GetMetersToInstallAsync(),
                RequestTypes.Ignore => _replacementRepository.GetIgnoresMetersAsync(),
                RequestTypes.ReplaceRequiredChannel => _replacementRepository.GetRequiredChannelAsync(),
                RequestTypes.MaintainRequiredChannel => _replacementRepository.GetRequiredChannelAsync(),
                RequestTypes.InstallationRequiredChannel => _replacementRepository.GetRequiredChannelAsync(),
                _ => throw new BadHttpRequestException("Invalid Type")
            });

       [HttpGet("{type}")]
        public  Task<ActionResult> UndefinedRows(RequestTypes type)
            => ExecuteAsync(() => _replacementRepository.GetUndefinedRows(type));

        [HttpGet]
        public Task<ActionResult> HandledRows(DateTime from, DateTime to)
        {
            return ExecuteAsync(() => _replacementRepository.GetHandledRowsAsync(from, to));
        }

        [HttpPost]
        public  Task<ActionResult> SetChannels()
            => ExecuteAsync(() => _replacementRepository.SetChannelsAsync());
       
        [HttpPost]
        public async Task<ActionResult> SaveApprovedRowsAsync([FromBody] Request request)
        {
            try
            {
                List<ResponseToFile> response;
                switch (request.Type)
                {
                    case RequestTypes.Maintain:
                        response = await _replacementRepository.SaveApprovedMaintainsAsync(request.SiteId);
                        break;
                    case RequestTypes.Replace:
                        response = await _replacementRepository.SaveApprovedReplacementsAsync(request.SiteId);
                        break;
                    case RequestTypes.Installation:
                        response = await _replacementRepository.SaveApprovedInstallationsAsync(request.SiteId);
                        break;
                    case RequestTypes.MaintainRequiredChannel:
                    case RequestTypes.InstallationRequiredChannel:
                    case RequestTypes.ReplaceRequiredChannel:
                        response = await _replacementRepository.SaveMissingChannelsAsync(request.SiteId);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(request.Type), request.Type, null);
                }

                if (response.Count == 0)
                {
                    return NoContent();
                }
                var stream = new MemoryStream();

                await using (var writeFile = new StreamWriter(stream, Encoding.GetEncoding(1255), leaveOpen: true))
                {
                    var csv = new CsvWriter(writeFile, CultureInfo.InvariantCulture, true);
                    var options = new TypeConverterOptions { Formats = new[] { "yyyy-MM-dd HH:mm" } };
                    csv.Context.TypeConverterOptionsCache.AddOptions<DateTime>(options);
                    await csv.WriteRecordsAsync(response);
                }
                stream.Position = 0;
                return File(stream, "text/csv");
            }
            catch (Exception)
            {
                return Status500InternalServerError();
            }
        }

        [HttpPost]
        public  Task<ActionResult> ChangeRowAsync(RowToApprove rowToApprove)
            => ExecuteAsync(() => _replacementRepository.ChangeRowAsync(rowToApprove));
        
       [HttpPost]
        public  Task<ActionResult> ImportReplacements()
            => ExecuteAsync(() => _replacementRepository.ImportReplacements());
      
        [HttpPost("{commandId}")]
        public  Task<ActionResult> PerformedManually(long commandId)
            => ExecuteAsync(() => _replacementRepository.PerformedManuallyAsync(commandId));
      
        [HttpPost]
        public  Task<ActionResult> MoveRowsToSiteAsync([FromBody] MoveRowsToSiteRequest request)
            => ExecuteAsync(() => _replacementRepository.MoveRowsToSiteAsync(request));
        
        [HttpPost]
        public  Task<ActionResult> CreateNewConsumers([FromBody] MoveRowsToSiteRequest request)
            => ExecuteAsync(() => _replacementRepository.CreateNewConsumersAsync(request));
       
        [HttpGet("{siteId}")]
        public Task<ActionResult> GetRowsToChargeAsync(int siteId, DateTime from, DateTime to)
            => ExecuteAsync(() => _replacementRepository.GetRowsToChargeAsync(siteId, from, to));
      

        [HttpPost("{commandId}")]
        public Task<ActionResult> IgnoreMeter(long commandId)
            => ExecuteAsync(() => _replacementRepository.IgnoreMeterAsync(commandId));
      
        [HttpPost("{commandId}")]
        public Task<ActionResult> RestoreMeter(long commandId)
            => ExecuteAsync(() => _replacementRepository.RestoreMeterAsync(commandId));
        [HttpPost]
        public  Task<ActionResult> UpdateDuplicatesAsync([FromBody] IEnumerable<ConsumerNumbers> request)
            => ExecuteAsync(() => _replacementRepository.UpdateDuplicatesAsync(request));
        public async Task<ActionResult> ExecuteAsync(Func<Task> func)
        {
            try
            {
                await func();
                return Ok();
            }
            catch (Exception e) when(e is not BadHttpRequestException)
            {
                return Status500InternalServerError();
            }
        }
        [HttpPost]
        public  Task<ActionResult> ChangeChargeRowAsync(RowToCharge rowToCharge)
            => ExecuteAsync(() => _replacementRepository.ChangeChargeRowAsync(rowToCharge));
        
        public async Task<ActionResult> ExecuteAsync<T>(Func<Task<T>> func)
        {
            try
            {
                return Ok(await func());
            }
            catch (Exception e) when(e is not BadHttpRequestException)
            {
                return Status500InternalServerError();
            }
        }

        private ObjectResult Status500InternalServerError()
        {
            return StatusCode(StatusCodes.Status500InternalServerError,
                "Error retrieving data from the database");
        }
        [HttpPost("{commandId}")]
        public Task<ActionResult> RemoveFromBilling(long commandId)
            => ExecuteAsync(() => _replacementRepository.RemoveFromBillingAsync(commandId));

    }
}
