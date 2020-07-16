using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.Users
{
    public interface IUserDataAccess : IMongoDataAccess<UserData>
    {
        Task<UserData> ReplaceAsync(UserData user);

        Task<IEnumerable<UserData>> GetAsync(string id = null, bool? isActive = true);

        Task<UserData> DeleteAscyn(string id);

        Task<bool> UpdateAsync(string id, UpdateDefinition<UserData> updateDefinition);
    }
}