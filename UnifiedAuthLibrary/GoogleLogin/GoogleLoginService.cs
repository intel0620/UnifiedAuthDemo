using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace UnifiedAuthLibrary
{
    public class GoogleLoginService
    {
        private readonly HttpClient _httpClient;

        public GoogleLoginService(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("GoogleLoginService");
        }

        /// <summary>
        /// 產生 Google Login 連動網址
        /// </summary>
        /// <param name="clientId"></param>
        /// <param name="redirectUri"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GenerateGoogleLoginUrl(string clientId, string redirectUri, string state, string scope)
        {
            var url = $"https://accounts.google.com/o/oauth2/v2/auth?response_type=code&client_id={clientId}&redirect_uri={redirectUri}&state={state}&scope={scope}";
            return url;
        }



        /// <summary>
        /// 取得 Google Login 的 access token
        /// </summary>
        /// <param name="code"></param>
        /// <param name="clientId"></param>
        /// <param name="clientSecret"></param>
        /// <param name="redirectUri"></param>
        /// <returns></returns>
        public async Task<GoogleAccessToken> GetAccessTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var endpoint = "https://oauth2.googleapis.com/token";
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
            return JsonSerializer.Deserialize<GoogleAccessToken>(responseStream);
        }


        /// <summary>
        /// 驗證 Google Login 的 access token
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns></returns>
        public async Task<GoogleLoginVerifyAccessTokenResult> VerifyAccessTokenAsync(string accessToken)
        {
            var endpoint = $"https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={accessToken}";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<GoogleLoginVerifyAccessTokenResult>(responseStream);
        }

        /// <summary>
        /// 驗證 Google Login 的 id token
        /// </summary>
        /// <param name="idToken"></param>
        /// <param name="cliendId"></param>
        /// <returns></returns>
        public async Task<GoogleLoginVerifyIdTokenResult> VerifyIdTokenAsync(string idToken)
        {
            var endpoint = $"https://oauth2.googleapis.com/tokeninfo?id_token={idToken}";
            var response = await _httpClient.GetAsync(endpoint);
            response.EnsureSuccessStatusCode();

            var responseStream = await response.Content.ReadAsStreamAsync();
            return JsonSerializer.Deserialize<GoogleLoginVerifyIdTokenResult>(responseStream);
        }
    }
}
