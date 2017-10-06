using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using WU16.BolindersBilAB.Web.ModelBinder;
using WU16.BolindersBilAB.BLL.Services;
using WU16.BolindersBilAB.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private CarSearchService _carSearchService;
        private EmailService _emailService;
        private CarService _carlistService;
        private CarBrandService _brandService;
        private LocationService _locationService;
        private CarService _carService;
        private ImageService _imageService;

        public CarsController(CarSearchService carSearchService, EmailService emailService, CarService carListService, CarBrandService carBrandService, LocationService locationService, CarService carService, ImageService imageService)
        {
            _carSearchService = carSearchService;
            _emailService = emailService;
            _carlistService = carListService;
            _brandService = carBrandService;
            _locationService = locationService;
            _carService = carService;
            _imageService = imageService;
        }

        #region public 
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

        [HttpGet]
        [Route("/bilar/{parameter?}")]
        public IActionResult Cars([ModelBinder(BinderType = typeof(QueryModelBinder))]CarListQuery query, string parameter = "", int page = 1)
        {
            query = _carSearchService.GetCarListQuery(query.Search, query);

            var cars = _carlistService
                .GetCars(query)
                .FilterByParameter(parameter).ToList();

            ViewBag.Query = Request.QueryString;
            ViewBag.Prices = CarHelper.GetPriceRange();
            ViewBag.Years = CarHelper.GetModelYears();
            ViewBag.Milages = CarHelper.GetMilageRange();
            ViewBag.Parameter = parameter;

            return View(new CarListViewModel()
            {
                Cars = cars.PaginateCars(page, 8, false).ToList(),
                Query = query,
                Pager = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = 8,
                    TotalItems = cars.Count()
                }
            });
        }

        [HttpPost]
        [Route("api/bil/dela")]
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
        [Authorize]
        [HttpGet]
        [Route("/admin/bilar")]
        public IActionResult CarList([ModelBinder(BinderType = typeof(QueryModelBinder))]CarListQuery query, int page = 1)
        {
            query = _carSearchService.GetCarListQuery(query.Search, query);
            var cars = _carlistService.GetCars(query);

            ViewBag.Query = Request.QueryString;

            return View(
                new CarListViewModel {
                    Cars = cars.PaginateCars(page, 20, true).ToList(),
                    Query = query,
                    Pager = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = 20,
                        TotalItems = cars.Count()
                    }
                });
        }
        #endregion  

        #region admin
        [Authorize]
        [HttpGet]
        [Route("/admin/bil/skapa")]
        public IActionResult AddCar()
        {
            ViewBag.CarBrands = _brandService.Get();
            ViewBag.Locations = _locationService.Get();
            return View();
        }
        [Authorize]
        [HttpPost]
        [Route("/admin/bil/skapa")]
        public IActionResult AddCar(AddCarViewModel car)
        {
            if (!ModelState.IsValid)
            {
                return View(car);
            }
            car.CreationDate = DateTime.Now;
            car.LastUpdated = DateTime.Now;
            if (car == null)
            {
                return BadRequest();
            }

            var newCar = new Car
            {
                LicenseNumber = car.LicenseNumber,
                Model = car.Model,
                Description = car.Description,
                ModelYear = car.ModelYear,
                IsLeaseable = car.IsLeaseable,
                Milage = car.Milage,
                Price = car.Price,
                Color = car.Color,
                HorsePower = car.HorsePower,
                Used = car.Used,
                LocationId = car.LocationId,
                CarBrand = car.CarBrand,
                CarBrandId = car.CarBrandId,
                Equipment = car.Equipment,
                CarType = car.CarType,
                FuelType = car.FuelType,
                Gearbox = car.Gearbox,
                CreationDate = DateTime.Now,
                LastUpdated = DateTime.Now

            };

            newCar = _imageService.AddImageToCar(newCar, car.Images.ToArray());
            _carService.SaveCar(newCar);
            return View("/");
        }

        [Route("/admin/bilmarke/skapa")]
        public IActionResult AddCarBrand()
        {
            return View();
        }
        [HttpPost]
        [Route("/admin/bilmarke/skapa")]
        public IActionResult AddCarBrand(AddBrandViewModel carBrand)
        {
            if (ModelState.IsValid)
            {
                var newCarBrand = new CarBrand
                {
                    BrandName = carBrand.BrandName
                };
                _brandService.Add(newCarBrand);
                newCarBrand = _imageService.ChangeImageOnCarBrand(newCarBrand, carBrand.Image);
                return View("/");
            }
            else
            {
                return View(carBrand);
            }
        }
        [Authorize]
        [Route("/admin/bil/{licenseNumber}")]
        public IActionResult EditCar(string licenseNumber)
        {
            ViewBag.CarBrands = _brandService.Get();
            ViewBag.Locations = _locationService.Get();
            var car = _carService.GetCar(licenseNumber);
            return View(car);
        }

        [Authorize]
        [HttpPost]
        [Route("/admin/bil/{licenseNumber}")]
        public IActionResult EditCar(Car car)
        {
            if (ModelState.IsValid)
            {
                _carService.UpdateCar(car);

                return RedirectToAction(nameof(CarList));
            }
            else
            {
                return View(car);
            }
        }


        #endregion
    }
}