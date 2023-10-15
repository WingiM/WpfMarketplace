using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WpfMarketplace.Data.Enum;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for AuthorizationPage.xaml
    /// </summary>
    public partial class AuthorizationPage : Page
    {
        public AuthorizationPage()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new RegisterPage());
        }

        private void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                MessageBox.Show("Логин или пароль не заполнены");
                return;
            }

            var user = App.Context.Users.FirstOrDefault(x => x.Login == LoginTextBox.Text && x.Password == PasswordTextBox.Password);
            if (user == null)
            {
                MessageBox.Show("Неверный логин или пароль");
                return;
            }

            while (NavigationService.CanGoBack)
            {
                NavigationService.RemoveBackEntry();
            }

            NavigationService.Navigated += NavigationService_Navigated;
            NavigationService.Navigate(new MainPage());
            App.AuthorizedUserRole = (Roles)user.RoleId;
        }

        private void NavigationService_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {
            (sender as Frame).RemoveBackEntry();
            (sender as Frame).Navigated -= NavigationService_Navigated;
        }
    }
}
