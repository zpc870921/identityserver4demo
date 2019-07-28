using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.AspNetCore.Cors;

namespace WebApi.Controllers
{
    [Route("api/[controller]")]
    [Route("[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private IHttpClientFactory _factory;
        public IdentityController(IHttpClientFactory factory)
        {
            _factory = factory;
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Get()
        {
            string access_token = await HttpContext.GetTokenAsync("access_token");
            string id_token = await HttpContext.GetTokenAsync("id_token");

            string token = HttpContext.Request.Headers["Authorization"].ToString();

            //var email = User.Claims.FirstOrDefault(c=>c.Type=="email")?.Value;
            IEnumerable<Claim> claims = null;
            using (HttpClient client = _factory.CreateClient())
            {
                client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", access_token);

                client.BaseAddress = new Uri("http://testauth.com");
                UserInfoResponse result = await client.GetUserInfoAsync(new UserInfoRequest
                {
                    Address = "/connect/userinfo",
                    Token = access_token
                });

                claims = result.Claims;
            }

            var email = claims.FirstOrDefault(c=>c.Type=="email")?.Value;
            return Ok(new[] { "v1", "v2", $"email:{email}" });
        }
    }
}