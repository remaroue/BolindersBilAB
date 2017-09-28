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
using WU16.BolindersBilAB.BLL.Configuration;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class FtpService
    {
        private FtpServiceConfiguration _config;
        //private CarBrandService _brandService;
        //private CarListService _carService;

        public FtpService(IOptions<FtpServiceConfiguration> config)
        {
            _config = config.Value;
            //_brandService = carBrandService;
            //_carService = carService;
        }

        private void HandleStream(Stream xml)
        {
            var document = XDocument.Load(xml);

            var s = document.Root.Elements("car");

            var e = s.Select(x => new CarXML()
            {
                CarBrandId = x.Attribute("brand").Value,
                LocationId = x.Attribute("station").Value,

                LastUpdated = DateTime.Parse(x.Attribute("updated").Value),
                CreationDate = DateTime.Parse(x.Attribute("added").Value),

                IsLeaseable = bool.Parse(x.Attribute("exkl_moms").Value),
                HorsePower = int.Parse(x.Attribute("horsepower").Value),
                Price = decimal.Parse(x.Attribute("price").Value),
                Color = x.Attribute("color").Value,
                FuelType = (FuelType)Enum.Parse(typeof(FuelType), x.Attribute("fueltype").Value),
                CarType = (CarType)Enum.Parse(typeof(CarType), x.Attribute("bodytype").Value),
                Equipment = x.Attribute("info").Value,
                CarImages = x.Attributes("images").Select(y => y.Value),
                Gearbox = (Gearbox)Enum.Parse(typeof(Gearbox), x.Attribute("gearboxtype").Value)
            });


        }

        public void Run()
        {
            try
            {
                var request = WebRequest.CreateDefault(new Uri($"ftp://{_config.Host}{_config.FilePath}"));

                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(_config.UserName, _config.Password);

                var response = request.GetResponse();

                using(var s = response.GetResponseStream())
                {
                    HandleStream(s);
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

        public string CarBrandId { get; set; }
        public string LocationId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreationDate { get; set; }
        public bool IsLeaseable { get; set; }
        public int HorsePower { get; set; }
        public decimal Price { get; set; }
        public string Color { get; set; }
        public FuelType FuelType { get; set; }
        public CarType CarType { get; set; }
        public string Equipment { get; set; }
        public IEnumerable<string> CarImages { get; set; }
        public Gearbox Gearbox { get; set; }
    }
}
