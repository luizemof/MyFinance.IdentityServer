using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.IdentityResource;

namespace IdentityServer.Services
{
    public interface IIdentityResourceService
    {
         Task<IEnumerable<IdentityResourceModel>> GetAllIdentityResources();
         
         Task InsertIdentityResource(IdentityResourceInputModel identityResourceInputModel);
    }
}