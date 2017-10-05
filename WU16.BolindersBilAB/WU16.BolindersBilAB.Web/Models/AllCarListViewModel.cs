using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class AllCarListViewModel
    {
        public string Model { get; set; }
        public string Descriptions { get; set; }
        public int ModelYear { get; set; }
        public int? Milage { get; set; }
        public decimal? Price { get; set; }
        public int? HorsePower { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastUpdated { get; set; }
        public CarType CarType { get; set; }
        public FuelType FuelType { get; set; }
        public Gearbox Gearbox { get; set; }
        
    }
}
