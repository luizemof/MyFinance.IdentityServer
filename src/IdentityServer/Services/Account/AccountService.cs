using System.Threading.Tasks;
using IdentityServer.Models.Account;

namespace IdentityServer.Services.Account
{
    public class AccountService : IAccountService
    {
        public Task<bool> ValidateCredentials(LoginModel loginModel)
        {
            throw new System.NotImplementedException();
        }
    }
}