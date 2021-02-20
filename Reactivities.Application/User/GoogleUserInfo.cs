using System.Linq;
using Newtonsoft.Json;

namespace Reactivities.User
{
    public record GoogleUserInfo
    {
        [JsonProperty(PropertyName = "sub")]
        public string Id { get; init; }
        public string Name { get; init; }
        [JsonProperty(PropertyName = "given_name")]
        public string GivenName { get; init; }
        [JsonProperty(PropertyName = "family_name")]
        public string FamilyName { get; init; }
        public string Picture { get; init; }
        public string Email { get; init; }
        [JsonProperty(PropertyName = "email_verified")]
        public bool EmailVerified { get; init; }
        public string Locale { get; init; }
        public string PictureUrl => this.Picture.Split("=").FirstOrDefault() + "=s500-c";
    }
}