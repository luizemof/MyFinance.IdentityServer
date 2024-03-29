using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Exceptions;
using IdentityServer.Extensions;
using IdentityServer.Models.Client;
using IdentityServer.Services;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Client
{
    [Authorize(Roles = Roles.ADMIN)]
    public class ClientController : Controller
    {
        public static string SCOPE = "SCOPE";
        public static string GRANT = "GRANT";
        public static string ADD = "ADD";
        public static string REMOVE = "REMOVE";

        private readonly IClientService ClientService;
        private readonly IApiScopeService ApiScopeService;
        private readonly IIdentityResourceService IdentityResourceService;

        public ClientController(IClientService clientService, IApiScopeService apiScopeService, IIdentityResourceService identityResourceService)
        {
            ClientService = clientService ?? throw new System.ArgumentNullException(nameof(clientService));
            ApiScopeService = apiScopeService ?? throw new System.ArgumentNullException(nameof(apiScopeService));
            IdentityResourceService = identityResourceService ?? throw new ArgumentNullException(nameof(identityResourceService));
        }

        public async Task<IActionResult> Index()
        {
            var models = await ClientService.GetAllClientsAsync();
            return View(models);
        }

        [ActionName(ControllerConstants.EDIT)]
        public async Task<IActionResult> Edit(string id)
        {
            var clientModel = default(ClientModel);

            if (!string.IsNullOrWhiteSpace(id))
                clientModel = await this.ClientService.GetClientByInternalIdAsync(id);

            var inputModel = clientModel?.ToInputModel() ?? new ClientInputModel();
            await SetScopeAndGrantToViewBag(inputModel);
            return View(ControllerConstants.EDIT, inputModel);
        }

        [HttpPost]
        public Task<IActionResult> Edit(ClientInputModel inputModel, string button, string listValue)
        {
            if (button == ControllerConstants.CANCEL)
                return Task.FromResult((IActionResult)RedirectToAction(nameof(Index)));

            if (button != ControllerConstants.SAVE)
                return HandleWithListChange(inputModel, button, listValue);
            else
                return HandleWithSave(inputModel);
        }

        private async Task<IActionResult> HandleWithSave(ClientInputModel inputModel)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await ClientService.UpsertClientAsync(inputModel);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (AlreadyExistsException ex)
            {
                ModelState.Merge(ex.ModelStateDictionary);
            }
            catch(ValidationException ex)
            {
                ModelState.Merge(ex.ModelStateDictionary);
            }
            catch(Exception ex)
            {
                ModelState.AddModelError("Error", ex.Message);
            }

            return await ReturnEditView(inputModel);
        }

        private async Task<IActionResult> ReturnEditView(ClientInputModel inputModel)
        {
            await SetScopeAndGrantToViewBag(inputModel);
            return View(ControllerConstants.EDIT, inputModel);
        }

        private Task<IActionResult> HandleWithListChange(ClientInputModel inputModel, string button, string listValue)
        {
            if (button.Contains(GRANT))
                HandleWithGrantListChange(inputModel, button, listValue);
            else if (button.Contains(SCOPE))
                HandleWithScopeListChange(inputModel, button, listValue);

            ModelState.Clear();
            return ReturnEditView(inputModel);
        }

        private void HandleWithGrantListChange(ClientInputModel inputModel, string button, string listValue)
        {
            if (inputModel.AllowedGrantTypes == null)
                inputModel.AllowedGrantTypes = new List<string>();

            if (button.Contains(ADD))
                inputModel.AllowedGrantTypes.Add(listValue);
            else if (button.Contains(REMOVE))
                inputModel.AllowedGrantTypes.Remove(listValue);
        }

        private void HandleWithScopeListChange(ClientInputModel inputModel, string button, string listValue)
        {
            if (inputModel.AllowedScopes == null)
                inputModel.AllowedScopes = new List<string>();

            if (button.Contains(ADD))
                inputModel.AllowedScopes.Add(listValue);
            else if (button.Contains(REMOVE))
                inputModel.AllowedScopes.Remove(listValue);
        }

        private async Task SetScopeAndGrantToViewBag(ClientInputModel inputModel)
        {
            var loadedApiScopes = await this.ApiScopeService.GetAllApiScopesAsync();
            var loadedIdentity = await this.IdentityResourceService.GetAllIdentityResourcesAsync();
            var scopes = loadedApiScopes.Select(apiScope => apiScope.Name).Concat(loadedIdentity.Select(identity => identity.Name)).Distinct();
            var allGrantTypes = GrantTypes.ClientCredentials
                                            .Concat(GrantTypes.Code)
                                            .Concat(GrantTypes.CodeAndClientCredentials)
                                            .Concat(GrantTypes.DeviceFlow)
                                            .Concat(GrantTypes.Hybrid)
                                            .Concat(GrantTypes.HybridAndClientCredentials)
                                            .Concat(GrantTypes.HybridAndClientCredentials)
                                            .Concat(GrantTypes.Implicit)
                                            .Concat(GrantTypes.ImplicitAndClientCredentials)
                                            .Concat(GrantTypes.ResourceOwnerPassword)
                                            .Concat(GrantTypes.ResourceOwnerPasswordAndClientCredentials)
                                            .Distinct();
            ViewBag.Scopes = scopes.Except(inputModel.AllowedScopes ?? Enumerable.Empty<string>());
            ViewBag.GrantTypes = allGrantTypes.Except(inputModel.AllowedGrantTypes ?? Enumerable.Empty<string>());
        }
    }
}