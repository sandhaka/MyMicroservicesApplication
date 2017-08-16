using System;
using System.Collections.Generic;
using System.Linq;
using Orders.Domain.AggregatesModel.Events;
using Orders.Domain.SeedWork;

namespace Orders.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// Order entity
    /// </summary>
    public class Order :
        Entity, IAggregateRoot
    {
        private DateTime _orderDate;
        private int? _buyerId;
        private int? _paymentMethodId;

        public decimal TotalAmount()
        {
            return _orderItems.Select(i => i.GetUnitPrice()).Sum();
        }

        private readonly List<OrderItem> _orderItems;

        public IEnumerable<OrderItem> OrderItems => _orderItems.AsReadOnly();
        
        public Order( 
            string cardNumber, 
            string cardSecurityNumber,
            string cardHolderName, 
            DateTime cardExpiration, 
            int? buyerId = null, 
            int? paymentMethodId = null)
        {
            _orderItems = new List<OrderItem>();
            _orderDate = DateTime.UtcNow;

            _buyerId = buyerId;
            _paymentMethodId = paymentMethodId;
            
            // Emit a order started domain event
            AddOrderStartedDomainEvent(cardNumber, cardHolderName, cardSecurityNumber, cardExpiration);
        }

        public void AddOrderItem(int productId, string productName, decimal unitPrize, int units)
        {
            var existingOrderForProduct = _orderItems.SingleOrDefault(o => o.ProductId == productId);

            if (existingOrderForProduct != null)
            {
                existingOrderForProduct.AddUnits(units);
            }
            else
            {
                _orderItems.Add(new OrderItem(productId, productName, unitPrize, units));
            }
        }

        private void AddOrderStartedDomainEvent(string cardNumber, string cardHolder, string cardSecurityNumber,
            DateTime cardExpiration)
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(
                this,cardNumber,cardHolder,cardSecurityNumber,cardExpiration);
            
            this.AddDomainEvent(orderStartedDomainEvent);
        }
    }
}