using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel;
using IdentityServer.Attributes;
using IdentityServer.Models.Users;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;

namespace IdentityServer.Services.Profile
{
    public class ProfileService : DefaultProfileService
    {
        private readonly IUserService UserService;
        public ProfileService(ILogger<DefaultProfileService> logger, IUserService userService) : base(logger)
        {
            this.UserService = userService ?? throw new System.ArgumentNullException(nameof(userService));
        }

        public override async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var id = context.Subject?.FindFirst(JwtClaimTypes.Subject)?.Value;
            if (!string.IsNullOrWhiteSpace(id))
            {
                var userModel = await this.UserService.GetUserAsync(id);
                context.IssuedClaims = GetIssuerClaims(userModel);
            }
        }

        private List<Claim> GetIssuerClaims(UserModel userModel)
        {
            var claims = new List<Claim>();
            var profileProperties = userModel.GetType().GetProperties()?.Where(prop => Attribute.IsDefined(prop, typeof(ProfileAttribute)));
            if (profileProperties != null)
            {
                foreach (var property in profileProperties)
                {
                    var value = property.GetValue(userModel);
                    if (value is IEnumerable<object> enumerable)
                        claims.AddRange(enumerable.Select(e => new Claim(property.Name, e.ToString())));
                    else if (!string.IsNullOrWhiteSpace(value.ToString()))
                        claims.Add(new Claim(property.Name, value.ToString()));
                }
            }

            return claims;
        }
    }
}