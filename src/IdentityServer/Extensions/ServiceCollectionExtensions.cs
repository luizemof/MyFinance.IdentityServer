using IdentityServer.Repository.ApiScopes;
using IdentityServer.Repository.Client;
using IdentityServer.Repository.IdentityResource;
using IdentityServer.Repository.Users;
using IdentityServer.Services;
using IdentityServer.Services.Account;
using IdentityServer.Services.ApiScope;
using IdentityServer.Services.Client;
using IdentityServer.Services.IdentityResource;
using IdentityServer.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;

namespace IdentityServer.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureDatabase(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration["DatabaseSettings:ConnectionString"];
            var databaseName = configuration["DatabaseSettings:DatabaseName"];

            var mongoClient = new MongoClient(connectionString);
            var mongoDatabase = mongoClient.GetDatabase(databaseName);

            services.AddSingleton<IMongoDatabase>(mongoDatabase);
        }

        public static void ConfigureDataAccess(this IServiceCollection services)
        {
            services.AddSingleton<IUserDataAccess, UserDataAccess>();
            services.AddSingleton<IApiScopeDataAccess, ApiScopeDataAccess>();
            services.AddSingleton<IIdentityResourceDataAccess, IdentityResourceDataAccess>();
            services.AddSingleton<IClientDataAccess, ClientDataAccess>();
        }

        public static void ConfigureServices(this IServiceCollection services)
        {
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IApiScopeService, ApiScopeService>();
            services.AddSingleton<IIdentityResourceService, IdentityResourceService>();
            services.AddSingleton<IClientService, ClientService>();
            services.AddSingleton<IAccountService, AccountService>();
        }

        public static void ConfigureIdentityServer(this IServiceCollection services)
        {
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMongoRepository()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants();

            
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", options =>
            {
                options.Authority = "https://localhost:5001";
                options.ClientId = "IdentityServer";
                options.ClientSecret = "secret";
                options.ResponseType = "code";
                options.SaveTokens = true;
                options.Scope.Add("IdentityServerAdmin");
            });


            // services.AddAuthorization(options =>
            // {
            //     options.AddPolicy("IdentityServerScope", policy =>
            //     {
            //         policy.RequireAuthenticatedUser();
            //         policy.RequireClaim("scope", "MyFinanceApi");
            //     });
            // });

            // not recommended for production - you need to store your key material somewhere secure
            // builder.AddDeveloperSigningCredential();
        }
    }
}