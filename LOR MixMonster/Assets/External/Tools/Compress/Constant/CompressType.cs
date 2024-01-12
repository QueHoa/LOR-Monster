namespace Zitga.Core.Toolkit.Compression
{
    public enum CompressType
    {
        GZIP = 0,

        // Can't be supported natively in .NetStandard 2.0
        // Must use Snappy for .NET Standard: https://github.com/DavidRouyer/Snappy
        // SNAPPY = 1
    }
}