using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.Mongo
{
    public interface IMongoDataAccess<T>
    {
        Task<IEnumerable<T>> GetAsync();

        Task<IEnumerable<T>> GetAsync(FilterDefinition<T> filter);

        Task InsertAsync(T apiScopeData);
    }
}