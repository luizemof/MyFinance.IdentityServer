using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;
using IdentityServer.Repository.ApiScopes;
using MongoDB.Driver;

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

        public async Task<IEnumerable<ApiScopeModel>> GetAllApiScopesAsync()
        {
            var apiScopesData = await ApiScopeDataAccess.GetAsync();
            return apiScopesData.Select(data => FromApiScopeData(data)).ToList();
        }

        public Task<ApiScopeModel> GetApiScopeById(string id)
        {
            return GetApiScopeByField(data => data.Id, id);
        }

        public Task<ApiScopeModel> GetApiScopeByName(string name)
        {
            return GetApiScopeByField(data => data.Name, name);
        }

        public Task UpsertApiScopeAsync(ApiScopeInputModel apiScopeInputModel)
        {
            var apiScopeData = new ApiScopeData(apiScopeInputModel.Id, apiScopeInputModel.Name, apiScopeInputModel.DisplayName, apiScopeInputModel.Description);
            Task upsertTask;
            if(string.IsNullOrEmpty(apiScopeData.Id))
                upsertTask= ApiScopeDataAccess.InsertAsync(apiScopeData);
            else
                upsertTask = ApiScopeDataAccess.ReplaceAsync(apiScopeData);

            return upsertTask;
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

        private async Task<ApiScopeModel> GetApiScopeByField<T>(Expression<Func<ApiScopeData, T>> field, T value)
        {
            var filterBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = filterBuilder.Eq(field, value);
            var apiScopesData = await ApiScopeDataAccess.GetAsync(filter);
            return FromApiScopeData(apiScopesData.Single());
        }
    }
}