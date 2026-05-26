using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using GameGuideApp.Core.Interfaces;

namespace GameGuideApp.Services.Encryption
{
    public class AesEncryptionService : IEncryptionService
    {
        private static readonly byte[] Key = Encoding.UTF8.GetBytes("GGA-STATIC-KEY-32-BYTES-LENGTH!!");
        private static readonly byte[] Iv = Encoding.UTF8.GetBytes("GGA-INIT-VECTOR!");

        public string Encrypt(string plainText)
        {
            if (plainText == null) throw new ArgumentNullException("plainText");

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = Iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var memoryStream = new MemoryStream())
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                using (var writer = new StreamWriter(cryptoStream))
                {
                    writer.Write(plainText);
                    writer.Flush();
                    cryptoStream.FlushFinalBlock();
                    return Convert.ToBase64String(memoryStream.ToArray());
                }
            }
        }

        public string Decrypt(string cipherText)
        {
            if (cipherText == null) throw new ArgumentNullException("cipherText");
            var encryptedBytes = Convert.FromBase64String(cipherText);

            using (var aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = Iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (var memoryStream = new MemoryStream(encryptedBytes))
                using (var cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                using (var reader = new StreamReader(cryptoStream))
                {
                    return reader.ReadToEnd();
                }
            }
        }
    }
}
