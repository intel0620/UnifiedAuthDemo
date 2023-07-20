using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace UnifiedAuthLibrary
{ 
    public class DropboxLoginService
    {
        private readonly HttpClient _httpClient;

        public DropboxLoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("DropboxLoginService");
        }


        /// <summary>
        /// 產生 Dropbox Login 連動網址
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GenerateDropboxLoginUrl(string clientId, string redirectUri, string state, string scope)
        {
            var url = $"https://www.dropbox.com/oauth2/authorize?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&state={state}&scope={scope}";
            return url;
        }

        /// <summary>
        /// 取得 Dropbox Login 的 access token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<DropboxLoginAccessToken> GetAccessTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var endpoint = "https://api.dropbox.com/oauth2/token";
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
            return JsonSerializer.Deserialize<DropboxLoginAccessToken>(responseStream);
        }

        public async Task<DropboxUserInfo> GetUserProfile(string accessToken)
        {
            var endpoint = "https://api.dropboxapi.com/2/users/get_current_account";
            var request = new HttpRequestMessage(HttpMethod.Post, endpoint);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            var response = await _httpClient.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<DropboxUserInfo>(responseStream);
        }
    }
}
