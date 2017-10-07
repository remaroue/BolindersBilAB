using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.BLL.Services;
using WU16.BolindersBilAB.BLL.Configuration;
using Microsoft.Extensions.Options;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {
        private CarBrandService _brandService;
        private CarService _carListService;

        public HomeController(CarBrandService brandService, CarService carListService)
        {
            _brandService = brandService;
            _carListService = carListService;
        }

        public IActionResult Search(string parameter, string search)
        {
            if(parameter != null)
            {
                return Redirect($"/bilar/{parameter}?search={search}");
            }
            else
            {
                return Redirect($"/bilar/?search={search}");
            }
        }

        public IActionResult Index()
        {
            ViewBag.CarCount = _brandService.Get()
                .Where(x => x.ImageName != null && x.Cars.Count > 0)
                .Select(x => new HomeViewModel()
                {
                    CarBrand = x.BrandName,
                    CarCount = x.Cars.Count,
                    CarImage = $"images/upload/{x.ImageName}"
                });
            return View();
        }
    }
}