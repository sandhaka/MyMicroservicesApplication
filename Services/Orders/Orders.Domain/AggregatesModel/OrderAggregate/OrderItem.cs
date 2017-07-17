using Orders.Domain.SeedWork;

namespace Orders.Domain.AggregatesModel.OrderAggregate
{
    public class OrderItem : Entity
    {
        private string _productName;
        private decimal _unitPrice;
        private int _units;
        
        public int ProductId { get; private set; }
        
        protected OrderItem() {}

        public OrderItem(int productId, string productName, decimal unitPrize, int units = 1)
        {
            if (units <= 0)
            {
                // TODO Order exception
            }

            ProductId = productId;

            _productName = productName;
            _unitPrice = unitPrize;
            _units = units;
        }

        public void AddUnits(int units)
        {
            if (units < 0)
            {
                // TODO the same
            }

            _units += units;
        }

        public decimal GetUnitPrice()
        {
            return _unitPrice;
        }
    }
}