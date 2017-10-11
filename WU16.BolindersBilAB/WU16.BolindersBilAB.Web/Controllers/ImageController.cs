using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using WU16.BolindersBilAB.BLL.Services;
using Microsoft.AspNetCore.Authorization;
using System.Diagnostics;

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

        [HttpGet]
        [Route("/ftptest")]
        public IActionResult FtpTester()
        {
            var s = new Stopwatch();
            s.Start();
            _ftpService.Run();
            s.Stop();

            ViewBag.Time = s.ElapsedMilliseconds;

            return View();
        }

        //[HttpPost]
        //[Route("/ftptest")]
        //public IActionResult FtpTesterPost()
        //{
        //    return View();
        //}
    }
}