using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Catalog.Application.DbModels;
using Catalog.Application.Infrastructure.Exceptions;
using Catalog.Application.IntegrationEvents.Events;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogContext _context;
        private readonly ILogger _logger;
        
        public CatalogRepository(CatalogContext context, ILogger<CatalogRepository> logger)
        {
            _logger = logger;
            _context = context;
        }
        
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products;
        }

        public Task<int> UpdateProductsAssetsAsync(List<OrderItemInfo> orderItemInfo)
        {
            foreach (var productInfo in orderItemInfo)
            {
                var product = _context.Products.Find(productInfo.ProductId);

                if (productInfo.Assets > product.Assets)
                {
                    _logger.LogError("A order item exceed the availability of the product");
                    continue;
                }
                
                product.Assets -= productInfo.Assets;
            }
            
            return _context.SaveChangesAsync();
        }
    }
}