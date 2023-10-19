using System.Linq;
using System.Windows.Controls;
using WpfMarketplace.Data;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for OrdersPage.xaml
    /// </summary>
    public partial class OrdersPage : Page
    {
        public Order[] Orders { get; set; }

        public OrdersPage()
        {
            var fetchAll = App.AuthorizedUserRole == Data.Enum.Roles.Administrator;
            if (fetchAll)
                Orders = App.Context.Orders.OrderByDescending(x => x.DateCreated).ToArray();
            else
                Orders = App.Context.Orders.Where(x => x.UserId == App.AuthorizedUserId).OrderByDescending(x => x.DateCreated).ToArray();
            InitializeComponent();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var order = (Order)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new OrderPage(order.Id));
        }
    }
}
