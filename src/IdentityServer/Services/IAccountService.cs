using System.Threading.Tasks;
using IdentityServer.Models.Account;

namespace IdentityServer.Services
{
    public interface IAccountService
    {
        Task<bool> ValidateCredentials(LoginModel loginModel);
    }
}