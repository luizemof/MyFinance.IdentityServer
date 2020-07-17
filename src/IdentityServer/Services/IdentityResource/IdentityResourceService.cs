using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer.Extensions;
using IdentityServer.Models.IdentityResource;
using IdentityServer.Repository.IdentityResource;
using MongoDB.Driver;

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
            return datas.Select(data => data.ToModel()).ToList();
        }

        public async Task UpsertIdentityResource(IdentityResourceInputModel model)
        {
            try
            {
                var data = new IdentityResourceData(model.Id, model.Name, model.DisplayName, model.Description, model.UserClaims);
                if (string.IsNullOrWhiteSpace(model.Id))
                    await IdentityResourceDataAccess.InsertAsync(data);
                else
                    await IdentityResourceDataAccess.ReplaceAsync(data, updateData => updateData.Id == data.Id);
            }
            catch (MongoWriteException ex)
            {
                ex.ThrowIfDuplicateKey(nameof(model.Name), $"O nome '{model.Name}' já existe.");
                throw ex;
            }
        }

        public async Task<IdentityResourceModel> GetIdentityResourceById(string id)
        {
            var data = await IdentityResourceDataAccess.GetByField(data => data.Id, id);
            return data.ToModel();
        }

        public Task<IdentityResourceModel> Enable(string id)
        {
            return EnableOrDisable(id, isEnabled: true);
        }

        public Task<IdentityResourceModel> Disable(string id)
        {
            return EnableOrDisable(id, isEnabled: false);
        }

        private async Task<IdentityResourceModel> EnableOrDisable(string id, bool isEnabled)
        {
            var updateDefinitionBuilder = new UpdateDefinitionBuilder<IdentityResourceData>();
            var updateDefinition = updateDefinitionBuilder.Set(data => data.Enabled, isEnabled);
            var updated = await IdentityResourceDataAccess.UpdateAsync(data => data.Id, id, updateDefinition);

            return updated ? await GetIdentityResourceById(id) : default(IdentityResourceModel);
        }
    }
}