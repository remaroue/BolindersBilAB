using System;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedDataTypeAttribute : Attribute
    {
        public SeederDataType DataType { get; private set; }

        public SeedDataTypeAttribute(SeederDataType dataType)
        {
            DataType = dataType;
        }
    }
}

