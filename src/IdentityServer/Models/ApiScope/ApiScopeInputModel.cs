using System.ComponentModel.DataAnnotations;

namespace IdentityServer.Models.ApiScope
{
    public class ApiScopeInputModel
    {
        public string Id { get; set; }

        [Required(ErrorMessage = "O nome é obrigatório")]
        public string Name { get; set; }

        [Required(ErrorMessage = "O nome de exibição é obrigatório")]
        public string DisplayName { get; set; }

        public string Description { get; set; }
    }
}