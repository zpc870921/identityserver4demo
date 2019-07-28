using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcClient.Models;

namespace MvcClient.Controllers
{
    
    public class HomeController : Controller
    {
        private IHttpClientFactory _factory;
        public HomeController(IHttpClientFactory httpClientFactory)
        {
            //_client = httpClientFactory.CreateClient();
            _factory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ajaxtest()
        {
            return Json(new
            {
                name="zpc",
                age=20
            });
        }

        [Authorize]
        public async Task<IActionResult> About()
        {
            var token =await HttpContext.GetTokenAsync("access_token");
            var _client = _factory.CreateClient();
            _client.BaseAddress=new Uri("http://testapi.com");
            _client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token}");
            var data = await _client.GetAsync("api/values").Result.Content.ReadAsAsync<List<string>>();

            _client = _factory.CreateClient();
            _client.BaseAddress = new Uri("http://testauth.com");
            _client.DefaultRequestHeaders.Add("Authorization",$"Bearer {token}");
            var userinfo=await _client.GetAsync("/connect/userinfo").Result.Content.ReadAsAsync<object>();


            return View(new ValuesViewModel {
                 Name="about",
                  Values=data
            });
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpGet]
        public IActionResult Logout()
        {
            //await HttpContext.SignOutAsync("Cookies");
            //await HttpContext.SignOutAsync("oidc");
            return SignOut("Cookies","oidc");
        }
    }
}
