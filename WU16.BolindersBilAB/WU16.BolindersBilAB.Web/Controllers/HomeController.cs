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
        private CarListService _carlistService;

        public IActionResult Index()
        {
            return View();
        }

        /*
         [HttpGet]
        [Route("/bil/{licenseNumber}")]
        public IActionResult Details(string licenseNumber)
        {
            var car = _carlistService.GetCar(licenseNumber);
            if (car == null) return BadRequest();

            var similarCars = _carlistService.GetCars(car.GetSimilarCarsQuery()).ToArray();
            
            return View(new CarDetailsViewModel()
            {
                Car = car,
                SimilarCars = similarCars
            });
        }*/
        /*[HttpPost]
        public IActionResult Index(HomeViewModel query, string returnUrl)
        {
            ViewData["ReturnUrl"] = returnUrl;
            //query.SearchQuery; //Holds the searchquery

            //Redirect to a search result view
            return View();
        }*/
    }
}