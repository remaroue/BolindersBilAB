using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;
using WU16.BolindersBilAB.DAL.Models;
using System.Xml;
using System.Xml.Schema;
using System.Globalization;
using WU16.BolindersBilAB.BLL.Models;

namespace WU16.BolindersBilAB.BLL.Helpers
{
    public class CarXmlDeserializer : IXmlSerializable
    {
        public XmlSchema GetSchema() => null;
        public void WriteXml(XmlWriter writer) => throw new NotImplementedException();

        public List<Car> Cars { get; set; }
        public List<FailedCar> FailedCars { get; set; }

        public CarXmlDeserializer()
        {
            Cars = new List<Car>();
            FailedCars = new List<FailedCar>();
        }

        #region helpers
        private T ParseEnum<T>(string name)
        {
            var n = Enum.GetNames(typeof(T)).FirstOrDefault(x => x.ToLower() == name.ToLower());

            if (n == null)
                throw new Exception($"Value:{name} could not be parsed into Enum:{nameof(T)}");

            return (T)Enum.Parse(typeof(T), n);
        }

        private DateTime ConvertFromUnixTime(string unixTimestamp) => new DateTime(1970, 1, 1).AddSeconds(int.Parse(unixTimestamp));

        private int? ParseInt(string value) => int.TryParse(value, out int result) ? result : (int?)null;
        private decimal? ParseDecimal(string value) => decimal.TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.GetCultureInfo("sv-SE"), out decimal result) ? result : (decimal?)null;
        private bool ParseBool(string value) => string.IsNullOrEmpty(value) || value == "0" ? false : true;

        private void ApplyValueToProperty(string name, string value, Car car)
        {
            switch (name)
            {
                // pure strings
                case "id": car.FtpId = value; break;
                case "regno": car.LicenseNumber = value; break;
                case "model": car.Model = value; break;
                case "modeldescription": car.ModelDescription = value; break;
                case "info": car.Equipment = value; break;
                case "color": car.Color = value; break;
                case "brand": car.CarBrandId = value; break;
                case "station": car.LocationId = value; break;

                // numeric
                case "yearmodel": car.ModelYear = (int)ParseInt(value); break;
                case "horsepower": car.HorsePower = ParseInt(value); break;
                case "miles":
                    car.Milage = ParseInt(value);
                    car.Used = false;
                    if (car.Milage != null)
                        if (car.Milage > 0) car.Used = true;
                    break;
                case "price": car.Price = ParseDecimal(value); break;
                case "exkl_moms": car.IsLeaseable = ParseBool(value); break;

                // enums
                case "fueltype": car.FuelType = ParseEnum<FuelType>(value); break;
                case "gearboxtype": car.Gearbox = ParseEnum<Gearbox>(value); break;
                case "bodytype": car.CarType = ParseEnum<CarType>(value); break;

                // datetime
                case "updated": car.LastUpdated = ConvertFromUnixTime(value); break;

                // other
                case "image":
                    car.CarImages.Add(new CarImage()
                    {
                        FileName = value
                    });
                    break;
            }
        }
        #endregion

        public void ReadXml(XmlReader reader)
        {
            Car currentCar = null;
            bool currentCarHasFailed = false;
            Exception failureException = null;

            while (reader.Read())
            {
                if (reader.Name == "cars" || reader.Name == "images" || !reader.IsStartElement()) continue;

                if (reader.Name == "car")
                {
                    if (currentCar != null && !currentCarHasFailed)
                        Cars.Add(currentCar);
                    else if (currentCar != null && currentCarHasFailed)
                        FailedCars.Add(new FailedCar()
                        {
                            Car = currentCar,
                            Exception = failureException
                        });

                    currentCar = new Car
                    {
                        CarImages = new List<CarImage>()
                    };

                    currentCarHasFailed = false;
                    failureException = null;
                }
                else
                {
                    var name = reader.Name;
                    if (!reader.Read()) continue;

                    try
                    {
                        ApplyValueToProperty(name, reader.Value, currentCar);
                    }
                    catch (Exception e)
                    {
                        failureException = e;
                        currentCarHasFailed = true;
                    }
                }
            }
        }
    }
}
