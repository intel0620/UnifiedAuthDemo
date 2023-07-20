using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace UnifiedAuthLibrary
{
    public class TwitterLoginService
    {
        private readonly HttpClient _httpClient;

        public TwitterLoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("TwitterLoginService");
        }

        /// <summary>
        /// 產生 Twitter Login 連動網址
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GenerateTwitterLoginUrl(string clientId, string redirectUri, string state, string scope)
        {
            var url = $"https://twitter.com/i/oauth2/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&state={state}&scope={scope}&code_challenge=challenge&code_challenge_method=plain";
            return url;
        }

        /// <summary>
        /// 取得 Twitter Login 的 access token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<TwitterAccessToken> GetAccessTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.twitter.com/2/oauth2/token");
            var authorization = Convert.ToBase64String(
               Encoding.UTF8.GetBytes($"{clientId}:{clientSecret}"));
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Basic {authorization}");

            request.Content = new FormUrlEncodedContent(
                new Dictionary<string, string>
                {
                    ["grant_type"] = "authorization_code",
                    ["code"] = code,
                    ["redirect_uri"] = redirectUri,
                    ["code_verifier"] = "challenge"
                });

            var response = await _httpClient.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK) return null;

            var content = await response.Content.ReadAsStringAsync();
            var result = JsonNode.Parse(content);

            return JsonSerializer.Deserialize<TwitterAccessToken>(result);
           
        }

        public async Task<TwitterUserInfo> GetUserProfile(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://api.twitter.com/2/users/me?user.fields=id,name,profile_image_url");
            request.Headers.Authorization = AuthenticationHeaderValue.Parse($"Bearer {accessToken}");
            var response = await _httpClient.SendAsync(request);
            var responseStream = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<TwitterUserInfo>(responseStream);
        }
    }
}
