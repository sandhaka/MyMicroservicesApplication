using System.Collections.Generic;
using System.Linq;
using Catalog.Application.DbModels;

namespace Catalog.Application.Infrastructure.Repositories
{
    public class CatalogRepository : ICatalogRepository
    {
        private readonly CatalogContext _context;
        
        public CatalogRepository(CatalogContext context)
        {
            _context = context;
        }
        
        public IEnumerable<Product> GetAllProducts()
        {
            return _context.Products;
        }
    }
}