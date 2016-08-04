using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Linq;

using System.Text.RegularExpressions;
using System.Text;

namespace JukeWeb.Foundry.Utilities
{
    public class IOUtils
    {
        public static byte[] GetStreamAsByteArray(Stream stream)
        {
            byte[] buffer = new byte[16 * 1024];
            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        public static string GetStreamAsString(Stream stream)
        {
            StreamReader reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }

    }
}
