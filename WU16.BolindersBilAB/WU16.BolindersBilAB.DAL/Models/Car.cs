using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Models
{
    public class Car
    {
        public Car()
        {
            CarImages = new HashSet<CarImage>();
        }

        [Display(Name ="Registreringsnummer")]
        [Key]
        public string LicenseNumber { get; set; }

        [ForeignKey("CarBrand")]
        public string CarBrandId { get; set; }

        [Display(Name =  "Bilmärke")]
        public virtual CarBrand CarBrand { get; set; }
        [ForeignKey("Location")]
        public string LocationId { get; set; }

        [Display(Name = "Plats")]
        public virtual Location Location { get; set; }

        [Display(Name = "Bil Model")]
        public string Model { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }

        [Display(Name = "Årsmodel")]
        public int ModelYear { get; set; }

        [Display(Name = "Leas bar")]
        public bool IsLeaseable { get; set; }

        [Display(Name = "Miltal")]
        public int Milage { get; set; }

        [Display(Name = "Pris")]
        public decimal Price { get; set; }

        [Display(Name = "Färg")]
        public string Color { get; set; }

        [Display(Name = "Miltal")]
        public int HorsePower { get; set; }

        [Display(Name = "Använd")]
        public bool Used { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime LastUpdated { get; set; }

        [Display(Name = "Bilder")]
        public virtual ICollection<CarImage>  CarImages { get; set; }

        [Display(Name = "BilTyp")]
        public CarType CarType { get; set; }

        [Display(Name = "Bränsle")]
        public FuelType FuelType { get; set; }

        [Display(Name = "Växelåda")]
        public Gearbox Gearbox { get; set; }

        [Display(Name = "Utrustning")]
        public string Equipment { get; set; }
    }
}
