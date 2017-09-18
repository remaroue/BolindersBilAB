using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
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

        public IEnumerable<Car> GetCars(Expression<Func<Car, bool>> filter = null)
        {
            if(filter != null)
            {
                return _repo.Get()
                    .Include(x => x.CarBrand)
                    .Include(x => x.Location)
                    .Where(filter);
            }

            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location);
        }

        public Car GetCar(string licenseNumber)
        {
            return _repo.Get()
                .Include(x => x.CarBrand)
                .Include(x => x.Location)
                .FirstOrDefault(x => x.LicenseNumber == licenseNumber);
        }
    }
}
