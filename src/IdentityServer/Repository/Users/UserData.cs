using System.Collections.Generic;
using System.Linq;
using IdentityServer.Cryptography;
using IdentityServer.Models.Users;
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

        public IEnumerable<string> Roles { get; set; }

        [BsonConstructor]
        public UserData(string id, string name, string email, string password, bool isActive, IEnumerable<string> roles)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsActive = isActive;
            Roles = roles;
        }

        public UserData(string id, string name, string email, string password, bool isActive)
        : this(
            id,
            name,
            email,
            password,
            isActive,
            roles: Enumerable.Empty<string>()
        )
        { }

        public UserData(string id, string name, string email, string password)
        : this(
            id,
            name,
            email,
            password,
            isActive: true,
            roles: Enumerable.Empty<string>())
        { }

        public UserData(string id, string name, string email, string password, IEnumerable<string> roles)
        : this(
            id,
            name,
            email,
            password,
            isActive: true,
            roles)
        { }

        public UserData(string name, string email)
        : this(
            id: string.Empty,
            name,
            email,
            password: string.Empty,
            isActive: true,
            roles: Enumerable.Empty<string>())
        { }
    }

    public static class UserExtensions
    {
        public static UserModel ToModel(this UserData userData, IdentityServerCryptography cryptography)
        {
            if (userData == null)
                return UserModel.Empty;

            var password = cryptography.Decrypt(userData.Password);
            return new UserModel(userData.Id, userData.Name, userData.Email, password, userData.IsActive, userData.Roles);
        }
    }
}