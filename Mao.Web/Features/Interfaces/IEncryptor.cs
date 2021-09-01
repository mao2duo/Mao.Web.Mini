using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mao.Web.Features.Interfaces
{
    public interface IEncryptor
    {
        /// <summary>
        /// 加密
        /// </summary>
        string Encrypt(string plaintext);
        /// <summary>
        /// 解密
        /// </summary>
        string Decrypt(string ciphertext);
    }
}
