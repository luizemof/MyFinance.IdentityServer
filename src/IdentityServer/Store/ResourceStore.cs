using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;
using IdentityServer.Repository;
using IdentityServer.Services;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace IdentityServer.Store
{
    public class CustomResourceStore : IResourceStore
    {
        private readonly IRepository _dbRepository;
        private readonly IApiScopeService ApiScopeService;

        public CustomResourceStore(IRepository repository, IApiScopeService apiScopeService)
        {
            _dbRepository = repository;
            ApiScopeService = apiScopeService;
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

        public async Task<IEnumerable<ApiScope>> FindApiScopesByNameAsync(IEnumerable<string> scopeNames)
        {
            var apiScopeTasks = new List<Task<ApiScopeModel>>();
            foreach(var scopeName in scopeNames)
                apiScopeTasks.Add(ApiScopeService.GetApiScopeByName(scopeName));
            
            var tasksResult = await Task.WhenAll(apiScopeTasks);
            return tasksResult;
        }

        public Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var identityResources = _dbRepository.All<IdentityResource>().Where(identityResource => scopeNames.Contains(identityResource.Name));
            return Task.FromResult(identityResources.AsEnumerable());
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiScopes = await GetAllApiScopes();
            var resources = new Resources(GetAllIdentityResources(), GetAllApiResources(), apiScopes);
            return resources;
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return _dbRepository.All<ApiResource>();
        }

        private IEnumerable<IdentityResource> GetAllIdentityResources()
        {
            return _dbRepository.All<IdentityResource>();
        }

        private Task<IEnumerable<ApiScopeModel>> GetAllApiScopes()
        {
            return ApiScopeService.GetAllApiScopesAsync();
        }
    }
}