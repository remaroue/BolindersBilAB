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
        private ImageService _imageService;

        public ImageController(ImageService imageService)
        {
            _imageService = imageService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(ICollection<IFormFile> images)
        {
            _imageService.UploadImages(images.ToArray());
            return View();
        }
    }
}