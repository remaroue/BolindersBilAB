using System;
using System.Collections.Generic;
using System.Text;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    class SeedingSettingsAttribute : Attribute
    {
        public SeedingType SeedingType { get; private set; }

        public SeedingSettingsAttribute(SeedingType seedingType)
        {
            SeedingType = seedingType;
        }
    }
}
