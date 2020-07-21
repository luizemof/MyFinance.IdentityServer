using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Exceptions;
using IdentityServer.Models.ApiScope;
using IdentityServer.Models.IdentityResource;
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
        private readonly IIdentityResourceService IdentityResourceService;

        public CustomResourceStore(IRepository repository, IApiScopeService apiScopeService, IIdentityResourceService identityResourceService)
        {
            _dbRepository = repository;
            ApiScopeService = apiScopeService;
            IdentityResourceService = identityResourceService;
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
            foreach (var scopeName in scopeNames)
                apiScopeTasks.Add(ApiScopeService.GetApiScopeByName(scopeName));

            var tasksResult = await Task.WhenAll(apiScopeTasks);
            return tasksResult.Where(result => result != null);
        }

        public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeNameAsync(IEnumerable<string> scopeNames)
        {
            var task = new List<Task<IdentityResourceModel>>();
            foreach (var scopeName in scopeNames)
                task.Add(GetIdentityResourceByName(scopeName));

            var result = await Task.WhenAll(task);
            return result.Where(res => res != null);
        }

        public async Task<Resources> GetAllResourcesAsync()
        {
            var apiScopesTask = GetAllApiScopes();
            var identityResourcesTask = GetAllIdentityResources();
            
            var apiScopes = await apiScopesTask;
            var identityResources = await identityResourcesTask;

            var resources = new Resources(identityResources, GetAllApiResources(), apiScopes);
            return resources;
        }

        private IEnumerable<ApiResource> GetAllApiResources()
        {
            return _dbRepository.All<ApiResource>();
        }

        private Task<IEnumerable<IdentityResourceModel>> GetAllIdentityResources()
        {
            return this.IdentityResourceService.GetAllIdentityResourcesAsync();
        }

        private Task<IEnumerable<ApiScopeModel>> GetAllApiScopes()
        {
            return ApiScopeService.GetAllApiScopesAsync();
        }

        private async Task<IdentityResourceModel> GetIdentityResourceByName(string name)
        {
            try
            {
                return await this.IdentityResourceService.GetIdentityResourceByName(name);
            }
            catch (NotFoundException)
            {
                return default(IdentityResourceModel);
            }
        }
    }
}