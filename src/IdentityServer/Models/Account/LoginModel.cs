using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Account
{
    public class LoginModel
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        public string RedirectURL { get; set; }

        public bool Remember { get; set; }
    }
}