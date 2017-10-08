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
        [Required(ErrorMessage="Registreringsnummer måste anges.")]
        [Display(Name = "Registreringsnummer")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "Model måste anges.")]
        [Display(Name = "Modell")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Model Beskrivning måste anges.")]
        [Display(Name = "Modell Beskrivning")]
        public string ModelDescription { get; set; }

        [Display(Name = "Beskrivning")]
        public string Description { get; set; }
        [Required(ErrorMessage = "Årsmodell måste anges.")]
        [Display(Name = "Årsmodell")]
        public int ModelYear { get; set; }

        [Required]
        [Display(Name = "Leasbar")]
        public bool IsLeaseable { get; set; }

        [Required(ErrorMessage = "Mätarställning(mil) måste anges.")]
        [Display(Name = "Mätarställning(mil)")]
        public int? Milage { get; set; }
        [Required(ErrorMessage = "Pris måste anges.")]
        [Display(Name = "Pris")]
        public decimal? Price { get; set; }
        [Required(ErrorMessage = "Färg måste anges.")]
        [Display(Name = "Färg")]
        public string Color { get; set; }
        [Required(ErrorMessage = "Hästkrafter måste anges.")]
        [Display(Name = "Hästkrafter")]
        public int? HorsePower { get; set; }
        [Required]
        [Display(Name = "Begagnad")]
        public bool Used { get; set; }
        [Required(ErrorMessage = "Anläggning måste anges.")]
        [Display(Name = "Anläggning")]
        public string LocationId { get; set; }
        [Required(ErrorMessage = "Bilmärke måste anges.")]
        [Display(Name = "Bilmärke")]
        public string CarBrandId { get; set; }
        [Required(ErrorMessage = "Utrustning måste anges.")]
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
