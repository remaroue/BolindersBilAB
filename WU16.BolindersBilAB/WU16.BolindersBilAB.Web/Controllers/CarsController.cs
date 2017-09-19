using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.DAL.Helpers;
using WU16.BolindersBilAB.DAL.DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private EmailService _emailService;
        private CarListService _carlistService;
        private ApplicationDbContext _ctx;

        public CarsController(EmailService emailService, CarListService carListService, ApplicationDbContext context)
        {
            _ctx = context;
            _emailService = emailService;
            _carlistService = carListService;
        }

        [Route("/bil/{licenseNumber}")]
        public IActionResult Details(string licenseNumber)
        {
            var car = _ctx.Cars
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .FirstOrDefault(x => x.LicenseNumber == licenseNumber);

            return View(car);
        }

        [Route("/api/bil/dela")]
        [HttpPost]
        public bool Share(string email, string licenseNumber)
        {
            try
            {
                var subject = "Någon Har delat en bil med dig.";
                _emailService.SendTo(email, subject, "localhost:24314/bil/" + licenseNumber);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }

       [Route("/bilar/{parameter?}")]
       public IActionResult Cars(string parameter)
        {
            var carListVm = new CarListViewModel
            {
                Cars = _carlistService.GetCars()
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
                Cars = _carlistService.GetCars()
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