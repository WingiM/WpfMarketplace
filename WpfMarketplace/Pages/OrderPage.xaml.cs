using System;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WpfMarketplace.Data;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for OrderPage.xaml
    /// </summary>
    public partial class OrderPage : Page
    {
        public Visibility AdminButtonsVisible { get; set; } = App.AuthorizedUserRole == Data.Enum.Roles.Administrator ? Visibility.Visible : Visibility.Hidden;

        public OrderStatu[] Statuses { get; set; }
        public OrderStatu SelectedStatus { get; set; }
        public Order Order { get; set; }
        public OrderProduct[] Products { get; set; }
        public int OrderSum { get; set; }

        public OrderStatusHistory[] History { get; set; }
        public OrderPage(int orderId)
        {
            Order = App.Context.Orders.Find(orderId);
            Products = App.Context.OrderProducts.Where(x => x.OrderId == orderId).ToArray();
            Statuses = App.Context.OrderStatus.ToArray();
            OrderSum = Products.Sum(x => x.Count * x.Product.Price);
            History = App.Context.OrderStatusHistories.Where(x => x.OrderId == orderId).OrderByDescending(x => x.DateChanged).ToArray();
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var product = (OrderProduct)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new ProductPage(product.Product.Id));
        }

        private void ChangeStatusButton_Click(object sender, RoutedEventArgs e)
        {
            var status = new OrderStatusHistory { OrderId = Order.Id, DateChanged = DateTime.Now, IsActual = true, StatusId = SelectedStatus.Id };
            var actualStatus = App.Context.OrderStatusHistories.First(x => x.OrderId == Order.Id && x.IsActual);
            actualStatus.IsActual = false;
            App.Context.OrderStatusHistories.AddOrUpdate(status);
            App.Context.OrderStatusHistories.AddOrUpdate(actualStatus);
            App.Context.SaveChanges();
            History = App.Context.OrderStatusHistories.Where(x => x.OrderId == Order.Id).OrderByDescending(x => x.DateChanged).ToArray();
            StatusListView.GetBindingExpression(ListView.ItemsSourceProperty).UpdateTarget();
        }
    }
}
