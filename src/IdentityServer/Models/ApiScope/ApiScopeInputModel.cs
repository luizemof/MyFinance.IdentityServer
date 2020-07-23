using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.ApiScope
{
    public class ApiScopeInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "The name is Required")]
        public string Name { get; set; }

        [Required(ErrorMessage = "The Display Name is required")]
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}