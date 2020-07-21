using System;
using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Models.Account;
using IdentityServer.Services;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using IdentityServer4.Services;

namespace IdentityServer.Controllers.Account
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        private readonly IAccountService AccountService;
        private readonly IUserService UserService;
        private readonly IIdentityServerInteractionService InteractionService;
        private readonly IHttpContextWrapper HttpContextWrapper;

        public AccountController(IAccountService accountService,
            IUserService userService,
            IIdentityServerInteractionService interaction,
            IHttpContextWrapper httpContextWrapper)
        {
            AccountService = accountService ?? throw new System.ArgumentNullException(nameof(accountService));
            UserService = userService ?? throw new System.ArgumentNullException(nameof(userService));
            InteractionService = interaction ?? throw new ArgumentNullException(nameof(interaction));
            HttpContextWrapper = httpContextWrapper ?? throw new ArgumentNullException(nameof(httpContextWrapper));
        }

        [ActionName(ControllerConstants.LOGIN)]
        public IActionResult Login(string returnUrl)
        {
            return View(new LoginModel() { RedirectURL = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginModel loginModel)
        {

            if (ModelState.IsValid)
            {
                var validCredentials = await AccountService.ValidateCredentials(loginModel);
                if (validCredentials)
                {
                    // check if we are in the context of an authorization request
                    var context = await InteractionService.GetAuthorizationContextAsync(loginModel.RedirectURL);
                    var user = await UserService.GetUserByEmail(loginModel.Email);

                    // only set explicit expiration here if user chooses "remember me". 
                    // otherwise we rely upon expiration configured in cookie middleware.
                    var props = default(AuthenticationProperties);
                    if (loginModel.Remember)
                    {
                        props = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.Add(TimeSpan.FromDays(30))
                        };
                    }

                    // issue authentication cookie with subject ID and username
                    var isuser = new IdentityServerUser(user.Id)
                    {
                        DisplayName = user.Name
                    };

                    await HttpContextWrapper.SignInAsync(HttpContext, isuser, props);

                    if (context != null)
                    {
                        // if (context.IsNativeClient())
                        // {
                        // The client is native, so this change in how to
                        // return the response is for better UX for the end user.
                        // return this.LoadingPage("Redirect", model.ReturnUrl);
                        //}

                        // we can trust model.ReturnUrl since GetAuthorizationContextAsync returned non-null
                        return Redirect(loginModel.RedirectURL);
                    }

                    // request for a local page
                    if (Url.IsLocalUrl(loginModel.RedirectURL))
                    {
                        return Redirect(loginModel.RedirectURL);
                    }
                    else if (string.IsNullOrEmpty(loginModel.RedirectURL))
                    {
                        return Redirect("~/");
                    }
                    else
                    {
                        // user might have clicked on a malicious link - should be logged
                        throw new Exception("invalid return URL");
                    }
                }
            }

            return View(ControllerConstants.LOGIN, loginModel);
        }
    }
}