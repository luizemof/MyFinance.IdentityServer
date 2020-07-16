using IdentityServer.Models.IdentityResource;
using IdentityServer.Repository.IdentityResource;

namespace IdentityServer.Extensions
{
    public static class IdentityResourceExtensions
    {
        public static IdentityResourceInputModel ToInputModel(this IdentityResourceModel model)
        {
            var inputModel  = new IdentityResourceInputModel()
            {
                Id = model.Id,
                Name = model.Name,
                DisplayName = model.DisplayName,
                Description = model.Description,
                UserClaims = model.UserClaims
            } ?? new IdentityResourceInputModel();

            return inputModel;
        }

        public static IdentityResourceModel ToModel(this IdentityResourceData data)
        {
            var model = new IdentityResourceModel()
            {
                Id = data.Id,
                Name = data.Name,
                DisplayName = data.DisplayName,
                Description = data.Description,
                Enabled = data.Enabled,
                UserClaims = data.UserClaims
            } ?? new IdentityResourceModel();

            return model;
        }
    }
}