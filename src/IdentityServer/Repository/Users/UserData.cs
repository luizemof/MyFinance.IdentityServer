using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IdentityServer.Repository.Users
{
    public class UserData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Email { get; set; }

        public string Password { get; set; }

        public bool IsActive { get; set; }

        [BsonConstructor]
        public UserData(string id, string name, string email, string password, bool isActive)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsActive = isActive;
        }

        public UserData(string id, string name, string email, string password)
        : this(
            id,
            name,
            email,
            password,
            isActive: true)
        { }

        public UserData(string name, string email)
        : this(
            id: string.Empty,
            name,
            email,
            password: string.Empty,
            isActive: true)
        { }
    }
}