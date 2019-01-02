using System.Threading.Tasks;
using Katalye.Components.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Katalye.Api.Controllers.Exporters
{
    [Route("api/v1/export/event")]
    public class AuthExportController : Controller
    {
        private readonly IMediator _mediator;

        public AuthExportController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("salt/auth")]
        public async Task<IActionResult> Auth([FromBody] MinionAuthenticated.Command command)
        {
            await _mediator.Send(command);
            return Ok();
        }
    }
}