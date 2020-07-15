using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Scopes
{
    public class ScopesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}