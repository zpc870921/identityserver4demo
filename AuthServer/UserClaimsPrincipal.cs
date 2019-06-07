using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Threading.Tasks;
using IdentityServer4;

namespace AuthServer
{
    //public class UserClaimsPrincipal : IUserClaimsPrincipalFactory<IdentityUser>
    //{
    //    private readonly IUserStoreService _storeService;
    //    public UserClaimsPrincipal(IUserStoreService storeService)
    //    {
    //        _storeService = storeService;
    //    }
    //    public async Task<ClaimsPrincipal> CreateAsync(IdentityUser user)
    //    {
    //        var claims = await _storeService.GetAllClaimsByUser(user);
    //        ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims);
    //        ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
    //        return await Task.FromResult(claimsPrincipal);

    //    }
    //}
}