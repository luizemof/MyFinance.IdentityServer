using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.Mongo
{
    public interface IMongoDataAccess<T>
    {
        Task<IEnumerable<T>> GetAsync();

        Task<IEnumerable<T>> GetAsync(FilterDefinition<T> filter);
        
        Task<T> GetByField<TField>(Expression<Func<T, TField>> field, TField value);

        Task InsertAsync(T apiScopeData);

        Task<bool> ReplaceAsync(T apiScopeData, Expression<Func<T, bool>> expression);
    }
}