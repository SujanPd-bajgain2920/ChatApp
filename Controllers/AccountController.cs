using ChatApp.Security;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using ChatApp.Models;

namespace LocationTrack.Controllers
{
    public class AccountController : Controller
    {
        private readonly ChatappContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly IDataProtector _protector;

        public AccountController(ChatappContext context, IWebHostEnvironment env, DataSecurityProvider key, IDataProtectionProvider provider)
        {
            _context = context;
            _env = env;
            _protector = provider.CreateProtector(key.Key);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(UserListEdit u)
        {
          // return Json(u);
            try
            {
                var users = _context.UserLists.Where(x => x.EmailAddress == u.EmailAddress).FirstOrDefault();
                if (users == null)
                {
                    short maxid;
                    if (_context.UserLists.Any())
                        maxid = Convert.ToInt16(_context.UserLists.Max(x => x.UserId) + 1);
                    else
                        maxid = 1;
                    u.UserId = maxid;


                    UserList userList = new()
                    {
                        UserId = u.UserId,
                        FullName = u.FullName,
                       
                        EmailAddress = u.EmailAddress,
                       
                        UserPassword = _protector.Protect(u.UserPassword),
                        Latitude = null,   // Latitude initially null
                        Longitude = null,  // Longitude initially null
                        LastUpdated = null // LastUpdated initially null

                    };
                    //return Json(userList);
                    _context.Add(userList);
                    _context.SaveChanges();
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    ModelState.AddModelError("", "User already exists with this email!");
                    return View(u);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"User Registration Failed: {ex.Message}");
                return View(u);
            }
        }


       

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserListEdit uEdit)
        {
            var users = _context.UserLists.ToList();
            if (users != null)
            {

                var u = users.Where(x => x.EmailAddress.ToUpper().Equals(uEdit.EmailAddress.ToUpper()) && _protector.Unprotect(x.UserPassword).Equals(uEdit.UserPassword)).FirstOrDefault();
                if (u != null)
                {
                    List<Claim> claims = new()
                    {
                        new Claim(ClaimTypes.Name,u.UserId.ToString()),
                        
                        new Claim("EmpName",u.FullName),
                        
                        new Claim("email",u.EmailAddress),
                    };

                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(identity));

                    return RedirectToAction("Dashboard");

                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid User");

            }
            return View(uEdit);
        }

        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        [Authorize]
        public IActionResult Dashboard()
        {
            return RedirectToAction("Index", "Home");

        }
    }
}
