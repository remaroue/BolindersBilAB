using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.DataAccess;
using WU16.BolindersBilAB.Web.Models;
using System.Text;
using System.Web;
using WU16.BolindersBilAB.BLL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class ContactsController : Controller
    {
        private ApplicationDbContext _ctx;
        private EmailService _emailService;
        private LocationService _locServ;

        public ContactsController(ApplicationDbContext context, EmailService emailService, LocationService locationService)
        {
            _ctx = context;
            _emailService = emailService;
            _locServ = locationService;

        }

        //Returns persons to contact.
        [HttpGet]

        [Route("/kontakt")]

        public IActionResult Index(bool? sent = null, ContactMailViewModel formModel = null)
        {

            return View(new ContactsViewModel()
            {
                Locations = _locServ.Get(),
                Sent = sent,
                FormModel = formModel ?? new ContactMailViewModel()
            });
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

        //Posts mail to corresponding location of place of sales.
        [HttpPost]
        [Route("/kontakt")]

        public IActionResult Index(ContactMailViewModel model)
        {
            if (!ModelState.IsValid) return Index(false, model);
            if (!_locServ.Get().Any(x => x.Email == model.Reciever)) return Index(false, model);

            _emailService.SendTo(model.Email, "Skickat Fråm Kontaktformulär", ConstructMessage(model), model.Email, isBodyHtml: true);

            return Index(true);
        }

    }

}