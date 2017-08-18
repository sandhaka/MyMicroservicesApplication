using System.Collections.Generic;

namespace Basket.Application.Models
{
    public class CustomerBasket
    {
        public string Identity { get; set; }
        public List<BasketItem> BasketItems { get; set; }

        protected CustomerBasket()
        {
            BasketItems = new List<BasketItem>();
        }

        public CustomerBasket(string userIdentity) : this()
        {
            Identity = userIdentity;
        }
    }
}