using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using IdentityModel;
using IdentityModel.AspNetCore;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace MvcHybridClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => false;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });
            services.AddHttpClient();


            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies",options=> {
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.Cookie.Name = "mvchybridautorefresh";
                options.AccessDeniedPath = "/authorization/accessdenied";
            }).AddAutomaticTokenManagement()
            .AddOpenIdConnect("oidc",options=> {
                options.Authority = "http://testauth.com";
                options.ClientId = "mvc_code";
                options.ClientSecret = "secret";
                options.SignInScheme = "Cookies";
                options.RequireHttpsMetadata = false;
                options.ResponseType = "id_token code";
                options.SaveTokens = true;
                options.Scope.Add("openid");
                options.Scope.Add("profile");
                options.Scope.Add("socialnetwork");
                options.Scope.Add("dataEventRecords");
                options.Scope.Add("offline_access");
                options.Scope.Add("email");
                //options.Scope.Add("roles");

                options.GetClaimsFromUserInfoEndpoint = true;
                //options.ClaimActions.MapUniqueJsonKey("role","role");

                options.ClaimActions.MapAllExcept("iss", "nbf", "exp", "aud", "nonce", "iat", "c_hash");

                options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                     NameClaimType=JwtClaimTypes.Name,
                     RoleClaimType=JwtClaimTypes.Role
                };
            });
            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys").SetApplicationName("hybridmvc");

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
