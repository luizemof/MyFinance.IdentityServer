using System.Collections.Generic;
using System.Linq;
using IdentityServer4.Models;
using IdentityServerClient = IdentityServer4.Models.Client;
namespace IdentityServer.Models.Client
{
    public class ClientModel : IdentityServerClient
    {
        public ClientModel()
        { }

        public ClientModel(string id, string decryptedSecret)
        {
            this.Id = id;
            this.DecryptedSecret = decryptedSecret;
        }

        public string Id { get; }

        private string _DecryptedSecret;
        public string DecryptedSecret
        {
            get { return _DecryptedSecret; }
            private set
            {
                _DecryptedSecret = value;
                if (!string.IsNullOrWhiteSpace(_DecryptedSecret))
                {
                    this.ClientSecrets = new List<Secret>() { new Secret(_DecryptedSecret.Sha256()) };
                    this.RequireClientSecret = true;
                }
                else
                {
                    this.ClientSecrets = null;
                    this.RequireClientSecret = false;
                }
            }
        }
    }

    public static class ClientExtensions
    {
        public static ClientInputModel ToInputModel(this ClientModel model)
        {
            var inputModel = new ClientInputModel();
            if (model != null)
            {
                inputModel = new ClientInputModel()
                {
                    Id = model.Id,
                    ClientId = model.ClientId,
                    ClientName = model.ClientName,
                    ClientSecret = model.DecryptedSecret,
                    Description = model.Description,
                    AllowedGrantTypes = model.AllowedGrantTypes.ToList() ?? new List<string>(),
                    AllowedScopes = model.AllowedScopes.ToList() ?? new List<string>(),
                    RedirectUrl = model.RedirectUris?.FirstOrDefault(),
                    PostLogoutRedirectUrl = model.PostLogoutRedirectUris?.FirstOrDefault()
                };
            }

            return inputModel;
        }
    }
}