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
using System.Text;
using WU16.BolindersBilAB.Web.ModelBinder;
using WU16.BolindersBilAB.BLL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private CarSearchService _carSearchService;
        private EmailService _emailService;
        private CarListService _carlistService;
        private CarBrandService _brandService;
        private LocationService _locationService;
        private CarService _carService;

        public CarsController(CarSearchService carSearchService, EmailService emailService, CarListService carListService, CarBrandService carBrandService, LocationService locationService, CarService carService)
        {
            _carSearchService = carSearchService;
            _emailService = emailService;
            _carlistService = carListService;
            _brandService = carBrandService;
            _locationService = locationService;
            _carService = carService;
        }

        [HttpGet]
        [Route("/bil/{licenseNumber}")]
        public IActionResult Details(string licenseNumber)
        {
            var car = _carlistService.GetCar(licenseNumber);
            if (car == null) return BadRequest();

            var similarCars = _carlistService.GetSimilarCars(car);

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

        [Route("/bil/ny")]
        public IActionResult AddCar()
        {
            ViewBag.CarBrands = _brandService.Get();
            ViewBag.Locations = _locationService.Get();
            return View();
        }

        [HttpPost]
        [Route("/bil/ny")]
        public IActionResult AddCar(Car car)
        {
            var location = _locationService.Get();

            //car.Location.Id = car.LocationId;
            //car.CarBrand.BrandName = car.CarBrandId;
            if (!ModelState.IsValid)
            {
                return BadRequest(); // Todo return to view.
            }
            else
            {

                car.CreationDate = DateTime.Now;
                car.LastUpdated = DateTime.Now;

                _carService.SaveCar(car);
                return Redirect("/"); // Todo Return to view.

            }
        }

        [HttpGet]
        [Route("/bilar/{parameter?}")]
        public IActionResult Cars([ModelBinder(BinderType = typeof(QueryModelBinder))]CarListQuery query, string parameter = "", int page = 1)
        {
            query = _carSearchService.GetCarListQuery(query.Search, query);

            var cars = _carlistService
                .GetCars(query)
                .FilterByParameter(parameter);

            var totalItems = cars.ToList().Count;

            ViewBag.Query = Request.QueryString.ToString();
            ViewBag.Prices = CarListHelper.GetPriceRange();
            ViewBag.Years = CarListHelper.GetModelYears();
            ViewBag.Milages = CarListHelper.GetMilageRange();
            ViewBag.Parameter = parameter;

            return View(new CarListViewModel()
            {
                Cars = cars.PaginateCars(page).ToList(),
                Query = query,
                Pager = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = 8,
                    TotalItems = totalItems
                }
            });
        }
    }
}