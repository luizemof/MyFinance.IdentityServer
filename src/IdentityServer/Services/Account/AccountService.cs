using System.Threading.Tasks;
using IdentityServer.Cryptography;
using IdentityServer.Models.Account;
using IdentityServer.Repository.Users;

namespace IdentityServer.Services.Account
{
    public class AccountService : IAccountService
    {
        private readonly IUserDataAccess UserDataAccess;
        private readonly IdentityServerCryptography Cryptography;

        public AccountService(IUserDataAccess userDataAccess, IdentityServerCryptography cryptography)
        {
            this.UserDataAccess = userDataAccess ?? throw new System.ArgumentNullException(nameof(userDataAccess));
            this.Cryptography = cryptography ?? throw new System.ArgumentNullException(nameof(cryptography));
        }

        public async Task<bool> ValidateCredentials(LoginModel loginModel)
        {
            var userData = await this.UserDataAccess.GetByField(userData => userData.Email, loginModel.Email);
            return userData != null && userData.IsActive && loginModel.Password == this.Cryptography.Decrypt(userData.Password);
        }
    }
}