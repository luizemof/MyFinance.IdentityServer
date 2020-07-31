using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using IdentityServer.Attributes;
using IdentityServer.Constants;

namespace IdentityServer.Models.Users
{
    public readonly struct UserModel
    {
        public static UserModel Empty
        {
            get
            {
                return new UserModel(string.Empty, string.Empty, string.Empty, string.Empty, false, Enumerable.Empty<string>());
            }
        }


        [ProfileAttribute("id")]
        public string Id { get; }
        
        [ProfileAttribute("name")]
        public string Name { get; }
        
        [ProfileAttribute("email")]
        public string Email { get; }
        
        public string Password { get; }
        
        [ProfileAttribute("isActive")]
        public bool IsActive { get; }
        
        [ProfileAttribute("roles")]
        public IEnumerable<string> Roles { get; }

        public UserModel(string id, string name, string email, string password, bool isActive, IEnumerable<string> roles)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            IsActive = isActive;
            Roles = roles;
        }

        public bool Equals([AllowNull] UserModel other)
        {
            return
            (
                Id == other.Id
                &&
                Name == other.Name
                &&
                Email == other.Email
                &&
                IsActive == other.IsActive
                &&
                Roles == other.Roles
            );
        }

        public override bool Equals(object obj)
        {
            return obj is UserModel userObj && Equals(userObj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(UserModel left, UserModel right) => left.Equals(right);
        public static bool operator !=(UserModel left, UserModel right) => !(left.Equals(right));
    }

    public static class UserExtensions
    {
        public static UserInputModel ToInputModel(this UserModel userModel)
        {
            return new UserInputModel()
            {
                Id = userModel.Id,
                Name = userModel.Name,
                Email = userModel.Email,
                Password = userModel.Password,
                PasswordConfirmation = userModel.Password,
                Roles = userModel.Roles?.ToList() ?? new List<string>()
            };
        }
    }
}