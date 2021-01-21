using System.Linq;
using System.Text.Json.Serialization;
using Reactivities.Application.Interfaces;
using Reactivities.Domain;

namespace Reactivities.Application.User
{
    public record UserDto
    {
        public UserDto(AppUser user, IJwtGenerator jwtGenerator, string refreshToken)
        {
            this.DisplayName = user.DisplayName;
            this.Token = jwtGenerator.CreateToken(user);
            this.UserName = user.UserName;
            this.Image = user.Photos.FirstOrDefault(p => p.IsMain)?.Url;
            this.RefreshToken = refreshToken;
        }

        public string DisplayName { get; init; }
        public string Token { get; init; }
        public string UserName { get; init; }
        public string Image { get; init; }

        [JsonIgnore]
        public string RefreshToken { get; init; }
    }
}