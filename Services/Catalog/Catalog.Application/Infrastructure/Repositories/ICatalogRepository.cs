using System.Collections.Generic;
using Catalog.Application.DbModels;

namespace Catalog.Application.Infrastructure.Repositories
{
    public interface ICatalogRepository
    {
        IEnumerable<Product> GetAllProducts();
    }
}