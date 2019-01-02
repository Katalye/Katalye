using System.Threading.Tasks;
using Katalye.Components.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class DefaultExportController : Controller
    {
        private readonly IMediator _mediator;

        public DefaultExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("{*tag}", Order = 99)]
        public async Task<IActionResult> Default([FromRoute] string tag, [FromBody] JObject data)
        {
            await _mediator.Send(new UnknownEventRecieved.Command
            {
                Data = data,
                Tag = tag
            });
            return Ok();
        }
    }
}