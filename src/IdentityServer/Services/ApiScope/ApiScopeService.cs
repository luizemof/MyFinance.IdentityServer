using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models.ApiScope;
using IdentityServer.Repository.ApiScopes;
using IdentityApiScope = IdentityServer4.Models.ApiScope;

namespace IdentityServer.Services.ApiScope
{
    public class ApiScopeService : IApiScopeService
    {
        private readonly IApiScopeDataAccess ApiScopesDataAccess;
        
        public ApiScopeService(IApiScopeDataAccess apiScopesDataAccess)
        {
            ApiScopesDataAccess = apiScopesDataAccess;
        }

        public async Task<IEnumerable<ApiScopeModel>> GetAllScopesAsync()
        {
            var apiScopesData = await ApiScopesDataAccess.GetAsync();
            return apiScopesData.Select(data => new ApiScopeModel(data.Id, data.Name, data.DisplayName, data.Description, data.Enabled)).ToList();
        }

        public Task InsertApiScopeAsync(IdentityApiScope apiScope)
        {
            return ApiScopesDataAccess.InsertAsync(new ApiScopeData(apiScope.Name, apiScope.DisplayName, apiScope.Description));
        }
    }
}