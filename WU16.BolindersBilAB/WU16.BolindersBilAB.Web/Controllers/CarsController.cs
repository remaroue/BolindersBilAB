using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.DAL.Helpers;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private CarListService _service;

        public CarsController(CarListService Service)
        {
            _service = Service;
        }

       [Route("/bilar/{parameter?}")]
       public IActionResult Cars(string parameter)
        {
            var carListVm = new CarListViewModel
            {
                Cars = _service.GetCars()
            };

            if(parameter != null)
            {
                if (parameter != "nya" && parameter != "begagnade")
                {
                    return Redirect("/bilar");
                }
                else
                {
                    carListVm.Cars = CarListHelper.Filter(parameter, carListVm.Cars);
                }
            }      
            return View(carListVm);
        }
        [HttpPost]
        [Route("/bilar/{parameter?}")]
        public IActionResult Cars(CarListQuery query, string parameter)
        {
            var carListVm = new CarListViewModel
            {
                Cars = _service.GetCars()
            };

            if (parameter != null)
            {
                if (parameter == "nya" || parameter == "begagnade")
                {
                    carListVm.Cars = CarListHelper.Filter(parameter, carListVm.Cars);
                }
            }

            carListVm.Cars = CarListHelper.FilterByQuery(query, carListVm.Cars);

            return View(carListVm);
        }
    }
}