using System.Threading.Tasks;
using Katalye.Components.Commands.Server;
using Katalye.Components.Queries.Server;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Katalye.Api.Controllers
{
    [Route("api/v1/server")]
    public class ServerController : Controller
    {
        private readonly IMediator _mediator;

        public ServerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetSettings(GetServerInfo.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("settings")]
        public async Task<IActionResult> GetSettings(GetServerSettings.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("settings")]
        public async Task<IActionResult> UpdateSettings(UpdateServerSettings.Command query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}