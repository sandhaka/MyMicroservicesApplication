using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Application.DbModels;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Infrastructure
{
    public class CatalogContextSeed
    {
        public async Task SeedAsync(CatalogContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                using (context)
                {
                    if (!context.Products.Any())
                    {
                        foreach (var product in GetDefaultProducts())
                        {
                            context.Products.Add(product);
                        }

                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception exception)
            {
                var log = loggerFactory.CreateLogger("catalog seed");                
                log.LogError(exception.Message);
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