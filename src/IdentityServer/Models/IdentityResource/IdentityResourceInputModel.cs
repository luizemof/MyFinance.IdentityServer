using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.IdentityResource
{
    public class IdentityResourceInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório.")]
        public string Name { get; set; }   

        [Required(ErrorMessage = "A exibição do nome é obrigatória.")]
        public string DisplayName { get; set; }

        public string Description { get; set; }

        public ICollection<string> UserClaims { get; set; }
    }
}