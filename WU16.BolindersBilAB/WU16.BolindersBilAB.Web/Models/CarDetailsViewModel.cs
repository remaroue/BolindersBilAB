using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class CarDetailsViewModel
    {
        public Car Car { get; set; }
        public IEnumerable<SimplifiedCar> SimilarCars { get; set; }
    }
}
