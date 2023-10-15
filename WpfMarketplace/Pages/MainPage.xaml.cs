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

            CreateProudctButton.Visibility = App.AuthorizedUserRole == Data.Enum.Roles.Administrator ? Visibility.Visible : Visibility.Hidden;
            pageMainFrame.Navigate(new ProductsPage());
            pageMainFrame.Navigated += PageMainFrame_Navigated;
        }

        private void PageMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var res = (e.Content as Page);
            if (res == null) return;

            if (res is ProductPage pp && pp.Title == "Создание товара") CreateProudctButton.Visibility = Visibility.Hidden;
            else CreateProudctButton.Visibility = App.AuthorizedUserRole == Data.Enum.Roles.Administrator ? Visibility.Visible : Visibility.Hidden;

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
    }
}
