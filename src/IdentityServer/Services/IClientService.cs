using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.Client;

namespace IdentityServer.Services
{
    public interface IClientService
    {
        Task UpsertClientAsync(ClientInputModel inputModel);

        Task<IEnumerable<ClientModel>> GetAllClientsAsync();

        Task<ClientModel> GetClientByClientIdAsync(string clientId);
        Task<ClientModel> GetClientByInternalIdAsync(string id);
    }
}