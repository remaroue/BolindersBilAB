using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Models
{
    public class CarListQuery
    {
        public int YearFrom { get; set; }
        public int YearTo { get; set; }
        public decimal PriceFrom { get; set; }
        public decimal PriceTo { get; set; }
        public int MilageFrom { get; set; }
        public int MilageTo { get; set; }
        public List<CarBrand> CarBrand { get; set; }
        public List<Gearbox>  Gearbox { get; set; }
        public List<FuelType> FuelType { get; set; }
        public List<CarType> CarType { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public string Search { get; set; }
    }
}
