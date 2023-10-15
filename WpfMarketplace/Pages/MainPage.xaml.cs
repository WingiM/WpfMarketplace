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

            pageMainFrame.Navigate(new ProductsPage());
            pageMainFrame.Navigated += PageMainFrame_Navigated;
        }

        private void PageMainFrame_Navigated(object sender, NavigationEventArgs e)
        {
            var res = (e.Content as Page);
            if (res == null) return;

            if (!pageMainFrame.NavigationService.CanGoBack) ButtonBack.Visibility = Visibility.Collapsed;
            else ButtonBack.Visibility = Visibility.Visible;
            TitleTb.Text = res.Title;
        }

        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            if (!pageMainFrame.NavigationService.CanGoBack) return;
            pageMainFrame.NavigationService.GoBack();
        }
    }
}
