using System;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedFixedValueAttribute : Attribute
    {
        public object Value { get; private set; }

        public SeedFixedValueAttribute(object value)
        {
            Value = value;
        }
    }
}

