using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;

namespace IdentityServer.Repository.Users
{
    public class UserDataAccess : IUserDataAccess
    {
        private readonly IMongoCollection<UserData> UserCollection;

        public UserDataAccess(IMongoDatabase database)
        {
            UserCollection = database.GetCollection<UserData>("UserCollection");
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var keys = Builders<UserData>.IndexKeys.Ascending("Email");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var model = new CreateIndexModel<UserData>(keys, indexOptions);
            UserCollection.Indexes.CreateOne(model);
        }

        public Task CreateAsync(UserData user)
        {
            return UserCollection.InsertOneAsync(user);
        }

        public Task<UserData> DeleteAscyn(string id)
        {
            return UserCollection.FindOneAndDeleteAsync<UserData>(data => data.Id == id);
        }

        public Task<IEnumerable<UserData>> GetAsync(string id = null, bool? isActive = true)
        {
            var filterBuilder = new FilterDefinitionBuilder<UserData>();
            var filter = FilterDefinition<UserData>.Empty;

            if (!string.IsNullOrWhiteSpace(id))
                filter &= filterBuilder.Eq<string>(userData => userData.Id, id);

            if (isActive.HasValue)
                filter &= filterBuilder.Eq<bool>(userData => userData.IsActive, isActive.Value);

            return GetAsync(filter);
        }

        public async Task<UserData> ReplaceAsync(UserData user)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<UserData>();
            var filter = filterDefinitionBuilder.Where(data => data.Id == user.Id);
            var replaceResult = await UserCollection.ReplaceOneAsync(filter, user);

            return replaceResult.IsAcknowledged ? user : default(UserData);
        }

        public async Task<bool> UpdateAsync(string id, UpdateDefinition<UserData> updateDefinition)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<UserData>();
            var filter = filterDefinitionBuilder.Eq(userData => userData.Id, id);

            var updateResult = await UserCollection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.IsAcknowledged;
        }

        public async Task<IEnumerable<UserData>> GetAsync(FilterDefinition<UserData> filter)
        {
            var result = await UserCollection.FindAsync<UserData>(filter);
            return result.ToList();
        }
    }
}