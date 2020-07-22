using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using IdentityServer.Cryptography;
using IdentityServer.Repository.Client;
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

    public static class ClientInputModelExtension
    {
        public static ClientData ToData(this ClientInputModel inputModel, IdentityServerCryptography cryptography)
        {
            var data = new ClientData();
            if (inputModel != null)
            {
                data = new ClientData()
                {
                    Id = inputModel.Id,
                    ClientId = inputModel.ClientId,
                    ClientSecret = cryptography.Encrypt(inputModel.ClientSecret),
                    AllowedGrantTypes = inputModel.AllowedGrantTypes,
                    AllowedScopes = inputModel.AllowedScopes,
                    RedirectUrl = inputModel.RedirectUrl,
                    PostLogoutRedirectUrl = inputModel.PostLogoutRedirectUrl,
                    ClientName = inputModel.ClientName,
                    Description = inputModel.Description
                };
            }

            return data;
        }
    }
}