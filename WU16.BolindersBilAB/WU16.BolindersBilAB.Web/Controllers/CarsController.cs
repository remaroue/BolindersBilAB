using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.DAL.Helpers;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using WU16.BolindersBilAB.BLL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private EmailService _emailService;
        private CarListService _carlistService;
        private CarBrandService _brandService;
        private LocationService _locationService;
        private CarService _carService;

        public CarsController(EmailService emailService, CarListService carListService, CarBrandService carBrandService, LocationService locationService, CarService CarService)
        {
            _emailService = emailService;
            _carlistService = carListService;
            _brandService = carBrandService;
            _locationService = locationService;
            _carService = CarService;
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

        [Route("/bilar/{parameter?}")]
        public IActionResult Cars(string parameter)
        {
            var cars = _carlistService.GetCars();

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
                Cars = cars.ToArray()
            });
        }

        [HttpPost]
        [Route("/bilar/{parameter?}")]
        public IActionResult Cars(CarListQuery query, string parameter)
        {
            var cars = _carlistService.GetCars(query);

            if (parameter != null)
            {
                if (parameter == "nya" || parameter == "begagnade")
                {
                    cars = cars.FilterByParameter(parameter);
                }
            }

            return View(new CarListViewModel()
            {
                Cars = cars.ToArray()
            });
        }
    }
}