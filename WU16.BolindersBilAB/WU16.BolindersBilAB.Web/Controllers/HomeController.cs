using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /*[HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string query, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View(Cars);
        }*/
    }
}