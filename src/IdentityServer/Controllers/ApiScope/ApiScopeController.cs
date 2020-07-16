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

        public async  Task<IActionResult> Index()
        {
            var scopes = await ApiScopeService.GetAllScopesAsync();
            return View(scopes);
        }
    }
}