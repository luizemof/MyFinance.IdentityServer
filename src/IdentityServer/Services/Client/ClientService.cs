using System.Threading.Tasks;
using IdentityServer.Cryptography;
using IdentityServer.Extensions;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;

namespace IdentityServer.Services.Client
{
    public class ClientService : IClientService
    {
        private readonly IClientDataAccess ClientDataAccess;

        private readonly IdentityServerCryptography Cryptography;
        
        public ClientService(IClientDataAccess clientDataAccess, IdentityServerCryptography cryptography)
        {
            ClientDataAccess = clientDataAccess;
            Cryptography = cryptography;
        }

        public Task UpsertClient(ClientInputModel inputModel)
        {
            return ClientDataAccess.InsertAsync(inputModel.ToData(Cryptography));
        }
    }
}