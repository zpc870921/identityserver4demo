using AuthServer.Configuration;
using IdentityModel;
using IdentityServer4;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Mappers;
//using IdentityServer4.EntityFramework.DbContexts;
//using IdentityServer4.EntityFramework.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace AuthServer
{
    public class Startup
    {
        public IConfiguration Configuration { get; set; }
        public Startup(IConfiguration configuration)
        {
            this.Configuration = configuration;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddHttpClient();

            services.AddDbContext<UserContext>(options => { options.UseMySql(Configuration.GetConnectionString("default")); });

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseMySql(Configuration.GetConnectionString("default"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
                options.Password.RequiredUniqueChars = 2;
                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
                // Signin settings
                options.SignIn.RequireConfirmedEmail = false;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                // User settings
                options.User.RequireUniqueEmail = false;
            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();
            //.AddClaimsPrincipalFactory<UserClaimsPrincipal>()

            //services.Configure<IISOptions>(iis =>
            //{
            //    iis.AuthenticationDisplayName = "Windows";
            //    iis.AutomaticAuthentication = true;
            //});



            string connectionString = this.Configuration.GetConnectionString("default");
            string migrationAssembly = typeof(Startup).Assembly.GetName().Name;


            services.AddIdentityServer(options =>
                {
                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseSuccessEvents = true;

                    //添加自定义端点endpoint
                    options.Discovery.CustomEntries.Add("my_setting", "foo");
                    options.Discovery.CustomEntries.Add("my_complex_setting",
                        new
                        {
                            foo = "foo",
                            bar = "bar"
                        });
                    options.Discovery.CustomEntries.Add("my_custom_endpoint", "~/user");

                    //设置client/resource的缓存时间
                    options.Caching.ClientStoreExpiration = TimeSpan.FromHours(5);
                    options.Caching.ResourceStoreExpiration = TimeSpan.FromHours(5);

                    options.Events.RaiseErrorEvents = true;
                    options.Events.RaiseFailureEvents = true;
                    options.Events.RaiseInformationEvents = true;
                    options.Events.RaiseSuccessEvents = true;
                }).AddDeveloperSigningCredential()
                //.AddInMemoryIdentityResources(InMemoryConfiguration.GetIdentityResource())
                //.AddInMemoryApiResources(InMemoryConfiguration.GetApiResource())
                //.AddInMemoryClients(InMemoryConfiguration.GetClients())
                //.AddClientStore<CustomClientStore>()

                //.AddResourceOwnerValidator<ResourceOwnerPasswordValidator>()
                //.AddProfileService<ProfileService>()
            //.AddTestUsers(InMemoryConfiguration.GetUsers());
            .AddOperationalStore(options =>
             {
                 options.ConfigureDbContext = builder =>
                 {
                     builder.UseMySql(connectionString, sql =>
                     {
                         sql.MigrationsAssembly(migrationAssembly);
                     });
                 };
                 //启用db过期数据自动清除，间隔1小时（3600秒）
                 options.EnableTokenCleanup = true;
                 options.TokenCleanupInterval = 3600;
             })
            .AddConfigurationStore(options =>
            {
                options.ConfigureDbContext = builder =>
                {
                    builder.UseMySql(connectionString, sql =>
                    {
                        sql.MigrationsAssembly(migrationAssembly);
                    });
                };
            }).AddExtensionGrantValidator<CustomUserService>()
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication().AddGoogle("Google", options =>
                {
                    options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                    options.ClientId = "323340473202-tttjha8qaa2q7892fiffof3nddrnsp15.apps.googleusercontent.com";
                    options.ClientSecret = "AD9lAQVSWKgZpzm87pmXWJ5r";
                }).AddOpenIdConnect("oidc", "Openid Connect", options =>
                 {
                     options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                     options.SignOutScheme = IdentityServerConstants.SignoutScheme;
                     options.SaveTokens = true;

                     options.Authority = "https://demo.identityserver.io";
                     options.ClientId = "implicit";

                     options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                     {
                         NameClaimType = JwtClaimTypes.Name,
                         RoleClaimType = JwtClaimTypes.Role
                     };
                 });

            services.AddMvc();
        }


        private void InitDatabase(IApplicationBuilder app)
        {
            using (IServiceScope scope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                ApplicationDbContext applicationdb = scope.ServiceProvider.GetService<ApplicationDbContext>();
                applicationdb.Database.Migrate();
                if (!applicationdb.Roles.Any())
                {
                    applicationdb.Roles.AddRange(new[]
                    {
                        new IdentityRole("admin")
                        {
                             NormalizedName="ADMIN"
                        },
                        new IdentityRole("user")
                        {
                             NormalizedName="USER"
                        }
                    });
                    applicationdb.SaveChanges();
                }
                //if (!applicationdb.Users.Any())
                //{
                //    applicationdb.Users.Add(new ApplicationUser
                //    {
                //        Email = "443813032@qq.com",
                //        DepartmentId = 1,
                //        EmailConfirmed = false,
                //        IdCode = "360436043604",
                //        LockoutEnabled = false,
                //        NormalizedEmail = "443813032@QQ.COM",
                //        CompanyId = 1,
                //        PhoneNumber = "15313150410",
                //        UserName = "zpc870921",
                //        TwoFactorEnabled = false,
                //        Birthday = DateTime.Now.AddYears(30),
                //        AccessFailedCount = 0,
                //        Address = "江西省九江市",
                //        Age = 30,
                //        NormalizedUserName = "ZPC870921"
                //    });
                //    applicationdb.SaveChanges();
                //}
                

                scope.ServiceProvider.GetService<PersistedGrantDbContext>().Database.Migrate();
                ConfigurationDbContext context = scope.ServiceProvider.GetService<ConfigurationDbContext>();
                context.Database.Migrate();
                context.Database.EnsureCreated();
                if (!context.Clients.Any())
                {
                    foreach (IdentityServer4.Models.Client item in InMemoryConfiguration.GetClients())
                    {
                        context.Clients.Add(item.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.ApiResources.Any())
                {
                    foreach (IdentityServer4.Models.ApiResource item in InMemoryConfiguration.GetApiResource())
                    {
                        context.ApiResources.Add(item.ToEntity());
                    }
                    context.SaveChanges();
                }
                if (!context.IdentityResources.Any())
                {
                    foreach (IdentityServer4.Models.IdentityResource item in InMemoryConfiguration.GetIdentityResource())
                    {
                        context.IdentityResources.Add(item.ToEntity());
                    }
                    context.SaveChanges();
                }
            }
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            InitDatabase(app);
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseStaticFiles();
            app.UseDefaultFiles();
            app.UseIdentityServer();
            app.UseMvc(routes =>
            {
                routes.MapRoute(name:"default",template:"/{controller=home}/{action=index}/{id?}");
            });
        }
    }
}
