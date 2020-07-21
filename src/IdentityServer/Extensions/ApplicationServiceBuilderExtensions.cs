using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Repository.IdentityResource;
using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer.Extensions
{
    public static class ApplicationServiceBuilderExtensions
    {
        public static Task InitializeDatabase(this IApplicationBuilder app)
        {
            return app.InitializeBasicIdentityResource();
        }

        private static async Task InitializeBasicIdentityResource(this IApplicationBuilder app)
        {
            var task = new List<Task>();
            task.Add(app.AddIdentityResourceIfExists(IdentityServerConstants.StandardScopes.OpenId, new IdentityResources.OpenId()));
            task.Add(app.AddIdentityResourceIfExists(IdentityServerConstants.StandardScopes.Profile, new IdentityResources.Profile()));

            await Task.WhenAll(task);
        }

        private static async Task AddIdentityResourceIfExists(this IApplicationBuilder app, string name, IdentityResource identity)
        {
            var identityResourceDataAccess = app.ApplicationServices.GetService<IIdentityResourceDataAccess>();
            var data = await identityResourceDataAccess.GetByField(data => data.Name, name);
            if (data == null)
            {
                var newIdentityData = new IdentityResourceData(string.Empty, identity.Name, identity.DisplayName, identity.Description, identity.UserClaims, identity.Enabled);
                await identityResourceDataAccess.InsertAsync(newIdentityData);
            }
        }
    }
}