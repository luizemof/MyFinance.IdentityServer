using System.Collections.Generic;
using IdentityServer4.Models;
using IdentityServerClient = IdentityServer4.Models.Client;
namespace IdentityServer.Models.Client
{
    public class ClientModel : IdentityServerClient
    {
        public ClientModel()
        { }

        public ClientModel(string id, string decryptedSecret)
        {
            this.Id = id;
            this.DecryptedSecret = decryptedSecret;
        }

        public string Id { get; }

        private string _DecryptedSecret;
        public string DecryptedSecret 
        { 
            get { return _DecryptedSecret; } 
            private set 
            { 
                _DecryptedSecret = value;
                this.ClientSecrets = new List<Secret>() { new Secret(_DecryptedSecret.Sha256()) };
            }
        }
    }
}