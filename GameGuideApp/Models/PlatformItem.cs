using GameGuideApp.ViewModels;

namespace GameGuideApp.ViewModels
{
    public class PlatformItem : BaseViewModel
    {
        private bool _isActive;
        public string Name { get; set; }
        public string Icon { get; set; }
        public bool IsActive { get => _isActive; set { _isActive = value; OnPropertyChanged(); } }
    }
}
