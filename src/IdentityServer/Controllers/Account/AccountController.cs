using IdentityServer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Account
{
    [AllowAnonymous]
    [Route(ControllerConstants.ACCOUNT_CONTROLLER)]
    public class AccountController : Controller
    {
        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            return View();
        }
    }
}