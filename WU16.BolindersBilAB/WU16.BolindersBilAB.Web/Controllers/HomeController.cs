using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.BLL.Services;
using WU16.BolindersBilAB.BLL.Configuration;
using Microsoft.Extensions.Options;
using System.Text;
using System.Web;
using System;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {
        private CarBrandService _brandService;
        private CarService _carListService;
        private LocationService _locationService;
        private EmailService _emailService;

        public HomeController(CarBrandService brandService, CarService carListService, LocationService locationService, EmailService emailService)
        {
            _brandService = brandService;
            _carListService = carListService;
            _locationService = locationService;
            _emailService = emailService;
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

        [HttpGet]
        [Route("/kontakt")]
        public IActionResult Contact(bool? sent = null, ContactMailViewModel formModel = null)
        {
            return View(new ContactsViewModel()
            {
                Locations = _locationService.Get(),
                Sent = sent,
                FormModel = formModel ?? new ContactMailViewModel()
            });
        }

        //Posts mail to corresponding location of place of sales.
        [HttpPost]
        [Route("/kontakt")]
        [ValidateAntiForgeryToken]
        public IActionResult Contact(ContactMailViewModel model)
        {
            if (!ModelState.IsValid) return Contact(false, model);

            if (string.IsNullOrEmpty(model.Reciever))
            {
                model.Reciever = "kontakt@bolindersbil.se";
            }
            else
            {
                if (!_locationService.Get().Any(x => x.Email == model.Reciever)) return Contact(false, model);
            }

            _emailService.SendTo(model.Email, $"Skickat Fråm Kontaktformulär: {DateTime.Now.ToShortDateString()}", ConstructMessage(model), model.Email, isBodyHtml: true);

            return Contact(true);
        }

        private string ConstructMessage(ContactMailViewModel model)
        {
            var sb = new StringBuilder();

            sb.Append($"<b>Förnamn:</b> {HttpUtility.HtmlEncode(model.FirstName)}<br>");
            sb.Append($"<b>Efternamn:</b> {HttpUtility.HtmlEncode(model.LastName)}<br>");
            sb.Append($"<b>Email:</b> <a href='mailto:{HttpUtility.HtmlEncode(model.Email)}?subject=Svar Från kontaktformulär'>{HttpUtility.HtmlEncode(model.Email)}</a><br>");
            sb.Append($"<br><br><b>Ämne</b>: {HttpUtility.HtmlEncode(model.Subject)}<br>");
            sb.Append($"<b>Meddelande:</b><br> {HttpUtility.HtmlEncode(model.Message)}");

            return sb.ToString();
        }
    }
}