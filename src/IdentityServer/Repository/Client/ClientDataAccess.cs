using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.Client
{
    public class ClientDataAccess : MongoDataAccess<ClientData>, IClientDataAccess
    {
        protected override string CollectionName => "Client";

        public ClientDataAccess(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
            CreateIndexes();
        }

        private void CreateIndexes()
        {
            var keys = Builders<ClientData>.IndexKeys.Ascending("ClientId");
            var indexOptions = new CreateIndexOptions { Unique = true };
            var model = new CreateIndexModel<ClientData>(keys, indexOptions);
            Collection.Indexes.CreateOne(model);
        }
    }
}