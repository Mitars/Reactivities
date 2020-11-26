using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Activities;
using Reactivities.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    [Route ("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly IMediator mediator;
        public ActivitiesController(IMediator mediator) {
            this.mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<Activity>>> List(CancellationToken ct) =>
            await this.mediator.Send(new List.Query());

        [HttpGet("{id}")]
        public async Task<ActionResult<Activity>> Details(Guid id) =>
            await this.mediator.Send(new Details.Query { Id = id });

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command) =>
            await this.mediator.Send(command);
        
        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await this.mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(Guid id) =>
            await this.mediator.Send(new Delete.Command { Id = id });
    }
}