using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reactivities.Application.Interfaces;
using Reactivities.User;

namespace Reactivities.Infrastructure.Security
{
    public class GoogleAccessor : IGoogleAccessor
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<FacebookAppSettings> config;

        public GoogleAccessor(IOptions<FacebookAppSettings> config)
        {
            this.config = config;
            this.httpClient = new HttpClient { BaseAddress = new System.Uri("https://www.googleapis.com/") };
            this.httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<GoogleUserInfo> Login(string accessToken) =>
            await this.IsValid(accessToken)
                ? await GetAsync<GoogleUserInfo>(accessToken, "oauth2/v3/userinfo")
                : null;

        private async Task<bool> IsValid(string accessToken) =>
            (await this.httpClient.GetAsync($"oauth2/v3/userinfo?access_token={accessToken}")).IsSuccessStatusCode;

        private async Task<T> GetAsync<T>(string accessToken, string endpoint)
        {
            var response = await this.httpClient.GetAsync($"{endpoint}?access_token={accessToken}");
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
