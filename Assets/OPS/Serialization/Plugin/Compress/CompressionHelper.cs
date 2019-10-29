using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;

namespace OPS.Serialization.Compress
{
    internal static class CompressionHelper
    {
        public static byte[] Compress(byte[] _Data)
        {
            if (_Data == null)
            {
                throw new Exception("_Data is null!");
            }

            byte[] var_Result = null;

            using (MemoryStream var_Output = new MemoryStream())
            {
                using (GZipStream compressionStream = new GZipStream(var_Output, CompressionMode.Compress))
                {
                    compressionStream.Write(_Data, 0, _Data.Length);
                }

                var_Result = var_Output.ToArray();
            }

            return var_Result;
        }

        public static byte[] DeCompress(byte[] _Data)
        {
            if(_Data == null)
            {
                throw new Exception("_Data is null!");
            }

            byte[] var_Result = null;

            using (GZipStream compressionStream = new GZipStream(new MemoryStream(_Data), CompressionMode.Decompress))
            {
                const int size = 128;
                byte[] buffer = new byte[size];
                using (MemoryStream memory = new MemoryStream())
                {
                    int count = 0;
                    do
                    {
                        count = compressionStream.Read(buffer, 0, size);
                        if (count > 0)
                        {
                            memory.Write(buffer, 0, count);
                        }
                    }
                    while (count > 0);
                    var_Result = memory.ToArray();
                }
            }

            return var_Result;
        }
    }
}
