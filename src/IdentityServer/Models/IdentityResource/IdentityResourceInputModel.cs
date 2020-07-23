using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.IdentityResource
{
    public class IdentityResourceInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The Name is required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Display Name is required")]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ICollection<string> UserClaims { get; set; } = new List<string>();
    }
}