using IdentityServer.Repository.Mongo;

namespace IdentityServer.Repository.Client
{
    public interface IClientDataAccess : IMongoDataAccess<ClientData>
    {
         
    }
}