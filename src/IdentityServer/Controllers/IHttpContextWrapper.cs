using System.Threading.Tasks;
using IdentityServer4;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;

namespace IdentityServer.Controllers
{
    public interface IHttpContextWrapper
    {
        Task SignInAsync(HttpContext httpContext, IdentityServerUser user, AuthenticationProperties properties);
    }
}