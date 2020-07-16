using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.IdentityResource
{
    public class IdentityResourceDataAccess : MongoDataAccess<IdentityResourceData>, IIdentityResourceDataAccess
    {
        protected override string CollectionName => "IdentityResource";

        public IdentityResourceDataAccess(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var keys = Builders<IdentityResourceData>.IndexKeys.Ascending("Name");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var model = new CreateIndexModel<IdentityResourceData>(keys, indexOptions);
            Collection.Indexes.CreateOne(model);
        }
    }
}