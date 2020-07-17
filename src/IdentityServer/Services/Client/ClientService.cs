using System.Collections.Generic;
using System.Linq;
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

        public async Task<IEnumerable<ClientModel>> GetAllClientsAsync()
        {
            var datas = await ClientDataAccess.GetAsync();
            return datas.Select(data => data.ToModel(Cryptography));
        }

        public async Task<ClientModel> GetClientByClientIdAsync(string clientId)
        {
            var data = await ClientDataAccess.GetByField(data => data.ClientId, clientId);
            return data.ToModel(Cryptography);
        }

        public Task UpsertClientAsync(ClientInputModel inputModel)
        {
            return ClientDataAccess.InsertAsync(inputModel.ToData(Cryptography));
        }
    }
}