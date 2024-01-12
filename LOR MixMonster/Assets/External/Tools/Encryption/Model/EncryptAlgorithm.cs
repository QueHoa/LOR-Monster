namespace Zitga.Core.Toolkit.Encryption
{
    public interface EncryptAlgorithm
    {
        string Encrypt(string input);

        string Decrypt(string input);
    }
}