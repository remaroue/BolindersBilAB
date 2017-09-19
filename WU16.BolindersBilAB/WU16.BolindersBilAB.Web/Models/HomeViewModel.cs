using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class HomeViewModel
    {
        [Required]
        [DisplayName("SearchQuery")]
        [DataType(DataType.Text)]
        public string SearchQuery { get; set; }
    }
}
