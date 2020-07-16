using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Models.IdentityResource;
using IdentityServer.Repository.IdentityResource;

namespace IdentityServer.Services.IdentityResource
{
    public class IdentityResourceService : IIdentityResourceService
    {
        private readonly IIdentityResourceDataAccess IdentityResourceDataAccess;

        public IdentityResourceService(IIdentityResourceDataAccess identityResourceDataAccess)
        {
            IdentityResourceDataAccess = identityResourceDataAccess;
        }

        public async Task<IEnumerable<IdentityResourceModel>> GetAllIdentityResources()
        {
            var datas = await IdentityResourceDataAccess.GetAsync();
            return datas.Select(data => FromIdentityResourceData(data)).ToList();
        }

        public Task InsertIdentityResource(IdentityResourceInputModel model)
        {
            var data = new IdentityResourceData(model.Id, model.Name, model.DisplayName, model.Description, model.UserClaims);
            return IdentityResourceDataAccess.InsertAsync(data);
        }

        public IdentityResourceModel FromIdentityResourceData(IdentityResourceData data)
        {
            return new IdentityResourceModel()
            {
                Id = data.Id,
                Name = data.Name,
                DisplayName = data.DisplayName,
                Description = data.Description,
                Enabled = data.Enabled,
                UserClaims = data.UserClaims
            };
        }
    }
}