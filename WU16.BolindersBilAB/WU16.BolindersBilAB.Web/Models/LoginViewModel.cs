using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WU16.BolindersBilAB.Web.Models
{
    public class LoginViewModel
    {
        [Required]
        [DisplayName("E-post")]
        [DataType(DataType.EmailAddress)]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Lösenord saknas.")]
        [DisplayName("Lösenord")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DisplayName("Kom ihåg mig")]
        public bool RememberMe { get; set; }
    }
}
