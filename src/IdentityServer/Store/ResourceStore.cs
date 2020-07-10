using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Repository;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Store
{
    public class CustomResourceStore : IResourceStore
    {
        private readonly IRepository _dbRepository;

        public CustomResourceStore(IRepository repository)
        {
            _dbRepository = repository;
        }

         public Task<IEnumerable<ApiResource>> FindApiResourcesByNameAsync(IEnumerable<string> apiResourceNames)
        {
            var apiResources = _dbRepository.All<ApiResource>().Where(api => apiResourceNames.Contains(api.Name));
            return Task.FromResult(apiResources.AsEnumerable());
        }

        public Task<IEnumerable<ApiResource>> FindApiResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var apiResources = _dbRepository
                .All<ApiResource>()
                .Where(api => api.Scopes.Any(apiScope => scopeNames.Contains(apiScope)));
            return Task.FromResult(apiResources.AsEnumerable());
        }

        public Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var apiScopes = _dbRepository.All<ApiScope>().Where(scope => scopeNames.Contains(scope.Name));
            return Task.FromResult(apiScopes.AsEnumerable());
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var identityResources = _dbRepository.All<IdentityResource>().Where(identityResource => scopeNames.Contains(identityResource.Name));
            return Task.FromResult(identityResources.AsEnumerable());
        }

        public Task<Resources> GetAllResourcesAsync()
        {
            var resources = new Resources(GetAllIdentityResources(), GetAllApiResources(), GetAllApiScopes());
            return Task.FromResult(resources);
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return _dbRepository.All<ApiResource>();
        }

        private IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return _dbRepository.All<IdentityResource>();
        }

        private IEnumerable<ApiScope> GetAllApiScopes()
        {
            return _dbRepository.All<ApiScope>();
        }
    }
}