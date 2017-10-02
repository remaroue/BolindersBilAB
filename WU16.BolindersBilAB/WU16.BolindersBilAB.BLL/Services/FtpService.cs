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

namespace WU16.BolindersBilAB.BLL.Services
{
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

        private T ParseEnum<T>(string name)
        {
            var n = Enum.GetNames(typeof(T)).FirstOrDefault(x => x.ToLower() == name.ToLower());
            return (T)Enum.Parse(typeof(T), n);
        }

        private DateTime GetDateTimeFromUnixTime(string timestamp)
        {
            return new DateTime(1970, 1, 1).AddSeconds(int.Parse(timestamp));
        }

        private void HandleStream(Stream xml)
        {
            var document = XDocument.Load(xml);
            var list = new List<CarXML>();

            foreach (var x in document.Element("cars").Elements("car"))
            {
                try
                {
                    var car = new CarXML()
                    {
                        LicenseNumber = x.Element("regno").Value,
                        Id = x.Element("id").Value,
                        CarBrandId = x.Element("brand").Value,
                        LocationId = x.Element("station").Value,
                        LastUpdated = GetDateTimeFromUnixTime(x.Element("updated").Value),

                        IsLeaseable = x.Element("exkl_moms").Value == "" ? false : true,
                        HorsePower = int.Parse(x.Element("horsepower").Value == "" ? "0" : x.Element("horsepower").Value),
                        Price = decimal.Parse((x.Element("price").Value == "" ? "0" : x.Element("price").Value).Replace(".", ""), System.Globalization.NumberStyles.AllowDecimalPoint),
                        Color = x.Element("color").Value,
                        FuelType = ParseEnum<FuelType>(x.Element("fueltype").Value),
                        CarType = ParseEnum<CarType>(x.Element("bodytype").Value),
                        Gearbox = ParseEnum<Gearbox>(x.Element("gearboxtype").Value),
                        Equipment = x.Element("info").Value,
                        CarImages = x.Attributes("images").Select(y => y.Value).ToArray(),
                        Milage = int.Parse(x.Element("miles").Value == "" ? "0" : x.Element("miles").Value),
                        Model = x.Element("model").Value,
                        ModelDescription = x.Element("modeldescription").Value,
                        ModelYear = int.Parse(x.Element("yearmodel").Value)
                    };

                    list.Add(car);
                }
                catch (Exception e)
                {
                    // TODO: log car that could not be added in some way.
                }
            }

            return list;
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
