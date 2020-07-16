// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer.Models.ApiScope;
using IdentityServer.Models.IdentityResource;
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
                    Name = "IdentityServerAdmin",
                    DisplayName = "Identity Server Administrator",
                    Description = "Scope for Identity Server Administrators"
                },
                new ApiScopeInputModel()
                {
                    Name = "MyFinanceApi",
                    DisplayName = "My Finance API",
                    Description = "Scope for My Finance API"
                }
            };

        public static IEnumerable<Client> Clients =>
            new Client[]
            {
                new Client
                {
                    ClientId = "client",

                    // no interactive user, use the clientid/secret for authentication
                    AllowedGrantTypes = GrantTypes.ClientCredentials,

                    // secret for authentication
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    // scopes that client has access to
                    AllowedScopes = { "MyFinanceApi" }
                },
            };

        public static List<TestUser> GetUsers()
        {
            return new List<TestUser>
            {
                new TestUser
                {
                    SubjectId = "1",
                    Username = "alice",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Alice"),
                        new Claim("website", "https://alice.com")
                    }
                },
                new TestUser
                {
                    SubjectId = "2",
                    Username = "bob",
                    Password = "password",

                    Claims = new List<Claim>
                    {
                        new Claim("name", "Bob"),
                        new Claim("website", "https://bob.com")
                    }
                }
            };
        }
    }
}