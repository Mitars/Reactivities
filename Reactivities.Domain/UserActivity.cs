using System;

namespace Reactivities.Domain
{
    public class UserActivity
    {
        public string AppUserId { get; set; }
        public virtual AppUser AppUser { get; init; }
        public Guid ActivityId { get; init; }
        public virtual Activity Activity { get; init; }
        public DateTime DateJoined { get; init; }
        public bool IsHost { get; init; }
    }
}