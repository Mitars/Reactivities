using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Reactivities.Application.Comments;

namespace Reactivities.Application.Activities {
    public record ActivityDto {
        public Guid Id { get; init; }
        public string Title { get; init; }
        public string Description { get; init; }
        public string Category { get; init; }
        public DateTime Date { get; init; }
        public string City { get; init; }
        public string Venue { get; init; }

        [JsonPropertyName ("attendees")]
        public ICollection<AttendeeDto> UserActivities { get; init; }
        public ICollection<CommentDto> Comments { get; init; }
    }
}
