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
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;

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
        [Route("/bil/{brand}/{model}/{modelDescription}/{unique}")]
        public IActionResult Details(string brand, string model, string modelDescription, string unique)
        {
            var car = _carlistService.GetCar(brand, model, modelDescription, unique);
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
        #endregion

        #region admin
        [Authorize]
        [HttpGet]
        [Route("/admin/bilar")]
        public IActionResult CarList([ModelBinder(BinderType = typeof(QueryModelBinder))]CarListQuery query, int page = 1)
        {
            query = _carSearchService.GetCarListQuery(query.Search, query);
            var cars = _carlistService.GetCars(query);

            ViewBag.Query = Request.QueryString;
            ViewBag.Locations = _locationService.Get();

            return View(
                new CarAdminListViewModel
                {
                    Cars = cars.PaginateCars(page, 20, true).ToList(),
                    Query = query,
                    Carbrands = _brandService.Get(),
                    Pager = new PagingInfo
                    {
                        CurrentPage = page,
                        ItemsPerPage = 20,
                        TotalItems = cars.Count()
                    }
                });
        }

        #region CrudCarBrand
        [Route("/admin/bilmarke/skapa")]
        public IActionResult AddCarBrand()
        {
            return View();
        }
        [HttpPost]
        [Route("/admin/bilmarke/skapa")]
        [ValidateAntiForgeryToken]
        public IActionResult AddCarBrand(AddBrandViewModel carBrand)
        {
            if (ModelState.IsValid)
            {
                var newCarBrand = new CarBrand
                {
                    BrandName = carBrand.BrandName
                };

                newCarBrand = _imageService.ChangeImageOnCarBrand(newCarBrand, carBrand.Image);
                _brandService.Add(newCarBrand);
                return RedirectToAction(nameof(CarList));
            }
            else
            {
                return View(carBrand);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("/admin/bilmarke/uppdatera/{brandName}")]
        public IActionResult EditCarBrand(string brandName)
        {
            var brand = _brandService.GetBrand(brandName);

            if (brand == null) return BadRequest();

            return View(new AddBrandViewModel()
            {
                BrandName = brandName
            });
        }

        [HttpPost]
        [Authorize]
        [Route("/admin/bilmarke/uppdatera/{brandName}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditCarBrand(AddBrandViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            var carBrand = _brandService.GetBrand(model.BrandName);
            if (carBrand == null) return BadRequest();

            if (!string.IsNullOrEmpty(carBrand.ImageName)) _imageService.RemoveImage(carBrand.ImageName);

            carBrand = _imageService.ChangeImageOnCarBrand(carBrand, model.Image);
            _brandService.Update(carBrand);

            return RedirectToAction(nameof(CarList));
        }
        #endregion

        #region CrudCar
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
        [ValidateAntiForgeryToken]
        public IActionResult AddCar([FromForm]CarFormViewModel car)
        {
            if (!CheckMediatype(car.Images))
                ModelState.AddModelError("Images", "Bilder måste vara av typen png. jpg eller jpeg.");

            if (!ModelState.IsValid)
            {
                ViewBag.CarBrands = _brandService.Get();
                ViewBag.Locations = _locationService.Get();
                return View(car);
            }
            if (car == null)
            {
                return BadRequest();
            }

            _carService.SaveCar(car);
            return RedirectToAction(nameof(CarList));
        }

        [Authorize]
        [Route("/admin/bil/uppdatera/{licenseNumber}")]
        public IActionResult EditCar(string licenseNumber)
        {
            ViewBag.CarBrands = _brandService.Get();
            ViewBag.Locations = _locationService.Get();

            var car = _carService.GetCar(licenseNumber).GetCarForm();

            return View(car);
        }

        [Authorize]
        [HttpPost]
        [Route("/admin/bil/uppdatera/{licenseNumber}")]
        [ValidateAntiForgeryToken]
        public IActionResult EditCar(string licenseNumber, CarFormViewModel carUpdate)
        {
            if(!CheckMediatype(carUpdate.Images))
                ModelState.AddModelError("Images", "Bilder måste vara av typen png. jpg eller jpeg.");

            if (!ModelState.IsValid)
            {
                ViewBag.CarBrands = _brandService.Get();
                ViewBag.Locations = _locationService.Get();
                return View(carUpdate);
            }

            var car = _carService.GetCar(licenseNumber);
            if (car == null) return BadRequest();

            _carService.UpdateCar(car, carUpdate);

            if(carUpdate.Images?.Count > 0)
            {
                return RedirectToAction(nameof(EditCar), new { licenseNumber = licenseNumber });
            }
            else
            {
                return RedirectToAction(nameof(CarList));

            }
        }

        [Authorize]
        [HttpPost]
        [Route("/admin/bil/pop/{licenseNumber}")]
        public IActionResult PopCar(string licenseNumber)
        {
            var car = _carService.GetCar(licenseNumber);
            if (car == null) return BadRequest();

            _carService.UpdateCar(car);

            return StatusCode(200);
        }

        [Authorize]
        [HttpPost]
        [Route("/admin/bilar")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCars(IEnumerable<string> licenseNumbers)
        {
            var removedCars = _carService.DeleteCars(licenseNumbers);

            _imageService.RemoveImages(removedCars);

            return RedirectToAction(nameof(CarList));
        }

        /// <summary>
        /// not safe
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        private bool CheckMediatype(ICollection<IFormFile> files)
        {
            if (files == null) return true;
            else if (files.Count == 0) return true;

            var okey = true;

            foreach (var file in files)
            {
                switch(file.ContentType)
                {
                    case "image/png":
                    case "image/jpg":
                    case "image/jpeg":
                        okey = true;
                        break;
                    default:
                        okey = false;
                        break;
                }
            }

            return okey;
        }
        #endregion
        #endregion
    }
}