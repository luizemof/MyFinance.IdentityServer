using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace IdentityServer.Models.Users
{
    public readonly struct UserModel
    {
        public static UserModel Empty
        {
            get
            {
                return new UserModel(string.Empty, string.Empty, string.Empty, false);
            }
        }

        public string Id { get; }
        public string Name { get; }
        public string Email { get; }
        public bool IsActive { get; }

        public UserModel(string id, string name, string email, bool isActive)
        {
            Id = id;
            Name = name;
            Email = email;
            IsActive = isActive;
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
}