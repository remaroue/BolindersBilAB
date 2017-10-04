using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class AddBrandViewModel
    {
        public string BrandName { get; set; }
         public IFormFile Image { get; set; }
    }
}
