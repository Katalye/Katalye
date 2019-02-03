using System.Threading.Tasks;
using Katalye.Components.Queries.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Katalye.Api.Controllers
{
    [Route("api/v1/tasks")]
    public class TasksController : Controller
    {
        private readonly IMediator _mediator;

        public TasksController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<IActionResult> GetTasks(GetTasks.Query query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{Id}")]
        public async Task<IActionResult> GetTask(GetTaskById.Query query)
        {
            var result = await _mediator.Send(query);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }
}