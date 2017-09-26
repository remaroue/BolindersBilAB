using System;
using System.Collections.Generic;
using System.Text;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class CarBrandService
    {
        private IRepository<CarBrand> _repo;

        public CarBrandService(IRepository<CarBrand> Repo)
        {
            _repo = Repo;
        }

        public IEnumerable<CarBrand> Get()
        {
            return _repo.Get();
        }
        public void SaveBrand(CarBrand carBrand)
        {
            _repo.Insert(carBrand);
            _repo.Save();

        }
        public void DeleteBrand(CarBrand carBrand)
        {
            _repo.Delete(carBrand);
            _repo.Save();
            
        }
    }
}
