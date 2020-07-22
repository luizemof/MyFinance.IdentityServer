using IdentityServer.Constants;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Home
{
    [AllowAnonymous]
    public class HomeController : Controller
    {

        //
        // Get: Home/
        public IActionResult Index()
        {
            return View();
        }
    }
}