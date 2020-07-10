using System.Threading.Tasks;
using IdentityServer.Repository;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Store
{
    public sealed class CustomClientStore : IClientStore
    {
        private readonly IRepository Repository;

        public CustomClientStore(IRepository repository)
        {
            Repository = repository;
        }

        public Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = Repository.Single<Client>(client => client.ClientId == clientId);
            return Task.FromResult(client);
        }
    }
}