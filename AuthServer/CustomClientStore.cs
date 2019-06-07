using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace AuthServer
{
    public class CustomClientStore : IClientStore
    {
        public static List<Client> ClientList = new List<Client>()
        {
             new Client()
             {
                  ClientId="client1",
                  ClientName="first client",
                  ClientSecrets=
                  {
                      new Secret("secret".Sha256())
                  },
                  AllowedScopes=
                  {
                      IdentityServerConstants.StandardScopes.OpenId,
                      IdentityServerConstants.StandardScopes.Profile,
                      "socialnetwork"
                  },
                  AllowedGrantTypes=GrantTypes.Hybrid,
                  AllowOfflineAccess=true,
                  AllowAccessTokensViaBrowser=true,
                  RedirectUris={"http://localhost:5004"},
                  PostLogoutRedirectUris={"http://localhost:5004"}
             },
             new Client(){
                 ClientId="mvc_implicit",
                 ClientName="mvcclient",
                 ClientSecrets ={ new Secret("secret".Sha256())},
                 AllowedGrantTypes=GrantTypes.Implicit,
                 AllowedScopes={
                     "socialnetwork",
                     IdentityServerConstants.StandardScopes.OpenId,
                     IdentityServerConstants.StandardScopes.Profile
                 },
                 //RequireConsent=false,
                 RedirectUris={ "http://localhost:5002/signin-oidc"},
                 PostLogoutRedirectUris={ "http://localhost:5002/signout-callback-oidc"},
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
                 //RequireConsent=false,
                 RedirectUris={ "http://localhost:5003/signin-oidc"},
                 PostLogoutRedirectUris={ "http://localhost:5003/signout-callback-oidc"},
                 AllowOfflineAccess=true,
                 AllowAccessTokensViaBrowser=true,
                 AlwaysIncludeUserClaimsInIdToken=true,
                 AlwaysSendClientClaims=true,
                 ClientClaimsPrefix="",
                 Claims=
                 {
                     new Claim("gender","male")
                 }
             }
        };
        public async Task<Client> FindClientByIdAsync(string clientId)
        {
            return ClientList.FirstOrDefault(c=>c.ClientId==clientId);
        }
    }
}
