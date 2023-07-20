using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace UnifiedAuthLibrary
{
    public class InstagramLoginService
    {

        private readonly HttpClient _httpClient;

        public InstagramLoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("InstagramLoginService");
        }

        /// <summary>
        /// 產生 Instagram Login 連動網址
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GenerateInstagramLoginUrl(string clientId, string redirectUri, string state, string scope)
        {
            var url = $"https://api.instagram.com/oauth/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&state={state}&scope={scope}";
            return url;
        }

        /// <summary>
        /// 取得 Line Login 的 access token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<InstagramLoginAccessToken> GetAccessTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var endpoint = "https://api.instagram.com/oauth/access_token";
            var response = await _httpClient.PostAsync(endpoint, new FormUrlEncodedContent(new Dictionary<string, string>
            {
                { "grant_type", "authorization_code" },
                { "code", code },
                { "client_id", clientId },
                { "client_secret", clientSecret },
                { "redirect_uri", redirectUri }
            }));
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<InstagramLoginAccessToken>(responseStream);
        }


        /// <summary>
        /// 取得 User資料
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<InstagramLoginUserInfo> GetUserProfile(string accessToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, "https://graph.instagram.com/me?fields=id,username,email&access_token=" + accessToken);
            var response = await _httpClient.SendAsync(request);
            var responseStream = await response.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<InstagramLoginUserInfo>(responseStream);

        }
    }
}
