using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.ApiScopes
{
    public class ApiScopeDataAccess : MongoDataAccess<ApiScopeData>, IApiScopeDataAccess
    {
        protected override string CollectionName => "ApiScope";

        public ApiScopeDataAccess(IMongoDatabase database) : base(database)
        {
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var keys = Builders<ApiScopeData>.IndexKeys.Ascending("Name");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var model = new CreateIndexModel<ApiScopeData>(keys, indexOptions);
            Collection.Indexes.CreateOne(model);
        }

        public async Task ReplaceAsync(ApiScopeData apiScopeData)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = filterDefinitionBuilder.Where(data => data.Id == apiScopeData.Id);
            var replaceResult = await Collection.ReplaceOneAsync(filter, apiScopeData);
        }

        public async Task<bool> UpdateAsync(string id, UpdateDefinition<ApiScopeData> updateDefinition)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<ApiScopeData>();
            var filter = filterDefinitionBuilder.Eq(data => data.Id, id);

            var updateResult = await Collection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.IsAcknowledged;
        }
    }
}