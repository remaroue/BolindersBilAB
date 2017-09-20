using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.DAL.DataAccess;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {

        public IActionResult Index()
        {
            return View();
        }
    }
}