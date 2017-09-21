using System;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedFromArrayAttribute : Attribute
    {
        public object[] Values { get; private set; }

        public SeedFromArrayAttribute(object[] values)
        {
            Values = values;
        }
    }
}

