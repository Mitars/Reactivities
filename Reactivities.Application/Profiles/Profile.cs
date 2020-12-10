using System.Collections.Generic;
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
    }
}