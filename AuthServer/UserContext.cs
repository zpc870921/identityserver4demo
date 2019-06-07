using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuthServer.Models;
using IdentityServer4.EntityFramework.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthServer
{
    public class UserContext:DbContext
    {


        public UserContext(DbContextOptions<UserContext> options):base(options)
        {
            
        }

        public DbSet<Users> Users { get; set; }
        public DbSet<UserClaims> UserClaims { get; set; }

        public DbSet<ApiResource> ApiResources { get; set; }
        public DbSet<IdentityResource> IdentityResources { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //}
    }
}
