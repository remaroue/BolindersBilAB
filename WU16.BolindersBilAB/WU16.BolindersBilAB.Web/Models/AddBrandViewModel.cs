using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class AddBrandViewModel
    {
        [Required(ErrorMessage ="Du måste ange ett namn")]
        [Display(Name = "Namn")]
        public string BrandName { get; set; }
         public IFormFile Image { get; set; }
    }
}
