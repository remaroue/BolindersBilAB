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
        private static readonly Regex _pattern = new Regex("[^a-z,å,ä,ö,0-9]", RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        public static string NormalizeLicenseNumber(string input)
        {
            return _pattern.Replace(input.ToUpper(), string.Empty);
        }

        public static IQueryable<Car> FilterByParameter(this IQueryable<Car> cars, string parameter)
        {
            bool isUsed = true;

            switch (parameter)
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
                cars = cars.Where(x => query.CarBrand.Select(y => x.CarBrandId).Contains(x.CarBrand.BrandName));
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

            // TODO: Free Search match
            if (query.FreeSearch != null)
            {
                IQueryable<Car> tempCars = null;
                if (cars.Any(x => x.Description.Contains(query.FreeSearch)))
                    tempCars = tempCars.Concat(cars.Where(x => x.Description.Contains(query.FreeSearch)));

                if (cars.Any(x => x.Equipment.Contains(query.FreeSearch)))
                    tempCars = tempCars.Concat(cars.Where(x => x.Equipment.Contains(query.FreeSearch)));

                if (cars.Any(x => query.FreeSearch.Contains(x.ToString())))
                {
                    tempCars = tempCars.Concat(cars.Where(x => query.FreeSearch.Contains(x.ToString())));
                }

                if (cars.Any(x => x.Model.Contains(query.FreeSearch)))
                    tempCars = tempCars.Concat(cars.Where(x => x.Model.Contains(query.FreeSearch)));

                cars = tempCars ?? cars;
            }

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
        public static IEnumerable<Car> PaginateCars(this IEnumerable<Car> cars, int page)
        {
            return cars.Take((8 * page)).AsEnumerable();
        }

        public static Dictionary<int, string> GetModelYears()
        {
            var years = new Dictionary<int, string>();
            var startYear = DateTime.Now.Year;
            while(startYear > 1940)
            {
                if(startYear > 1980)
                {
                years.Add(startYear, startYear.ToString());
                startYear--;
                }
                else
                {
                    years.Add(startYear, startYear.ToString());
                    startYear -= 10;
                }
            };

            years.Add(1940, "1940 eller äldre");

            return years;
        }

        public static Dictionary<int, string> GetPriceRange()
        {
            var prices = new Dictionary<int, string>();
            var startPrice = 5000;

            while(startPrice < 1000000)
            {
                prices.Add(startPrice, startPrice.ToString("# ### ### ###kr"));
                if(startPrice < 10000)
                {
                    startPrice += 1000;
                }
                else if(startPrice < 200000)
                {
                    startPrice += 10000;
                }
                else if(startPrice < 500000)
                {
                    startPrice += 50000;
                }
                else if(startPrice < 1000000)
                {
                    startPrice += 100000;
                }
            }

            prices.Add(1000001, "1 000 000kr +");

            return prices;
        }
        public static Dictionary<int, string> GetMilageRange()
        {
            var milages = new Dictionary<int, string>();
            var startMilage = 999;

            milages.Add(0, "0");

            while (startMilage < 30000)
            {
                milages.Add(startMilage, startMilage.ToString("## ###"));

                startMilage += 1000;
            }

            milages.Add(30001, "30 000 +");

            return milages;
        }
    }
}
