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
        public ICollection<string> AllowedGrantTypes { get; set; }

        public string RedirectUrl { get; set; }

        public string PostLogoutRedirectUrl { get; set; }

        public ICollection<string> AllowedScopes { get; set; }
    }
}