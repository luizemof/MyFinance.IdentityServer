using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer.Repository.IdentityResource
{
    public class IdentityResourceData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        [BsonRequired]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public bool Enabled { get; set; }

        public ICollection<string> UserClaims { get; set; } = new List<string>();

        [BsonConstructor]
        public IdentityResourceData(string id, string name, string displayName, string description, ICollection<string> userClaims, bool enabled)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            Description = description;
            UserClaims = userClaims;
            Enabled = enabled;
        }

        public IdentityResourceData(string id, string name, string displayName, string description, ICollection<string> userClaims) 
        : this(id, name, displayName, description, userClaims, enabled: true)
        { }
    }
}