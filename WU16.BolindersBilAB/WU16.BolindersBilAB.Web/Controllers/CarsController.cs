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
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using System.IO;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private EmailService _emailService;
        private CarListService _carlistService;

        public CarsController(EmailService emailService, CarListService carListService)
        {
            _emailService = emailService;
            _carlistService = carListService;
        }

        [HttpGet]
        [Route("/bil/{licenseNumber}")]
        public IActionResult Details(string licenseNumber)
        {
            var car = _carlistService.GetCar(licenseNumber);

            var query = new CarListQuery()
            {
                CarType = new List<CarType>(){ car.CarType },
                Gearbox = new List<Gearbox>() { car.Gearbox },
                FuelType = new List<FuelType>() { car.FuelType },
                MilageFrom = (int)(car.Milage * 0.8),
                MilageTo = (int)(car.Milage * 1.2),
                PriceFrom = car.Price * 0.8m,
                PriceTo = car.Price * 1.2m,
                YearFrom = car.ModelYear - 5,
                YearTo = car.ModelYear + 5
            };

            var similarCars = CarListHelper.FilterByQuery(query, _carlistService.GetCars(x => x.LicenseNumber != licenseNumber)).Take(4);

            if (car == null) return BadRequest();
            return View(new CarDetailsViewModel()
            {
                Car = car,
                SimilarCars = similarCars
            });
        }

        [HttpPost]
        [Route("/bil/dela")]
        public bool Share([FromBody]ShareViewModel model)
        {
            var subject = "Någon Har delat en bil med dig.";

            TagBuilder tagBuilder = new TagBuilder("a");
            var url = $"http://localhost:63037/bil/{model.LicenseNumber}";
            tagBuilder.Attributes["href"] = url;
            tagBuilder.InnerHtml.AppendHtml(url);

            var writer = new System.IO.StringWriter();
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);

            return _emailService.SendTo(model.Email, subject, writer.ToString(), isBodyHtml: true);
        }

        [Route("/bilar/{parameter?}")]
        public IActionResult Cars(string parameter)
        {
            var carListVm = new CarListViewModel
            {
                Cars = _carlistService.GetCars()
            };

            if (parameter != null)
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