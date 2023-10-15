using System;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;
using WpfMarketplace.Data;

namespace WpfMarketplace.Pages
{
    /// <summary>
    /// Interaction logic for ProductsPage.xaml
    /// </summary>
    public partial class ProductsPage : Page
    {
        private static readonly ProductType _allProductType = new ProductType() { Name = "Все" };
        public static string[] Sortings { get; set; } = new[] { "По возрастанию цены", "По убыванию цены", "По умолчанию" };

        public Product[] Products { get; set; }
        public ProductType[] ProductTypes { get; set; }
        public int SelectedSortingIndex { get; set; } = 2;
        public ProductType SelectedType { get; set; } = _allProductType;
        public string SearchText { get; set; } = string.Empty;
        public int TotalItemsCount { get; set; }
        public int FilteredItemsCount { get; set; }

        public ProductsPage()
        {
            var productTypes = App.Context.ProductTypes.ToArray();
            ProductTypes = productTypes.Concat(new[] { _allProductType }).ToArray();
            Loaded += ProductsPage_Loaded;
            InitializeComponent();
        }

        private void ProductsPage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateSource();
        }

        private void ListViewItem_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var product = (Product)((ListView)sender).SelectedItem;
            NavigationService.Navigate(new ProductPage(product.Id));
        }

        private void UpdateSource()
        {
            Func<Product, object> sorting;
            switch (SelectedSortingIndex)
            {
                case 0:
                    sorting = x => x.Price;
                    break;
                case 1:
                    sorting = x => -x.Price;
                    break;
                default:
                    sorting = x => x.Id;
                    break;
            }


            Func<Product, bool> filter;
            if (SelectedType == _allProductType)
            {
                filter = x => x.Name.ToLower().Contains(SearchText.ToLower());
            }
            else
            {
                filter = x => x.Name.ToLower().Contains(SearchText.ToLower()) && x.ProductTypeId == SelectedType.Id;
            }
            var query = App.Context.Products.Where(x => !x.IsDeleted).ToArray();
            TotalItemsCount = query.Length;
            Products = query.Where(x => filter(x)).OrderBy(x => sorting(x)).ToArray();
            FilteredItemsCount = Products.Length;
            MainListView.GetBindingExpression(ItemsControl.ItemsSourceProperty).UpdateTarget();
            BindingOperations.GetBindingExpressionBase(StatusTextBlock, TextBlock.TextProperty).UpdateTarget();
        }

        private void ApplySearchButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            UpdateSource();
        }
    }
}
