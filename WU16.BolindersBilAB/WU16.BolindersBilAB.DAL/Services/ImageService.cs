using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;
using WU16.BolindersBilAB.DAL.Repository;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Microsoft.Extensions.Configuration;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class ImageService
    {
        private string _basePath;

        public ImageService(IConfiguration configuration)
        {
            _basePath = configuration.GetSection("ImageUploadFolder").Value;
            Directory.CreateDirectory(_basePath);
        }

        const int _size = 800;
        const int _quality = 75;

        private void OptimizeAndSaveImage(Stream imgStream, string fileName)
        {
            try
            {
                using (var image = new Bitmap(Image.FromStream(imgStream)))
                {
                    int height = image.Height,
                        width = image.Width;

                    if (image.Height > image.Width)
                    {
                        if (image.Height > _size)
                        {
                            height = _size;
                            width = Convert.ToInt32(image.Width * ((double)height / (double)image.Height));
                        }
                    }
                    else
                    {
                        if (image.Width > _size)
                        {
                            width = _size;
                            height = Convert.ToInt32(image.Height * ((double)width / (double)image.Width));
                        }
                    }

                    var resized = new Bitmap(width, height);
                    using (var graphics = Graphics.FromImage(resized))
                    {
                        graphics.CompositingQuality = CompositingQuality.HighSpeed;
                        graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphics.CompositingMode = CompositingMode.SourceCopy;

                        graphics.DrawImage(image, 0, 0, width, height);
                        var qualityParamId = Encoder.Quality;
                        var encoderParameters = new EncoderParameters(1);
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _quality);

                        ImageCodecInfo codec = null;
                        switch (fileName.Split(".")[1])
                        {
                            case "png":
                                codec = ImageCodecInfo.GetImageDecoders()
                                    .FirstOrDefault(x => x.FormatID == ImageFormat.Png.Guid);
                                break;
                            case "jpg":
                            case "jpeg":
                                codec = ImageCodecInfo.GetImageDecoders()
                                    .FirstOrDefault(x => x.FormatID == ImageFormat.Jpeg.Guid);
                                break;
                        }

                        using (var fStream = File.Open($"{_basePath}{fileName}", FileMode.Create, FileAccess.Write))
                            resized.Save(fStream, codec, encoderParameters);
                    }
                }
            }
            catch (Exception e)
            {

            }
        }


        public IEnumerable<string> UploadImages(params IFormFile[] images)
        {
            var fileNames = new List<string>();

            foreach (var image in images)
            {
                var id = Guid.NewGuid();
                var contentType = image.ContentType;

                var fileType = "";
                switch (contentType)
                {
                    case "image/png":
                        fileType = "png";
                        break;
                    case "image/jpeg":
                    case "image/jpg":
                        fileType = "jpg";
                        break;
                }

                var fileName = $"{id}.{fileType}";

                using (var imgStream = image.OpenReadStream())
                {
                    using (var ms = new MemoryStream())
                    {
                        imgStream.CopyTo(ms);
                        OptimizeAndSaveImage(ms, fileName);
                    }
                }

                fileNames.Add(fileName);
            }

            return fileNames;
        }

        public Car AddImageToCar(Car car, params IFormFile[] images)
        {
            var fileNames = UploadImages(images);

            if (car.CarImages == null) car.CarImages = new List<CarImage>();

            var i = 0;
            foreach (var fileName in fileNames)
            {
                car.CarImages.Add(new CarImage()
                {
                    FileName = fileName,
                    Priority = i
                });

                i++;
            }

            return car;
        }

        public CarBrand ChangeImageOnCarBrand(CarBrand carBrand, IFormFile image)
        {
            carBrand.ImageName = UploadImages(image).First();

            return carBrand;
        }

        public void RemoveImage(string imageName)
        {
            File.Delete($"{_basePath}{imageName}");
        }
    }
}
