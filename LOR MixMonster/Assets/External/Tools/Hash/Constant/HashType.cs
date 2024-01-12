namespace Zitga.Core.Toolkit.Hash
{
    public enum HashType
    {
        MD5 = 0,
        SHA_256 = 1,
        SHA_512 = 2,

        // Can't be supported natively in .NetStandard 2.0
        // Must use Bouncy Castle C#: https://www.bouncycastle.org/csharp/ (~3MB)
        // SHA3_256 = 3,
        // SHA3_512 = 4,
    }
}