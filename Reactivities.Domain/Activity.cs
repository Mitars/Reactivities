using System;
using System.Collections.Generic;

namespace Reactivities.Domain
{
    public class Activity
    {
        public Guid Id { get; init; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Category { get; set; }
        public DateTime Date { get; set; }
        public string City { get; set; }
        public string Venue { get; set; }
        public virtual ICollection<UserActivity> UserActivities { get; init; }
        public virtual ICollection<Comment> Comments { get; init; }
    }
}