using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net6WebApp.Entities;
using Net6WebApp.Models;
using NETCore.Encrypt.Extensions;
using System.ComponentModel.DataAnnotations;
using System.Security.Claims;

namespace Net6WebApp.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly DataBaseContext _dataBaseContext;

        public AccountController(DataBaseContext dataBaseContext, IConfiguration configuration)
        {
            _dataBaseContext = dataBaseContext;
            _configuration = configuration;
        }
        [AllowAnonymous]
        public IActionResult Login()
        {
            return View();
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                string hashedPassword = MD5HshedPassword(model.Password);
                User user = _dataBaseContext.Users.SingleOrDefault(x => x.UserName.ToLower() == model.Username.ToLower() && x.Password == hashedPassword);

                if (user != null)
                {
                    if (user.Locked)
                    {
                        ModelState.AddModelError(nameof(model.Username), "Bu hesaba şimdilik erişilemiyor.");
                        return View(model);
                    }
                    List<Claim> claims = new List<Claim>();
                    claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
                    claims.Add(new Claim(ClaimTypes.Name, user.FullName ?? string.Empty));
                    claims.Add(new Claim(ClaimTypes.Role, user.Role));
                    claims.Add(new Claim("UserName", user.UserName));

                    ClaimsIdentity idenity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(idenity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                    return RedirectToAction("Index", "Home");

                }
                else
                {
                    ModelState.AddModelError("", "Kullanıcı adı veya şifre hatalı.");
                }
            }
            return View(model);
        }

        private string MD5HshedPassword(string password)
        {
            string md5Salt = _configuration.GetValue<string>("AppSettings:MD5Salt");
            string saltedPassword = password + md5Salt;
            string hashedPassword = saltedPassword.MD5();
            return hashedPassword;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            return View();

        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {

            if (ModelState.IsValid)
            {
                if (_dataBaseContext.Users.Any(x => x.UserName.ToLower() == model.Username.ToLower()))
                {
                    ModelState.AddModelError(nameof(model.Username), "Kullanıcı Adı çoktan alındı");
                    return View(model);
                }
                string hashedPassword = MD5HshedPassword(model.Password);
                User user = new()
                {   
                    UserName=model.Username,
                    Password= hashedPassword,
                };
                _dataBaseContext.Users.Add(user);
                var effectedRowCount=_dataBaseContext.SaveChanges();
                if (effectedRowCount == 0)
                {
                    ModelState.AddModelError("","Kullanıcı eklenemedi");
                }
                else
                {
                    return RedirectToAction(nameof(Login));
                }
            }

            return View(model);

        }
        [AllowAnonymous]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(Login));

        }
        [AllowAnonymous]
        public IActionResult Profile()
        {
            ProfileInfoLoader();

            return View();
        }

        private void ProfileInfoLoader()
        {
            Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
            User user = _dataBaseContext.Users.SingleOrDefault(x => x.Id == userId);
            ViewData["FullName"] = user.FullName;
        }

        [HttpPost]
        public IActionResult ProfileChangeFulName([Required][StringLength(50)]string? fullname)
        {
            if (ModelState.IsValid)
            {
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _dataBaseContext.Users.SingleOrDefault(x => x.Id == userId);
                user.FullName = fullname;
                _dataBaseContext.SaveChanges();
                return RedirectToAction(nameof(Profile));
            }
                ProfileInfoLoader();  
                return View(nameof(Profile));                  
        }

        public IActionResult ProfileChangePassword([Required(ErrorMessage ="Boş bırakılamaz")][MinLength(6, ErrorMessage = "Şifre En az 6 karakter olmalıdır.")][MaxLength(15, ErrorMessage = "Şifre En fazla 15 karakter olmalıdır.")] string? password)
        {
            if (ModelState.IsValid)
            {
                Guid userId = new Guid(User.FindFirstValue(ClaimTypes.NameIdentifier));
                User user = _dataBaseContext.Users.SingleOrDefault(x => x.Id == userId);
                string hashedPassword = MD5HshedPassword(password);
                user.Password = hashedPassword;
                _dataBaseContext.SaveChanges();
                ViewData["result"] = "PasswordChanged";
            }
            ProfileInfoLoader();
            return View(nameof(Profile));
        }
    }
}
