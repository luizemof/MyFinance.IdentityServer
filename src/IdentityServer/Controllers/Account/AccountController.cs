using IdentityServer.Constants;
using IdentityServer.Models.Account;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Account
{
    [AllowAnonymous]
    public class AccountController : Controller
    {
        public IActionResult Login(string returnUrl)
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(LoginModel loginModel)
        {
            return View();
        }
    }
}