using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Controllers
{
    public sealed class HttpContextWrapper : IHttpContextWrapper
    {
        public bool UserIsAuthenticated(HttpContext context)
        {
            return context?.User?.Identity != null && context.User.Identity.IsAuthenticated;
        }

        public Task SignInAsync(HttpContext httpContext, IdentityServerUser user, AuthenticationProperties properties)
        {
            return httpContext.SignInAsync(user, properties);
        }

        public Task SignOutAsync(HttpContext httpContext)
        {
            return httpContext.SignOutAsync();
        }
    }
}