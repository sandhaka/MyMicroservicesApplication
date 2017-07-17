using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.AggregatesModel.OrderAggregate;
using Orders.Domain.SeedWork;

namespace Orders.Infrastructure.Repositories
{
    /// <summary>
    /// Orders repository
    /// </summary>
    public class OrderRepository : IOrderRepository
    {
        private readonly OrdersContext _context;
        
        public IUnitOfWork UnitOfWork => _context;

        public OrderRepository(OrdersContext ordersContext)
        {
            _context = ordersContext;
        }
        
        public Order Add(Order order)
        {
            return _context.Orders.Add(order).Entity;
        }

        public void Update(Order order)
        {
            _context.Entry(order).State = EntityState.Modified;
        }

        public async Task<Order> GetAsync(int orderId)
        {
            return await _context.Orders.FindAsync(orderId);
        }
    }
}