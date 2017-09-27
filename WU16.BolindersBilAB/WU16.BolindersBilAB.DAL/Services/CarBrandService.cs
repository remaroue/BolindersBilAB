using System;
using System.Collections.Generic;
using System.Linq;
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

        public IQueryable<CarBrand> Get()
        {
            return _repo.Get();
        }

        public CarBrand GetBrand(string id)
        {
            return _repo.Get().FirstOrDefault(x => x.BrandName == id);
        }

        public void Update(CarBrand carbrand)
        {
            _repo.Edit(carbrand);
            _repo.Save();
        }
    }
}
