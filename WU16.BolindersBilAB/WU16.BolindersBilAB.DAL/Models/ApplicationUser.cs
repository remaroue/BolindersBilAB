using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual Location Location { get; set; }
    }
}
