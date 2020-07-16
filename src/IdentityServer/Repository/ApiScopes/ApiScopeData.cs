using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer.Repository.ApiScopes
{
    public class ApiScopeData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonRequired]
        public string Name { get; set; }

        [BsonRequired]
        public string DisplayName { get; set; }

        [BsonDefaultValue(true)]
        public bool Enabled { get; set; }

        public string Description { get; set; }

        [BsonConstructor]
        public ApiScopeData(string id, string name, string displayName, string description, bool enabled)
        {
            Id = id;
            Name = name;
            DisplayName = displayName;
            Description = description;
            Enabled = enabled;
        }

        public ApiScopeData(string name, string displayName, string description) : this(id: string.Empty, name, displayName, description, enabled: true)
        { }
    }
}