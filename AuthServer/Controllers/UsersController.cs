using AuthServer.Models;
using IdentityModel;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthServer.Controllers
{
    [Route("[controller]/[action]")]
    public class UsersController : Controller
    {
        private UserManager<ApplicationUser> _userManager;
        private SignInManager<ApplicationUser> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
        public UsersController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        [HttpGet]
        public IActionResult Index()
        {
            UserRegisterViewModel model = new UserRegisterViewModel { };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Index(UserRegisterViewModel login)
        {
            var user = await _userManager.FindByNameAsync(login.UserName);
            if (null == user)
            {
                user = new ApplicationUser
                {
                    UserName = login.UserName,
                    Email = login.UserName,
                    CompanyId = 1,
                    DepartmentId = 1,
                    Age = 30,
                    Address = "江西省九江市",
                    Birthday = DateTime.Now.AddYears(-30),
                    IdCode = "360436043604",
                    IsAdmin = login.UserName == "443813032@qq.com",
                    DataEventRecordsRole = "dataEventRecords.admin",
                    SecuredFilesRole = ""
                };
                //var pass= _userManager.PasswordHasher.HashPassword(user, login.Password);
                // user.PasswordHash = pass;
                var result= await _userManager.CreateAsync(user, login.Password);
                //var claims = new[]
                //{
                //    new Claim(JwtClaimTypes.Subject, user.Id),
                //    new Claim(JwtClaimTypes.Name,user.UserName),
                //    new Claim(JwtClaimTypes.GivenName, user.UserName),
                //    new Claim(JwtClaimTypes.FamilyName, user.UserName),
                //    new Claim(JwtClaimTypes.Email,user.Email),
                //    //new Claim(JwtClaimTypes.Role,login.UserName=="443813032@qq.com"?"admin":"user")
                //};
                //result= await _userManager.AddClaimsAsync(user, claims);
                //result = await _userManager.AddToRoleAsync(user, login.UserName == "443813032@qq.com" ? "admin" : "user");
                //await _signInManager.PasswordSignInAsync(user, login.Password, false, false);
            }
            return Redirect("/");
        }


    }
}