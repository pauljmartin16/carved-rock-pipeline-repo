using CarvedRock.Api.ApiModels;
using CarvedRock.Api.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Collections.Generic;

namespace CarvedRock.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductLogic _productLogic;

        public ProductsController(IProductLogic productLogic)
        {
            _productLogic = productLogic;
        }

        [HttpGet("Products")]
        public IEnumerable<Product> GetProducts(string category = "all")
        {
            //_logger.LogInformation("Starting controller action GetProducts for {category}", category);

            // example of static Serilog Logger
            Log.Information("Starting controller action GetProducts for {category}", category);
            return _productLogic.GetProductsForCategory(category);
        }
    }
}