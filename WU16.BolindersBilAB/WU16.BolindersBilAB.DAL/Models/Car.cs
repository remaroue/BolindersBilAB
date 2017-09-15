using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Models
{
    public class Car
    {
        [Key]
        public string LicenseNumber { get; set; }
        [ForeignKey("CarBrand")]
        public virtual string CarBrandId { get; set; }
        public virtual CarBrand CarBrand { get; set; }
        public virtual Location Location { get; set; }
        public int ModelYear { get; set; }
        public bool IsLeaseable { get; set; }
        public int Milage { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public int HorsePower { get; set; }
        public bool Used { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }
        public virtual ICollection<CarImage>  CarImages { get; set; }
        public CarType CarType { get; set; }
        public FuelType FuelType { get; set; }
        public Gearbox Gearbox { get; set; }

    }
}
