using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Controllers
{
    public sealed class HttpContextWrapper : IHttpContextWrapper
    {
        public Task SignInAsync(HttpContext httpContext, IdentityServerUser user, AuthenticationProperties properties)
        {
            return httpContext.SignInAsync(user, properties);
        }
    }
}