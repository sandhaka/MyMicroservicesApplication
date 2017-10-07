using System;
using Orders.Domain.SeedWork;

namespace Orders.Domain.AggregatesModel.BuyerAggregate
{
    public class PaymentMethod : Entity
    {
        private string _cardNumber;
        private string _securityNumber;
        private string _cardHolder;
        private DateTime _expiration;

        protected PaymentMethod() { }
        
        public PaymentMethod(string cardNumber, string securityNumber, string cardHolder, DateTime expiration)
        {
            _cardHolder = cardHolder;
            _cardNumber = cardNumber;
            _expiration = expiration;
            _securityNumber = securityNumber;
            // TODO: check if any arguments is null
        }
        
        public bool IsEqualTo(string cardNumber, DateTime expiration)
        {
            return _cardNumber == cardNumber && _expiration == expiration;
        }
    }
}