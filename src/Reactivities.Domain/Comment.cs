using System;

namespace Reactivities.Domain
{
    public record Comment
    {
        public Guid Id { get; init; }
        public string Body { get; init; }
        public virtual AppUser Author { get; init; }
        public virtual Activity Activity { get; init; }
        public DateTime CreatedAt { get; init; }
    }
}