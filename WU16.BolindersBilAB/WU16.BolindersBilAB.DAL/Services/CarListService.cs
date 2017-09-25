using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using WU16.BolindersBilAB.DAL.Helpers;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class CarListService
    {
        private IRepository<Car> _repo;

        public CarListService(IRepository<Car> Repo)
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
            licenseNumber = CarListHelper.NormalizeLicenseNumber(licenseNumber);

            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .FirstOrDefault(x => x.LicenseNumber == licenseNumber);
        }

        public IEnumerable<SimilarCarViewModel> GetSimilarCars(Car car)
        {
            var query = car.GetSimilarCarsQuery();

            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .Include(x => x.CarImages)
                .Where(x => x.LicenseNumber != car.LicenseNumber)
                .FilterByQuery(query)
                .Select(x => new SimilarCarViewModel()
                {
                    Title = $"{x.CarBrand.BrandName} {x.Model} {x.ModelYear}",
                    LicenseNumber = x.LicenseNumber,
                    ImageName = x.CarImages.FirstOrDefault().FileName ?? null
                }).ToArray();
        }
    }
}
