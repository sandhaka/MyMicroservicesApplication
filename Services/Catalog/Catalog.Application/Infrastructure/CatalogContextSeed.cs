using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Application.DbModels;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Infrastructure
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(IApplicationBuilder applicationBuilder, ILoggerFactory loggerFactory, int retry = 0)
        {
            var retryForAvailability = retry;
            try
            {
                var context = (CatalogContext)applicationBuilder.ApplicationServices.GetService(typeof(CatalogContext));

                if (!context.Products.Any())
                {
                    context.Products.AddRange(GetDefaultProducts());
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                var log = loggerFactory.CreateLogger("catalog seed");
                
                log.LogError(exception.Message);
                
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    await SeedAsync(applicationBuilder,loggerFactory, retryForAvailability);
                }
            }
        }

        private List<Product> GetDefaultProducts()
        {
            return new List<Product>()
            {
                new Product() {ProductName = "Chai", Assets = 10, Package = "10 Boxes x 20 bags", UnitPrice = 19 },
                new Product() {ProductName = "Chang", Assets = 10, Package = "24 - 12 oz Bottles", UnitPrice = 18 },
                new Product() {ProductName = "Aniseed Syrup", Assets = 10, Package = "12 - 550 ml bottles", UnitPrice = 19 }
            };
        }
    }
}