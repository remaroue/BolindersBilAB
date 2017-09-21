using System;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedAttribute : Attribute
    {
        public SeederDataType DataType { get; private set; }

        public SeedAttribute(SeederDataType dataType)
        {
            DataType = dataType;
        }
    }
}

