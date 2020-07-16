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

        public async Task<IEnumerable<ApiScopeData>> GetAsync()
        {
            var filterBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = FilterDefinition<ApiScopeData>.Empty;
            var apiScopes=  (await ApiScopeCollection.FindAsync(filter));
            
            return apiScopes.ToEnumerable();;
        }

        public Task InsertAsync(ApiScopeData apiScopeData)
        {
            return ApiScopeCollection.InsertOneAsync(apiScopeData);
        }
    }
}