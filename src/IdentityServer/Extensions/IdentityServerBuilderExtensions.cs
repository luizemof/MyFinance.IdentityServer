using IdentityServer.Repository;
using IdentityServer.Repository.Mongo;
using IdentityServer.Store;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.Extensions.DependencyInjection;

namespace IdentityServer
{
    public static class IdentityServerBuilderExtensions
    {
        public static IIdentityServerBuilder AddMongoRepository(this IIdentityServerBuilder builder)
        {
            builder.Services.AddSingleton<IRepository, MongoRepository>();
            return builder;
        }

        public static IIdentityServerBuilder AddClients(this IIdentityServerBuilder builder)
        {
            builder.AddClientStore<CustomClientStore>();
            builder.AddCorsPolicyService<InMemoryCorsPolicyService>();
            return builder;
        }

        public static IIdentityServerBuilder AddIdentityApiResources(this IIdentityServerBuilder builder)
        {
            builder.AddResourceStore<CustomResourceStore>();
            return builder;
        }

        public static IIdentityServerBuilder AddPersistedGrants(this IIdentityServerBuilder builder)
        {
            builder.AddPersistedGrantStore<CustomPersistedGrantStore>();
            return builder;
        }
    }
}