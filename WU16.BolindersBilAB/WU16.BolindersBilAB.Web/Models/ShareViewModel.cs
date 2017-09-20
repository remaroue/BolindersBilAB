using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Helpers;

namespace WU16.BolindersBilAB.Web.Models
{
    public class ShareViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string LicenseNumber { get; set; }
    }
}
