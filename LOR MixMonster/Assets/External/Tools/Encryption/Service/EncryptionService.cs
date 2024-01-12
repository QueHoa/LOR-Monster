namespace Zitga.Core.Toolkit.Encryption
{
    public class EncryptionService
    {
        #region Encrypt
        public string Encrypt(string secretKey, string saltKey, string input)
        {
            return Encrypt(EncryptionType.AES_CBC, secretKey, saltKey, input);
        }

        public string Encrypt(EncryptionType encryptionType, string secretKey, string saltKey, string input)
        {
            EncryptAlgorithm algorithm = CreateEncryptAlgorithm(encryptionType, secretKey, saltKey);
            return algorithm.Encrypt(input);
        }
        #endregion

        #region Decrypt
        public string Decrypt(string secretKey, string saltKey, string input)
        {
            return Decrypt(EncryptionType.AES_CBC, secretKey, saltKey, input);
        }

        public string Decrypt(EncryptionType encryptionType, string secretKey, string saltKey, string input)
        {
            EncryptAlgorithm algorithm = CreateEncryptAlgorithm(encryptionType, secretKey, saltKey);
            return algorithm.Decrypt(input);
        }
        #endregion

        #region Factory
        public EncryptAlgorithm CreateEncryptAlgorithm(EncryptionType encryptionType, string secretKey, string saltKey)
        {
            switch (encryptionType)
            {
                case EncryptionType.AES_CBC_SHA_256:
                    return new AesCbcSha256Algorithm(secretKey, saltKey);

                case EncryptionType.AES_CBC:
                    return new AesCbcAlgorithm(secretKey, saltKey);
            }

            return null;
        }
        #endregion
    }
}
