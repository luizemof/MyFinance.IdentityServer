using System.Threading.Tasks;
using IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Scopes
{
    public class ApiScopeController : Controller
    {
        private readonly IApiScopeService ApiScopeService;

        public ApiScopeController(IApiScopeService apiScopeService)
        {
            ApiScopeService = apiScopeService;
        }

        public async Task<IActionResult> Index()
        {
            var scopes = await ApiScopeService.GetAllScopesAsync();
            return View(scopes);
        }

        public async Task<IActionResult> Enabled(string id, bool isEnabled)
        {
            if (!isEnabled)
                await ApiScopeService.EnableApiScopeAsync(id);
            else
                await ApiScopeService.DisableApiScopeAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}