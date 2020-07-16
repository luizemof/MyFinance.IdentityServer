using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.ApiScopes
{
    public interface IApiScopeDataAccess
    {
        Task<IEnumerable<ApiScopeData>> GetAsync();
        Task<IEnumerable<ApiScopeData>> GetAsync(FilterDefinition<ApiScopeData> filter);
        Task InsertAsync(ApiScopeData apiScopeData);
        Task ReplaceAsync(ApiScopeData apiScopeData);
        Task<bool> UpdateAsync(string id, UpdateDefinition<ApiScopeData> updateDefinition);
    }
}