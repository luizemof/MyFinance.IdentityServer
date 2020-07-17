using IdentityServer.Cryptography;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;
using IdentityServer4.Models;

namespace IdentityServer.Extensions
{
    public static class ClientExtensions
    {
        public static ClientData ToData(this ClientInputModel model, IdentityServerCryptography cryptography)
        {
            var data = new ClientData()
            {
                Id = model.Id,
                ClientId = model.ClientId,
                ClientSecret = cryptography.Encrypt(model.ClientSecret),
                AllowedGrantTypes = model.AllowedGrantTypes,
                AllowedScopes = model.AllowedScopes,
                RedirectUrl = model.RedirectUrl,
                PostLogoutRedirectUrl = model.PostLogoutRedirectUrl,
                ClientName = model.ClientName,
                Description = model.Description
            };

            return data;
        }

        public static ClientModel ToModel(this ClientData data, IdentityServerCryptography cryptography)
        {
            return new ClientModel()
            {
                Id = data.Id,
                ClientId = data.ClientId,
                ClientSecrets = { new Secret(cryptography.Decrypt(data.ClientSecret).Sha256()) },
                AllowedGrantTypes = data.AllowedGrantTypes,
                AllowedScopes = data.AllowedScopes,
                PostLogoutRedirectUris = { data.PostLogoutRedirectUrl },
                RedirectUris = { data.RedirectUrl },
                Enabled = data.Enabled,
                Description = data.Description,
                ClientName = data.ClientName
            };
        }
    }
}