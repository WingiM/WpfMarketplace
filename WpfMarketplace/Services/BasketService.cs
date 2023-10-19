using System;
using System.Data.Entity.Migrations;
using System.Linq;
using WpfMarketplace.Data;

namespace WpfMarketplace.Services
{
    public static class BasketService
    {
        public static int AddProductToBasket(int clientId, int productId)
        {
            var basketEntry = App.Context.Baskets
                .FirstOrDefault(x => x.UserId == clientId && x.ProductId == productId) 
                ?? new Data.Basket { ProductId = productId, UserId = clientId };
            basketEntry.Count++;
            App.Context.Baskets.AddOrUpdate(basketEntry);
            App.Context.SaveChanges();
            return basketEntry.Count;
        }

        public static int CreateOrder(int clientId, int pickupPointId)
        {
            var basketForUser = App.Context.Baskets.Where(x => x.UserId == clientId);
            var order = new Order { DateCreated = DateTime.Now, PickupPointId = pickupPointId, UserId = App.AuthorizedUserId };
            foreach (var product in basketForUser)
            {
                App.Context.OrderProducts.Add(new OrderProduct { Order = order, ProductId = product.ProductId, Count = product.Count, SavedPrice = product.Product.Price });
                App.Context.Baskets.Remove(product);
            }
            var status = new OrderStatusHistory { Order = order, StatusId = 1, DateChanged = DateTime.Now, IsActual = true };
            App.Context.OrderStatusHistories.Add(status);
            App.Context.SaveChanges();
            return order.Id;
        }
    }
}
