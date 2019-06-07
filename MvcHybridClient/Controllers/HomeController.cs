using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using MvcHybridClient.Models;
using static IdentityModel.OidcConstants;

namespace MvcHybridClient.Controllers
{
    public class HomeController : Controller
    {
        private IHttpClientFactory _factory;
        public HomeController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        public IActionResult Index()
        {
            return View();
        }

        [Authorize]//(Roles = "admin")
        public IActionResult About()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public async Task refresh_token()
        {
            var client = _factory.CreateClient();
            var disco = await client.GetDiscoveryDocumentAsync("http://localhost:5000");
            if (disco.IsError) throw new Exception(disco.Error);


            var id_token =await HttpContext.GetTokenAsync("id_token");
            var refresh_token = await HttpContext.GetTokenAsync("refresh_token");
            var tokenResponse= await client.RequestRefreshTokenAsync(new RefreshTokenRequest
            {
                 Address=disco.TokenEndpoint,
                 ClientId="mvc_code",
                 ClientSecret="secret",
                 RefreshToken=refresh_token
            });
            var expires_at =DateTime.Now+ TimeSpan.FromSeconds(tokenResponse.ExpiresIn);
            var tokens = new[] {
                new AuthenticationToken(){
                     Name=OpenIdConnectParameterNames.IdToken,
                     Value=id_token
                },
                new AuthenticationToken(){
                     Name=OpenIdConnectParameterNames.AccessToken,
                     Value=tokenResponse.AccessToken
                },
                new AuthenticationToken(){
                     Name=OpenIdConnectParameterNames.RefreshToken,
                     Value=tokenResponse.RefreshToken
                },
                new AuthenticationToken(){
                     Name="expires_at",
                     Value=expires_at.ToString()
                }
            };

           var authenticateInfo = await HttpContext.AuthenticateAsync("Cookies");
            authenticateInfo.Properties.StoreTokens(tokens);
            await HttpContext.SignInAsync("Cookies", authenticateInfo.Principal, authenticateInfo.Properties);
        }

        public async Task<IActionResult> GetIdentity()
        {
            await refresh_token();

            var client = _factory.CreateClient();
            client.BaseAddress = new Uri("http://localhost:5001");
            var token = await HttpContext.GetTokenAsync("access_token");
            client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token}");
            var data=await client.GetStringAsync("api/identity");
            return Content(data);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult logout()
        {
            //await HttpContext.SignOutAsync("Cookies");
            //await HttpContext.SignOutAsync("oidc");
            return SignOut("Cookies", "oidc");
        }
    }
}
