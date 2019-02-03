using System.Threading.Tasks;
using Katalye.Components.Commands.Minions;
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

        [HttpGet("_search-grains")]
        public async Task<IActionResult> GetMinionSearchGrains(GetSearchGrains.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("_search-grain-values")]
        public async Task<IActionResult> GetMinionSearchGrainValues(GetSearchGrainValues.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetMinion(GetMinionBySlug.Query query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{Id}/jobs")]
        public async Task<IActionResult> GetMinionJobs(GetMinionJobs.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{Id}/jobs/{Jid}")]
        public async Task<IActionResult> GetMinionJob(GetMinionJobByJid.Query query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpPost("{Id}/grains/refresh")]
        public async Task<IActionResult> RefreshMinionGrains(RefreshMinionGrains.Command command)
        {
            var result = await _mediator.Send(command);
            return Ok(result);
        }
    }
}