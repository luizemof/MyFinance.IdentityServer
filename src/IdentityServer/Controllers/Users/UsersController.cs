using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Exceptions;
using IdentityServer.Models.Users;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Users
{
    [Authorize(Roles = Roles.ADMIN)]
    public class UsersController : Controller
    {
        public class RolesOperations
        {
            [BindProperty(Name=ControllerConstants.BUTTON)]
            public string Button { get; set; }

            [BindProperty(Name="listValue")]
            public string ListValue { get; set; }
            
            [BindProperty(Name="newRole")]
            public string NewRole { get; set; }

            public string RoleValue
            {
                get
                {
                    return string.IsNullOrWhiteSpace(ListValue) ? NewRole : ListValue;
                }
            }
        }

        public const string ROLES = "Roles";

        private readonly IUserService UserService;

        public UsersController(IUserService userService)
        {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        public async Task<IActionResult> Index()
        {
            var userTasks = UserService.GetUsersAsync();
            var inactiveUsersTask = UserService.GetInactiveUsersAsync();
            var taksResult = await Task.WhenAll(new[] { userTasks, inactiveUsersTask });
            var users = taksResult[0].Concat(taksResult[1]);

            return View(users);
        }

        public async Task<IActionResult> Edit(string id)
        {
            var userInputModel = new UserInputModel();
            if (!string.IsNullOrWhiteSpace(id))
            {
                var user = await UserService.GetUserAsync(id);
                userInputModel = user.ToInputModel();
            }

            return await ReturnEditView(userInputModel);
        }

        [HttpPost]
        public Task<IActionResult> Edit(UserInputModel userInputModel, RolesOperations rolesOperations)
        {
            var button = rolesOperations.Button;
            if (button == ControllerConstants.CANCEL)
            {
                return Task.FromResult((IActionResult)RedirectToAction(nameof(Index)));
            }
            else if (button == ControllerConstants.SAVE)
            {
                return HandleWithSave(userInputModel);
            }
            else
                return HandleWithRoles(userInputModel, rolesOperations);
        }

        private async Task<IActionResult> HandleWithSave(UserInputModel userInputModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(userInputModel.Id))
                        await UserService.CreateUserAsync(userInputModel);
                    else
                        await UserService.UpdateUserAsync(userInputModel);

                    return RedirectToAction(nameof(Index));
                }
                catch (AlreadyExistsException ex)
                {
                    ModelState.Merge(ex.ModelStateDictionary);
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(ControllerConstants.ERROR, ex.Message);
                }
            }

            return await ReturnEditView(userInputModel);
        }

        private Task<IActionResult> HandleWithRoles(UserInputModel userInputModel, RolesOperations rolesOperations)
        {
            var button = rolesOperations.Button;
            var roleValue = rolesOperations.RoleValue;

            if (userInputModel.Roles == null)
                userInputModel.Roles = new List<string>();

            if (button.Contains(ControllerConstants.ADD_TO_LIST))
                userInputModel.Roles.Add(roleValue);
            else if (button.Contains(ControllerConstants.REMOVE_FROM_LIST))
                userInputModel.Roles.Remove(roleValue);


            ModelState.Clear();
            return ReturnEditView(userInputModel);
        }

        private async Task<IActionResult> ReturnEditView(UserInputModel userInputModel)
        {
            await SetRolesToViewBag(userInputModel);
            return View(ControllerConstants.EDIT, userInputModel);
        }

        private async Task SetRolesToViewBag(UserInputModel userInputModel)
        {
            var loadedRoles = await this.UserService.GetRolesAsync();
            ViewBag.Roles = loadedRoles.Except(userInputModel.Roles ?? Enumerable.Empty<string>());
        }

        [ActionName("Deactivate")]
        public async Task<IActionResult> DeactivateReactivate(string id, bool isActive)
        {
            if (isActive)
                await UserService.DeactiveUserAsync(id);
            else
                await UserService.ReactiveUserAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}