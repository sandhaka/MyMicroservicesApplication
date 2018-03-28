using System;
using Microsoft.Extensions.Logging;

namespace Orders.Infrastructure
{
    public class OrdersDbContextSeed
    {
        public void Seed(OrdersContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                using (context)
                {
                    // Nothing to initialize
                }
            }
            catch (Exception exception)
            {
                var log = loggerFactory.CreateLogger("Orders seed");
                log.LogError(exception.Message);
            }
        }
    }
}