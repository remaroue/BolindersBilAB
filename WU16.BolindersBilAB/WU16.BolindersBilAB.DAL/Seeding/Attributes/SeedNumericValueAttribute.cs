using System;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedNumericValueAttribute : Attribute
    {
        public int Min { get; set; }
        public int Max { get; set; }

        public SeedNumericValueAttribute(int min, int max)
        {
            Min = min;
            Max = max;
        }
    }
}

