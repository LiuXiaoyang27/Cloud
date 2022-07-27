using java.security;
using javax.crypto;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
    /// <summary>
    /// 利用IKVM类库，在.NET中对JAVA的AES加密字符串进行解密处理。
    /// </summary>
    public class AESEncrypt
    {

        #region ========加密========

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Encrypt(string toEncrypt, string key)
        {
            if (string.IsNullOrEmpty(toEncrypt)) return null;
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(toEncrypt);

            KeyGenerator kgen = KeyGenerator.getInstance("AES");
            SecureRandom secureRandom = SecureRandom.getInstance("SHA1PRNG");
            secureRandom.setSeed(Encoding.ASCII.GetBytes(key));
            kgen.init(128, secureRandom);
            SecretKey secretKey = kgen.generateKey();
            byte[] enCodeFormat = secretKey.getEncoded();

            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = enCodeFormat;
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateEncryptor())
                {
                    byte[] inputBuffers = toEncryptArray;
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Convert.ToBase64String(results);
                }
            }
        }


        #endregion

        #region ========解密========

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="toEncrypt"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string Decrypt(string toDecrypt, string key)
        {
            if (string.IsNullOrEmpty(toDecrypt)) return null;
            byte[] toEncryptArray = Convert.FromBase64String(toDecrypt);

            KeyGenerator kgen = KeyGenerator.getInstance("AES");
            SecureRandom secureRandom = SecureRandom.getInstance("SHA1PRNG");
            secureRandom.setSeed(Encoding.ASCII.GetBytes(key));
            kgen.init(128, secureRandom);
            SecretKey secretKey = kgen.generateKey();
            byte[] enCodeFormat = secretKey.getEncoded();

            using (AesCryptoServiceProvider aesProvider = new AesCryptoServiceProvider())
            {
                aesProvider.Key = enCodeFormat;
                aesProvider.Mode = CipherMode.ECB;
                aesProvider.Padding = PaddingMode.PKCS7;
                using (ICryptoTransform cryptoTransform = aesProvider.CreateDecryptor())
                {
                    byte[] inputBuffers = toEncryptArray;
                    byte[] results = cryptoTransform.TransformFinalBlock(inputBuffers, 0, inputBuffers.Length);
                    aesProvider.Clear();
                    return Encoding.UTF8.GetString(results);
                }
            }
        }

        #endregion

    }
}
