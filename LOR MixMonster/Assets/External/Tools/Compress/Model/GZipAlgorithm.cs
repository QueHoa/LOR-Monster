using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace Zitga.Core.Toolkit.Compression
{
    public class GZipAlgorithm : CompressAlgorithm
    {
        public string Compress(string input)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(input);
            using (var stream = new MemoryStream())
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Compress, true))
                {
                    gzip.Write(inputBytes, 0, inputBytes.Length);
                }

                var output = stream.ToArray();
                return Convert.ToBase64String(output);
            }
        }

        public string Decompress(string input)
        {
            byte[] inputBytes = Convert.FromBase64String(input);
            using (var stream = new MemoryStream(inputBytes))
            {
                using (var gzip = new GZipStream(stream, CompressionMode.Decompress))
                {
                    using (var outputStream = new MemoryStream())
                    {
                        gzip.CopyTo(outputStream);

                        var output = outputStream.ToArray();
                        return Encoding.UTF8.GetString(output);
                    }
                }
            }
        }
    }
}