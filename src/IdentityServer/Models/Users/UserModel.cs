using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.Users
{
    public sealed class UserModel
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; }
        public string ActiveOrDeactive { get { return IsActive ? "Deactivate" : "Activate"; } }
    }
}