using System.Collections.Generic;
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
        public string ClientSecret { get; set; }

        [BsonRequired]
        public ICollection<string> AllowedGrantTypes { get; set; }

        public bool Enabled { get; set; } = true;

        public string RedirectUrl { get; set; }

        public string PostLogoutRedirectUrl { get; set; }

        public ICollection<string> AllowedScopes { get; set; }

    }
}