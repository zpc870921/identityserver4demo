using IdentityServer4.Models;
using IdentityServer4.Test;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityServer4;

namespace AuthServer.Configuration
{
    public class InMemoryConfiguration
    {
        public static IEnumerable<ApiResource> GetApiResource()
        {
            return new List<ApiResource> {
                new ApiResource("socialnetwork","socialnetwork api"){
                     UserClaims={ "email","role","given_name","family_name","name","sub","UserId"},
                     ApiSecrets=
                     {
                         new Secret("secret".Sha256())
                     }
                }
            };
        }

        public static IEnumerable<Client> GetClients()
        {
            return new List<Client> {
                new Client(){
                    ClientId="userservices",
                    ClientName="自定义用户服务client",
                    ClientSecrets=new List<Secret> {
                        new Secret("secret".Sha256())
                    },
                    AllowedGrantTypes= new List<string>{
                        "customuserservice"
                    },
                    AccessTokenType= AccessTokenType.Jwt,
                    RequireConsent=false,
                    AccessTokenLifetime=900,
                    AllowOfflineAccess=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AbsoluteRefreshTokenLifetime=86400,
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "socialnetwork"
                    },
                },
                new Client
                {
                    ClientId = "winform.client",
                    ClientName = "a winform LAS.NET Client",
                    AllowedGrantTypes = GrantTypes.HybridAndClientCredentials,
                    PostLogoutRedirectUris={"http://localhost/winforms.client/signout-callback-oidc"},
                    FrontChannelLogoutUri="http://localhost/winforms.client/signout-oidc",
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },

                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.Phone,
                        "socialnetwork",
                        "roles"
                    },

                    RedirectUris = { "http://localhost/winforms.client" },
                    AllowAccessTokensViaBrowser=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AllowOfflineAccess = true,
                    RequireConsent = false,
                    RequirePkce=true
                },
                new Client
                {
                    ClientId = "xamarin.client",
                    ClientName = "eShop Xamarin OpenId Client",
                    AllowedGrantTypes = GrantTypes.Hybrid,
                    ClientSecrets =
                    {
                        new Secret("secret".Sha256())
                    },
                    //RequireClientSecret=false,
                    RedirectUris = { "http://127.0.0.1:50006" },
                    RequireConsent = false,
                    RequirePkce = true,
                    PostLogoutRedirectUris = { $"http://127.0.0.1:50006" },
                    AllowedCorsOrigins = { "http://127.0.0.1:50006" },
                    AllowedScopes = new List<string>
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        IdentityServerConstants.StandardScopes.OfflineAccess,
                        "socialnetwork",
                        "roles"
                    },
                    AllowOfflineAccess = true,
                    AllowAccessTokensViaBrowser = true,
                    Properties=new Dictionary<string,string>
                    {
                        ["jh_android_client"]="jh_android_client"
                    }
                },
                new Client()
                {
                    ClientId="native.code",
                    ClientName=".net core native console client",

                    RedirectUris={"http://127.0.0.1:50005"},
                    PostLogoutRedirectUris={"http://127.0.0.1:50005"},

                    RequireClientSecret =false,

                    AllowedGrantTypes =GrantTypes.Code,
                    RequirePkce=true,
                    AllowedScopes=
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "socialnetwork",
                        "roles"
                    },
                    AllowAccessTokensViaBrowser =true,
                    AllowOfflineAccess=true,
                    RefreshTokenUsage=TokenUsage.ReUse
                    //RequireConsent=false
                },
                new Client(){
                    ClientId="socialnetwork",
                    ClientName="socialnetwork client",
                    ClientSecrets={ new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.ClientCredentials,
                    AllowedScopes={ "socialnetwork"},
                    Claims=
                    {
                        new Claim("role","admin"),
                        new Claim("given_name","zpc2"),
                        new Claim("gender","female"),
                        new Claim("email","443813032@qq.com")
                    },
                    ClientClaimsPrefix="",
                    AllowAccessTokensViaBrowser=true,
                    AllowOfflineAccess=true
                },
                new Client()
                {
                    ClientId="ro.client",
                    ClientName="resource password client",
                    ClientSecrets={new Secret("secret".Sha256())},
                    AllowOfflineAccess=true,
                    Description="password pattern",
                    AccessTokenType=AccessTokenType.Jwt,
                    AllowedGrantTypes=GrantTypes.ResourceOwnerPasswordAndClientCredentials,
                    AllowedScopes=
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "socialnetwork"
                    },
                    Claims=
                    {
                        new Claim("role","admin"),
                        new Claim("email","443813032@qq.com"),
                        new Claim("given_name","zpc"),
                        new Claim("gender","male"),
                    },
                    ClientClaimsPrefix="",
                    AlwaysSendClientClaims=true
                },
                new Client(){
                    ClientId="mvc_implicit",
                    ClientName="mvcclient",
                    ClientSecrets ={ new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.Implicit,
                    FrontChannelLogoutUri="http://implicitmvc.com/signout-oidc",
                    AllowedScopes={
                        "socialnetwork",
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile
                    },
                    //RequireConsent=false,
                    RedirectUris={"http://implicitmvc.com/signin-oidc"},
                    PostLogoutRedirectUris={ "http://implicitmvc.com/signout-callback-oidc"},
                    AllowAccessTokensViaBrowser=true
                },
                new Client{
                    ClientId="mvc_code",
                    ClientName="mvc code client",
                    ClientSecrets={ new Secret("secret".Sha256())},
                    AllowedGrantTypes=GrantTypes.HybridAndClientCredentials,
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "socialnetwork",
                        "roles"
                    },
                    RequireConsent=false,
                    RedirectUris={ "http://hybridmvc.com/signin-oidc"},
                    FrontChannelLogoutUri= "http://hybridmvc.com/signout-oidc",
                    PostLogoutRedirectUris={ "http://hybridmvc.com/signout-callback-oidc"},
                    AllowOfflineAccess=true,
                    AllowAccessTokensViaBrowser=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                    AlwaysSendClientClaims=true,
                    ClientClaimsPrefix="",
                    Claims=
                    {
                        new Claim("gender","male")
                    }
                },
                new Client{
                     ClientId="js",
                     ClientName="Javascript Client",
                     AllowedGrantTypes=GrantTypes.Code,
                     RequirePkce=true,
                     RequireClientSecret=false,

                     RedirectUris={"http://localhost:5005/callback.html"},
                     PostLogoutRedirectUris={"http://localhost:5005/index.html"},
                     AllowedCorsOrigins={"http://localhost:5005"},
                     //AllowAccessTokensViaBrowser=true,
                     //AllowOfflineAccess=true,
                    // AccessTokenLifetime=60*10,
                     //AlwaysIncludeUserClaimsInIdToken=true,
                     AllowedScopes={
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        IdentityServerConstants.StandardScopes.Email,
                        "socialnetwork"
                    }
                }
            };
        }

        public static IEnumerable<IdentityResource> GetIdentityResource()
        {
            return new List<IdentityResource> {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
                new IdentityResources.Email(),
                new IdentityResource("roles","role",new List<string>{ "role"})
            };
        }



        public static List<TestUser> GetUsers()
        {
            return new List<TestUser> {
                new TestUser(){
                     Username="mail@qq.com",
                     Password="password",
                     SubjectId="3",
                     Claims={
                        new Claim("email","443813032@qq.com"),
                        new Claim("given_name","mail_givename"),
                        new Claim("family_name","mail_familyname"),
                        new Claim("role","admin")
                    }
                },
                new TestUser(){
                    Username="Nick",
                    SubjectId="1",
                    Password="password",
                    Claims={
                        new Claim("given_name","Nick"),
                        new Claim("family_name","Carter"),
                        new Claim("role","admin")
                    }
                },
                new TestUser(){
                    Username="Dave",
                    SubjectId="2",
                    Password="password",
                    Claims={
                        new Claim("given_name","Dave"),
                        new Claim("family_name","Mustaine"),
                        new Claim("role","user")
                    }
                }
            };
        }
    }
}
