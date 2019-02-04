using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NLog;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class JidCreationExportController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpPost("{id:regex(\\d{{20}})}")]
        public IActionResult JidCreated([FromRoute] string jid, [FromBody] JToken data)
        {
            Logger.Info($"Got jid creation event for {jid}. {data}");
            return Ok();
        }
    }
}