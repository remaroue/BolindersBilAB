using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class CarSearchService
    {
        private IRepository<CarBrand> _carbrandRepository;

        public CarSearchService(IRepository<CarBrand> carbrandRepository)
        {
            _carbrandRepository = carbrandRepository;
        }

        private CarListQuery GetEnums(string input)
        {
            var splitInput = input.Split('\n', ',', '.', ' ');

            var fuelTypes = Enum.GetNames(typeof(FuelType))
                .Where(x => splitInput.Any(y => y == x.ToLower()))
                .Select(x => (FuelType)Enum.Parse(typeof(FuelType), x));

            var carTypes = Enum.GetNames(typeof(CarType))
                .Where(x => splitInput.Any(y => y == x.ToLower()))
                .Select(x => (CarType)Enum.Parse(typeof(CarType), x));

            var gearboxes = Enum.GetNames(typeof(Gearbox))
                .Where(x => splitInput.Any(y => y == x.ToLower()))
                .Select(x => (Gearbox)Enum.Parse(typeof(Gearbox), x));

            return new CarListQuery()
            {
                FuelType = fuelTypes.ToList(),
                CarType = carTypes.ToList(),
                Gearbox = gearboxes.ToList()
            };
        }

        public CarListQuery GetCarListQuery(string input)
        {
            if (input == null) return null;

            input = input.ToLower();

            var query = GetEnums(input);
            query.CarBrand = _carbrandRepository.Get().Where(x => input.Contains(x.BrandName.ToLower())).ToList();
            query.FreeSearch = input;

            return query;
        }
    }
}
