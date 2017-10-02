﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WU16.BolindersBilAB.Web.Models;
using WU16.BolindersBilAB.DAL.DataAccess;
using WU16.BolindersBilAB.DAL.Services;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Controllers
{
    public class HomeController : Controller
    {
        private CarBrandService _brandService;
        private CarListService _carListService;

        public HomeController(CarBrandService brandService, CarListService carListService)
        {
            _brandService = brandService;
            _carListService = carListService;
        }

        public IActionResult Index()
        {
            //var sortedBrands = _carListService.Get().GroupBy(i => i.CarBrand);

            //List<HomeViewModel> brandCount = new List<HomeViewModel>();
            //foreach (var brand in sortedBrands)
            //{
            //    string imgUrl = "";
            //    foreach(var item in _brandService.Get())
            //    {
            //        if(item.BrandName == brand.Key.BrandName)
            //        {
            //            imgUrl = "/images/upload/" + item.ImageName;
            //        }
            //    }

            //    brandCount.Add(new HomeViewModel()
            //    {
            //        CarBrand = brand.Key.BrandName,
            //        CarCount = brand.Count(),
            //        CarImage = imgUrl
            //    });
            //}

            ViewBag.CarCount = _brandService.Get().Select(x => new HomeViewModel() {
                CarBrand = x.BrandName,
                CarCount = x.Cars.Count,
                CarImage = x.ImageName
            });
            return View();
        }
    }
}