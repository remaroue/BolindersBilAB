using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WU16.BolindersBilAB.BLL.Helpers;
using WU16.BolindersBilAB.BLL.Models;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class CarService
    {
        private IRepository<Car> _repo;

        public CarService(IRepository<Car> Repo)
        {
            _repo = Repo;
        }

        public IQueryable<Car> GetCars(CarListQuery query = null)
        {
            var cars = _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .Include(x => x.CarImages)
                .OrderBy(x => x.LastUpdated != null ? x.LastUpdated : x.CreationDate)
                .AsQueryable()
                .FilterByQuery(query);

            return cars;
        }

        public Car GetCar(string licenseNumber)
        {
            licenseNumber = CarHelper.NormalizeLicenseNumber(licenseNumber);

            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .Include(x => x.CarImages)
                .FirstOrDefault(x => x.LicenseNumber == licenseNumber);
        }

        public IEnumerable<SimplifiedCar> GetSimilarCars(Car car)
        {
            var query = car.GetSimilarCarsQuery();

            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .Include(x => x.CarImages)
                .Where(x => x.LicenseNumber != car.LicenseNumber)
                .FilterByQuery(query)
                .Select(x => new SimplifiedCar()
                {
                    Title = $"{x.CarBrand.BrandName} {x.Model} {x.ModelYear}",
                    LicenseNumber = x.LicenseNumber,
                    ImageName = x.CarImages.FirstOrDefault().FileName ?? null,
                    Price = x.Price
                }).ToArray();
        }

        public void SaveCar(Car car)
        {
            car.LicenseNumber = CarHelper.NormalizeLicenseNumber(car.LicenseNumber);

            _repo.Insert(car);
            _repo.Save();
        }

        public Car DeleteCar(Car car)
        {
            _repo.Delete(car);
            _repo.Save();
            return car;
        }

    }
}
