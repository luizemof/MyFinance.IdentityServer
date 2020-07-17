// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Models.ApiScope;
using IdentityServer.Models.Client;
using IdentityServer.Models.IdentityResource;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Security.Claims;

namespace IdentityServer
{
    public static class Config
    {
        public static IEnumerable<IdentityResourceInputModel> IdentityResources
        {
            get
            {
                var openId = new IdentityResources.OpenId();
                var profile = new IdentityResources.Profile();
                var openIdModel = new IdentityResourceInputModel()
                {
                    Name = openId.Name,
                    DisplayName = openId.DisplayName,
                    Description = openId.Description,
                    UserClaims = openId.UserClaims
                };
                var profileModel = new IdentityResourceInputModel()
                {
                    Name = profile.Name,
                    DisplayName = profile.DisplayName,
                    Description = profile.Description,
                    UserClaims = profile.UserClaims
                };

                return new IdentityResourceInputModel[]
                {
                    openIdModel,
                    profileModel
                };
            }

        }

        public static IEnumerable<ApiScopeInputModel> ApiScopes =>
            new ApiScopeInputModel[]
            {
                new ApiScopeInputModel()
                {
                    Name = "IdentityServerApiSystem",
                    DisplayName = "Identity Server Api System",
                    Description = "Escopo para a API do Sistema IdentityServer"
                },
                new ApiScopeInputModel()
                {
                    Name = "MyFinanceApi",
                    DisplayName = "Minhas Finanças API",
                    Description = "Escopo para a API do Minha Finanças"
                }
            };

        public static IEnumerable<ClientInputModel> Clients
        {
            get
            {
                var client1 = new ClientInputModel
                {
                    ClientId = "client",
                    ClientSecret = "secret",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    AllowedScopes = new List<string>()
                    {
                        "MyFinanceApi"
                    }
                };

                var client2 = new ClientInputModel
                {
                    ClientId = "MyFinanceIdentityServer",
                    ClientSecret = "secret",
                    AllowedGrantTypes = GrantTypes.Code,
                    RedirectUrl = "https://localhost:5001/signin-oidc",
                    PostLogoutRedirectUrl = "https://localhost:5001/signout-callback-oidc",
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "IdentityServerApiSystem"
                    }
                };

                return new[] { client1, client2 };
            }
        }
    }
}