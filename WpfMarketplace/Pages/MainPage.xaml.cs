using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for MainPage.xaml
    /// </summary>
    public partial class MainPage : Page
    {
        public MainPage()
        {
            InitializeComponent();

            CreateProductButton.Visibility = App.AuthorizedUserRole == Data.Enum.Roles.Administrator ? Visibility.Visible : Visibility.Hidden;
            BasketButton.Visibility = App.AuthorizedUserRole == Data.Enum.Roles.Client ? Visibility.Visible : Visibility.Hidden;
            pageMainFrame.Navigate(new ProductsPage());
            pageMainFrame.Navigated += PageMainFrame_Navigated;
        }

        private void PageMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var res = (e.Content as Page);
            if (res == null) return;

            if (res is ProductPage pp && pp.Title == "Создание товара") CreateProductButton.Visibility = Visibility.Hidden;
            else CreateProductButton.Visibility = App.AuthorizedUserRole == Data.Enum.Roles.Administrator ? Visibility.Visible : Visibility.Hidden;

            if (res is OrdersPage) OrdersButton.Visibility = Visibility.Hidden;
            else OrdersButton.Visibility = Visibility.Visible;

            if (res is BasketPage) BasketButton.Visibility = Visibility.Hidden;
            else BasketButton.Visibility = App.AuthorizedUserRole == Data.Enum.Roles.Client ? Visibility.Visible : Visibility.Hidden;

            if (!pageMainFrame.NavigationService.CanGoBack) ButtonBack.IsEnabled = false;
            else ButtonBack.IsEnabled = true;
            TitleTb.Text = res.Title;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (!pageMainFrame.NavigationService.CanGoBack) return;
            pageMainFrame.NavigationService.GoBack();
        }

        private void CreateProudctButton_Click(object sender, RoutedEventArgs e)
        {
            pageMainFrame.Navigate(new ProductPage());
        }

        private void BasketButton_Click(object sender, RoutedEventArgs e)
        {
            pageMainFrame.Navigate(new BasketPage());
        }

        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            pageMainFrame.Navigate(new OrdersPage());
        }
    }
}
