using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Orders.Infrastructure;

namespace Orders.Infrastructure
{
    public class OrdersDbContextSeed
    {
        public async Task SeedAsync(IServiceProvider services, ILoggerFactory loggerFactory, int retry = 0)
        {
            var retryForAvailability = retry;
            try
            {
                var context =
                    (OrdersContext) services.GetService(typeof(OrdersContext));

                context.Database.Migrate();
            }
            catch (Exception exception)
            {
                var log = loggerFactory.CreateLogger("orders seed");
                log.LogError(exception.Message);
                
                if (retryForAvailability < 10)
                {
                    retryForAvailability++;
                    await SeedAsync(services, loggerFactory, retryForAvailability);
                }
            }
        }
    }
}