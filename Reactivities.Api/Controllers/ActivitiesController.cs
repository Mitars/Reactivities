using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Activities;
using Reactivities.Domain;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Reactivities.Api.Controllers
{
    public class ActivitiesController : MediatorControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<Activity>>> List(CancellationToken ct) =>
            await this.Mediator.Send(new List.Query());

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Activity>> Details(Guid id) =>
            await this.Mediator.Send(new Details.Query { Id = id });

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command) =>
            await this.Mediator.Send(command);

        [HttpPut("{id}")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await this.Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<Unit>> Delete(Guid id) =>
            await this.Mediator.Send(new Delete.Command { Id = id });
    }
}