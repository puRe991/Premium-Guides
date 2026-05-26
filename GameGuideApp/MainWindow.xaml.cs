using System.Windows;
using GameGuideApp.Services.Encryption;
using GameGuideApp.Services.FileStorage;
using GameGuideApp.Services.Licensing;
using GameGuideApp.ViewModels;

namespace GameGuideApp
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var encryptionService = new AesEncryptionService();
            var repository = new EncryptedGuideRepository(encryptionService);
            var licenseService = new MockLicenseService();

            DataContext = new MainViewModel(repository, licenseService);
        }
    }
}
