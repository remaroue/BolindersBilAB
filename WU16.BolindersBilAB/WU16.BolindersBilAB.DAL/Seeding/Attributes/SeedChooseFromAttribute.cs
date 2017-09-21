using System;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedChooseFromAttribute : Attribute
    {
        public object[] Values { get; private set; }

        public SeedChooseFromAttribute(params object[] values)
        {
            Values = values;
        }
    }
}

