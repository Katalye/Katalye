using System.Threading.Tasks;
using Katalye.Components.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event/salt/job/{jid:regex(\\d{{20}})}")]
    public class JobExportController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;

        public JobExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("new")]
        public async Task<IActionResult> NewJobEvent([FromRoute] string jid, [FromBody] JobCreated.CreationData data)
        {
            var result = await _mediator.Send(new JobCreated.Command
            {
                Jid = jid,
                Data = data
            });
            return Ok(result);
        }

        [HttpPost("ret/{minionId}")]
        public async Task<IActionResult> ReturnEvent([FromRoute] string jid, [FromRoute] string minionId, [FromBody] JobReturned.Data data)
        {
            var result = await _mediator.Send(new JobReturned.Command
            {
                Data = data,
                Jid = jid,
                MinionSlug = minionId
            });
            
            return Ok(result);
        }

        [HttpPost("prog/{minionId}/{runNumber}")]
        public IActionResult ProgressEvent([FromRoute] string jid, [FromRoute] string minionId, [FromRoute] int runNumber, [FromBody] JToken data)
        {
            Logger.Info($"Progress update from {minionId} for job with jid {jid} occurred. {data}");
            return Ok();
        }
    }
}