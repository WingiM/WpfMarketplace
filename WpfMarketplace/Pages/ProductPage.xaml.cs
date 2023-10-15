using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfMarketplace.Data;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for ProductPage.xaml
    /// </summary>
    public partial class ProductPage : Page
    {
        private static readonly Regex _numberRegex = new Regex("[^0-9]+");

        public Product Product { get; set; } = new Product();

        private bool _isReadonly = true;
        private bool _isNew = true;

        public ProductType[] ProductTypes { get; set; }
        public bool IsReadonly => _isReadonly;
        public bool IsNotReadonly => !_isReadonly;
        public Visibility AdminButtonsVisibility => _isReadonly ? Visibility.Hidden : Visibility.Visible;
        public Visibility DeleteButtonVisibility => _isReadonly || _isNew ? Visibility.Hidden : Visibility.Visible;

        public ProductPage(int? productId = null)
        {
            if (productId != null)
            {
                _isNew = false;
                Product = App.Context.Products.Find(productId);
            }
            _isReadonly = App.AuthorizedUserRole == Data.Enum.Roles.Client;
            ProductTypes = App.Context.ProductTypes.ToArray();
            InitializeComponent();
            Title = productId == null ? "Создание товара" : "Товар";
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(Product.Description))
            {
                MessageBox.Show("Для товара не указано описание");
                return;
            }

            if (string.IsNullOrEmpty(Product.Name))
            {
                MessageBox.Show("Для товара не указано название");
                return;
            }

            if (Product.Price == 0)
            {
                MessageBox.Show("Для товара не указана цена");
                return;
            }

            if (Product.ProductType == null)
            {
                MessageBox.Show("Для товара не указан тип");
                return;
            }

            App.Context.Products.AddOrUpdate(Product);
            App.Context.SaveChanges();
            NavigationService.GoBack();
        }

        private void PriceTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = _numberRegex.IsMatch(e.Text);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Вы уверены?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Question, MessageBoxResult.No);
            if (result == MessageBoxResult.Yes)
            {
                Product.IsDeleted = true;
                App.Context.Products.AddOrUpdate(Product);
                App.Context.SaveChanges();
                NavigationService.GoBack();
            }
        }

        private void SelectImageButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new OpenFileDialog();
            if (window.ShowDialog() == true)
            {
                MemoryStream ms = new MemoryStream();
                using (Stream fileStream = window.OpenFile())
                {
                    fileStream.CopyTo(ms);
                    Product.Image = ms.ToArray();
                    MainImage.Source = GenerateImageSource(ms);
                }
            }
        }

        private ImageSource GenerateImageSource(MemoryStream file)
        {
            var bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.StreamSource = file;
            bitmap.EndInit();
            return bitmap;
        }
    }
}
