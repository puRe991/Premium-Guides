using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using GameGuideApp.Core.Interfaces;
using GameGuideApp.Core.Models;

namespace GameGuideApp.Services.FileStorage
{
    public class EncryptedGuideRepository : IGuideRepository
    {
        private readonly IEncryptionService _encryptionService;
        private readonly string _storagePath;

        public EncryptedGuideRepository(IEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
            _storagePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "GameGuideApp", "guides.enc");
        }

        public IList<Guide> LoadAll()
        {
            if (!File.Exists(_storagePath)) return new List<Guide>();

            var encrypted = File.ReadAllText(_storagePath);
            var json = _encryptionService.Decrypt(encrypted);
            var serializer = new DataContractJsonSerializer(typeof(List<Guide>));
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(json)))
            {
                return (List<Guide>)serializer.ReadObject(stream);
            }
        }

        public void SaveAll(IList<Guide> guides)
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_storagePath));
            var serializer = new DataContractJsonSerializer(typeof(List<Guide>));
            using (var stream = new MemoryStream())
            {
                serializer.WriteObject(stream, new List<Guide>(guides));
                var json = Encoding.UTF8.GetString(stream.ToArray());
                var encrypted = _encryptionService.Encrypt(json);
                File.WriteAllText(_storagePath, encrypted);
            }
        }
    }
}
