using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.Users
{
    public interface IUserDataAccess
    {
        Task CreateAsync(UserData user);

        Task<UserData> ReplaceAsync(UserData user);

        Task<IEnumerable<UserData>> GetAsync(string id = null, bool? isActive = true);
        
        Task<IEnumerable<UserData>> GetAsync(FilterDefinition<UserData> filter);

        Task<UserData> DeleteAscyn(string id);

        Task<bool> UpdateAsync(string id, UpdateDefinition<UserData> updateDefinition);
    }
}