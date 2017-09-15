using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Models
{
    public class Location
    {
        [Key]
        public string Id { get; set; }
        public string LocationName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public string Zip { get; set; }
        public string City { get; set; }
        public virtual ICollection<Car> Cars { get; set; }
    }
}
