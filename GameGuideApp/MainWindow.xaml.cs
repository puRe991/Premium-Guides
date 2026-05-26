using System.Windows;
using GameGuideApp.Services.Encryption;
using GameGuideApp.Services.FileStorage;
using GameGuideApp.Services.Licensing;
using GameGuideApp.ViewModels;

namespace GameGuideApp
{
    // Startpunkt für UI-Komposition.
    // Hier wird nur verdrahtet (kein Business-Code), damit MVVM sauber bleibt.
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Service-Instanzen (später durch DI-Container ersetzbar).
            var encryptionService = new AesEncryptionService();
            var repository = new EncryptedGuideRepository(encryptionService);
            var licenseService = new MockLicenseService();

            DataContext = new MainViewModel(repository, licenseService);
        }
    }
}
