using IdentityServer.Cryptography;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;

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
                AllowedScopes = model.AllowedGrantTypes,
                RedirectUrl = model.RedirectUrl,
                PostLogoutRedirectUrl = model.PostLogoutRedirectUrl
            };

            return data;
        }
    }
}