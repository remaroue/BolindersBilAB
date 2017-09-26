using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class ContactMailViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }


        public string Subject { get; set; }
        public string Message { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
