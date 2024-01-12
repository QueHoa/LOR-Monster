namespace Zitga.Core.Toolkit.Encryption
{
    public enum EncryptionType
    {
        AES_CBC_SHA_256 = 0,
        AES_CBC = 1,

        // Can't be supported natively in .NetStandard 2.0
        // Must use Bouncy Castle C#: https://www.bouncycastle.org/csharp/ (~3MB)
        // AES_GCM = 2,
    }
}