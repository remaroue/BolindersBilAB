using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using WU16.BolindersBilAB.BLL.Configuration;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.DAL.Services
{
    public class ImageService
    {
        private ImageUploadConfiguration _config;

        public ImageService(IOptions<ImageUploadConfiguration> config)
        {
            _config = config.Value;
            Directory.CreateDirectory(_config.BasePath);
        }

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
                        if (image.Height > _config.MaxSize)
                        {
                            height = _config.MaxSize;
                            width = Convert.ToInt32(image.Width * ((double)height / (double)image.Height));
                        }
                    }
                    else
                    {
                        if (image.Width > _config.MaxSize)
                        {
                            width = _config.MaxSize;
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
                        encoderParameters.Param[0] = new EncoderParameter(qualityParamId, _config.Quality);

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

                        using (var fStream = File.Open($"{_config.BasePath}{fileName}", FileMode.Create, FileAccess.Write))
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
            File.Delete($"{_config.BasePath}{imageName}");
        }
    }
}
