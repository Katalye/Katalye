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
    }
}