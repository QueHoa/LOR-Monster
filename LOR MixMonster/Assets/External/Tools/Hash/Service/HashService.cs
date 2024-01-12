using System.Security.Cryptography;
using System.Text;

namespace Zitga.Core.Toolkit.Hash
{
    public class HashService
    {
        public const string STRING_FORMAT = "x2";

        #region Hash
        public string Hash(string input)
        {
            return Hash(HashType.SHA_256, input);
        }

        public string Hash(HashType hashType, string input)
        {
            using (var algorithm = CreateHashAlgorithm(hashType))
            {
                byte[] output = algorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder hash = new StringBuilder();
                for (int i = 0; i < output.Length; i++)
                {
                    hash.Append(output[i].ToString(STRING_FORMAT));
                }

                return hash.ToString();
            }
        }
        #endregion

        #region Factory
        public HashAlgorithm CreateHashAlgorithm(HashType hashType)
        {
            switch (hashType)
            {
                case HashType.MD5:
                    return MD5.Create();

                case HashType.SHA_256:
                    return SHA256.Create();

                case HashType.SHA_512:
                    return SHA512.Create();
            }

            return null;
        }
        #endregion
    }
}