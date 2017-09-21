using System;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    public class SeedPatternCreateStringAttribute : Attribute
    {
        public CharacterDescription[] Description { get; private set; }

        public SeedPatternCreateStringAttribute(params CharacterDescription[] description)
        {
            Description = description;
        }
    }
}

