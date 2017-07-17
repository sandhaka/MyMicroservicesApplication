using System;
using System.Collections.Generic;
using System.Linq;
using Orders.Domain.AggregatesModel.Events;
using Orders.Domain.SeedWork;

namespace Orders.Domain.AggregatesModel.OrderAggregate
{
    /// <summary>
    /// Order domain object
    /// </summary>
    public class Order :
        Entity, IAggregateRoot
    {
        private DateTime _orderDate;

        public decimal TotalAmount()
        {
            return _orderItems.Select(i => i.GetUnitPrice()).Sum();
        }

        private readonly List<OrderItem> _orderItems;

        public IEnumerable<OrderItem> OrderItems => _orderItems.AsReadOnly();
        
        public Order()
        {
            _orderItems = new List<OrderItem>();
            _orderDate = DateTime.UtcNow;
            
            // Emit a order started domain event
            AddOrderStartedDomainEvent();
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

        private void AddOrderStartedDomainEvent()
        {
            var orderStartedDomainEvent = new OrderStartedDomainEvent(this);
            this.AddDomainEvent(orderStartedDomainEvent);
            // TODO handler not yet implemented
        }
    }
}