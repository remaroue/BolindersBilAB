using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using WU16.BolindersBilAB.BLL.Services;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.BLL.Helpers;
using WU16.BolindersBilAB.DAL.Models;
using Microsoft.AspNetCore.Http;
using WU16.BolindersBilAB.Web.ModelBinder;

namespace WU16.BolindersBilAB.Web.Controllers
{
    [Authorize]
    public class AdminController : Controller
    {
        private CarSearchService _carSearchService;
        private EmailService _emailService;
        private CarBrandService _brandService;
        private LocationService _locationService;
        private CarService _carService;
        private ImageService _imageService;

        public AdminController(CarSearchService carSearchService, EmailService emailService, CarBrandService carBrandService, LocationService locationService, CarService carService, ImageService imageService)
        {
            _carSearchService = carSearchService;
            _emailService = emailService;
            _brandService = carBrandService;
            _locationService = locationService;
            _carService = carService;
            _imageService = imageService;
        }

        [Authorize]
        [HttpGet]
        [Route("/admin")]
        public IActionResult Index([ModelBinder(BinderType = typeof(QueryModelBinder))]CarListQuery query, int page = 1)
        {
            query = _carSearchService.GetCarListQuery(query.Search, query);
            var cars = _carService.GetCars(query);

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
                return RedirectToAction(nameof(Index));
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

            return RedirectToAction(nameof(Index));
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
            return RedirectToAction(nameof(Index));
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
            if (!CheckMediatype(carUpdate.Images))
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

            if (carUpdate.Images?.Count > 0)
            {
                return RedirectToAction(nameof(EditCar), new { licenseNumber = licenseNumber });
            }
            else
            {
                return RedirectToAction(nameof(Index));
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
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCars(IEnumerable<string> licenseNumbers)
        {
            var removedCars = _carService.DeleteCars(licenseNumbers);

            _imageService.RemoveImages(removedCars);

            return RedirectToAction(nameof(Index));
        }

        private bool CheckMediatype(ICollection<IFormFile> files)
        {
            if (files == null) return true;
            else if (files.Count == 0) return true;

            var okey = true;

            foreach (var file in files)
            {
                switch (file.ContentType)
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
    }
}