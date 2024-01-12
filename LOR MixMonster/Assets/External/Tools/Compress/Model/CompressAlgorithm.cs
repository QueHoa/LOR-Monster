namespace Zitga.Core.Toolkit.Compression
{
    public interface CompressAlgorithm
    {
        string Compress(string input);

        string Decompress(string input);
    }
}