using GameGuideApp.Core.Models;

namespace GameGuideApp.Core.Interfaces
{
    public interface ILicenseService
    {
        LicenseInfo GetCurrentLicense();
        bool Activate(string licenseKey, string customerName);
    }
}
