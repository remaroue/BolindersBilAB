using System.ComponentModel.DataAnnotations;

namespace WU16.BolindersBilAB.Web.Models
{
    public class ShareViewModel
    {
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        public string LicenseNumber { get; set; }
    }
}
