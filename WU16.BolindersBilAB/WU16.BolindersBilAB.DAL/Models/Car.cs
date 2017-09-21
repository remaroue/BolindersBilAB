using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using WU16.BolindersBilAB.DAL.DataAccess;
using WU16.BolindersBilAB.DAL.Seeding.Attributes;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Models
{
    [SeedingSettings(SeedingType.Implicit)]
    public class Car
    {
        [Key]
        [Display(Name ="Registreringsnummer")]
        [SeedPatternCreateStringAttribute(CharacterDescription.AToZ, CharacterDescription.AToZ, CharacterDescription.AToZ, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine)]
        public string LicenseNumber { get; set; }

    
        [ForeignKey("CarBrand")]
        [SeedChooseFrom("Volvo", "BMW", "Audi", "Ford", "Mercedes-benz", "Volkswagen")]
        public string CarBrandId { get; set; }

        [SeedIgnore]
        public virtual CarBrand CarBrand { get; set; }

        [ForeignKey("Location")]
        [SeedChooseFrom("BB1", "BB2", "BB3")]
        public string LocationId { get; set; }

        [SeedIgnore]
        public virtual Location Location { get; set; }

        [SeedFixedValue("test model")]
        public string Model { get; set; }

        [SeedDataType(SeederDataType.LoremIpsum)]
        public string Description { get; set; }

        [SeedDataType(SeederDataType.Year)]
        public int ModelYear { get; set; }

        public bool IsLeaseable { get; set; }

        [SeedNumericValue(100, 20000)]
        public int Milage { get; set; }

        [SeedNumericValue(30000, 150000)]
        public decimal Price { get; set; }

        [SeedChooseFrom("grå", "grön", "mörkblå", "rengbåge")]
        public string Color { get; set; }

        [SeedNumericValue(80, 200)]
        public int HorsePower { get; set; }

        public bool Used { get; set; }

        [SeedDataType(SeederDataType.Now)]
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
