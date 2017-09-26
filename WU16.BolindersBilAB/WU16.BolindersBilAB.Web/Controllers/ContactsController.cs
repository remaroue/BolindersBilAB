using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.DataAccess;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class ContactsController : Controller
    {
        private ApplicationDbContext _ctx;
        private EmailService _emailService;

        public ContactsController(ApplicationDbContext context, EmailService emailService)
        {
            _ctx = context;
            _emailService = emailService;
        }

        //Returns persons to contact.
        [HttpGet]
        public IActionResult Index()
        {

            return View(new ContactsViewModel() {
                Locations = new List<Location>()
                {
                    new Location() {City = "Värnamo"}
                }

            });
        }
        
        
        //Posts mail to corresponding location of place of sales.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Contacts(ContactMailViewModel model)
        {
            if (!ModelState.IsValid) return View(model);

            _emailService.SendTo("johanwallenba@hotmail.com", model.Subject, model.Message, model.Email);

            return View();
        }

    }

}