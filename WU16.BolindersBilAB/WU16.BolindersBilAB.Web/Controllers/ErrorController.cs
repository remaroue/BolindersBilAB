using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class ErrorController : Controller
    {
        [Route("/404")]
        public IActionResult Error404()
        {
            return View();
        }
    }
}