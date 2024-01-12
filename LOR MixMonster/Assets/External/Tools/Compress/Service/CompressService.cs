namespace Zitga.Core.Toolkit.Compression
{
    public class CompressService
    {
        #region Compress
        public string Compress(string input)
        {
            return Compress(CompressType.GZIP, input);
        }

        public string Compress(CompressType compressType, string input)
        {
            CompressAlgorithm algorithm = CreateCompressAlgorithm(compressType);
            return algorithm.Compress(input);
        }
        #endregion

        #region Decompress
        public string Decompress(string input)
        {
            return Decompress(CompressType.GZIP, input);
        }

        public string Decompress(CompressType compressType, string input)
        {
            CompressAlgorithm algorithm = CreateCompressAlgorithm(compressType);
            return algorithm.Decompress(input);
        }
        #endregion

        #region Factory
        public CompressAlgorithm CreateCompressAlgorithm(CompressType compressType)
        {
            switch (compressType)
            {
                case CompressType.GZIP:
                    return new GZipAlgorithm();
            }

            return null;
        }
        #endregion
    }
}