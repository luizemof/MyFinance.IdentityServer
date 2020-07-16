using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;
using IdentityServer.Repository.ApiScopes;
using MongoDB.Driver;
using IdentityApiScope = IdentityServer4.Models.ApiScope;

namespace IdentityServer.Services.ApiScope
{
    public class ApiScopeService : IApiScopeService
    {
        private readonly IApiScopeDataAccess ApiScopeDataAccess;
        
        public ApiScopeService(IApiScopeDataAccess apiScopesDataAccess)
        {
            ApiScopeDataAccess = apiScopesDataAccess;
        }

        public Task<ApiScopeModel> EnableApiScopeAsync(string id)
        {
            return Enable(id, isEnabled: true);
        }

        public Task<ApiScopeModel> DisableApiScopeAsync(string id)
        {
            return Enable(id, isEnabled: false);
        }

        public async Task<IEnumerable<ApiScopeModel>> GetAllScopesAsync()
        {
            var apiScopesData = await ApiScopeDataAccess.GetAsync();
            return apiScopesData.Select(data => FromApiScopeData(data)).ToList();
        }

        private async Task<ApiScopeModel> GetApiScopeById(string id)
        {
            var filterBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = filterBuilder.Eq(data => data.Id, id);
            var apiScopesData = await ApiScopeDataAccess.GetAsync(filter);
            return FromApiScopeData(apiScopesData.Single());
        }

        public Task InsertApiScopeAsync(IdentityApiScope apiScope)
        {
            return ApiScopeDataAccess.InsertAsync(new ApiScopeData(apiScope.Name, apiScope.DisplayName, apiScope.Description));
        }

        private ApiScopeModel FromApiScopeData(ApiScopeData data)
        {
            return new ApiScopeModel(data.Id, data.Name, data.DisplayName, data.Description, data.Enabled);
        }

        private async Task<ApiScopeModel> Enable(string id, bool isEnabled)
        {
            var updateDefinitionBuilder = new UpdateDefinitionBuilder<ApiScopeData>();
            var updateDefinition = updateDefinitionBuilder.Set(scopeData => scopeData.Enabled, isEnabled);
            var updated = await ApiScopeDataAccess.UpdateAsync(id, updateDefinition);

            return updated ? await GetApiScopeById(id) : default(ApiScopeModel);
        }
    }
}