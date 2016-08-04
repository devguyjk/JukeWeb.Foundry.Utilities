using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace System.IO
{
    public static class StreamExtensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            int length = 1000;
            List<byte> bytes = new List<byte>();
            using (BinaryReader reader = new BinaryReader(stream))
            {
                byte[] buffer = null;
                do
                {
                    buffer = reader.ReadBytes(length);
                    if (buffer.Length > 0)
                        bytes.AddRange(buffer.ToList());
                } while (buffer.Length > 0);
            }

            return bytes.ToArray();
        }
    }
}
