using IdentityServer.Constants;
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
    }
}