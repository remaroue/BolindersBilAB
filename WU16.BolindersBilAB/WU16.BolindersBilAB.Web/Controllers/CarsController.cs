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
        private CarSearchService _carSearchService;
        private EmailService _emailService;
        private CarListService _carlistService;

        public CarsController(EmailService emailService, CarListService carListService, CarSearchService carSearchService)
        {
            _carSearchService = carSearchService;
            _emailService = emailService;
            _carlistService = carListService;
        }

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
        public IActionResult Cars(string parameter, string searchquery)
        {
                var query = _carSearchService.GetCarListQuery(searchquery);
                var cars = _carlistService.GetCars(query);
            if (parameter != null)
            {
                if (parameter != "nya" && parameter != "begagnade")
                {
                    return Redirect("/bilar");
                }
                else
                {
                    cars = cars.FilterByParameter(parameter);
                }
            }

            return View(new CarListViewModel
            {
                Cars = cars.ToList(),
                Query = query
            });
        }

        [HttpGet]
        [Route("/bilar/{parameter?}")]
        public IActionResult Cars(CarListQuery query, string parameter="", int page=1)
        {
            var cars = _carlistService.GetCars(query);
            if (parameter.Length > 0)
            {
                if (parameter == "nya" || parameter == "begagnade")
                {
                    cars = cars.FilterByParameter(parameter);
                }
                else
                {
                    return Redirect("/bilar");
                }
            }

            var totalItems = cars.ToList().Count;

            cars.PaginateCars(page);

            ViewBag.Prices = CarListHelper.GetPriceRange();
            ViewBag.Years = CarListHelper.GetModelYears();
            ViewBag.Milages = CarListHelper.GetMilageRange();

            return View(new CarListViewModel()
            {
                Cars = cars.ToList(),
            });
        }
    }
}