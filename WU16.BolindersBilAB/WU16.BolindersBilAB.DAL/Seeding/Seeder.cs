using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Reflection;
using WU16.BolindersBilAB.DAL.Seeding.Attributes;
using WU16.BolindersBilAB.DAL.Seeding.Enums;
using WU16.BolindersBilAB.DAL.Seeding.Helper;

namespace WU16.BolindersBilAB.DAL.Seeding
{
    public static class Seeder<T> where T : class, new()
    {
        private static readonly Dictionary<Type, Func<Attribute, PropertyInfo, T[], T[]>> _handlers = new Dictionary<Type, Func<Attribute, PropertyInfo, T[], T[]>>()
        {
            { typeof(SeedDataTypeAttribute), HandleSeedDataTypeAttribute },
            { typeof(SeedChooseFromAttribute), HandleSeedChooseFromAttribute },
            { typeof(SeedFixedValueAttribute), HandleSeedFixedValueAttribute },
            { typeof(SeedNumericValueAttribute), HandleSeedNumericValueAttribute },
            { typeof(SeedPatternCreateStringAttributeAttribute), HandleSeedStringFromEnumArrayAttribute }
        };

        public static void Seed(DbContext dbContext, int numberOfRows)
        {
            var useOnlyAttributes = true;

            if (Attribute.IsDefined(typeof(T), typeof(SeedingSettingsAttribute)))
            {
                var attr = Attribute.GetCustomAttribute(typeof(T), typeof(SeedingSettingsAttribute)) as SeedingSettingsAttribute;

                switch (attr.SeedingType)
                {
                    case SeedingType.Implicit:
                        useOnlyAttributes = false;
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

                // Find Handler
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

            var set = dbContext.Set<T>();
            set.AddRange(rows);
            dbContext.SaveChanges();
        }

        #region Handlers
        private static T[] HandleSeedNumericValueAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(SeedNumericValueAttribute)) as SeedNumericValueAttribute;

            if (property.PropertyType.IsPrimitive)
            {
                if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Int32)
                {
                    var rand = new Random();
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], rand.Next(attr.Min, attr.Max));
                }
                else if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Double)
                {
                    var rand = new Random();
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], Convert.ToDouble(rand.Next(attr.Min, attr.Max)));
                }
            }
            else if (property.PropertyType.Equals(typeof(decimal)))
            {
                var rand = new Random();
                for (int i = 0; i < rows.Length; i++)
                    property.SetValue(rows[i], rand.NextDecimal(Convert.ToDecimal(attr.Min), Convert.ToDecimal(attr.Max)));
            }

            return rows;
        }

        private static T[] HandleSeedFixedValueAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(SeedFixedValueAttribute)) as SeedFixedValueAttribute;

            for (int i = 0; i < rows.Length; i++)
                property.SetValue(rows[i], attr.Value);

            return rows;
        }

        private static T[] HandleSeedChooseFromAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(SeedChooseFromAttribute)) as SeedChooseFromAttribute;
            var rand = new Random();
            for (int i = 0; i < rows.Length; i++)
                property.SetValue(rows[i], attr.Values[rand.Next(0, attr.Values.Length)]);

            return rows;
        }

        private static T[] HandleSeedDataTypeAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = attribute as SeedDataTypeAttribute;

            switch (attr.DataType)
            {
                case SeederDataType.LoremIpsum:
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum quis velit dolor. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tempor diam et elit varius blandit. Curabitur convallis justo eu dui semper, sed aliquet elit sodales. Maecenas sapien justo, rhoncus lacinia vestibulum eget, sollicitudin non odio. Aliquam finibus augue in massa sollicitudin, ut lacinia tortor pulvinar. Vivamus sed auctor velit. Quisque sagittis elit at tellus suscipit, nec ultricies ligula mollis. Maecenas congue lacus quis metus efficitur gravida. Maecenas facilisis, est a fringilla consequat, nulla ante pharetra erat, quis venenatis tortor libero eu mi.");
                    break;
                case SeederDataType.Now:
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], DateTime.Now);
                    break;
                case SeederDataType.Year:
                    var rand = new Random();
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], rand.Next(1960, 2021));
                    break;
            }

            return rows;
        }

        private static T[] HandleSeedStringFromEnumArrayAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = attribute as SeedPatternCreateStringAttributeAttribute;

            var aToZ = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            var zeroToNine = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };

            var generated = new string[rows.Length];
            for (int i = 0; i < rows.Length; i++)
            {
                generated[i] = "";

                for (int x = 0; x < attr.Description.Length; x++)
                {
                    var rand = new Random();
                    switch (attr.Description[x])
                    {
                        case CharacterDescription.AToZ:
                            generated[i] += aToZ[rand.Next(0, aToZ.Length)];
                            break;
                        case CharacterDescription.ZeroToNine:
                            generated[i] += zeroToNine[rand.Next(0, zeroToNine.Length)];
                            break;
                    }
                }
            }

            for (int i = 0; i < rows.Length; i++)
                property.SetValue(rows[i], generated[i]);

            return rows;
        }
        #endregion
    }
}

