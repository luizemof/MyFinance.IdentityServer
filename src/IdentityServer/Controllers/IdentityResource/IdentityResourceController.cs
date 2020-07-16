using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Extensions;
using IdentityServer.Models.IdentityResource;
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

        public async Task<IActionResult> Edit(string id)
        {
            var inputModel = new IdentityResourceInputModel();
            if(!string.IsNullOrWhiteSpace(id))
            {
                var model = await IdentityResourceService.GetIdentityResourceById(id);
                inputModel = model.ToInputModel();
            }

            return View(inputModel);
        }

        [HttpPost]    
        public async Task<IActionResult> Edit(IdentityResourceInputModel input, string button)
        {
            if(button == ControllerConstants.CANCEL)
                return RedirectToAction(nameof(Index));

            if(ModelState.IsValid)
            {
                await IdentityResourceService.UpsertIdentityResource(input);
                return RedirectToAction(nameof(Index));
            }

            return View(input);
        }
    }
}