using IdentityServer.Repository.ApiScopes;
using IdentityServer.Repository.Client;
using IdentityServer.Repository.IdentityResource;
using IdentityServer.Repository.Users;
using IdentityServer.Services;
using IdentityServer.Services.Account;
using IdentityServer.Services.ApiScope;
using IdentityServer.Services.Client;
using IdentityServer.Services.IdentityResource;
using IdentityServer.Services.Profile;
using IdentityServer.Services.Users;
using IdentityServer4;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
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
                .AddPersistedGrants()
                .AddProfileService<ProfileService>();


            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            })
            .AddCookie("Cookies")
            .AddOpenIdConnect("oidc", "MyFinance Open Id Connect", options =>
            {
                options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                options.SaveTokens = true;
                
                options.Authority = "https://localhost:5001";
                options.ClientId = "IdentityServer";
                options.ClientSecret = "secret";
                options.ResponseType = "code";

                options.Scope.Add("IdentityServerAdmin");

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name",
                    RoleClaimType = "role"
                };
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