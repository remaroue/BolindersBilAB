using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WU16.BolindersBilAB.DAL.DataAccess;

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
        [SeedStringFromEnumArray(new CharacterDescription[] { CharacterDescription.AToZ, CharacterDescription.AToZ, CharacterDescription.AToZ, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine })]
        public string LicenseNumber { get; set; }

        [SeedFromArray(new object[] { "Volvo", "BMW", "Audi", "Ford", "Mercedes-benz", "Volkswagen" })]
        [ForeignKey("CarBrand")]
        public string CarBrandId { get; set; }

        [SeedIgnore]
        public virtual CarBrand CarBrand { get; set; }

        [SeedFromArray(new object[] { "BB1", "BB2", "BB3" })]
        [ForeignKey("Location")]
        public string LocationId { get; set; }
        [SeedIgnore]
        public virtual Location Location { get; set; }

        [SeedFixedValue("test model")]
        public string Model { get; set; }

        [Seed(SeederDataType.LoremIpsum)]
        public string Description { get; set; }

        [Seed(SeederDataType.Year)]
        public int ModelYear { get; set; }

        public bool IsLeaseable { get; set; }

        [SeedNumericValue(100, 20000)]
        public int Milage { get; set; }

        [SeedNumericValue(30000, 150000)]
        public decimal Price { get; set; }

        [SeedFromArray(new object[] {"grå", "grön", "mörkblå", "rengbåge"})]
        public string Color { get; set; }

        [SeedNumericValue(80, 200)]
        public int HorsePower { get; set; }

        public bool Used { get; set; }

        [Seed(SeederDataType.Now)]
        public DateTime CreationDate { get; set; }
        [SeedIgnore]
        public DateTime? LastUpdated { get; set; }
        [SeedIgnore]
        public virtual ICollection<CarImage>  CarImages { get; set; }

        public CarType CarType { get; set; }
        public FuelType FuelType { get; set; }
        public Gearbox Gearbox { get; set; }

        [SeedFixedValue("test|elsäten|laxskinsäten")]
        public string Equipment { get; set; }
    }
}
