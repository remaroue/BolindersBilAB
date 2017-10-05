using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.BLL.Services;

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
            //var sortedBrands = _carListService.Get().GroupBy(i => i.CarBrand);

            //List<HomeViewModel> brandCount = new List<HomeViewModel>();
            //foreach (var brand in sortedBrands)
            //{
            //    string imgUrl = "";
            //    foreach(var item in _brandService.Get())
            //    {
            //        if(item.BrandName == brand.Key.BrandName)
            //        {
            //            imgUrl = "/images/upload/" + item.ImageName;
            //        }
            //    }

            //    brandCount.Add(new HomeViewModel()
            //    {
            //        CarBrand = brand.Key.BrandName,
            //        CarCount = brand.Count(),
            //        CarImage = imgUrl
            //    });
            //}

            ViewBag.CarCount = _brandService.Get().Select(x => new HomeViewModel() {
                CarBrand = x.BrandName,
                CarCount = x.Cars.Count,
                CarImage = x.ImageName
            });
            return View();
        }
    }
}