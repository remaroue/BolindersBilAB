using System;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedStringFromEnumArrayAttribute : Attribute
    {
        public CharacterDescription[] Description { get; private set; }

        public SeedStringFromEnumArrayAttribute(CharacterDescription[] description)
        {
            Description = description;
        }
    }
}

