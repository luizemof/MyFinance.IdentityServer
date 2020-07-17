using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.Client
{
    public class ClientDataAccess : MongoDataAccess<ClientData>, IClientDataAccess
    {
        protected override string CollectionName => "Client";
        
        public ClientDataAccess(IMongoDatabase mongoDatabase) : base(mongoDatabase)
        {
        }
    }
}