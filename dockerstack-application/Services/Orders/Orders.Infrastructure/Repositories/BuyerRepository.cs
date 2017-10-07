using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Orders.Domain.AggregatesModel.BuyerAggregate;
using Orders.Domain.SeedWork;

namespace Orders.Infrastructure.Repositories
{
    public class BuyerRepository : IBuyerRepository
    {
        private readonly OrdersContext _context;
        
        public IUnitOfWork UnitOfWork => _context;

        public BuyerRepository(OrdersContext ordersContext)
        {
            _context = ordersContext;
        }
        
        public Buyer Add(Buyer buyer)
        {
            if (buyer.IsTransient())
            {
                return _context.Buyers
                    .Add(buyer)
                    .Entity;
            }
            
            return buyer;
        }

        public Buyer Update(Buyer buyer)
        {
            return _context.Buyers
                .Update(buyer)
                .Entity;
        }

        public async Task<Buyer> FindAsync(string identity)
        {
            var buyer = await _context.Buyers
                .FirstOrDefaultAsync(b => b.IdentityGuid == identity);

            return buyer;
        }
    }
}