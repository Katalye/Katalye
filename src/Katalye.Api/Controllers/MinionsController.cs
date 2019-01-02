using System.Threading.Tasks;
using Katalye.Components.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Katalye.Api.Controllers
{
    [Route("api/v1/minions")]
    public class MinionsController : Controller
    {
        private readonly IMediator _mediator;

        public MinionsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetMinions(GetMinions.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}