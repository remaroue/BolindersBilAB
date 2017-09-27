using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class ImageController : Controller
    {
        private CarService _c;
        private CarListService _cs;
        private CarBrandService _bs;
        private ImageService _imageService;

        public ImageController(ImageService imageService, CarBrandService bs, CarListService carListService, CarService c)
        {
            _c = c;
            _cs = carListService;
            _bs = bs;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ICollection<IFormFile> images)
        {
            var carbrand = _imageService.AddImageToCar(_cs.GetCar("aas1984"), images.ToArray());
            _c.Update(carbrand);
            return View();
        }
    }
}