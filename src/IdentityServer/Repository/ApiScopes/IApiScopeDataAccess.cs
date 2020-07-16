using System.Collections.Generic;
using System.Threading.Tasks;

namespace IdentityServer.Repository.ApiScopes
{
    public interface IApiScopeDataAccess
    {
         Task<IEnumerable<ApiScopeData>> GetAsync();
         Task InsertAsync(ApiScopeData apiScopeData);
    }
}