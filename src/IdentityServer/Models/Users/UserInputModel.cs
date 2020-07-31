using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using IdentityServer.Constants;
using IdentityServer.Cryptography;
using IdentityServer.Repository.Users;

namespace IdentityServer.Models.Users
{
    public class UserInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Email is required")]
        public string Email { get; set; }

        [Required(ErrorMessage = "The Password is required")]
        public string Password { get; set; }

        [Required(ErrorMessage = "The Password Confirmation is required")]
        [Compare(nameof(Password), ErrorMessage = "Passwords don't match.")]
        public string PasswordConfirmation { get; set; }

        public List<string> Roles { get; set; } = new List<string>();
    }

    public static class UserExtension
    {
        public static UserData ToData(this UserInputModel userInputModel, IdentityServerCryptography cryptography)
        {
            var password = cryptography.Encrypt(userInputModel.Password);
            return new UserData(userInputModel.Id, userInputModel.Name, userInputModel.Email, password, userInputModel.Roles);
        }
    }
}