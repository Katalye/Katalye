using System.Threading.Tasks;

using Katalye.Components.Commands;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using NLog;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class JobExportController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IMediator _mediator;

        public JobExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("salt/job/{jid:regex(\\d{{20}})}/new")]
        public async Task<IActionResult> NewJob([FromRoute] string jid, [FromBody] JobCreated.CreationData data)
        {
            var result = await _mediator.Send(new JobCreated.Command
            {
                Jid = jid,
                Data = data
            });
            return Ok(result);
        }

        [HttpPost("salt/job/{jid:regex(\\d{{20}})}/ret/{minionId}")]
        public IActionResult NewJob([FromRoute] string jid, [FromRoute] string minionId, [FromBody] JObject data)
        {
            Logger.Info($"Return from {minionId} for job with jid {jid} occurred. {data}");
            return Ok();
        }
    }
}