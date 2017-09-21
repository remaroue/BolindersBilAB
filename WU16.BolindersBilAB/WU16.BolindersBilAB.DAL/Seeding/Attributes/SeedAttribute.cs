using System;
using System.Collections.Generic;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Seeding.Attributes
{
    /// <summary>
    /// Tells the Seeder it's okay to try to seed this without explicit instruction.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = false)]
    class SeedAttribute : Attribute
    {
    }
}
