using System.Data.Entity.Migrations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfMarketplace.Data;
using WpfMarketplace.Services;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for BasketPage.xaml
    /// </summary>
    public partial class BasketPage : Page
    {
        public int TotalPrice { get; set; }
        public Basket[] BasketEntries { get; set; }
        public PickupPoint[] PickupPoints { get; set; }
        public PickupPoint SelectedPickupPoint { get; set; }

        public BasketPage()
        {
            Loaded += BasketPage_Loaded;
            PickupPoints = App.Context.PickupPoints.ToArray();
            InitializeComponent();
            UpdateList();
        }

        private void BasketPage_Loaded(object sender, RoutedEventArgs e)
        {
            UpdateList();
        }

        private void ReduceCountButton_Click(object sender, RoutedEventArgs e)
        {
            var basket = (Basket)((Button)sender).Tag;
            basket.Count--;
            if (basket.Count == 0)
            {
                App.Context.Baskets.Remove(basket);
                App.Context.SaveChanges();
            }
            else
            {
                App.Context.Baskets.AddOrUpdate(basket);
                App.Context.SaveChanges();
            }
            UpdateList();
        }

        private void IncreaseCountButton_Click(object sender, RoutedEventArgs e)
        {
            var basket = (Basket)((Button)sender).Tag;
            basket.Count++;
            App.Context.Baskets.AddOrUpdate(basket);
            App.Context.SaveChanges();            
            UpdateList();

        }

        private void UpdateList()
        {
            BasketEntries = App.Context.Baskets.Where(x => x.UserId == App.AuthorizedUserId).ToArray();
            TotalPrice = BasketEntries.Sum(x => x.Product.Price * x.Count);
            TotalTextBlock.GetBindingExpression(TextBlock.TextProperty).UpdateTarget();
            MainListView.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
        }

        private void CreateOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if (!App.Context.Baskets.Any(x => x.UserId == App.AuthorizedUserId))
            {
                MessageBox.Show("Корзина пустая!");
                return;
            }

            if (SelectedPickupPoint == null)
            {
                MessageBox.Show("Не выбран пункт выдачи заказа");
                return;
            }

            var res = BasketService.CreateOrder(App.AuthorizedUserId, SelectedPickupPoint.Id);
            NavigationService.Navigate(new OrderPage(res));
            MessageBox.Show("Заказ успешно оформлен");
            UpdateList();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var product = (Basket)((ListView)sender).SelectedItem;
            if (product == null)
                return;
            NavigationService.Navigate(new ProductPage(product.Product.Id));
        }
    }
}
