using System.Threading.Tasks;
using IdentityServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.IdentityResource
{
    public class IdentityResourceController : Controller
    {
        private readonly IIdentityResourceService IdentityResourceService;
        public IdentityResourceController(IIdentityResourceService identityResourceService)
        {
            IdentityResourceService = identityResourceService ?? throw new System.ArgumentNullException(nameof(identityResourceService));
        }

        public async Task<IActionResult> Index()
        {
            var models = await IdentityResourceService.GetAllIdentityResources();
            return View(models);
        }
    }
}