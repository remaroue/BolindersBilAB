using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.Models
{
    public class ContactsViewModel
    {
        public IEnumerable<Location> Locations { get; set; }
        public bool? Sent { get; set; }
        public ContactMailViewModel FormModel { get; set; }
    }
}