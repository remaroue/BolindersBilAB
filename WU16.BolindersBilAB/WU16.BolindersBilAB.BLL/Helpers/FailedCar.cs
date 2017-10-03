using System;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.BLL.Helpers
{
    public class FailedCar
    {
        public Car Car { get; set; }
        public Exception Exception { get; set; }
    }
}
