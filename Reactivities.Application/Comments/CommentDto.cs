using System;

namespace Reactivities.Application.Comments
{
    public record CommentDto
    {
        public Guid Id { get; init; }
        public string Body { get; init; }
        public DateTime CreatedAt { get; init; }
        public string Username { get; init; }
        public string DisplayName { get; init; }
        public string Image { get; init; }
    }
}