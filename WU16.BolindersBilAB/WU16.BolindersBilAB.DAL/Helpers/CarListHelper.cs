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
        private static Regex _pattern = new Regex("[^a-z,0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        public static string NormalizeLicenseNumber(string input)
        {
            return _pattern.Replace(input.ToUpper(), string.Empty);
        }

        public static IQueryable<Car> FilterByParameter(this IQueryable<Car> cars, string parameter)
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

            return cars.Where(x => x.Used == isUsed);
        }

        public static IQueryable<Car> FilterByQuery(this IQueryable<Car> cars, CarListQuery query)
        {
            if (query == null) return cars;

            if (query.CarType?.Count > 0)
                cars = cars.Where(x => query.CarType.Contains(x.CarType));
            if (query.CarBrand?.Count > 0)
                cars = cars.Where(x => query.CarBrand.Contains(x.CarBrand));
            if (query.Gearbox?.Count > 0)
                cars = cars.Where(x => query.Gearbox.Contains(x.Gearbox));
            if (query.FuelType?.Count > 0)
                cars = cars.Where(x => query.FuelType.Contains(x.FuelType));


            if (query.MilageFrom > 0)
                cars = cars.Where(x => x.Milage >= query.MilageFrom);
            if (query.MilageTo > 0)
                cars = cars.Where(x => x.Milage <= query.MilageTo);

            if (query.PriceFrom > 0)
                cars = cars.Where(x => x.Price >= query.PriceFrom);
            if (query.PriceTo > 0)
                cars = cars.Where(x => x.Price <= query.PriceTo);

            if (query.YearFrom > 0)
                cars = cars.Where(x => x.ModelYear >= query.YearFrom);
            if (query.YearTo > 0)
                cars = cars.Where(x => x.ModelYear <= query.YearTo);

            if (query.Skip > 0)
                cars = cars.Skip(query.Skip);
            if (query.Take > 0)
                cars = cars.Skip(query.Take);

            return cars;
        }

        public static CarListQuery GetSimilarCarsQuery(this Car car)
        {
            return new CarListQuery()
            {
                CarType = new List<CarType>() { car.CarType },
                Gearbox = new List<Gearbox>() { car.Gearbox },
                FuelType = new List<FuelType>() { car.FuelType },
                MilageFrom = (int)(car.Milage * 0.8),
                MilageTo = (int)(car.Milage * 1.2),
                PriceFrom = car.Price,
                PriceTo = car.Price * 1.4m,
                YearFrom = car.ModelYear - 5,
                YearTo = car.ModelYear + 5,
                CarBrand = new List<CarBrand>() { car.CarBrand },
                Take = 4
            };
        }
    }
}
