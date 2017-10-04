using Microsoft.AspNetCore.Http;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class AddCarViewModel
    {
        public string LicenseNumber {get;set;}
        public string Model { get; set; }
        public string Description { get; set; }
        public int ModelYear { get; set; }
        public bool IsLeaseable { get; set; }
        public int? Milage { get; set; }
        public decimal? Price { get; set; }
        public string Color { get; set; }
        public int? HorsePower { get; set; }
        public bool Used { get; set; }
        public string LocationId { get; set; }
        public virtual CarBrand CarBrand { get; set; }
        public string CarBrandId { get; set; }
        public string Equipment { get; set; }
        public CarType CarType { get; set; }
        public FuelType FuelType { get; set; }
        public Gearbox Gearbox { get; set; }
        public ICollection<IFormFile> Images { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }

    }
}
