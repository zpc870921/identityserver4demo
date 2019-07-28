using IdentityModel;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthServer
{
    public class CustomUserProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public CustomUserProfileService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            context.IssuedClaims.AddRange(context.Subject.Claims);
            var user = await _userManager.GetUserAsync(context.Subject);
            var roles = await _userManager.GetRolesAsync(user);
            context.IssuedClaims.AddRange(roles.Select(r => new System.Security.Claims.Claim(JwtClaimTypes.Role,r)));
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            return;
        }
    }
}
