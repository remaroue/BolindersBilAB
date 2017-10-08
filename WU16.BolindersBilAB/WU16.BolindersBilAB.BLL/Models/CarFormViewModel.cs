using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class CarFormViewModel
    {
        [Required]
        [Display(Name = "Registreringsnummer")]
        public string LicenseNumber { get; set; }
        [Required]
        [Display(Name = "Modell")]
        public string Model { get; set; }
        [Required]
        [Display(Name = "Modell Beskrivning")]
        public string ModelDescription { get; set; }
        [Required]
        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        [Required]
        [Display(Name = "Årsmodell")]
        public int ModelYear { get; set; }
        [Required]
        [Display(Name = "Leasing")]
        public bool IsLeaseable { get; set; }
        [Required]
        [Display(Name = "Mätarställning(mil)")]
        public int? Milage { get; set; }
        [Required]
        [Display(Name = "Pris")]
        public decimal? Price { get; set; }
        [Required]
        [Display(Name = "Färg")]
        public string Color { get; set; }
        [Required]
        [Display(Name = "Hästkrafter")]
        public int? HorsePower { get; set; }
        [Required]
        [Display(Name = "Begagnad")]
        public bool Used { get; set; }
        [Required]
        [Display(Name = "Plats")]
        public string LocationId { get; set; }
        [Required]
        [Display(Name = "Bilmärke")]
        public string CarBrandId { get; set; }
        [Required]
        [Display(Name = "Utrustning")]
        public string Equipment { get; set; }

        [Required]
        [Display(Name = "Kaross")]
        public CarType CarType { get; set; }
        [Required]
        [Display(Name = "Bränsletyp")]
        public FuelType FuelType { get; set; }
        [Required]
        [Display(Name = "Växellåda")]
        public Gearbox Gearbox { get; set; }

        public string[] ExistingImages { get; set; }

        [Display(Name = "Bilder")]
        public ICollection<IFormFile> Images { get; set; }
    }
}
