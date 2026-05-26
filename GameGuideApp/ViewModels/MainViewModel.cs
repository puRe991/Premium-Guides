using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using GameGuideApp.Core.Interfaces;
using GameGuideApp.Core.Models;

namespace GameGuideApp.ViewModels
{
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
            RefreshLicenseState();

            if (Guides.Any()) SelectedGuide = Guides[0];
        }

        public ObservableCollection<Guide> Guides { get; private set; }
        public ICommand LoadDemoDataCommand { get; private set; }

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
                ? "Licensed to " + license.LicensedTo
                : "Unlicensed (locked premium guides)";
        }

        private void LoadDemoData()
        {
            Guides.Clear();
            Guides.Add(new Guide
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Beginner Quest Route",
                Category = "Leveling",
                Content = "Step 1: Complete the town quests...",
                UpdatedAtUtc = DateTime.UtcNow,
                IsLocked = false
            });
            Guides.Add(new Guide
            {
                Id = Guid.NewGuid().ToString("N"),
                Title = "Endgame Dungeon Mechanics",
                Category = "PvE",
                Content = "Premium strategy for final boss phases...",
                UpdatedAtUtc = DateTime.UtcNow,
                IsLocked = true
            });

            _repository.SaveAll(Guides.ToList());
            SelectedGuide = Guides[0];
        }
    }
}
