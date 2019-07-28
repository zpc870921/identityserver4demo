using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace MvcClient
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
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddHttpClient();

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "oidc";
            }).AddCookie("Cookies", options =>
                {
                    options.Cookie.Expiration = TimeSpan.FromHours(12);
                    options.ExpireTimeSpan = TimeSpan.FromHours(12);
                })
            .AddOpenIdConnect("oidc",options=> {
                options.SignInScheme = "Cookies";
                options.Authority = "http://testauth.com"; 
                options.RequireHttpsMetadata = false;
                options.SaveTokens = true;
                options.ResponseType = "id_token token";
                options.ClientId = "mvc_implicit";
                options.ProtocolValidator=new OpenIdConnectProtocolValidator()
                {
                    NonceLifetime = TimeSpan.FromHours(12),
                    RequireStateValidation = false
                };
                options.Events.OnTicketReceived = context =>
                {
                    context.Properties.IsPersistent = true;
                    context.Properties.ExpiresUtc = DateTimeOffset.UtcNow.AddHours(12);
                    return Task.CompletedTask;
                };
                //options.ClientSecret = "secret";
                //options.GetClaimsFromUserInfoEndpoint = true;
                options.Scope.Add("socialnetwork");
                options.ClaimActions.Remove("nbf");
                options.ClaimActions.Remove("amr");
                options.ClaimActions.DeleteClaim("idp");
            });

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
