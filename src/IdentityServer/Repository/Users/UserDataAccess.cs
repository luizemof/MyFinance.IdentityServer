using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.Users
{
    public class UserDataAccess : MongoDataAccess<UserData>, IUserDataAccess
    {
        protected override string CollectionName => "UserCollection";

        public UserDataAccess(IMongoDatabase database) : base(database)
        {
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var keys = Builders<UserData>.IndexKeys.Ascending("Email");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var model = new CreateIndexModel<UserData>(keys, indexOptions);
            Collection.Indexes.CreateOne(model);
        }

        public Task<UserData> DeleteAscyn(string id)
        {
            return Collection.FindOneAndDeleteAsync<UserData>(data => data.Id == id);
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
            var replaceResult = await Collection.ReplaceOneAsync(filter, user);

            return replaceResult.IsAcknowledged ? user : default(UserData);
        }

        public async Task<bool> UpdateAsync(string id, UpdateDefinition<UserData> updateDefinition)
        {
            var filterDefinitionBuilder = new FilterDefinitionBuilder<UserData>();
            var filter = filterDefinitionBuilder.Eq(userData => userData.Id, id);

            var updateResult = await Collection.UpdateOneAsync(filter, updateDefinition);

            return updateResult.IsAcknowledged;
        }
    }
}