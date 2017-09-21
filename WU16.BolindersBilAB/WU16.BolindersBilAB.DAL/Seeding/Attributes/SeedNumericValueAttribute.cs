using System;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    /// <summary>
    /// Supports Int, Double, Decimal
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedNumericValueAttribute : Attribute
    {
        public object Min { get; set; }
        public object Max { get; set; }

        public SeedNumericValueAttribute(object min, object max)
        {
            Min = min;
            Max = max;
        }
    }
}

