using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer.Services.Client;

namespace IdentityServer.Models.Client
{
    public class ClientInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = ClientValidations.CLIENT_ID_VALIDATION_MESSAGE)]
        public string ClientId { get; set; }

        [Required(ErrorMessage = ClientValidations.CLIENT_NAME_VALIDATION_MESSAGE)]
        public string ClientName { get; set; }
        
        [Required(ErrorMessage = ClientValidations.ALLOWED_GRANT_TYPES_VALIDATION_MESSAGE)]
        public List<string> AllowedGrantTypes { get; set; } = new List<string>();
        
        public List<string> AllowedScopes { get; set; } = new List<string>();

        public string ClientSecret { get; set; }

        public string Description { get; set; }

        public string RedirectUrl { get; set; }

        public string PostLogoutRedirectUrl { get; set; }
    }
}