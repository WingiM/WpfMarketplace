using System.Windows;
using WpfMarketplace.Data;
using WpfMarketplace.Data.Enum;

namespace WpfMarketplace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static readonly MarketplaceContext _context = new MarketplaceContext();
        public static MarketplaceContext Context => _context;

        public static Roles AuthorizedUserRole { get; set; }
    }
}
