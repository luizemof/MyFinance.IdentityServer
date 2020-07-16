using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.ApiScopes
{
    public class ApiScopeDataAccess : IApiScopeDataAccess
    {
        private readonly IMongoCollection<ApiScopeData> ApiScopeCollection;
        
        public ApiScopeDataAccess(IMongoDatabase database)
        {
            ApiScopeCollection = database.GetCollection<ApiScopeData>("ApiScope");
        }

        public Task<IEnumerable<ApiScopeData>> GetAsync()
        {
            var filter = FilterDefinition<ApiScopeData>.Empty;   
            return GetAsync(filter);
        }

        public async Task<IEnumerable<ApiScopeData>> GetAsync(FilterDefinition<ApiScopeData> filter)
        {
            var apiScopes=  await ApiScopeCollection.FindAsync(filter);
            return apiScopes.ToEnumerable();
        }

        public Task InsertAsync(ApiScopeData apiScopeData)
        {
            return ApiScopeCollection.InsertOneAsync(apiScopeData);
        }

        public async Task ReplaceAsync(ApiScopeData apiScopeData)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = filterDefinitionBuilder.Where(data => data.Id == apiScopeData.Id);
            var replaceResult = await ApiScopeCollection.ReplaceOneAsync(filter, apiScopeData);
        }

        public async Task<bool> UpdateAsync(string id, UpdateDefinition<ApiScopeData> updateDefinition)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = filterDefinitionBuilder.Eq(data => data.Id, id);

            var updateResult = await ApiScopeCollection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.IsAcknowledged;
        }
    }
}