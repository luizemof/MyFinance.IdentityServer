using IdentityServerClient = IdentityServer4.Models.Client;
namespace IdentityServer.Models.Client
{
    public class ClientModel : IdentityServerClient
    {
        public string Id { get; set; }
    }
}