using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WU16.BolindersBilAB.BLL.Helpers;
using WU16.BolindersBilAB.BLL.Models;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;
using System;

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
                .OrderByDescending(x => x.LastUpdated)
                .ThenByDescending(x => x.CreationDate)
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

        public Car GetCar(string brand, string model, string modelDescription, string unique)
        {
            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .Include(x => x.CarImages)
                .Where(x => x.CarBrandId == brand && x.Model == model && x.ModelDescription == modelDescription)
                .FirstOrDefault(x => x.GetUnique() == unique);
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
                    Price = x.Price,
                    Url = x.GetUrl()
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

        public void UpdateCar(Car car, Car model = null)
        {
            car.LastUpdated = DateTime.Now;

            if(model != null)
            {
                car.IsLeaseable = model.IsLeaseable;
                car.Used = car.Used;

                car.Milage = model.Milage;
                car.Price = model.Price;
                car.HorsePower = model.HorsePower;

                car.Model = model.Model;
                car.ModelDescription = model.ModelDescription;
                car.ModelYear = model.ModelYear;

                car.LocationId = model.LocationId;
                car.CarBrandId = model.CarBrandId;


                car.CarType = model.CarType;
                car.FuelType = model.FuelType;
                car.Gearbox = model.Gearbox;

                car.Color = car.Color;
                car.Equipment = model.Equipment;
                car.Description = model.Description;
            }

            _repo.Edit(car);
            _repo.Save();
        }

        public List<Car> DeleteCars(IEnumerable<string> licenseNumbers)
        {
            var cars = _repo.Get().Include(x => x.CarImages).Where(x => licenseNumbers.Contains(x.LicenseNumber)).ToList();

            _repo.Delete(cars);
            _repo.Save();

            return cars;
        }
    }
}
