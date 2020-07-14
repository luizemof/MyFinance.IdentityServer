using System;
using IdentityServer.Repository;
using IdentityServer4.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MongoDB.Bson.Serialization;

namespace IdentityServer
{
    public class Startup
    {
        public IWebHostEnvironment Environment { get; }
        public readonly IConfiguration Configuration;

        public Startup(IWebHostEnvironment environment)
        {
            Environment = environment;
            var builder = new ConfigurationBuilder()
                            .SetBasePath(environment.ContentRootPath)
                            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                            .AddJsonFile($"appsettings.{environment}.json", optional: true)
                            .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // uncomment, if you want to add an MVC-based UI
            services.AddControllersWithViews();

            services.AddSingleton(new DatabaseSettings(){ ConnectionString = "mongodb://localhost:27017", DatabaseName = "IdentityServer"});

            // ---  configure identity server with MONGO Repository for stores, keys, clients and scopes ---
            services.AddIdentityServer()
                .AddDeveloperSigningCredential()
                .AddMongoRepository()
                .AddClients()
                .AddIdentityApiResources()
                .AddPersistedGrants()
                .AddTestUsers(Config.GetUsers());

            
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

        public void Configure(IApplicationBuilder app)
        {
            if (Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseIdentityServer();

            ConfigureMongoDriver2IgnoreExtraElements();

            InitializeDatabase(app);

            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
               endpoints.MapDefaultControllerRoute();
            });
        }

        private static void InitializeDatabase(IApplicationBuilder app)
        {
            bool createdNewRepository = false;
            var repository = app.ApplicationServices.GetService<IRepository>();

            //  --Client
            if (!repository.CollectionExists<Client>())
            {
                foreach (var client in Config.Clients)
                {
                    repository.Add<Client>(client);
                }
                createdNewRepository = true;
            }

            //  --IdentityResource
            if (!repository.CollectionExists<IdentityResource>())
            {
                foreach (var res in Config.IdentityResources)
                {
                    repository.Add<IdentityResource>(res);
                }
                createdNewRepository = true;
            }


            //  --ApiResource
            if (!repository.CollectionExists<ApiScope>())
            {
                foreach (var api in Config.ApiScopes)
                {
                    repository.Add<ApiScope>(api);
                }
                createdNewRepository = true;
            }

            // If it's a new Repository (database), need to restart the website to configure Mongo to ignore Extra Elements.
            if (createdNewRepository)
            {
                var newRepositoryMsg = $"Mongo Repository created/populated! Please restart you website, so Mongo driver will be configured  to ignore Extra Elements - e.g. IdentityServer \"_id\" ";
                throw new Exception(newRepositoryMsg);
            }
        }

        /// <summary>
        /// Configure Classes to ignore Extra Elements (e.g. _Id) when deserializing
        /// As we are using "IdentityServer4.Models" we cannot add something like "[BsonIgnore]"
        /// </summary>
        private static void ConfigureMongoDriver2IgnoreExtraElements()
        {
            BsonClassMap.RegisterClassMap<Client>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<IdentityResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ApiResource>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<PersistedGrant>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });

            BsonClassMap.RegisterClassMap<ApiScope>(cm =>
            {
                cm.AutoMap();
                cm.SetIgnoreExtraElements(true);
            });
        }
    }
}
