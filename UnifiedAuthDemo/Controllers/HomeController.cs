using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using System.IO.Pipes;
using System.Net.Http.Headers;
using System.Text;
using UnifiedAuthDemo.Models;
using UnifiedAuthLibrary;

namespace UnifiedAuthDemo.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly LineLoginModel _lineLoginModel;
        private readonly GoogleLoginModel _googleLoginModel;
        private readonly FaceBookLoginModel _faceBookLoginModel;
        private readonly TwitterLoginModel _twitterLoginModel;
        private readonly DropboxLoginModel _dropboxLoginModel;
        private readonly InstagramLoginModel _instagramLoginModel;


        private readonly LineLoginService _lineLoginService;
        private readonly GoogleLoginService _googleLoginService;
        private readonly FaceBookLoginService _faceBookLoginService;
        private readonly TwitterLoginService _twitterLoginService;
        private readonly DropboxLoginService _dropboxLoginService;
        private readonly InstagramLoginService _instagramLoginService;

        public HomeController(ILogger<HomeController> logger, IOptions<LineLoginModel> LineLoginModel, IOptions<GoogleLoginModel> GoogleLoginModel, IOptions<FaceBookLoginModel> FaceBookLoginModel, IOptions<TwitterLoginModel> TwitterLoginModel, IOptions<DropboxLoginModel> DropboxLoginModel, IOptions<InstagramLoginModel> InstagramLoginModel, LineLoginService lineLoginService, GoogleLoginService googleLoginService, FaceBookLoginService faceBookLoginService, TwitterLoginService twitterLoginService, DropboxLoginService dropboxLoginService, InstagramLoginService instagramLoginService)
        {
            _logger = logger;
            _lineLoginModel = LineLoginModel.Value;
            _lineLoginService = lineLoginService;
            _googleLoginModel = GoogleLoginModel.Value;
            _googleLoginService = googleLoginService;
            _faceBookLoginModel = FaceBookLoginModel.Value;
            _faceBookLoginService = faceBookLoginService;
            _twitterLoginModel = TwitterLoginModel.Value;
            _twitterLoginService = twitterLoginService;
            _dropboxLoginModel = DropboxLoginModel.Value;
            _dropboxLoginService = dropboxLoginService;
            _instagramLoginModel = InstagramLoginModel.Value;
            _instagramLoginService = instagramLoginService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult InstagramLogin()
        {
            var clientId = _instagramLoginModel.ClientId;
            var redirectUri = _instagramLoginModel.RedirectUri;
            var scope = _instagramLoginModel.Scope;
            string state = "123123"; //state 要額外處理 避免 CSRF 攻擊
            var url = _instagramLoginService.GenerateInstagramLoginUrl(clientId, redirectUri, state, scope);
            return Redirect(url);
        }


        public async Task<IActionResult> InstagramCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            // 透過 code 取得 access token
            var accessToken = await _instagramLoginService.GetAccessTokenAsync(code, _instagramLoginModel.ClientId, _instagramLoginModel.ClientSecret, _instagramLoginModel.RedirectUri);

            var userProfile = await _instagramLoginService.GetUserProfile(accessToken.AccessToken);
            TempData["Email"] = "";
            TempData["UserId"] = userProfile.id;
            TempData["DisplayName"] = userProfile.username;
            TempData["PictureUrl"] = "";
            TempData["LoginType"] = "Instagram";

            return RedirectToAction("UserInfo");
        }


        public IActionResult DropboxLogin()
        {
            var clientId = _dropboxLoginModel.ClientId;
            var redirectUri = _dropboxLoginModel.RedirectUri;
            var scope = _dropboxLoginModel.Scope;
            string state = "123123"; //state 要額外處理 避免 CSRF 攻擊
            var url = _dropboxLoginService.GenerateDropboxLoginUrl(clientId, redirectUri, state, scope);
            return Redirect(url);
        }

        public async Task<IActionResult> DropboxCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            // 透過 code 取得 access token
            var accessToken = await _dropboxLoginService.GetAccessTokenAsync(code, _dropboxLoginModel.ClientId, _dropboxLoginModel.ClientSecret, _dropboxLoginModel.RedirectUri);
           
            //取得user 資料
            var userProfile = await _dropboxLoginService.GetUserProfile(accessToken.AccessToken);
            TempData["Email"] = userProfile.email;
            TempData["UserId"] = userProfile.account_id;
            TempData["DisplayName"] = userProfile.name.given_name + userProfile.name.surname;
            TempData["PictureUrl"] = userProfile.profile_photo_url;
            TempData["LoginType"] = "Dropbox";
            return RedirectToAction("UserInfo");
        }


        #region Twitter
        public IActionResult TwitterLogin()
        {
            var clientId = _twitterLoginModel.ClientId;
            var redirectUri = _twitterLoginModel.RedirectUri;
            var scope = _twitterLoginModel.Scope;
            string state = "123123"; //state 要額外處理 避免 CSRF 攻擊
            var url = _twitterLoginService.GenerateTwitterLoginUrl(clientId, redirectUri, state, scope);
            return Redirect(url);
        }
        public async Task<IActionResult> TwitterCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }
            // 透過 code 取得 access token
            var accessToken = await _twitterLoginService.GetAccessTokenAsync(code, _twitterLoginModel.ClientId, _twitterLoginModel.ClientSecret, _twitterLoginModel.RedirectUri);
            //取得user 資料
            var userProfile = await _twitterLoginService.GetUserProfile(accessToken.AccessToken);
            var profileImageUrl = userProfile.data.profile_image_url.Replace("_normal", "_bigger");
            TempData["Email"] = "未提供";
            TempData["UserId"] = userProfile.data.id;
            TempData["DisplayName"] = userProfile.data.name;
            TempData["PictureUrl"] = profileImageUrl;
            TempData["LoginType"] = "Twitter";
            return RedirectToAction("UserInfo");
        }
        #endregion

        #region FaceBook
        public IActionResult FaceBookLogin()
        {
            var clientId = _faceBookLoginModel.ClientId;
            var redirectUri = _faceBookLoginModel.RedirectUri;
            var scope = _faceBookLoginModel.Scope;
            string state = "123123"; //state 要額外處理 避免 CSRF 攻擊
            var url = _faceBookLoginService.GenerateFaceBookLoginUrl(clientId, redirectUri, state, scope);
            return Redirect(url);
        }

        public async Task<IActionResult> FaceBookCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            // 透過 code 取得 access token
            var accessToken = await _faceBookLoginService.GetAccessTokenAsync(code, _faceBookLoginModel.ClientId, _faceBookLoginModel.ClientSecret, _faceBookLoginModel.RedirectUri);
            //取得user 資料
            var userProfile = await _faceBookLoginService.GetUserProfile(accessToken.AccessToken);


            TempData["Email"] = userProfile.email;
            TempData["UserId"] = userProfile.id;
            TempData["DisplayName"] = userProfile.name;
            TempData["PictureUrl"] =  userProfile.picture.data.url;

            TempData["LoginType"] = "FaceBook";

            return RedirectToAction("UserInfo");
        }
        #endregion

        #region Google
        public IActionResult GoogleLogin()
        {
            var clientId = _googleLoginModel.ClientId;
            var redirectUri = _googleLoginModel.RedirectUri;
            var scope = _googleLoginModel.Scope;
            string state = "123123"; //state 要額外處理 避免 CSRF 攻擊
            var url = _googleLoginService.GenerateGoogleLoginUrl(clientId, redirectUri, state, scope);
            return Redirect(url);
        }

        public async Task<IActionResult> GoogleCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }


            // 透過 code 取得 access token
            var accessToken = await _googleLoginService.GetAccessTokenAsync(code, _googleLoginModel.ClientId, _googleLoginModel.ClientSecret, _googleLoginModel.RedirectUri);
            // 驗證 GoogleLogin 的 access token
            GoogleLoginVerifyAccessTokenResult accessTokenVerifyResult = null;
            try
            {
                accessTokenVerifyResult = await _googleLoginService.VerifyAccessTokenAsync(accessToken.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }

            // 驗證 GoogleLogin 的 id token
            GoogleLoginVerifyIdTokenResult idTokenVerifyResult = null;
            try
            {
                idTokenVerifyResult = await _googleLoginService.VerifyIdTokenAsync(accessToken.IdToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }

            TempData["Email"] = idTokenVerifyResult.Email;
            TempData["UserId"] = idTokenVerifyResult.Sub;
            TempData["DisplayName"] = idTokenVerifyResult.Name;
            TempData["PictureUrl"] = idTokenVerifyResult.Picture;

            TempData["LoginType"] = "Google";

            return RedirectToAction("UserInfo");
        }
        #endregion

        #region LINE
        public IActionResult LineLogin()
        {
            var clientId = _lineLoginModel.ClientId;
            var redirectUri = _lineLoginModel.RedirectUri;
            var scope = _lineLoginModel.Scope;
            string state = "123123"; //state 要額外處理 避免 CSRF 攻擊

            var url = _lineLoginService.GenerateLineLoginUrl(clientId, redirectUri, state, scope);
            return Redirect(url);
        }

        public async Task<IActionResult> LineLoginCallback([FromQuery(Name = "code")] string code, [FromQuery(Name = "state")] string state)
        {
            if (String.IsNullOrWhiteSpace(code))
            {
                return BadRequest();
            }

            // 驗證 state 簽章自行實作

            // 透過 code 取得 access token
            var accessToken = await _lineLoginService.GetAccessTokenAsync(code, _lineLoginModel.ClientId, _lineLoginModel.ClientSecret, _lineLoginModel.RedirectUri);

            // 驗證 LineLogin 的 access token
            LineLoginVerifyAccessTokenResult accessTokenVerifyResult = null;
            try
            {
                accessTokenVerifyResult = await _lineLoginService.VerifyAccessTokenAsync(accessToken.AccessToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }

            // 驗證 LineLogin 的 id token
            LineLoginVerifyIdTokenResult idTokenVerifyResult = null;
            try
            {
                idTokenVerifyResult = await _lineLoginService.VerifyIdTokenAsync(accessToken.IdToken, _lineLoginModel.ClientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return View();
            }

            // 取得目前的 user profile
            var user = await _lineLoginService.GetUserProfileAsync(accessToken.AccessToken);
            TempData["Email"] = idTokenVerifyResult.Email;
            TempData["UserId"] = idTokenVerifyResult.Sub;
            TempData["DisplayName"] = user.DisplayName;
            TempData["PictureUrl"] = user.PictureUrl;

            TempData["LoginType"] = "Line";
            return RedirectToAction("UserInfo");
        }

        #endregion

        public async Task<IActionResult> UserInfo()
        {
            ViewBag.LoginType = TempData["LoginType"];

            var userInfo = new UserInfo
            {
                email = TempData["Email"].ToString(),
                userId = TempData["UserId"].ToString(),
                displayName = TempData["DisplayName"].ToString(),
                pictureUrl = TempData["PictureUrl"].ToString()
            };
            
            return View(userInfo);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}