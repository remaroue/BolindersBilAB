using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private EmailService _emailService;

        public CarsController(EmailService emailService)
        {
            _emailService = emailService;
        }

        [Route("/bil/{registrationNumer}")]
        public IActionResult Details(string registrationNumer)
        {
            return View(new Car() {
                CarBrand = new CarBrand { BrandName = "Volvo" },
                CarType = CarType.Sedan,
                Color = "Moon grey",
                CreationDate = DateTime.Now,
                IsLeaseable = false,
                LicenseNumber = "abc505",
                HorsePower = 1000,
                ModelYear = 2016,
                Location = new Location() { City = "Värnamo", Address = "kaptensgaten 9", Id = "bb3", Name = "Värnamo", PhoneNumber = "0705731798", Zip = "33152" },
                Milage = 20000,
                Used = true,
                Price = 120000,
                Description = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Donec ultricies, dui ut condimentum fermentum, ligula ligula ultrices nibh, sed feugiat eros lorem id leo. Curabitur at lacus sit amet massa congue tempus maximus id nunc. Etiam pretium ultricies blandit. Duis congue orci quis magna porttitor, eu congue odio feugiat. Quisque tempus purus magna, ac imperdiet odio tempus vel. Fusce sed varius massa. Sed turpis nisi, porta non feugiat consequat, consequat tempus risus. Pellentesque maximus dictum libero scelerisque ultricies. Curabitur at cursus nisl, eu commodo leo. Ut in nisl eu justo scelerisque aliquam.",
                Model = "s60",
                Equipment = "Elsäten|Krockkuddar",
                CarImages = new CarImage[]
                {
                    new CarImage() { Id = Guid.Parse("A93918A5-E96C-4AA1-9328-03BB51D28A5C"), Priority = 1 },
                    new CarImage() { Id = Guid.Parse("7BCDFC19-80DC-4FCE-A7AF-ADC48E7B166E"), Priority = 2 },
                    new CarImage() { Id = Guid.Parse("C31ADF26-CD8F-497C-9FA3-3E7B8B6F4D6F"), Priority = 3 }
                }
            });
        }

        [Route("/api/bil/dela")]
        [HttpPost]
        public async Task<bool> Share(string email, string licenseNumber)
        {
            try
            {
                var subject = "Någon Har delat en bil med dig.";
                _emailService.SendTo(email, subject, "localhost:24314/bil/" + licenseNumber);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}