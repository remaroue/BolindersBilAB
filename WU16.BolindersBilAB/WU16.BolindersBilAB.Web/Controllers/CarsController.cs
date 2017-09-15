using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult GetCar(string registrationNumer)
        {
            return View();
        }
    }
}