using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Constants;
using IdentityServer.Models.Users;
using IdentityServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Users
{
    // [Authorize]
    [Route(ControllerConstants.USERS_CONTROLLER)]
    public class UsersController : Controller
    {
        private readonly IUserService UserService;
        
        public UsersController(IUserService userService)
        {
            UserService = userService ?? throw new ArgumentNullException(nameof(userService));
        }
        
        [HttpGet, ActionName("Index")]
        public async Task<IActionResult> Index()
        {
            var users = await UserService.GetUsersAsync();
            return View(users);
        }

        [HttpGet("Edit"), ActionName("Edit")]
        public IActionResult Edit(string id)
        {
            return View("Edit", new UserInputModel());
        }

        [HttpPost, ActionName("Save")]
        public IActionResult Save(UserInputModel userInputModel, string button)
        {
            if (button == ControllerConstants.SAVE)
            {
                var id = string.IsNullOrWhiteSpace(userInputModel.Id) ? Guid.NewGuid().ToString() : userInputModel.Id;
            }

            return Redirect(ControllerConstants.USERS_CONTROLLER);
        }
    }
}