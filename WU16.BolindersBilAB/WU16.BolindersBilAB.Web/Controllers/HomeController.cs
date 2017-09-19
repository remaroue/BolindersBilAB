using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(HomeViewModel query, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //query.SearchQuery; //Holds the searchquery

            //Redirect to a search result view
            return View();
        }
    }
}