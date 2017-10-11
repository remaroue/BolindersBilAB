using System.Collections.Generic;
using System.Linq;
using WU16.BolindersBilAB.BLL.Models;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.Web.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class CarAdminListViewModel
    {
        public List<Car> Cars { get; set; }
        public CarListQuery Query { get; set; }
        public PagingInfo Pager { get; set; }
        public IEnumerable<CarBrand> Carbrands { get; internal set; }
    }
}