using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;

namespace IdentityServer.Services
{
    public interface IApiScopeService
    {
         Task<IEnumerable<ApiScopeModel>> GetAllApiScopesAsync();

         Task<ApiScopeModel> GetApiScopeById(string id);

         Task<ApiScopeModel> GetApiScopeByName(string name);
         
         Task UpsertApiScopeAsync(ApiScopeInputModel apiScopeInputModel);

         Task<ApiScopeModel> EnableApiScopeAsync(string id);

         Task<ApiScopeModel> DisableApiScopeAsync(string id);
    }
}