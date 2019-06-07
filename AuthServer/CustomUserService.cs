using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AuthServer
{
    public class CustomUserService : IExtensionGrantValidator
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public CustomUserService(IHttpClientFactory httpClientFactory,UserManager<ApplicationUser> userManager,SignInManager<ApplicationUser> signInManager)
        {
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public string GrantType => "customuserservice";

        public async Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var model = new userLoginDto
            {
                phoneNumber = context.Request.Raw["Phone"],
                passWord = context.Request.Raw["PassWord"]
            };
            //var client = _httpClientFactory.CreateClient("userApi");
            //var response = await client.PostAsJsonAsync("/api/userLogin/login", model);//调用服务接口进行密码验证
            //response.EnsureSuccessStatusCode();
            //if (response.IsSuccessStatusCode)
            //{
            //    string operatorT = await response.Content.ReadAsStringAsync();
            //    var result = JsonConvert.DeserializeObject<OperatorResult>(operatorT);
            //    if (result.Result == ResultType.Success)
            //    {
            //        var user = JsonConvert.DeserializeObject<UserInfo>(result.Data.ToString());
            //        List<Claim> list = new List<Claim>();
            //        list.Add(new Claim("username", user.UserName ?? ""));
            //        list.Add(new Claim("role", string.IsNullOrEmpty(user.Role) ? "" : user.Role));
            //        list.Add(new Claim("realname", string.IsNullOrEmpty(user.RealName) ? "" : user.RealName));
            //        list.Add(new Claim("company", string.IsNullOrEmpty(user.Company) ? "" : user.Company));
            //        list.Add(new Claim("roleid", string.IsNullOrEmpty(user.RoleId) ? "" : user.RoleId));
            //        context.Result = new GrantValidationResult(subject: user.Id.ToString(), authenticationMethod: GrantType, claims: list);
            //    }
            //    else
            //    {
            //        context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, result.Message);
            //    }
            //}
            //else
            //    context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "用户名密码错误");

            var result = await _signInManager.PasswordSignInAsync(model.phoneNumber, model.passWord, false, false);
            if (null == result||!result.Succeeded)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant,"用户名或密码错误");
            }

            var user = await _userManager.FindByNameAsync(model.phoneNumber);

            var claims = await _userManager.GetClaimsAsync(user);
                context.Result = new GrantValidationResult(subject: user.Id, authenticationMethod: GrantType, claims: claims);
            

            await Task.CompletedTask;
        }
    }
}
