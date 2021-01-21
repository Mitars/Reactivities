using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Reactivities.Application.Interfaces;
using Reactivities.User;

namespace Reactivities.Infrastructure.Security
{
    public class FacebookAccessor : IFacebookAccessor
    {
        private readonly HttpClient httpClient;
        private readonly IOptions<FacebookAppSettings> config;

        public FacebookAccessor(IOptions<FacebookAppSettings> config)
        {
            this.config = config;
            this.httpClient = new HttpClient { BaseAddress = new System.Uri("https://graph.facebook.com/") };
            this.httpClient.DefaultRequestHeaders
                .Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<FacebookUserInfo> FacebookLogin(string accessToken) =>
            await this.IsValid(accessToken)
                ? await GetAsync<FacebookUserInfo>(accessToken, "me", "fields=name,email,picture.width(100).height(100)")
                : null;

        private async Task<bool> IsValid(string accessToken) =>
            (await this.httpClient.GetAsync($"debug_token?input_token={accessToken}&access_token={this.config.Value.AppId}|{this.config.Value.AppSecret}")).IsSuccessStatusCode;

        private async Task<T> GetAsync<T>(string accessToken, string endpoint, string args)
        {
            var response = await this.httpClient.GetAsync($"{endpoint}?access_token={accessToken}&{args}");
            if (!response.IsSuccessStatusCode)
            {
                return default;
            }

            var result = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(result);
        }
    }
}
