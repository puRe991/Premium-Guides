using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GameGuideApp.Core.Interfaces;
using GameGuideApp.Core.Models;

namespace GameGuideApp.ViewModels
{
    // Zentrales ViewModel des Startbildschirms.
    // Enthält nur UI-nahe Orchestrierung und delegiert Speicherung/Lizenz an Services.
    public class MainViewModel : BaseViewModel
    {
        private readonly IGuideRepository _repository;
        private readonly ILicenseService _licenseService;
        private Guide _selectedGuide;
        private string _licenseStatus;

        public MainViewModel(IGuideRepository repository, ILicenseService licenseService)
        {
            _repository = repository;
            _licenseService = licenseService;
            Guides = new ObservableCollection<Guide>(_repository.LoadAll());

            LoadDemoDataCommand = new RelayCommand(_ => LoadDemoData());
            SaveGuidesCommand = new RelayCommand(_ => SaveGuides(), _ => Guides.Any());
            RefreshLicenseState();

            if (Guides.Any()) SelectedGuide = Guides[0];
        }

        public ObservableCollection<Guide> Guides { get; private set; }
        public ICommand LoadDemoDataCommand { get; private set; }
        public ICommand SaveGuidesCommand { get; private set; }

        public Guide SelectedGuide
        {
            get { return _selectedGuide; }
            set
            {
                _selectedGuide = value;
                OnPropertyChanged();
            }
        }

        public string LicenseStatus
        {
            get { return _licenseStatus; }
            private set
            {
                _licenseStatus = value;
                OnPropertyChanged();
            }
        }

        private void RefreshLicenseState()
        {
            var license = _licenseService.GetCurrentLicense();
            LicenseStatus = license.IsLicensed
                ? "Lizenziert für " + license.LicensedTo
                : "Unlizenziert (Premium-Guides gesperrt)";
        }

        // Erstellt Beispieldaten für deinen gewünschten Workflow:
        // - Spiel eintragen (GameName)
        // - Guide-Text eintragen (Content)
        // - Grafik/Map eintragen (MapAssetPath)
        private void LoadDemoData()
        {
            Guides.Clear();

            Guides.Add(new Guide
            {
                Id = Guid.NewGuid().ToString("N"),
                GameName = "Elden Ring",
                Title = "Limgrave Start-Route",
                Category = "Leveling",
                Content = "1) Kirche von Elleh besuchen\n2) Pferd freischalten\n3) Südliche Halbinsel zuerst clearen",
                MapAssetPath = @"C:\\Guides\\Maps\\elden-ring-limgrave-route.png",
                UpdatedAtUtc = DateTime.UtcNow,
                IsLocked = false
            });

            Guides.Add(new Guide
            {
                Id = Guid.NewGuid().ToString("N"),
                GameName = "Diablo IV",
                Title = "Nightmare Dungeon Affix-Guide",
                Category = "PvE Endgame",
                Content = "Priorität: Defensive Affixe > Mobilität > AoE-Kontrolle.\nBoss-Phasen mit Cooldowns timen.",
                MapAssetPath = "https://example.com/maps/diablo4-nightmare-layout.jpg",
                UpdatedAtUtc = DateTime.UtcNow,
                IsLocked = true
            });

            SaveGuides();
            SelectedGuide = Guides[0];
        }

        // Speichert den aktuellen Stand verschlüsselt lokal (offline-first).
        private void SaveGuides()
        {
            _repository.SaveAll(Guides.ToList());
        }
    }
}
