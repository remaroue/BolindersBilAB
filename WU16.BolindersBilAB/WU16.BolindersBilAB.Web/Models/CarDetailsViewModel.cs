using System.Collections.Generic;
using WU16.BolindersBilAB.BLL.Models;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class CarDetailsViewModel
    {
        public Car Car { get; set; }
        public IEnumerable<SimplifiedCar> SimilarCars { get; set; }
    }
}
