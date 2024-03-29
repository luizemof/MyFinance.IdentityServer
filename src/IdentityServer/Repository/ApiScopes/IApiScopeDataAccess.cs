using System.Collections.Generic;
using System.Threading.Tasks;
using IdentityServer.Repository.Mongo;
using MongoDB.Driver;

namespace IdentityServer.Repository.ApiScopes
{
    public interface IApiScopeDataAccess : IMongoDataAccess<ApiScopeData>
    {
    }
}