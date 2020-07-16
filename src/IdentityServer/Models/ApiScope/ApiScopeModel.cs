using IdentityApiScope = IdentityServer4.Models.ApiScope;
namespace IdentityServer.Models.ApiScope
{
    public class ApiScopeModel : IdentityApiScope
    {
        public string Id { get; set; }

        public ApiScopeModel(string id, string name, string displayName, string description, bool enabled) : base(name, displayName)
        {
            Id = id;
            Description = description;
            Enabled = enabled;
        }
    }
}