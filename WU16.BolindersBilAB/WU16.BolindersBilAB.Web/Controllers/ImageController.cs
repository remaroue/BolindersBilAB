using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WU16.BolindersBilAB.BLL.Services;
using Microsoft.AspNetCore.Authorization;

namespace WU16.BolindersBilAB.Web.Controllers
{
    [Authorize]
    public class ImageController : Controller
    {
        private CarBrandService _bs;
        private ImageService _imageService;
        private FtpService _ftpService;

        public ImageController(ImageService imageService, CarBrandService bs, FtpService ftpService)
        {
            _bs = bs;
            _imageService = imageService;
            _ftpService = ftpService;
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

        [Route("/ftptest")]
        public IActionResult FtpTester()
        {
            return View();
        }

        [HttpPost]
        [Route("/ftptestpost")]
        public IActionResult FtpTesterPost()
        {
            _ftpService.Run();

            return Redirect("/ftptest");
        }
    }
}