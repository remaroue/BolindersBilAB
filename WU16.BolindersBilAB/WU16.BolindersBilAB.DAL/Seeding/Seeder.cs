using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using WU16.BolindersBilAB.DAL.Seeding.Attributes;
using WU16.BolindersBilAB.DAL.Seeding.Enums;
using WU16.BolindersBilAB.DAL.Seeding.Helper;

namespace WU16.BolindersBilAB.DAL.Seeding
{
    public class Seeder<T> where T : class, new()
    {
        private static readonly Dictionary<Type, Func<Attribute, PropertyInfo, T[], T[]>> _handlers = new Dictionary<Type, Func<Attribute, PropertyInfo, T[], T[]>>()
        {
            { typeof(SeedDataTypeAttribute), DefaultHandlers<T>.HandleSeedDataTypeAttribute },
            { typeof(SeedChooseFromAttribute), DefaultHandlers<T>.HandleSeedChooseFromAttribute },
            { typeof(SeedFixedValueAttribute), DefaultHandlers<T>.HandleSeedFixedValueAttribute },
            { typeof(SeedNumericValueAttribute), DefaultHandlers<T>.HandleSeedNumericValueAttribute },
            { typeof(SeedPatternCreateStringAttribute), DefaultHandlers<T>.HandleSeedPatternCreateStringAttribute }
        };

        public static T[] Seed(int numberOfRows)
        {
            var useOnlyAttributes = false;

            if (Attribute.IsDefined(typeof(T), typeof(SeedingSettingsAttribute)))
            {
                var attr = Attribute.GetCustomAttribute(typeof(T), typeof(SeedingSettingsAttribute)) as SeedingSettingsAttribute;

                switch (attr.SeedingType)
                {
                    case SeedingType.Explicit:
                        useOnlyAttributes = true;
                        break;
                }
            }

            var rows = new T[numberOfRows];
            for (int i = 0; i < numberOfRows; i++)
                rows[i] = new T();

            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (Attribute.IsDefined(property, typeof(SeedIgnoreAttribute)))
                    continue;

                // Find Appropritive Handler
                var pair = _handlers.FirstOrDefault(x => Attribute.IsDefined(property, x.Key));

                if (pair.Key != null)
                {
                    // Invoke Handler
                    rows = pair.Value.Invoke(Attribute.GetCustomAttribute(property, pair.Key), property, rows);
                }
                else
                {
                    if (useOnlyAttributes)
                        if (!Attribute.IsDefined(property, typeof(SeedAttribute)))
                            continue;

                    if (property.PropertyType.IsPrimitive)
                    {
                        if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Boolean)
                        {
                            var rand = new Random();
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], Convert.ToBoolean(rand.Next(2)));
                        }
                    }

                    else if (property.PropertyType.IsEnum)
                    {
                        var rand = new Random();
                        var enums = Enum.GetNames(property.PropertyType).Select(x => Enum.Parse(property.PropertyType, x)).ToArray();

                        for (int i = 0; i < numberOfRows; i++)
                            property.SetValue(rows[i], enums[rand.Next(0, enums.Length)]);
                    }
                }
            }

            return rows;
        }

        public static void SeedDbContext(DbContext dbContext, int numberOfRows, SeedDbContextSettings settings = SeedDbContextSettings.AppendToExisting)
        {
            switch (settings)
            {
                case SeedDbContextSettings.ReplaceExisting:
                    dbContext.Database.ExecuteSqlCommand($"TRUNCATE TABLE {dbContext.GetTableName<T>()}");
                    break;
                case SeedDbContextSettings.LeaveIfExists:
                    if (dbContext.Set<T>().Any())
                        return;
                    break;
                default:
                    break;
            }

            var rows = Seed(numberOfRows);

            dbContext.Set<T>().AddRange(rows);
            dbContext.SaveChanges();
        }
    }
}


