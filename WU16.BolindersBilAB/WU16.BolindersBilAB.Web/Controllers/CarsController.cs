using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private CarService _service;
        private CarBrandService _brandService;
        private LocationService _locationService;
        

        public CarsController(CarService Service, CarBrandService carBrandService, LocationService locationService)
        {
            
            _service = Service;
            _brandService = carBrandService;
            _locationService = locationService;


        }

        [Route("/bilar/ny")]
        public IActionResult AddCar()
        {
            ViewBag.CarBrands = _brandService.Get();
            ViewBag.Locations = _locationService.Get();
            return View();
        }

        [HttpPost]
        [Route("/bilar/ny")]
        public IActionResult AddCar(Car car)
        {
            if (ModelState.IsValid)
            {
                //Check if location is null.
               if(car.Location == null)
                {
                    return View();
                }
                
                car.CreationDate = DateTime.Now.AddDays(7);
                _service.SaveCar(car);
                return View(); //TODO Return to start.
            }
            else
            {
                return View(car);
            }

        }



    }
}