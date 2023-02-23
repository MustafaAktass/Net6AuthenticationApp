using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net6WebApp.Models;
using System.Diagnostics;

namespace Net6WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
   
        
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult Privacy()
        {
            return View();
        }
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}