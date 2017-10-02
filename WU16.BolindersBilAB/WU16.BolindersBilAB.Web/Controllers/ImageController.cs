using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.BLL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class ImageController : Controller
    {
        private CarBrandService _bs;
        private ImageService _imageService;

        public ImageController(ImageService imageService, CarBrandService bs)
        {
            _bs = bs;
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ICollection<IFormFile> images, string id)
        {
            var carbrand = _imageService.ChangeImageOnCarBrand(_bs.GetBrand(id), images.First());
            _bs.Update(carbrand);
            return View();
        }
    }
}