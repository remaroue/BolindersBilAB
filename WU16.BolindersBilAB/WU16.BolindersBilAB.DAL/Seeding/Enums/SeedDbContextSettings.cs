using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WU16.BolindersBilAB.DAL.Seeding.Enums
{
    public enum SeedDbContextSettings
    {
        /// <summary>
        /// Default
        /// </summary>
        AppendToExisting,
        /// <summary>
        /// Removes Existing Rows before seeding
        /// </summary>
        ReplaceExisting,
        /// <summary>
        /// Do not seed if there are rows
        /// </summary>
        LeaveIfExists
    }
}
