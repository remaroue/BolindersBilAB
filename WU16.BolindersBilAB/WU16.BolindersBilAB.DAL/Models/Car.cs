using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using WU16.BolindersBilAB.DAL.Seeding.Attributes;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Models
{
    [XmlType("car")]
    public class Car
    {
        // [XmlElement("updated")]string lastUpdated, [XmlElement("exkl_moms")]string isLeasable, [XmlElement("price")]string price, [XmlElement("miles")]string milage, [XmlElement("horsepower")]string horsepower
        [Key]
        [Display(Name = "Registreringsnummer")]
        [SeedPatternCreateString(CharacterDescription.AToZ, CharacterDescription.AToZ, CharacterDescription.AToZ, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine, CharacterDescription.ZeroToNine)]
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

        [SeedChooseFrom("v70", "TT3", "turbo", "TT4", "v60", "v40", "i3", "i5", "roadster")]
        public string Model { get; set; }

        [SeedIgnore]
        public string ModelDescription { get; set; }

        [SeedDataType(SeederDataType.LoremIpsum)]
        public string Description { get; set; }

        [SeedDataType(SeederDataType.Year)]
        public int ModelYear { get; set; }

        //[XmlElement("exkl_moms")]
        
        public bool IsLeaseable { get; set; }

        [SeedNumericValue(100, 20000)]
        //[XmlElement("milage")]
        
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
        public virtual ICollection<CarImage> CarImages { get; set; }

        

        public CarType CarType { get; set; }
        

        public FuelType FuelType { get; set; }
        

        public Gearbox Gearbox { get; set; }

        [SeedFixedValue("test|elsäten|laxskinsäten")]
        public string Equipment { get; set; }

        [SeedIgnore]
        public string FtpId { get; set; }
    }
}
