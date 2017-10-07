using Microsoft.AspNetCore.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WU16.BolindersBilAB.DAL.Models;

namespace WU16.BolindersBilAB.Web.ModelBinder
{
    public class QueryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            var minYear = bindingContext.ActionContext.HttpContext.Request.Query["min-year"];
            var maxYear = bindingContext.ActionContext.HttpContext.Request.Query["max-year"];
            var minPrice = bindingContext.ActionContext.HttpContext.Request.Query["min-price"];
            var maxPrice = bindingContext.ActionContext.HttpContext.Request.Query["max-price"];
            var minMilage = bindingContext.ActionContext.HttpContext.Request.Query["min-milage"];
            var maxMilage = bindingContext.ActionContext.HttpContext.Request.Query["max-milage"];
            var gearbox = bindingContext.ActionContext.HttpContext.Request.Query["gearbox"];
            var carType = bindingContext.ActionContext.HttpContext.Request.Query["type"];
            var fuel = bindingContext.ActionContext.HttpContext.Request.Query["fuel"];


            var fuelTypes = new List<FuelType>();
            var carTypes = new List<CarType>();
            var gearboxes = new List<Gearbox>();

            foreach(var x in carType)
            {
                carTypes.Add(Enum.Parse<CarType>(x));
            }

            foreach(var x in gearbox)
            {
                gearboxes.Add(Enum.Parse<Gearbox>(x));
            }

            foreach(var x in fuel)
            {
                fuelTypes.Add(Enum.Parse <FuelType>(x));
            }

            var query = new CarListQuery
            {
                YearFrom = Convert.ToInt32(minYear),
                YearTo = Convert.ToInt32(maxYear),
                MilageFrom = Convert.ToInt32(minMilage),
                MilageTo = Convert.ToInt32(maxMilage),
                PriceFrom = Convert.ToDecimal(minPrice),
                PriceTo = Convert.ToDecimal(maxPrice),
                FuelType = fuelTypes,
                CarType = carTypes,
                Gearbox = gearboxes
            };


            bindingContext.Result = ModelBindingResult.Success(query);

            return Task.CompletedTask;
        }
    }
}
