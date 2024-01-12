using System;
using Zitga.Core.Toolkit.Hash;

namespace Zitga.Core.Toolkit.Encryption
{
    public class AesCbcSha256Algorithm : EncryptAlgorithm
    {
        private readonly char[] DELIMITERS = new char[] { '.' };
        private readonly char DELIMITER = '.';

        private readonly AesCbcAlgorithm aes;
        private readonly HashService hashService;

        public AesCbcSha256Algorithm(string secretKey, string saltKey)
        {
            aes = new AesCbcAlgorithm(secretKey, saltKey);
            hashService = new HashService();
        }

        #region Encrypt
        public string Encrypt(string input)
        {
            string encrypted = aes.Encrypt(input);
            string hashed = hashService.Hash(HashType.SHA_256, input);

            return hashed + DELIMITER + encrypted;
        }
        #endregion

        #region Decrypt
        public string Decrypt(string input)
        {
            string[] parts = input.Split(DELIMITERS, StringSplitOptions.RemoveEmptyEntries);

            string hashed = parts[0];
            if (hashService.Hash(HashType.SHA_256, input) != hashed)
            {
                return null;
            }

            return aes.Decrypt(parts[1]);
        }
        #endregion
    }
}