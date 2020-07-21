using System;
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
    //[Authorize]
    public class UsersController : Controller
    {
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
                userInputModel.Id = user.Id;
                userInputModel.Email = user.Email;
                userInputModel.Name = user.Name;
            }

            return View(userInputModel);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserInputModel userInputModel, string button)
        {
            if (button == ControllerConstants.CANCEL)
                return RedirectToAction(nameof(Index));

            if (ModelState.IsValid)
            {
                try
                {

                    if (button == ControllerConstants.SAVE)
                    {
                        if (string.IsNullOrWhiteSpace(userInputModel.Id))
                            await UserService.CreateUserAsync(userInputModel);
                        else
                            await UserService.UpdateUserAsync(userInputModel);
                    }

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

            return View(userInputModel);
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