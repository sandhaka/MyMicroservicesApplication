using System.Collections.Generic;
using System.Threading.Tasks;
using Catalog.Application.DbModels;
using EventBusAwsSns.Shared.IntegrationEvents;

namespace Catalog.Application.Infrastructure.Repositories
{
    public interface ICatalogRepository
    {
        IEnumerable<Product> GetAllProducts();
        Task<int> UpdateProductsAssetsAsync(List<OrderItemInfo> orderItemInfo);
    }
}