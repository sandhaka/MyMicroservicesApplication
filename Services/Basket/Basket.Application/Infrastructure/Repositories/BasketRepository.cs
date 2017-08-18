using System.Threading.Tasks;
using Basket.Application.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Basket.Application.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly ConnectionMultiplexer _redisConnection;
        private ILogger<BasketRepository> _logger;

        public BasketRepository(ILoggerFactory logger, ConnectionMultiplexer connectionMultiplexer)
        {
            _logger = logger.CreateLogger<BasketRepository>();
            _redisConnection = connectionMultiplexer;
        }
        
        public async Task<CustomerBasket> GetBasketAsync(string customerId)
        {
            var database = _redisConnection.GetDatabase();
            var data = await database.StringGetAsync(customerId);
            if (data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public async Task<CustomerBasket> UpdateBasketAsync(CustomerBasket basket)
        {
            var database = _redisConnection.GetDatabase();
            var created = await database.StringSetAsync(basket.Identity, JsonConvert.SerializeObject(basket));
            if (!created)
            {
                _logger.LogWarning("Problem occur on basket set");
                return null;
            }
            
            _logger.LogInformation("Basket item persisted successfully");

            return await GetBasketAsync(basket.Identity);
        }

        public async Task<bool> DeleteBasketAsync(string id)
        {
            var database = _redisConnection.GetDatabase();
            return await database.KeyDeleteAsync(id.ToString());
        }
    }
}