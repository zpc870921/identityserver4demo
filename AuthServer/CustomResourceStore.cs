using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.EntityFramework.Mappers;
using IdentityServer4.Models;
using IdentityServer4.Stores;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    //public class CustomResourceStore : IResourceStore
    //{
    //    private UserContext _dbContext;
    //    public CustomResourceStore(UserContext dbContext)
    //    {
    //        _dbContext = dbContext;
    //    }

    //    public async Task<ApiResource> FindApiResourceAsync(string name)
    //    {
    //        var apiResource=await _dbContext.ApiResources.FirstOrDefaultAsync(r=>r.Name==name);
    //        return apiResource.ToModel();
    //    }

    //    public async Task<IEnumerable<ApiResource>> FindApiResourcesByScopeAsync(IEnumerable<string> scopeNames)
    //    {
    //        var apiResources = await _dbContext.ApiResources.Where(r=>r.Scopes.Contains());
    //    }

    //    public async Task<IEnumerable<IdentityResource>> FindIdentityResourcesByScopeAsync(IEnumerable<string> scopeNames)
    //    {
    //        throw new NotImplementedException();
    //    }

    //    public async Task<Resources> GetAllResourcesAsync()
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
