using System;
using System.Collections.Generic;
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

        public IEnumerable<Car> GetCars()
        {        
            return _repo.Get();
        }

    }
}
