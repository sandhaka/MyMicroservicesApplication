using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Application.DbModels;
using Catalog.Application.IntegrationEvents.Events;

namespace Catalog.Application.Infrastructure.Repositories
{
    public interface ICatalogRepository
    {
        IEnumerable<Product> GetAllProducts();
        Task<int> UpdateProductsAssetsAsync(List<OrderItemInfo> orderItemInfo);
    }
}