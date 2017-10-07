using Orders.Domain.SeedWork;

namespace Orders.Domain.AggregatesModel.OrderAggregate
{
    public class OrderItem : Entity
    {
        private string _productName;
        private decimal _unitPrice;
        
        public int Units { get; private set; }
        public int ProductId { get; private set; }
        
        protected OrderItem() {}

        public OrderItem(int productId, string productName, decimal unitPrize, int units = 1)
        {
            if (units <= 0)
            {
                // TODO Order exception
            }

            ProductId = productId;
            Units = units;

            _productName = productName;
            _unitPrice = unitPrize;
        }

        public void AddUnits(int units)
        {
            if (units < 0)
            {
                // TODO the same
            }

            Units += units;
        }

        public decimal GetUnitPrice()
        {
            return _unitPrice;
        }
    }
}