using System;
using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Exceptions;
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
            if (!string.IsNullOrWhiteSpace(id))
            {
                var model = await IdentityResourceService.GetIdentityResourceById(id);
                inputModel = model.ToInputModel();
            }

            return View(inputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(IdentityResourceInputModel input, string button)
        {
            if (button == ControllerConstants.CANCEL)
                return RedirectToAction(nameof(Index));

            try
            {
                if (ModelState.IsValid)
                {
                    await IdentityResourceService.UpsertIdentityResource(input);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch(AlreadyExistsException ex)
            {
                ModelState.Merge(ex.ModelStateDictionary);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(ControllerConstants.ERROR, ex.Message);
            }

            return View(input);
        }

        public async Task<IActionResult> Enabled(string id, bool isEnabled)
        {
            if (isEnabled)
                await IdentityResourceService.Disable(id);
            else
                await IdentityResourceService.Enable(id);

            return RedirectToAction(nameof(Index));
        }
    }
}