using System.Collections.Generic;
using System.Linq;
using IdentityServer.Cryptography;
using IdentityServer.Models.Client;
using IdentityServer4.Models;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer.Repository.Client
{
    public class ClientData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string ClientId { get; set; }

        [BsonRequired]
        public string ClientName { get; set; }

        [BsonRequired]
        public ICollection<string> AllowedGrantTypes { get; set; }

        public string ClientSecret { get; set; }
        
        public string Description { get; set; }

        public bool Enabled { get; set; } = true;

        public string RedirectUrl { get; set; }

        public string PostLogoutRedirectUrl { get; set; }

        public ICollection<string> AllowedScopes { get; set; }
    }

    public static class ClientExtensions
    {
        public static ClientModel ToModel(this ClientData data, IdentityServerCryptography cryptography)
        {
            var model = new ClientModel();
            if (data != null)
            {
                model = new ClientModel(data.Id, cryptography.Decrypt(data.ClientSecret))
                {
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
    }
}