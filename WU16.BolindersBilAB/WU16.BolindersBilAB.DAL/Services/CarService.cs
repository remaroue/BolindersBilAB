using System;
using System.Collections.Generic;
using System.Text;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class CarService
    {
        private IRepository<Car> _repo;
        public CarService(IRepository<Car> Repo)
        {
            _repo = Repo;
        }

        public void SaveCar(Car car)
        {
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
