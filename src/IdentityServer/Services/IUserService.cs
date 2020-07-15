using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Models.Users;

namespace IdentityServer.Services
{
    public interface IUserService
    {
         Task<UserModel> GetUserAsync(string id);

        Task<IEnumerable<UserModel>> GetUsersAsync();

        Task<UserModel> CreateUserAsync(UserInputModel inputUserModel);

        Task<IEnumerable<UserModel>> GetInactiveUsersAsync();

        Task<UserModel> DeactiveUserAsync(string id);
        
        Task<UserModel> ReactiveUserAsync(string id);

        Task<UserModel> UpdateUserAsync(UserInputModel inputUserModel);
    }
}