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

        public async Task<ApiScopeModel> GetApiScopeById(string id)
        {
            var data = await ApiScopeDataAccess.GetByField(data => data.Id, id);
            return FromApiScopeData(data);
        }

        public async Task<ApiScopeModel> GetApiScopeByName(string name)
        {
            var data = await ApiScopeDataAccess.GetByField(data => data.Name, name);
            return FromApiScopeData(data);
        }

        public Task UpsertApiScopeAsync(ApiScopeInputModel apiScopeInputModel)
        {
            var apiScopeData = new ApiScopeData(apiScopeInputModel.Id, apiScopeInputModel.Name, apiScopeInputModel.DisplayName, apiScopeInputModel.Description);
            Task upsertTask;
            if(string.IsNullOrEmpty(apiScopeData.Id))
                upsertTask= ApiScopeDataAccess.InsertAsync(apiScopeData);
            else
                upsertTask = ApiScopeDataAccess.ReplaceAsync(apiScopeData, data => data.Id == apiScopeData.Id);

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
            var updated = await ApiScopeDataAccess.UpdateAsync(data => data.Id, id, updateDefinition);

            return updated ? await GetApiScopeById(id) : default(ApiScopeModel);
        }
    }
}