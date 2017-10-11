using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.BLL.Models;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class CarListViewModel
    {
        public CarListQuery Query { get; set; }
        public ICollection<Car> Cars { get; set; }
        public PagingInfo Pager { get; set; }
    }
}
