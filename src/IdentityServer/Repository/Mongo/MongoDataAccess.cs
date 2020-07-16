using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.Mongo
{
    public abstract class MongoDataAccess<T> : IMongoDataAccess<T>
    {
        protected readonly IMongoCollection<T> Collection;

        protected abstract string CollectionName { get; }

        public MongoDataAccess(IMongoDatabase mongoDatabase)
        {
            Collection = mongoDatabase.GetCollection<T>(CollectionName);
        }

        public Task<IEnumerable<T>> GetAsync()
        {
            var filter = FilterDefinition<T>.Empty;   
            return GetAsync(filter);
        }
        
        public async Task<IEnumerable<T>> GetAsync(FilterDefinition<T> filter)
        {
            var apiScopes=  await Collection.FindAsync(filter);
            return apiScopes.ToEnumerable();
        }
        
        public Task InsertAsync(T apiScopeData)
        {
            return Collection.InsertOneAsync(apiScopeData);
        }
    }
}