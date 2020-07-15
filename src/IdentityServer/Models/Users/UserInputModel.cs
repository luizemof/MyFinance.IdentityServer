using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{
    public class UserInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O Nome é obrigatório.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O Email é obrigatório.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "A Senha é obrigatória.")]
        public string Password { get; set; }

        [Required(ErrorMessage = "A Confirmação da Senha é obrigatória.")]
        [Compare(nameof(Password), ErrorMessage = "As Senhas não conferem.")]
        public string PasswordConfirmation { get; set; }
    }
}