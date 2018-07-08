using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using NLog;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class DefaultExportController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpPost("{*tag}", Order = 99)]
        public IActionResult Default([FromRoute] string tag, [FromBody] JObject data)
        {
            Logger.Warn($"Got unknown tag {tag} with {data}.");
            return Ok();
        }
    }
}