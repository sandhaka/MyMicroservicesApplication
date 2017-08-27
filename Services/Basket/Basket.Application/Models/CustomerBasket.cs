using System.Collections.Generic;

namespace Basket.Application.Models
{
    public class CustomerBasket
    {
        public List<BasketItem> BasketItems { get; set; }

        protected CustomerBasket()
        {
            BasketItems = new List<BasketItem>();
        }
    }
}