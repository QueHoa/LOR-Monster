using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Zitga.Core.Toolkit.Encryption
{
    public class AesCbcAlgorithm : EncryptAlgorithm
    {
        private const int KEY_ITERATIONS = 100;

        private readonly Rfc2898DeriveBytes key;

        public AesCbcAlgorithm(string secretKey, string saltKey)
        {
            key = new Rfc2898DeriveBytes(Encoding.UTF8.GetBytes(secretKey), Encoding.UTF8.GetBytes(saltKey), KEY_ITERATIONS);
        }

        #region Encrypt
        public string Encrypt(string input)
        {
            using (var stream = new MemoryStream())
            {
                using (var aes = new AesManaged())
                {
                    aes.Key = key.GetBytes(16);
                    aes.IV = key.GetBytes(16);

                    using (var aesStream = new CryptoStream(stream, aes.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                        aesStream.Write(inputBytes, 0, inputBytes.Length);
                        aesStream.FlushFinalBlock();

                        return Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
        }
        #endregion

        #region Decrypt
        public string Decrypt(string input)
        {
            using (var stream = new MemoryStream())
            {
                using (var aes = new AesManaged())
                {
                    aes.Key = key.GetBytes(16);
                    aes.IV = key.GetBytes(16);

                    using (var aesStream = new CryptoStream(stream, aes.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        byte[] inputBytes = Convert.FromBase64String(input);
                        aesStream.Write(inputBytes, 0, inputBytes.Length);
                        aesStream.FlushFinalBlock();

                        byte[] decryptBytes = stream.ToArray();

                        return Encoding.UTF8.GetString(decryptBytes, 0, decryptBytes.Length);
                    }
                }
            }
        }
        #endregion
    }
}