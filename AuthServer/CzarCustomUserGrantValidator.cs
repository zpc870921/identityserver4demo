using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.IServices;
using IdentityServer4.Models;
using IdentityServer4.Validation;

namespace AuthServer
{
    public class CzarCustomUserGrantValidator : IExtensionGrantValidator
    {
        public string GrantType => "CzarCustomUser";

        private readonly ICzarCustomUserServices czarCustomUserServices;

        public CzarCustomUserGrantValidator(ICzarCustomUserServices czarCustomUserServices)
        {
            this.czarCustomUserServices = czarCustomUserServices;
        }

        public Task ValidateAsync(ExtensionGrantValidationContext context)
        {
            var userName = context.Request.Raw.Get("czar_name");
            var userPassword = context.Request.Raw.Get("czar_password");

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userPassword))
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
            //校验登录
            var result = czarCustomUserServices.FindUserByuAccount(userName, userPassword);
            if (result == null)
            {
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant);
            }
            //添加指定的claims
            context.Result = new GrantValidationResult(
                subject: result.iid.ToString(),
                authenticationMethod: GrantType,
                claims: result.Claims);
            return Task.CompletedTask;
        }
    }
}
