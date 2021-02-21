using System;

namespace Reactivities.Application.Profiles
{
    public record UserActivityDto
    {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Category { get; init; }
        public DateTime Date { get; init; }
    }
}