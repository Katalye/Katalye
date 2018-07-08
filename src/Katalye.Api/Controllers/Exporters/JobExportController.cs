using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using NLog;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class JobExportController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpPost("salt/job/{jid:regex(\\d{{20}})}/new")]
        public IActionResult NewJob([FromRoute] string jid, [FromBody] JObject data)
        {
            Logger.Info($"New job with jid {jid} was created. {data}");
            return Ok();
        }

        [HttpPost("salt/job/{jid:regex(\\d{{20}})}/ret/{minionId?}")]
        public IActionResult NewJob([FromRoute] string jid, [FromRoute] string minionId, [FromBody] JObject data)
        {
            Logger.Info($"Return from {minionId} for job with jid {jid} occurred. {data}");
            return Ok();
        }
    }
}