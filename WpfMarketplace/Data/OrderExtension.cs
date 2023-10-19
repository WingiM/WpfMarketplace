using System.Linq;

namespace WpfMarketplace.Data
{
    public partial class Order
    {
        public OrderStatusHistory ActualStatus => OrderStatusHistories.First(x => x.IsActual);
        public int OrderSum => OrderProducts.Sum(x => x.Count * x.Product.Price);
    }
}
