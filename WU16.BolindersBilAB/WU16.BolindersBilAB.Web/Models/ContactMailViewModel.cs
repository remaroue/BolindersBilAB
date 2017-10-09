using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class ContactMailViewModel
    {
        public string Reciever { get; set; }

        [Required(ErrorMessage = "Förnamn måste anges.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Efternamn måste anges.")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Ämne måste anges.")]
        public string Subject { get; set; }
        [Required(ErrorMessage = "Medelande måste anges.")]
        public string Message { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
    }
}
