using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using WU16.BolindersBilAB.DAL.Seeding.Attributes;
using WU16.BolindersBilAB.DAL.Seeding.Enums;

namespace WU16.BolindersBilAB.DAL.Seeding.Helper
{
    internal static class DefaultHandlers<T> where T : class, new()
    {
        public static T[] HandleSeedNumericValueAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(SeedNumericValueAttribute)) as SeedNumericValueAttribute;

            if (property.PropertyType.IsPrimitive)
            {
                if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Int32)
                {
                    var min = Convert.ToInt32(attr.Min);
                    var max = Convert.ToInt32(attr.Min);

                    var rand = new Random();
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], rand.Next(min, max));
                }
                else if (Convert.GetTypeCode(property.PropertyType) != TypeCode.Double)
                {
                    var min = Convert.ToDouble(attr.Min);
                    var max = Convert.ToDouble(attr.Min);

                    var rand = new Random();
                    for (int i = 0; i < rows.Length; i++)
                        property.SetValue(rows[i], Convert.ToDouble(min + rand.NextDouble() * (max - min)));
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

        public static T[] HandleSeedFixedValueAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(SeedFixedValueAttribute)) as SeedFixedValueAttribute;

            for (int i = 0; i < rows.Length; i++)
                property.SetValue(rows[i], attr.Value);

            return rows;
        }

        public static T[] HandleSeedChooseFromAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = Attribute.GetCustomAttribute(property, typeof(SeedChooseFromAttribute)) as SeedChooseFromAttribute;
            var rand = new Random();
            for (int i = 0; i < rows.Length; i++)
                property.SetValue(rows[i], attr.Values[rand.Next(0, attr.Values.Length)]);

            return rows;
        }

        public static T[] HandleSeedDataTypeAttribute(Attribute attribute, PropertyInfo property, T[] rows)
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

        public static T[] HandleSeedPatternCreateStringAttribute(Attribute attribute, PropertyInfo property, T[] rows)
        {
            var attr = attribute as SeedPatternCreateStringAttribute;

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

    }
}
