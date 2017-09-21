using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Models
{
    public class CarBrand
    {
        [Key]
        public string BrandName { get; set; }
        public string ImageUrl { get; set; }
        public virtual ICollection<Car> Cars { get; set; }

    }
}
