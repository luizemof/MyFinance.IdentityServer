using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{
    public class UserInputModel
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }
        
        [Required]
        public string PasswordConfirmation { get; set; }
    }
}