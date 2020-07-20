using System.Collections.Generic;
using System.Linq;
using IdentityServer.Cryptography;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IdentityServer.Extensions
{
    public static class ClientExtensions
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

        public static ClientModel ToModel(this ClientData data, IdentityServerCryptography cryptography)
        {
            var model = new ClientModel();
            if (data != null)
            {
                model = new ClientModel()
                {
                    Id = data.Id,
                    ClientId = data.ClientId,
                    ClientSecrets = { new Secret(cryptography.Decrypt(data.ClientSecret).Sha256()) },
                    AllowedGrantTypes = data.AllowedGrantTypes.ToList(),
                    AllowedScopes = data.AllowedScopes.ToList(),
                    PostLogoutRedirectUris = { data.PostLogoutRedirectUrl },
                    RedirectUris = { data.RedirectUrl },
                    Enabled = data.Enabled,
                    Description = data.Description,
                    ClientName = data.ClientName
                };
            }

            return model;
        }

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
                    ClientSecret = model.ClientSecrets?.SingleOrDefault()?.Value,
                    Description = model.Description,
                    AllowedGrantTypes = model.AllowedGrantTypes.ToList() ?? new List<string>(),
                    AllowedScopes = model.AllowedScopes.ToList() ?? new List<string>()
                };
            }

            return inputModel;
        }
    }
}