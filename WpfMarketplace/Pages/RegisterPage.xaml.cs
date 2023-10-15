using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using WpfMarketplace.Data;
using WpfMarketplace.Data.Enum;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for RegisterPage.xaml
    /// </summary>
    public partial class RegisterPage : Page
    {
        public RegisterPage()
        {
            InitializeComponent();
        }

        private void AuthorizeButton_Click(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new AuthorizationPage());
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(LoginTextBox.Text) || string.IsNullOrEmpty(NameTextBox.Text) || string.IsNullOrEmpty(PasswordTextBox.Password))
            {
                MessageBox.Show("Не все данные корректно заполнены");
                return;
            }

            var userExists = App.Context.Users.Any(x => x.Login == LoginTextBox.Text);
            if (userExists)
            {
                MessageBox.Show("Пользователь с таким логином уже существует");
                return;
            }

            var user = new User { Login = LoginTextBox.Text, Name = NameTextBox.Text, Password = PasswordTextBox.Password, RoleId = (int)Roles.Client };
            App.Context.Users.Add(user);
            App.Context.SaveChanges();
            MessageBox.Show("Успешная регистрация");
            AuthorizeButton_Click(sender, e);
        }
    }
}
