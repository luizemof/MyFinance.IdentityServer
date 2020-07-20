using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Client
{
    public class ClientInputModel
    {
        public string Id { get; set; }

        [Required]
        public string ClientId { get; set; }

        [Required]
        public string ClientSecret { get; set; }

        [Required]
        public string ClientName { get; set; }

        [Required]
        public List<string> AllowedGrantTypes { get; set; } = new List<string>();

        public string Description { get; set; }

        public string RedirectUrl { get; set; }

        public string PostLogoutRedirectUrl { get; set; }

        public List<string> AllowedScopes { get; set; } = new List<string>();
    }
}