namespace Zitga.Core.Toolkit.HMac
{
    public enum HMacType
    {
        HMAC_MD5 = 0,
        HMAC_SHA_256 = 1,
        HMAC_SHA_512 = 2,

        // Can't be supported natively in .NetStandard 2.0
        // Must use Bouncy Castle C#: https://www.bouncycastle.org/csharp/ (~3MB)
        // HMAC_SHA3_256 = 3,
        // HMAC_SHA3_512 = 4,
    }
}