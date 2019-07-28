using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc.Authorization;
using StackExchange.Redis;

namespace WebApi
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
            services.AddHttpClient();

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("doc1", new Swashbuckle.AspNetCore.Swagger.Info
                {
                    Description = "",
                    Contact = new Swashbuckle.AspNetCore.Swagger.Contact { },
                    Title = "my api",
                    Version = "v1"
                });
            });
           

            services.AddCors(options =>
            {
                options.AddPolicy("corstest",policy =>
                {
                    policy.AllowAnyOrigin()//WithOrigins("http://localhost:5005")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            services.AddAuthentication("Bearer").AddIdentityServerAuthentication(options =>
            {
                options.Authority = "http://testauth.com";
                options.ApiName = "dataEventRecords"; //"socialnetwork";
                options.ApiSecret = "dataEventRecordsSecret";// "secret";
                options.EnableCaching = true;
                options.CacheDuration = TimeSpan.FromMinutes(10);
                options.RequireHttpsMetadata = false;
            });

            var guestPolicy=new AuthorizationPolicyBuilder().RequireAuthenticatedUser()
                .RequireClaim("scope", "dataEventRecords").Build();

            services.AddAuthorization(options =>
            {
                options.AddPolicy("dataEventRecordsAdmin", policy => { policy.RequireClaim("role", "dataEventRecords.admin"); });
                options.AddPolicy("dataEventRecordsUser", policy => { policy.RequireClaim("role", "dataEventRecords.user"); });
                options.AddPolicy("dataEventRecords", policy => { policy.RequireClaim("scope", "dataEventRecords"); });
            });

            var redis = ConnectionMultiplexer.Connect("localhost:6379");
            services.AddDataProtection()
                .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys").SetApplicationName("testapi");

            //services.AddAuthorization();
            services.AddMvc(options => { options.Filters.Add(new AuthorizeFilter(guestPolicy)); }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/doc1/swagger.json", "doc1");
            });
            app.UseCors("corstest");
            //app.UseCors();
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
