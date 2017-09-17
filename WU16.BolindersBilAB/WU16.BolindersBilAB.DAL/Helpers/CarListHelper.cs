using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.DAL.Helpers
{
    public static class CarListHelper
    {
        public static IEnumerable<Car> Filter(string parameter, IEnumerable<Car> cars)
        {
            bool isUsed = true;

            switch(parameter)
            {
                case "nya":
                    isUsed = false;
                    break;
                case "begagnade":
                    isUsed = true;
                    break;
                default:
                    return cars;
            }

            return cars.Where(x => x.Used == isUsed).AsEnumerable();
        }

    }
}
