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
using WU16.BolindersBilAB.BLL.Helpers;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;
using WU16.BolindersBilAB.DAL.Services;

namespace WU16.BolindersBilAB.BLL.Services
{
    public class FtpService
    {
        private FtpServiceConfiguration _config;
        private CarBrandService _brandService;
        private CarListService _carService;
        private IRepository<Car> _carRepo;
        private ImageService _imageService;
        private LocationService _locationService;

        public FtpService(IOptions<FtpServiceConfiguration> config, CarListService carService, IRepository<Car> carRepo, ImageService imageService, CarBrandService carBrandService, LocationService locationService)
        {
            _config = config.Value;
            _brandService = carBrandService;
            _carService = carService;
            _carRepo = carRepo;
            _imageService = imageService;
            _locationService = locationService;
        }

        private CarXmlDeserializer DeserializeStrem(Stream xml)
        {
            var ds = new XmlSerializer(typeof(CarXmlDeserializer), new XmlRootAttribute("cars"));

            return (CarXmlDeserializer)ds.Deserialize(xml);
        }
        private Stream DownloadFile()
        {
            var request = WebRequest.CreateDefault(new Uri($"ftp://{_config.Host}{_config.FilePath}"));

            request.Method = WebRequestMethods.Ftp.DownloadFile;
            request.Credentials = new NetworkCredential(_config.UserName, _config.Password);

            return request.GetResponse().GetResponseStream();
        }

        private void MapUpdates(Car to, Car from, List<CarBrand> addedCarBrands)
        {
            // string
            to.LocationId       = from.LocationId;
            to.Milage           = from.Milage;
            to.Model            = from.Model;
            to.ModelDescription = from.ModelDescription;

            // boolean
            to.IsLeaseable      = from.IsLeaseable;

            // datetime
            to.LastUpdated      = from.LastUpdated;

            // numeric
            to.ModelYear        = from.ModelYear;
            to.HorsePower       = from.HorsePower;
            to.Price            = from.Price;
            to.Equipment        = from.Equipment;
            to.Color            = from.Color;

            // enums
            to.CarType          = from.CarType;
            to.FuelType         = from.FuelType;
            to.Gearbox          = from.Gearbox;

            // carbrand
            to.CarBrandId = from.CarBrandId;
            var carBrand = _brandService.GetBrand(to.CarBrandId);
            if (carBrand == null)
            {

                var addedBrand = _brandService.Add(new CarBrand()
                {
                    BrandName = to.CarBrandId
                });

                addedCarBrands.Add(addedBrand);
            }
            
            // car images
            if (to.CarImages != null)
            {
                foreach (var img in to.CarImages)
                {
                    _imageService.RemoveImage(img.FileName);
                }
            }

            var i = 0;
            to.CarImages = _imageService
                .DownloadImages(from.CarImages.Select(x => x.FileName).ToArray())
                .Select(x => new CarImage()
                {
                    CarId = to.LicenseNumber,
                    FileName = x,
                    Priority = ++i
                }).ToList();

            to.LocationId = from.LocationId;
            if (!_locationService.Get().Select(x => x.Id.ToLower()).Contains(to.LocationId.ToLower()))
                throw new Exception($"LocationId {to.LocationId} does not exist.");
        }

        private string BuildLiTag(Car car) => $"<li>{car.CarBrandId} {car.Model} {car.ModelDescription} - <a href=localhost:63037/bil/{car.LicenseNumber}>Till Annonsen.</a></li>";

        private void ConstructAndSendEmails(List<Car> addedCars, List<Car> updatedCars, List<FailedCar> failedCars, List<CarBrand> addedCarBrands)
        {
            var locations = _locationService.Get();

            var messeges = new Dictionary<string, StringBuilder>();

            foreach (var location in locations)
            {
                var sb = new StringBuilder();

                var locAdded = addedCars.Where(x => x.LocationId == location.Id);
                var locUpdated = updatedCars.Where(x => x.LocationId == location.Id);

                sb.Append($"<h2>Import av bilar Körd {DateTime.Now.ToShortDateString()}.</h2><p>Tillagda: {locAdded.Count()}st<p>");

                if (locAdded.Count() > 0)
                {
                    sb.Append("<ul>");
                    foreach (var car in locAdded)
                        sb.Append(BuildLiTag(car));
                    sb.Append("</ul>");
                }

                sb.Append($"</ul><br><p>Uppdaterade: {locUpdated.Count()}</p><ul>");
                if (locUpdated.Count() > 0)
                {
                    sb.Append("<ul>");
                    foreach (var car in locUpdated)
                        sb.Append(BuildLiTag(car));
                    sb.Append("</ul>");
                }

                if(failedCars.Count() > 0)
                {
                    sb.Append("<h2 style='color:#f00'>något gick fel under importen</h2><p>Kunde inte importera dessa bilar:</p>");
                    sb.Append("<table><thead>");
                    sb.Append("<th>Registrerings Nummer</th>");
                    sb.Append("<th>Import Id</th>");
                    sb.Append("<th>Felmeddelande</th>");
                    sb.Append("</tr></thead><tbody>");

                    foreach (var failedCar in failedCars)
                        sb.Append($"<tr><td>{failedCar.Car.LicenseNumber}</td><td>{failedCar.Car.FtpId}</td><td>{failedCar.Exception.Message}</td></tr>");

                    sb.Append("</tbody></table>");
                }
            }
        }

        public void Run()
        {
            var email = new StringBuilder();

            CarXmlDeserializer result = null;
            using (var s = DownloadFile())
                result = DeserializeStrem(s);

            // handle succesful cars
            var addedCars = new List<Car>();
            var updatedCars = new List<Car>();
            var failedCars = result.FailedCars;

            var addedCarBrands = new List<CarBrand>();

            foreach (var car in result.Cars)
            {
                try
                {
                    var dbCar = _carService.GetCars()
                        .FirstOrDefault(x => x.LicenseNumber == car.LicenseNumber || x.FtpId == car.FtpId);

                    bool added = false;
                    if (dbCar == null)
                    {
                        dbCar = new Car();
                        added = true;

                        dbCar.LicenseNumber = car.LicenseNumber;
                        dbCar.FtpId = car.FtpId;
                        dbCar.CreationDate = DateTime.Now;
                    }

                    if (dbCar.LastUpdated != car.LastUpdated)
                    {
                        MapUpdates(dbCar, car, addedCarBrands);

                        if (added)
                        {
                            _carRepo.Insert(dbCar);
                            _carRepo.Save();
                            addedCars.Add(dbCar);
                        }
                        else
                        {
                            _carRepo.Edit(dbCar);
                            _carRepo.Save();
                            updatedCars.Add(dbCar);
                        }
                    }
                }
                catch(Exception e)
                {
                    failedCars.Add(new FailedCar()
                    {
                        Car = car,
                        Exception = e
                    });
                }
            }

            ConstructAndSendEmails(addedCars, updatedCars, failedCars, addedCarBrands);
        }
    }
}
