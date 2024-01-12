using System.Security.Cryptography;
using System.Text;

namespace Zitga.Core.Toolkit.HMac
{
    public class HMacService
    {
        public const string STRING_FORMAT = "x2";

        #region HMac
        public string HMac(string key, string input)
        {
            return HMac(HMacType.HMAC_SHA_256, key, input);
        }

        public string HMac(HMacType hMacType, string key, string input)
        {
            using (var algorithm = CreateHMacAlgorithm(hMacType, key))
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
        public HMAC CreateHMacAlgorithm(HMacType hMacType, string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            switch (hMacType)
            {
                case HMacType.HMAC_MD5:
                    return new HMACMD5(keyBytes);

                case HMacType.HMAC_SHA_256:
                    return new HMACSHA256(keyBytes);

                case HMacType.HMAC_SHA_512:
                    return new HMACSHA512(keyBytes);
            }

            return null;
        }
        #endregion
    }
}