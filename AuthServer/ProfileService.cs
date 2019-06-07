using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AuthServer.Models;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class ProfileService : IProfileService
    {
        private UserContext _userContext;
        public ProfileService(UserContext userContext)
        {
            _userContext = userContext;
        }
        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            try
            {
                //var claims = context.Subject.Claims.ToList();
                //set issued claims to return

                //depending on the scope accessing the user data.

                if (!string.IsNullOrWhiteSpace(context.Subject.Identity.Name))
                {
                    var user = _userContext.Users.FirstOrDefault(u => u.Id == Guid.Parse(context.Subject.Identity.Name));
                    if (null == user)
                    {
                        //context.IssuedClaims = claims.ToList();
                        return;
                    }
                    //获取user中的Claims
                    //claims = claims.Concat(GetUserClaims(user)).ToList();
                    var claims = ResourceOwnerPasswordValidator.GetUserClaims(user);
                    context.IssuedClaims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
                }
                else
                {
                    var userId = context.Subject.Claims.FirstOrDefault(x => x.Type == "sub")?.Value;
                    if (string.IsNullOrWhiteSpace(userId))
                    {
                        //context.IssuedClaims = claims.ToList();
                        return;
                    }
                    var user = _userContext.Users.FirstOrDefault(u => u.Id == Guid.Parse(userId));
                    if (null == user)
                    {
                        //context.IssuedClaims = claims.ToList();
                        return;
                    }
                    //获取user中的Claims
                    //claims = claims.Concat(GetUserClaims(user)).ToList();
                    var claims = ResourceOwnerPasswordValidator.GetUserClaims(user);
                    context.IssuedClaims = claims.Where(c => context.RequestedClaimTypes.Contains(c.Type)).ToList();
                }
            }
            catch (Exception ex)
            {
                //log your error
            }
        }

        //public Claim[] GetUserClaims(Users user)
        //{
        //    List<Claim> claims = new List<Claim>();
        //    var userClaims = _userContext.UserClaims.Where(c=>c.UsersId==user.Id).ToList();
        //    Claim claim;
        //    foreach (var itemClaim in userClaims)
        //    {
        //        claim = new Claim(itemClaim.Type, itemClaim.Value);
        //        claims.Add(claim);
        //    }
        //    return claims.ToArray();
        //}

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }
    }
}
