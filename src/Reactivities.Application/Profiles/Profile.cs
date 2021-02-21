using System.Collections.Generic;
using System.Text.Json.Serialization;
using Reactivities.Domain;

namespace Reactivities.Application.Profiles
{
    public record Profile
    {
        public string DisplayName { get; init; }
        public string Username { get; init; }
        public string Image { get; init; }
        public string Bio { get; init; }
        public ICollection<Photo> Photos { get; init; }
        [JsonPropertyName("following")]
        public bool IsFollowed { get; init; }
        public int FollowersCount { get; init; }
        public int FollowingCount { get; init; }
    }
}