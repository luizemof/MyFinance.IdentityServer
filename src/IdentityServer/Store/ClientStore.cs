using System.Threading.Tasks;
using IdentityServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using System;

namespace IdentityServer.Store
{
    public sealed class CustomClientStore : IClientStore
    {
        private readonly IClientService ClientService;

        public CustomClientStore(IClientService clientService)
        {
            ClientService = clientService;
        }

        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            var client = await ClientService.GetClientByClientIdAsync(clientId);
            client.AccessTokenLifetime = (int)(TimeSpan.FromDays(1).TotalSeconds);
            return client;
        }
    }
}