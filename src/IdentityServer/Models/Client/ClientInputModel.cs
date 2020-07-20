using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Client
{
    public class ClientInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O id do client é obrigatório.")]
        public string ClientId { get; set; }

        [Required(ErrorMessage = "A chave do cliente é obrigatória.")]
        public string ClientSecret { get; set; }

        [Required(ErrorMessage = "O nome do cliente é obrigatório.")]
        public string ClientName { get; set; }

        public List<string> AllowedGrantTypes { get; set; } = new List<string>();
        
        public List<string> AllowedScopes { get; set; } = new List<string>();

        public string Description { get; set; }

        public string RedirectUrl { get; set; }

        public string PostLogoutRedirectUrl { get; set; }
    }
}