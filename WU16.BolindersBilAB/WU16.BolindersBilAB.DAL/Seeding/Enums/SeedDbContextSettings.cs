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
        /// Tries Removing Existing Rows before seeding, else crash and burn.
        /// </summary>
        ReplaceExisting,
        /// <summary>
        /// Do not seed if there are existing rows.
        /// </summary>
        LeaveIfExists
    }
}
