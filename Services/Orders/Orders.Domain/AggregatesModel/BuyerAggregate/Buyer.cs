using System;
using System.Collections.Generic;
using System.Linq;
using Orders.Domain.AggregatesModel.Events;
using Orders.Domain.SeedWork;

namespace Orders.Domain.AggregatesModel.BuyerAggregate
{
    /// <summary>
    /// Buyer entity
    /// </summary>
    public class Buyer :
        Entity, IAggregateRoot
    {
        public string IdentityGuid { get; private set; }
        private List<PaymentMethod> _paymentMethods;
        public IEnumerable<PaymentMethod> PaymentMethods => _paymentMethods.AsReadOnly();

        protected Buyer()
        {
            _paymentMethods = new List<PaymentMethod>();
        }
        
        public Buyer(string identity) : this()
        {
            IdentityGuid = !string.IsNullOrWhiteSpace(identity) ? identity : throw new ArgumentNullException(nameof(identity));
        }

        public PaymentMethod VerifyOrAddPaymentMethod(string cardNumber, string securityNumber, string cardHolder,
            DateTime expiration, int orderId)
        {
            var existingPaymentMethod = _paymentMethods.FirstOrDefault(i => i.IsEqualTo(cardNumber, expiration));

            if (existingPaymentMethod != null)
            {
                AddDomainEvent(new PaymentMethodVerifiedEvent(this, existingPaymentMethod, orderId));

                return existingPaymentMethod;
            }
            
            var paymentMethod = new PaymentMethod(cardNumber, securityNumber, cardHolder, expiration);
            
            _paymentMethods.Add(paymentMethod);
            
            AddDomainEvent(new PaymentMethodVerifiedEvent(this, paymentMethod, orderId));

            return paymentMethod;
        }
    }
}