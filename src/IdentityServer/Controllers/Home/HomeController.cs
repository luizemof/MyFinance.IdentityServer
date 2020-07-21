using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Models.Home;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;

namespace IdentityServer.Controllers.Home
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IWebHostEnvironment _environment;

        public HomeController(IIdentityServerInteractionService interaction, IWebHostEnvironment environment)
        {
            _interaction = interaction;
            _environment = environment;
        }

        //
        // Get: Home/
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Welcome(string name, int numTimes = 1)
        {
            ViewData["Message"] = "Hello " + name;
            ViewData["NumTimes"] = numTimes;

            return View("Index");
        }

        // public string Welcome()
        // { 
        //     return "Welcome Luiz";
        // }

        // /// <summary>
        // /// Shows the error page
        // /// </summary>
        // [HttpGet("Error")]
        // public async Task<IActionResult> Error(string errorId)
        // {
        //     var vm = new ErrorViewModel();

        //     // retrieve error details from identityserver
        //     var message = await _interaction.GetErrorContextAsync(errorId);
        //     if (message != null)
        //     {
        //         vm.Error = message;

        //         if (!_environment.IsDevelopment())
        //         {
        //             // only show in development
        //             message.ErrorDescription = null;
        //         }
        //     }

        //     return View("Error", vm);
        // }
    }
}