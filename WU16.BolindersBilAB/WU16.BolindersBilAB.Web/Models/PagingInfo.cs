using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class PagingInfo
    {
        public int TotalItems { get; set; }
        public int ItemsPerPage { get; set; }
        public int CurrentPage { get; set; }
    }
}
