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

        public CarsController(CarService Service, CarBrandService BrandService)
        {
            _service = Service;
            _brandService = BrandService;
        }

        [Route("/bilar/ny")]
        public IActionResult AddCar()
        {
            ViewBag.CarBrands = _brandService.Get();
            return View();
        }

        [HttpPost]
        [Route("/bilar/ny")]
        public IActionResult AddCar(Car car)
        {
            if (ModelState.IsValid)
            {


                
               
                //Default 7days.
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