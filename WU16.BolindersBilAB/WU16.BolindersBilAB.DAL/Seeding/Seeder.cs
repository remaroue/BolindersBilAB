using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
using System.Reflection;
using WU16.BolindersBilAB.DAL.Seeding.Attributes;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding
{
    public static class Seeder 
    {
        public static void Seed<T>(DbContext dbContext, int numberOfRows) where T : class, new()
        {
            var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            var rows = new T[numberOfRows];

            for (int i = 0; i < numberOfRows; i++)
                rows[i] = new T();

            foreach (var property in properties)
            {
                var attributes = property.GetCustomAttributes(false);

                if (attributes.Any(x => x is SeedIgnoreAttribute)) continue;

                if (attributes.FirstOrDefault(x => x is SeedAttribute) is SeedAttribute sAttr)
                {
                    switch (sAttr.DataType)
                    {
                        case SeederDataType.LoremIpsum:
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum quis velit dolor. Lorem ipsum dolor sit amet, consectetur adipiscing elit. Integer tempor diam et elit varius blandit. Curabitur convallis justo eu dui semper, sed aliquet elit sodales. Maecenas sapien justo, rhoncus lacinia vestibulum eget, sollicitudin non odio. Aliquam finibus augue in massa sollicitudin, ut lacinia tortor pulvinar. Vivamus sed auctor velit. Quisque sagittis elit at tellus suscipit, nec ultricies ligula mollis. Maecenas congue lacus quis metus efficitur gravida. Maecenas facilisis, est a fringilla consequat, nulla ante pharetra erat, quis venenatis tortor libero eu mi.");
                            break;
                        case SeederDataType.Name:
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], "Eric Eriksson");
                            break;
                        case SeederDataType.Now:
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], DateTime.Now);
                            break;
                        case SeederDataType.Year:
                            var rand = new Random();
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], rand.Next(1960, 2021));
                            break;
                    }

                    continue;
                }
                else if (attributes.FirstOrDefault(x => x is SeedNumericValueAttribute) is SeedNumericValueAttribute snAttr)
                {
                    if (property.PropertyType.IsPrimitive)
                    {
                        if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Int32)
                        {
                            var rand = new Random();
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], rand.Next(snAttr.Min, snAttr.Max));
                        }
                        else if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Double)
                        {
                            var rand = new Random();
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], Convert.ToDouble(rand.Next(snAttr.Min, snAttr.Max)));
                        }
                    }
                    else if (property.PropertyType.Equals(typeof(decimal)))
                    {
                        var rand = new Random();
                            for (int i = 0; i < numberOfRows; i++)
                                property.SetValue(rows[i], Convert.ToDecimal(rand.Next(snAttr.Min, snAttr.Max)));
                    }
                }
                else if (attributes.FirstOrDefault(x => x is SeedFixedValueAttribute) is SeedFixedValueAttribute sfAttr)
                {
                    for (int i = 0; i < numberOfRows; i++)
                        property.SetValue(rows[i], sfAttr.Value);
                }
                else if (attributes.FirstOrDefault(x => x is SeedFromArrayAttribute) is SeedFromArrayAttribute sfaAttr)
                {
                    var rand = new Random();
                    for (int i = 0; i < numberOfRows; i++)
                        property.SetValue(rows[i], sfaAttr.Values[rand.Next(0, sfaAttr.Values.Length)]);
                }
                else if (attributes.FirstOrDefault(x => x is SeedStringFromEnumArrayAttribute) is SeedStringFromEnumArrayAttribute sfeAttr)
                {
                    var aToZ = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z", "å", "ä", "ö" };
                    var zeroToNine = new string[] { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };
                    var generated = new string[numberOfRows];
                    for (int i = 0; i < numberOfRows; i++)
                    {
                        generated[i] = "";

                        for (int x = 0; x < sfeAttr.Description.Length; x++)
                        {
                            var rand = new Random();
                            switch(sfeAttr.Description[x])
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

                    for (int i = 0; i < numberOfRows; i++)
                        property.SetValue(rows[i], generated[i]);
                }
                else
                {
                    if (property.PropertyType.IsPrimitive)
                    {
                        if(Convert.GetTypeCode(property.PropertyType) != TypeCode.Boolean)
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
                continue;
            }

            var set = dbContext.Set<T>();
            set.AddRange(rows);
            dbContext.SaveChanges();
        }
    }
}

