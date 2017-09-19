using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.DAL.Helpers
{
    public static class CarListHelper
    {
        //private static Regex _pattern = new Regex("[^a-z,0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        //public static string LicenseNumberNormalizer(string input)
        //{
        //    return _pattern.Replace(input.ToUpper(), string.Empty);
        //}

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
        public static IEnumerable<Car> FilterByQuery(CarListQuery query,IEnumerable<Car> cars)
        {
            if (query.CarType.Count > 0)
                cars = cars.Where(x => query.CarType.Contains(x.CarType));
            if (query.MilageFrom > 0)
                cars = cars.Where(x => x.Milage >= query.MilageFrom);
            if (query.MilageTo > 0)
                cars = cars.Where(x => x.Milage <= query.MilageTo);
            if (query.Gearbox.Count > 0)
                cars = cars.Where(x => query.Gearbox.Contains(x.Gearbox));
            if (query.FuelType.Count > 0)
                cars = cars.Where(x => query.FuelType.Contains(x.FuelType));
            if (query.PriceFrom > 0)
                cars = cars.Where(x => x.Price >= query.PriceFrom);
            if (query.PriceTo > 0)
                cars = cars.Where(x => x.Price <= query.PriceTo);
            if (query.YearFrom > 0)
                cars = cars.Where(x => x.ModelYear >= query.YearFrom);
            if (query.YearTo > 0)
                cars = cars.Where(x => x.ModelYear <= query.YearTo);

            return cars;
        }
    }
}
