using System;
using IdentityServer.Cryptography;
using IdentityServer.Extensions;
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
            services.AddControllersWithViews();

            services.AddSingleton(new DatabaseSettings() { ConnectionString = "mongodb://localhost:27017", DatabaseName = "IdentityServer" });
            services.AddSingleton(new IdentityServerCryptography(Configuration["EncryptKey"]));

            services.ConfigureDatabase(Configuration);
            services.ConfigureDataAccess();
            services.ConfigureServices();
            services.ConfigureIdentityServer();
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
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
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
                var apiScopeService = app.ApplicationServices.GetService<Services.IApiScopeService>();
                foreach (var api in Config.ApiScopes)
                {
                    apiScopeService.InsertApiScopeAsync(api).GetAwaiter().GetResult();
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
