using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;
using IdentityApiScope = IdentityServer4.Models.ApiScope;

namespace IdentityServer.Services
{
    public interface IApiScopeService
    {
         Task<IEnumerable<ApiScopeModel>> GetAllScopesAsync();
         
         Task InsertApiScopeAsync(IdentityApiScope apiScope);
    }
}