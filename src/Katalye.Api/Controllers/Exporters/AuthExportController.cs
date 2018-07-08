using Microsoft.AspNetCore.Mvc;

using Newtonsoft.Json.Linq;

using NLog;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class AuthExportController : Controller
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        [HttpPost("salt/auth")]
        public IActionResult Auth([FromBody] JObject data)
        {
            Logger.Info($"Got auth. {data}");
            return Ok();
        }
    }
}