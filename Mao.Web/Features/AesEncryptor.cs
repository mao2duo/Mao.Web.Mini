using Mao.Web.Features.Interfaces;
using Mao.Web.Features.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Mao.Web.Features
{
    public class AesEncryptor : IEncryptor
    {
        private readonly AppSettings _appSettings;
        public AesEncryptor(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        /// <summary>
        /// AES 加密
        /// </summary>
        public string Encrypt(string plaintext)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_appSettings.AesKey);
            var ivBytes = Encoding.UTF8.GetBytes(_appSettings.AesIV);
            var dataBytes = Encoding.UTF8.GetBytes(plaintext);
            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var encryptor = aes.CreateEncryptor();
                var encrypt = encryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Convert.ToBase64String(encrypt);
            }
        }

        /// <summary>
        /// AES 解密
        /// </summary>
        public string Decrypt(string ciphertext)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_appSettings.AesKey);
            var ivBytes = Encoding.UTF8.GetBytes(_appSettings.AesIV);
            var dataBytes = Convert.FromBase64String(ciphertext);
            using (var aes = Aes.Create())
            {
                aes.Key = keyBytes;
                aes.IV = ivBytes;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;
                var decryptor = aes.CreateDecryptor();
                var decrypt = decryptor.TransformFinalBlock(dataBytes, 0, dataBytes.Length);
                return Encoding.UTF8.GetString(decrypt);
            }
        }
    }
}