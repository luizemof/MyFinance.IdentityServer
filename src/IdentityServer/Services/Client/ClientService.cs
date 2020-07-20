using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer.Cryptography;
using IdentityServer.Extensions;
using IdentityServer.Models.Client;
using IdentityServer.Repository.Client;
using MongoDB.Driver;

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

        public Task<ClientModel> GetClientByClientIdAsync(string clientId)
        {
            return GetClientByFieldAsync(data => data.ClientId, clientId);
        }

        public Task<ClientModel> GetClientByInternalIdAsync(string id)
        {
            return GetClientByFieldAsync(data => data.Id, id);
        }

        public async Task UpsertClientAsync(ClientInputModel inputModel)
        {
            try
            {
                var clientData = inputModel.ToData(Cryptography);
                if (string.IsNullOrWhiteSpace(clientData.Id))
                    await ClientDataAccess.InsertAsync(clientData);
                else
                    await ClientDataAccess.ReplaceAsync(clientData, (data) => data.Id == clientData.Id);
            }
            catch (MongoWriteException ex)
            {
                ex.ThrowIfDuplicateKey(nameof(inputModel.ClientId), $"O id do client '{inputModel.ClientId}' já existe");
                throw ex;
            }
        }

        private async Task<ClientModel> GetClientByFieldAsync<T>(Expression<Func<ClientData, T>> expression, T value)
        {
            var data = await ClientDataAccess.GetByField(expression, value);
            return data.ToModel(Cryptography);
        }
    }
}