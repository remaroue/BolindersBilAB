using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.DAL.DataAccess;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {
        private CarBrandService _brandService;
        private CarListService _carListService;

        public HomeController(CarBrandService brandService, CarListService carListService)
        {
            _brandService = brandService;
            _carListService = carListService;
        }

        public IActionResult Index()
        {
            ViewBag.CarBrands = _brandService.Get();
            var carCount = _carListService.Get();
            foreach(var item in carCount)

            ViewBag.CarCount = _carListService.Get();
            return View();
        }
    }
}