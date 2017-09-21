using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class HomeViewModel
    {
        public string CarBrand { get; set; }
        public int CarCount { get; set; }
        public string CarImage { get; set; }
    }
}
