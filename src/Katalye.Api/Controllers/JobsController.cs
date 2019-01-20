using System.Threading.Tasks;
using Katalye.Components.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Katalye.Api.Controllers
{
    [Route("api/v1/jobs")]
    public class JobsController : Controller
    {
        private readonly IMediator _mediator;

        public JobsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetJobs(GetJobs.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{Jid}")]
        public async Task<IActionResult> GetJob(GetJobByJid.Query query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        [HttpGet("{Jid}/minions")]
        public async Task<IActionResult> GetJobMinions(GetJobMinions.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}