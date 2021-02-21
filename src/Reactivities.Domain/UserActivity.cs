using System;

namespace Reactivities.Domain
{
    public record UserActivity
    {
        public string AppUserId { get; init; }
        public virtual AppUser AppUser { get; init; }
        public Guid ActivityId { get; init; }
        public virtual Activity Activity { get; init; }
        public DateTime DateJoined { get; init; }
        public bool IsHost { get; init; }
    }
}