using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Models.ApiScope;
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
            var scopes = await ApiScopeService.GetAllApiScopesAsync();
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

        [ActionName(ControllerConstants.EDIT)]
        public async Task<IActionResult> Edit(string id)
        {
            var apiScopeInputModel = new ApiScopeInputModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var scope = await ApiScopeService.GetApiScopeById(id);
                apiScopeInputModel.Id = scope.Id;
                apiScopeInputModel.Name = scope.Name;
                apiScopeInputModel.DisplayName = scope.DisplayName;
                apiScopeInputModel.Description = scope.Description;
            }

            return View(apiScopeInputModel);
        }

        [ActionName(ControllerConstants.EDIT)]
        [HttpPost]
        public async Task<IActionResult> Edit(ApiScopeInputModel apiScopeInputModel, string button)
        {
            if(button == ControllerConstants.CANCEL)
                return RedirectToAction(nameof(Index));

            if(ModelState.IsValid)
            {
                await ApiScopeService.UpsertApiScopeAsync(apiScopeInputModel);
                return RedirectToAction(nameof(Index));
            }

            return View(apiScopeInputModel);
        }
    }
}