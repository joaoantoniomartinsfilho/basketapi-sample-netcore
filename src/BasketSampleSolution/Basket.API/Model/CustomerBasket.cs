using Newtonsoft.Json;
using System.Collections.Generic;

namespace Basket.API.Model
{
    public class CustomerBasket
    {
        public string BuyerId { get; }
        public ICollection<BasketItem> Items { get; } 

        public CustomerBasket()
        {              
        }

        [JsonConstructor]
        public CustomerBasket(string buyerId, ICollection<BasketItem> items)
        {
            BuyerId = buyerId;
            Items = items;
        }
    }
}
