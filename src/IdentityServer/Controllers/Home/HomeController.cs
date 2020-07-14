using IdentityServer.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Home
{
    [AllowAnonymous]
    [Route(ControllerConstants.HOME_CONTROLLER)]
    [Route("")]
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}