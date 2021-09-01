using Mao.Web.ApiActions.User;
using MediatR;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Mao.Web.Controllers
{
    public class UserController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IHttpClientFactory _httpClientFactory;
        public UserController(IMediator mediator, IHttpClientFactory httpClientFactory)
        {
            _mediator = mediator;
            _httpClientFactory = httpClientFactory;
        }

        // GET: User
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        /// <summary>
        /// 登入後的導向頁面
        /// </summary>
        private ActionResult LoginDefaultRedirect() => RedirectToAction("Index", "Home");
        /// <summary>
        /// 登出後的導向頁面
        /// </summary>
        private ActionResult LogoutDefaultRedirect() => RedirectToAction("Index", "Home");

        /// <summary>
        /// 登入
        /// </summary>
        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }
                return LoginDefaultRedirect();
            }
            return View();
        }
        /// <summary>
        /// 登入
        /// </summary>
        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(string account, string password, string returnUrl)
        {
            #region 驗證並取得 token
            var validateUserAccount = await _mediator.Send(new ValidateUserAccount.Request()
            {
                Account = account,
                Password = password
            });
            #endregion
            #region 取得 claims
            var getUserClaims = await _mediator.Send(new GetUserClaims.Request()
            {
                UserId = validateUserAccount.UserId
            });
            #endregion
            if (!getUserClaims.IsValid)
            {
                // 驗證失敗
                ViewBag.NoticeMessage = "登入失敗";
                ViewBag.NoticeStyle = "danger";
                return View();
            }
            // 設置 Claims
            var claimsIdentity = new ClaimsIdentity(getUserClaims.Claims, DefaultAuthenticationTypes.ApplicationCookie);
            // 把 token 加進 Claims
            claimsIdentity.AddClaim(new Claim($"{ClaimTypes.UserData}/token", validateUserAccount.Token));
            // 登入
            HttpContext.GetOwinContext().Authentication
                .SignIn(new AuthenticationProperties(new Dictionary<string, string>
                {
                    { "UserId", claimsIdentity.GetClaimValue(ClaimTypes.Sid) },
                    { "Account", claimsIdentity.GetClaimValue(ClaimTypes.NameIdentifier) },
                    { "Name", claimsIdentity.GetClaimValue(ClaimTypes.Name) }
                }), claimsIdentity);
            // 導頁
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return LoginDefaultRedirect();
        }

        /// <summary>
        /// 登出
        /// </summary>
        public ActionResult Logout(string returnUrl, bool relogin = false)
        {
            HttpContext.GetOwinContext().Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            if (relogin)
            {
                return RedirectToAction("Login", new { returnUrl = returnUrl });
            }
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return LogoutDefaultRedirect();
        }

        public ActionResult DatabaseList()
        {
            return View();
        }
    }
}