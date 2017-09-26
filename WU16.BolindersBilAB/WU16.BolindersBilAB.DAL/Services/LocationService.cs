using System;
using System.Collections.Generic;
using System.Text;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;

namespace WU16.BolindersBilAB.DAL.Services
{
   public class LocationService
    {
        private IRepository<Location> _repo;
        public LocationService(IRepository<Location> Repo)
        {
            _repo = Repo;
        }

        public IEnumerable<Location> Get()
        {
            return _repo.Get();
        }
    }
}
