using System;
using System.IO;
using GameGuideApp.Core.Interfaces;
using GameGuideApp.Core.Models;

namespace GameGuideApp.Services.Licensing
{
    public class MockLicenseService : ILicenseService
    {
        private readonly string _licensePath;

        public MockLicenseService()
        {
            _licensePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameGuideApp", "license.dat");
        }

        public LicenseInfo GetCurrentLicense()
        {
            if (!File.Exists(_licensePath)) return new LicenseInfo { IsLicensed = false };
            var payload = File.ReadAllText(_licensePath);
            var parts = payload.Split('|');
            if (parts.Length != 2) return new LicenseInfo { IsLicensed = false };
            return new LicenseInfo { IsLicensed = true, LicensedTo = parts[0], LicenseKey = parts[1] };
        }

        public bool Activate(string licenseKey, string customerName)
        {
            if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(licenseKey)) return false;
            if (!licenseKey.StartsWith("GUIDE-", StringComparison.OrdinalIgnoreCase)) return false;

            Directory.CreateDirectory(Path.GetDirectoryName(_licensePath));
            File.WriteAllText(_licensePath, customerName + "|" + licenseKey);
            return true;
        }
    }
}
