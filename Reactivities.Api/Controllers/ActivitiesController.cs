using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reactivities.Application.Activities;

namespace Reactivities.Api.Controllers
{
    public class ActivitiesController : MediatorControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List.Response>> List([FromQuery] List.Query query) =>
            await this.Mediator.Send(query);

        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<ActivityDto>> Details(Guid id) =>
            await this.Mediator.Send(new Details.Query { Id = id });

        [HttpPost]
        public async Task<ActionResult<Unit>> Create(Create.Command command) =>
            await this.Mediator.Send(command);

        [HttpPut("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Edit(Guid id, Edit.Command command)
        {
            command.Id = id;
            return await this.Mediator.Send(command);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "IsActivityHost")]
        public async Task<ActionResult<Unit>> Delete(Guid id) =>
            await this.Mediator.Send(new Delete.Command { Id = id });

        [HttpPost("{id}/attend")]
        public async Task<ActionResult<Unit>> Attend(Guid id) =>
            await this.Mediator.Send(new Attend.Command { Id = id });

        [HttpDelete("{id}/attend")]
        public async Task<ActionResult<Unit>> Unattend(Guid id) =>
            await this.Mediator.Send(new Unattend.Command { Id = id });
    }
}