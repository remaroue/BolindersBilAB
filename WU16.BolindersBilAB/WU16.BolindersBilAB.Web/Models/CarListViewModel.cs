using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class CarListViewModel
    {
        public CarListQuery Query { get; set; }
        public IEnumerable<Car> Cars { get; set; }
        public PagingInfo Pager { get; set; }

    }
}
