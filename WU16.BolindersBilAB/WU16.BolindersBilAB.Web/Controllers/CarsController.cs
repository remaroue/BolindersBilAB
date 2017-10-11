using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using WU16.BolindersBilAB.Web.ModelBinder;
using WU16.BolindersBilAB.BLL.Services;
using WU16.BolindersBilAB.BLL.Helpers;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Net;
using Microsoft.AspNetCore.Http;
using WU16.BolindersBilAB.BLL.Models;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class CarsController : Controller
    {
        private CarSearchService _carSearchService;
        private EmailService _emailService;
        private CarService _carService;

        public CarsController(CarSearchService carSearchService, EmailService emailService, CarService carService)
        {
            _carSearchService = carSearchService;
            _emailService = emailService;
            _carService = carService;
        }

        #region public 
        [HttpGet]
        [Route("/bil/{brand}/{model}/{modelDescription}/{unique}")]
        public IActionResult Details(string brand, string model, string modelDescription, string unique)
        {
            var car = _carService.GetCar(brand, model, modelDescription, unique);
            if (car == null) return BadRequest();

            var similarCars = _carService.GetSimilarCars(car);

            return View(new CarDetailsViewModel()
            {
                Car = car,
                SimilarCars = similarCars
            });
        }

        [HttpGet]
        [Route("/bilar/{parameter?}")]
        public IActionResult Cars([ModelBinder(BinderType = typeof(QueryModelBinder))]CarListQuery query, string parameter = "", int page = 1)
        {
            query = _carSearchService.GetCarListQuery(query.Search, query);

            var cars = _carService
                .GetCars(query)
                .FilterByParameter(parameter).ToList();

            ViewBag.Query = Request.QueryString;
            ViewBag.Prices = CarHelper.GetPriceRange();
            ViewBag.Years = CarHelper.GetModelYears();
            ViewBag.Milages = CarHelper.GetMilageRange();
            ViewBag.Parameter = parameter;

            return View(new CarListViewModel()
            {
                Cars = cars.PaginateCars(page, 8, false).ToList(),
                Query = query,
                Pager = new PagingInfo
                {
                    CurrentPage = page,
                    ItemsPerPage = 8,
                    TotalItems = cars.Count()
                }
            });
        }

        [HttpPost]
        [Route("api/bil/dela")]
        public bool Share([FromBody]ShareViewModel model)
        {
            var subject = "Någon Har delat en bil med dig.";

            TagBuilder tagBuilder = new TagBuilder("a");
            var url = $"http://localhost:63037/bil/{model.LicenseNumber}";
            tagBuilder.Attributes["href"] = url;
            tagBuilder.InnerHtml.AppendHtml(url);

            var writer = new System.IO.StringWriter();
            tagBuilder.WriteTo(writer, HtmlEncoder.Default);

            return _emailService.SendTo(model.Email, subject, writer.ToString(), isBodyHtml: true);
        }
        #endregion
    }
}