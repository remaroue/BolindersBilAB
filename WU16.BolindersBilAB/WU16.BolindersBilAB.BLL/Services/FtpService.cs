using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using WU16.BolindersBilAB.BLL.Configuration;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;
using WU16.BolindersBilAB.DAL.Services;
using System.Xml;
using System.Xml.Schema;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class CarFtpDeserializer : IXmlSerializable
    {
        public XmlSchema GetSchema() => null;
        public void WriteXml(XmlWriter writer) => throw new NotImplementedException();

        public List<Car> Cars { get; set; }
        public List<Car> FailedCars { get; set; }
        public bool AnyFailed { get; set; }

        private Car _currentCar { get; set; }
        private bool _currentCarFailed { get; set; }

        public CarFtpDeserializer()
        {
            Cars = new List<Car>();
            _currentCarFailed = false;
        }

        #region helpers
        private T ParseEnum<T>(string name)
        {
            var n = Enum.GetNames(typeof(T)).FirstOrDefault(x => x.ToLower() == name.ToLower());
            return (T)Enum.Parse(typeof(T), n);
        }


        private string PrepareNumeric(string value) => string.IsNullOrEmpty(value) || string.IsNullOrWhiteSpace(value) ? "0" : value.Replace(".", "");
        private bool GetBoolean(string value) => string.IsNullOrEmpty(value) ? false : true;
        #endregion

        public void ReadXml(XmlReader reader)
        {
            while (reader.Read())
            {
                if (reader.Name == "cars") continue;

                if (reader.Name == "car")
                {
                    if (_currentCar != null && !_currentCarFailed)
                        Cars.Add(_currentCar);
                    else if (_currentCar != null && _currentCarFailed)
                        FailedCars.Add(_currentCar);

                    _currentCar = new Car();
                    _currentCarFailed = false;
                }

                var name = reader.Name;
                if (reader.IsEmptyElement) reader.Read();

                try
                {
                    switch (name)
                    {
                        // pure strings
                        case "id": _currentCar.FtpId = reader.Value; break;
                        case "regno": _currentCar.LicenseNumber = reader.Value; break;
                        case "model": _currentCar.Model = reader.Value; break;
                        case "modeldescription": _currentCar.ModelDescription = reader.Value; break;
                        case "info": _currentCar.Equipment = reader.Value; break;
                        case "color": _currentCar.Color = reader.Value; break;
                        case "brand": _currentCar.CarBrandId = reader.Value; break;
                        case "station": _currentCar.LocationId = reader.Value; break;

                        // numeric
                        case "yearmodel": _currentCar.ModelYear = int.Parse(PrepareNumeric(reader.Value)); break;
                        case "horsepower": _currentCar.HorsePower = int.Parse(PrepareNumeric(reader.Value)); break;
                        case "miles": _currentCar.Milage = int.Parse(PrepareNumeric(reader.Value)); break;
                        case "price": _currentCar.Price = decimal.Parse(PrepareNumeric(reader.Value)); break;
                        case "exkl_moms": _currentCar.ModelYear = int.Parse(PrepareNumeric(reader.Value)); break;

                        // enums
                        case "fueltype": _currentCar.FuelType = ParseEnum<FuelType>(reader.Value); break;
                        case "gearboxtype": _currentCar.Gearbox = ParseEnum<Gearbox>(reader.Value); break;
                        case "bodytype": _currentCar.CarType = ParseEnum<CarType>(reader.Value); break;
                    }
                }
                catch (Exception e)
                {
                    _currentCarFailed = true;
                    AnyFailed = true;
                }
            }
        }
    }

    public class FtpService
    {
        private FtpServiceConfiguration _config;
        //private CarBrandService _brandService;
        private CarListService _carService;
        private IRepository<Car> _carRepo;
        private ImageService _imageService;

        public FtpService(IOptions<FtpServiceConfiguration> config, CarListService carService, IRepository<Car> carRepo, ImageService imageService)
        {
            _config = config.Value;
            //_brandService = carBrandService;
            _carService = carService;
            _carRepo = carRepo;
            _imageService = imageService;
        }


        private DateTime GetDateTimeFromUnixTime(string timestamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(int.Parse(timestamp));
        }

        private IEnumerable<Car> HandleStream(Stream xml)
        {
            var ds = new XmlSerializer(typeof(CarFtpDeserializer), new XmlRootAttribute("cars"));
            var carDs = (CarFtpDeserializer)ds.Deserialize(xml);

            return carDs.Cars;
        }

        public void Run()
        {
            try
            {
                var request = WebRequest.CreateDefault(new Uri($"ftp://{_config.Host}{_config.FilePath}"));

                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_config.UserName, _config.Password);

                var response = request.GetResponse();

                List<CarXML> cars = null;
                using (var s = response.GetResponseStream())
                    HandleStream(s);

                return;

                foreach (var car in cars)
                {
                    var dbCar = _carService.GetCars()
                        .FirstOrDefault(x => x.LicenseNumber == car.LicenseNumber || x.FtpId == car.Id);

                    bool added = false;
                    if (dbCar == null)
                    {
                        dbCar = new Car();
                        added = true;

                        dbCar.LicenseNumber = car.LicenseNumber;
                        dbCar.FtpId = car.Id;
                        dbCar.CreationDate = DateTime.Now;
                    }

                    if (dbCar.LastUpdated != car.LastUpdated)
                    {
                        dbCar.LastUpdated = car.LastUpdated;
                        dbCar.IsLeaseable = car.IsLeaseable;
                        dbCar.LocationId = car.LocationId;
                        dbCar.Milage = car.Milage;
                        dbCar.Model = car.Model;
                        dbCar.ModelDescription = car.ModelDescription;
                        dbCar.ModelYear = car.ModelYear;
                        dbCar.LocationId = car.LocationId;
                        dbCar.HorsePower = car.HorsePower;
                        dbCar.Price = car.Price;
                        dbCar.Equipment = car.Equipment;
                        dbCar.Color = car.Color;

                        dbCar.CarType = car.CarType;
                        dbCar.CarBrandId = car.CarBrandId;
                        dbCar.FuelType = car.FuelType;
                        dbCar.Gearbox = car.Gearbox;

                        var i = 0;
                        dbCar.CarImages = _imageService.DownloadImages(car.CarImages.ToArray()).Select(x => new CarImage()
                        {
                            CarId = dbCar.LicenseNumber,
                            FileName = x,
                            Priority = ++i
                        }).ToList();

                        if (added)
                        {
                            _carRepo.Insert(dbCar);
                        }
                        else
                        {
                            _carRepo.Edit(dbCar);
                        }
                        _carRepo.Save();
                    }
                }
            }
            catch (Exception e)
            {

            }
        }
    }

    internal class CarXML
    {
        public CarXML()
        {
        }

        public string Id { get; set; }
        public string LicenseNumber { get; set; }
        public string CarBrandId { get; set; }
        public string LocationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public bool IsLeaseable { get; set; }
        public int HorsePower { get; set; }
        public int Milage { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public FuelType FuelType { get; set; }
        public CarType CarType { get; set; }
        public string Equipment { get; set; }
        public IEnumerable<string> CarImages { get; set; }
        public Gearbox Gearbox { get; set; }
        public string Model { get; internal set; }
        public string ModelDescription { get; internal set; }
        public int ModelYear { get; internal set; }
    }
}
