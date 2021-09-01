using Mao.Web.Features.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace Mao.Web.Features
{
    public class Sha512Encryptor : IEncryptor
    {
        public string Decrypt(string ciphertext)
        {
            throw new NotSupportedException("非對稱式加密無法解密");
        }

        public string Encrypt(string plaintext)
        {
            HashAlgorithm hashAlgorithm = new SHA512CryptoServiceProvider();
            return Convert.ToBase64String(
                hashAlgorithm.ComputeHash(
                    System.Text.Encoding.UTF8.GetBytes(plaintext)));
        }
    }
}