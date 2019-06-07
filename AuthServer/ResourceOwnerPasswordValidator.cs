using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Models;
using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Validation;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class ResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private UserContext _dbcontext;
        public ResourceOwnerPasswordValidator(UserContext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        public async Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            //根据context.UserName和context.Password与数据库的数据做校验，判断是否合法
            var user = _dbcontext.Users.Include(u=>u.UserClaims).FirstOrDefault(u=>u.UserName==context.UserName&&u.Password==context.Password);
            if(user!=null)
            {
                context.Result = new GrantValidationResult(
                    subject: context.UserName,
                    authenticationMethod: "custom",
                    claims: GetUserClaims(user));
            }
            else
            {

                //验证失败
                context.Result = new GrantValidationResult(TokenRequestErrors.InvalidGrant, "invalid custom credential");
            }
        }
        //可以根据需要设置相应的Claim
        public static Claim[] GetUserClaims(Users user)
        {
            return new []
            {
               // new Claim(JwtClaimTypes.Subject, user.Id.ToString()),
                new Claim(JwtClaimTypes.Name,user.UserName),
                new Claim(JwtClaimTypes.GivenName, user.UserName),
                new Claim(JwtClaimTypes.FamilyName, user.UserName),
                new Claim(JwtClaimTypes.Email,user.Email )
               // new Claim(JwtClaimTypes.Role,user.Role)
            };
        }
    }
}
