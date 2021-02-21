using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Reactivities.Application.Comments;

namespace Reactivities.Api.SignalR
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IMediator mediator;

        public ChatHub(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public async Task SendComment(Create.Command command)
        {
            var comment = await this.mediator.Send(command with { Username = GetUserName() });
            await Clients.Group(command.ActivityId.ToString()).SendAsync("ReceiveComment", comment);
        }

        public async Task AddToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(this.Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{GetUserName()} has joined the group");
        }

        public async Task RemoveFromGroup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(this.Context.ConnectionId, groupName);
            await Clients.Group(groupName).SendAsync("Send", $"{GetUserName()} has left the group");
        }

        private string GetUserName() =>
            this.Context.User?.Claims?.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
    }
}