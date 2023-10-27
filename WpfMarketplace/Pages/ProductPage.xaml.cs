using Microsoft.Win32;
using QRCoder;
using System.Drawing;
using System.Data.Entity.Migrations;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using WpfMarketplace.Data;
using WpfMarketplace.Services;
using System.Drawing.Imaging;

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
            Unloaded += ProductPage_Unloaded;
            InitializeComponent();
            if (productId != null)
            {
                var generator = new QRCodeGenerator();
                var data = generator.CreateQrCode($"https://www.wildberries.ru/catalog/0/search.aspx?search={string.Join("", Product.Name.Split())}", QRCodeGenerator.ECCLevel.L);
                var qrCode = new QRCode(data);
                var pic = qrCode.GetGraphic(100);
                using (var ms = new MemoryStream())
                {
                    pic.Save(ms, ImageFormat.Png);
                    ms.Seek(0, SeekOrigin.Begin);
                    QRCodeImage.Source = GenerateImageSource(ms);
                }
            }
            Title = productId == null ? "Создание товара" : "Товар";
        }

        private void ProductPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (Product.Id != 0)
            {
                App.Context.Entry(Product).Reload();
            }
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
                using (MemoryStream ms = new MemoryStream())
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
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.EndInit();
            return bitmap;
        }

        private void AddToCartButton_Click(object sender, RoutedEventArgs e)
        {
            var res = BasketService.AddProductToBasket(App.AuthorizedUserId, Product.Id);
            MessageBox.Show($"Товар добавлен в корзину. Текущее количество: {res}", "Успешно", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }
    }
}
