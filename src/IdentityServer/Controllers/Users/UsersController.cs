using System;
using System.Collections.Generic;
using IdentityServer.Constants;
using IdentityServer.Models.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace IdentityServer.Controllers.Users
{
    // [Authorize]
    [Route(ControllerConstants.USERS_CONTROLLER)]
    public class UsersController : Controller
    {
        private static Dictionary<string, UserModel> Models = new Dictionary<string, UserModel>()
        {
            { "1", new UserModel(){ Id="1", Name = "Luiz", Email="luiz.emof@gmail.com", IsActive = true }},
            { "2", new UserModel(){ Id="2", Name = "May", Email="may.veriss@gmail.com", IsActive = false }}
        };

        [HttpGet(), ActionName("Index")]
        public IActionResult Index()
        {
            return View(Models.Values);
        }

        [HttpGet("Edit"), ActionName("Edit")]
        public IActionResult Edit(string id)
        {
            var inputUserModel = new UserInputModel();
            if (!string.IsNullOrWhiteSpace(id) && Models.TryGetValue(id, out UserModel user))
            {
                inputUserModel.Id = user.Id;
                inputUserModel.Name = user.Name;
                inputUserModel.Email = user.Email;
            };

            return View("Edit", inputUserModel);
        }

        [HttpPost, ActionName("Save")]
        public IActionResult Save(UserInputModel userInputModel, string button)
        {
            if (button == ControllerConstants.SAVE)
            {
                if (!string.IsNullOrWhiteSpace(userInputModel.Id) && Models.TryGetValue(userInputModel.Id, out UserModel user))
                {
                    user.Name = userInputModel.Name;
                    user.Email = userInputModel.Email;
                    user.IsActive = true;
                }
                else
                {
                    string id = Guid.NewGuid().ToString();
                    Models.Add(id, new UserModel() 
                    { 
                        Id=id, 
                        Email = userInputModel.Email, 
                        Name = userInputModel.Name,
                        IsActive = true
                    });
                }
            }

            return Redirect(ControllerConstants.USERS_CONTROLLER);
        }
    }
}