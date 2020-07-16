using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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

        public async Task<bool> ReplaceAsync(T apiScopeData, Expression<Func<T, bool>> expression)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<T>();
            var filter = filterDefinitionBuilder.Where(expression);
            var replaceResult = await Collection.ReplaceOneAsync(filter, apiScopeData);

            return replaceResult.IsAcknowledged;
        }

        public async Task<T> GetByField<TField>(Expression<Func<T, TField>> field, TField value)
        {
            var filterBuilder = new FilterDefinitionBuilder<T>();
            var filter = filterBuilder.Eq(field, value);
            var apiScopesData = await GetAsync(filter);
            
            return apiScopesData.SingleOrDefault();
        }
    }
}