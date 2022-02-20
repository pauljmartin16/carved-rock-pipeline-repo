using CarvedRock.Api.ApiModels;
using CarvedRock.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Prometheus;
using Serilog;
using System.Collections.Generic;

namespace CarvedRock.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductLogic _productLogic;

        // pjm - Prometheus POC
        Counter othersCounter = Metrics.CreateCounter("others_counter", "Others Counter");
        Counter allProductsCounter = Metrics.CreateCounter("all_counter", "All Products Counter");

        public ProductsController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        [HttpGet("Products")]
        public IEnumerable<Product> GetProducts(string category = "all")
        {
            if (category == "all")
            {
                allProductsCounter.Inc();
            } else
            {
                othersCounter.Inc();
            }
            //_logger.LogInformation("Starting controller action GetProducts for {category}", category);

            // example of static Serilog Logger
            Log.Information("Starting controller action GetProducts for {category}", category);
            return _productLogic.GetProductsForCategory(category);
        }
    }
}