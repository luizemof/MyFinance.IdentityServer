using IdentityServer.Repository.Mongo;

namespace IdentityServer.Repository.IdentityResource
{
    public interface IIdentityResourceDataAccess : IMongoDataAccess<IdentityResourceData>
    {
         
    }
}