using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.IdentityResource;

namespace IdentityServer.Services
{
    public interface IIdentityResourceService
    {
         Task<IEnumerable<IdentityResourceModel>> GetAllIdentityResourcesAsync();
         
         Task<IdentityResourceModel> GetIdentityResourceById(string id);
         
         Task<IdentityResourceModel> GetIdentityResourceByName(string scopeName);
         
         Task UpsertIdentityResource(IdentityResourceInputModel identityResourceInputModel);

         Task<IdentityResourceModel> Enable(string id);
         
         Task<IdentityResourceModel> Disable(string id);
    }
}