using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using WU16.BolindersBilAB.BLL.Helpers;
using WU16.BolindersBilAB.BLL.Models;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;
using System;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class CarService
    {
        private IRepository<Car> _repo;
        private ImageService _imageService;

        public CarService(IRepository<Car> Repo, ImageService imageService)
        {
            _repo = Repo;
            _imageService = imageService;
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
            var cars = _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .Include(x => x.CarImages)
                .AsQueryable();

            if (brand == "brand")
                cars = cars.Where(x => string.IsNullOrEmpty(x.CarBrandId));
            else
                cars = cars.Where(x => x.CarBrandId == brand);

            if (model == "model")
                cars = cars.Where(x => string.IsNullOrEmpty(x.Model));
            else
                cars = cars.Where(x => x.Model == model);

            if (modelDescription == "model-desc")
                cars = cars.Where(x => string.IsNullOrEmpty(x.ModelDescription));
            else
                cars = cars.Where(x => x.ModelDescription == modelDescription);

            return cars.FirstOrDefault(x => x.GetUnique() == unique);
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
                    ImageName = x.CarImages.OrderBy(y => y.Priority).FirstOrDefault().FileName ?? null,
                    Price = x.Price,
                    Url = x.GetUrl()
                }).ToArray();
        }

        /// <summary>
        /// Should be internal, leave it
        /// </summary>
        /// <param name="car"></param>
        internal void SaveCar(Car car)
        {
            car.LicenseNumber = CarHelper.NormalizeLicenseNumber(car.LicenseNumber);
            car.CreationDate = DateTime.Now;

            _repo.Insert(car);
            _repo.Save();
        }

        public void SaveCar(CarFormViewModel model)
        {
            var car = new Car
            {
                LicenseNumber = CarHelper.NormalizeLicenseNumber(model.LicenseNumber),
                Model = model.Model,
                Description = model.Description,
                ModelYear = model.ModelYear,
                IsLeaseable = model.IsLeaseable,
                Milage = model.Milage,
                Price = model.Price,
                Color = model.Color,
                HorsePower = model.HorsePower,
                Used = model.Used,
                LocationId = model.LocationId,
                CarBrandId = model.CarBrandId,
                Equipment = model.Equipment,
                CarType = model.CarType,
                FuelType = model.FuelType,
                Gearbox = model.Gearbox,
                ModelDescription = model.ModelDescription,
                CreationDate = DateTime.Now
            };

            if (model.Images?.Count > 0)
            {
                car = _imageService.AddImageToCar(car, model.Images.ToArray());
            }

            _repo.Insert(car);
            _repo.Save();
        }

        public Car DeleteCar(Car car)
        {
            _repo.Delete(car);
            _repo.Save();
            return car;
        }

        public void UpdateCar(Car car, CarFormViewModel model = null)
        {
            car.LastUpdated = DateTime.Now;

            if (model != null)
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

                if (car.CarImages?.Count > 0)
                {
                    
                    var images = car.CarImages.ToList();
                    var newOrder = (model.ExistingImages ?? new string[0]);

                    var removed = images.Where(x => !newOrder.Contains(x.FileName)).ToArray();
                    foreach (var img in removed)
                    {
                        _imageService.RemoveImage(img.FileName);
                        car.CarImages.Remove(img);
                    }

                    if (car.CarImages.Count > 0 && newOrder.Length > 0)
                    {
                        var imgs = new List<CarImage>();
                        var i = 0;
                        foreach (var name in newOrder)
                        {
                            var img = images.FirstOrDefault(x => x.FileName == name);
                            img.Priority = i;

                            imgs.Add(img);
                            i++;
                        }

                        car.CarImages = imgs;
                    }

                    if (model.Images?.Count > 0)
                        car = _imageService.AddImageToCar(car, model.Images.ToArray());
                }
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
