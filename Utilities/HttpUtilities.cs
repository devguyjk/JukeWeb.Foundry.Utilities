using System;
using System.Drawing;
using System.IO;
using System.Net;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

using System.Text.RegularExpressions;
using System.Text;
using System.Web;
using System.IO.Compression;

namespace JukeWeb.Foundry.Utilities
{
    public class HTTPUtilities
    {
        public static Image GetWebItemAsImage(string url)
        {
            byte[] imageData = GetWebItemAsByteArray(url);
            MemoryStream stream = new MemoryStream(imageData);
            Image img = Image.FromStream(stream);
            stream.Close();
            return img;
        }

        public static byte[] GetWebItemAsByteArray(string url)
        {
            Stream stream = GetWebItemAsStream(url);
            byte[] bytes = IOUtils.GetStreamAsByteArray(stream);
            stream.Close();
            return bytes;
        }

        public static Stream GetWebItemAsStream(string url)
        {
            WebRequest req = WebRequest.Create(url);
            WebResponse response = req.GetResponse();
            Stream stream = response.GetResponseStream();
            return stream;
        }

        public static string GetHTML(string url)
        {
            Uri address = new Uri(url);
            HttpWebRequest request = WebRequest.Create(address) as HttpWebRequest;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";

            StringBuilder data = new StringBuilder();
            data.Append("DPT_Date=" + "17-05-2008")
            .Append(@"&RET_Date=" + @"20-05-2008")
            .Append("&dpt_station=" + "LIS")
            .Append("&arv_station=" + "LHR")
            .Append("&non_stops=" + "on");

            // Create a byte array of the data we want to send
            byte[] byteData = UTF8Encoding.UTF8.GetBytes(data.ToString());
            // Set the content length in the request headers
            request.ContentLength = byteData.Length;
            // Write data
            using (Stream postStream = request.GetRequestStream())
            {
                postStream.Write(byteData, 0, byteData.Length);
            }

            // Get response
            string htmlResponse;
            using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
            {
                // Get the response stream
                StreamReader reader = new StreamReader(response.GetResponseStream());
                // Get the response string
                htmlResponse = reader.ReadToEnd();
            }

            return htmlResponse;
        }


        public static MatchCollection GetUrlRegexMatches(string url, string pattern)
        {
            Regex regex = regex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string html = "";
            using (WebClient client = new WebClient())
            {
                client.Headers.Add("user-agent", "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");
                byte[] dataBytes = client.DownloadData(url);
                html = Encoding.ASCII.GetString(dataBytes);
            }

            return regex.Matches(html);
        }

        public static string NormalizeRequestParameters(NameValueCollection parameters)
        {
            StringBuilder sb = new StringBuilder();
            foreach (string myKey in parameters.AllKeys)
            {
                sb.AppendFormat("{0}={1}", myKey, parameters[myKey]);
                sb.Append("&");
            }

            return sb.ToString();
        }

        public static byte[] ZipString(HttpContext context, string str)
        {
            var encodingsAccepted = context.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted))
                return new byte[] { };

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = context.Response;

            Stream zipStream = null;
            using (MemoryStream stream = new MemoryStream())
            {
                if (encodingsAccepted.Contains("deflate"))
                {
                    try
                    {
                        response.AppendHeader("Content-encoding", "deflate");
                    }
                    catch (Exception ex) { }
                    zipStream = new DeflateStream(stream, CompressionMode.Compress);
                }
                else if (encodingsAccepted.Contains("gzip"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    zipStream = new GZipStream(stream, CompressionMode.Compress);
                }
                if (zipStream == null)
                    return new byte[] { };
                using (StreamWriter writer = new StreamWriter(zipStream, Encoding.UTF8))
                {
                    writer.Write(str);
                    writer.Close();
                }
                zipStream.Close();
                zipStream.Dispose();

                return stream.ToArray();
            }
        }

        public static byte[] Zipbytes(HttpContext context, byte[] bytes)
        {
            var encodingsAccepted = context.Request.Headers["Accept-Encoding"];
            if (string.IsNullOrEmpty(encodingsAccepted))
                return new byte[] { };

            encodingsAccepted = encodingsAccepted.ToLowerInvariant();
            var response = context.Response;

            Stream zipStream = null;
            using (MemoryStream stream = new MemoryStream())
            {
                if (encodingsAccepted.Contains("deflate"))
                {
                    response.AppendHeader("Content-encoding", "deflate");
                    zipStream = new DeflateStream(stream, CompressionMode.Compress);
                }
                else if (encodingsAccepted.Contains("gzip"))
                {
                    response.AppendHeader("Content-encoding", "gzip");
                    zipStream = new GZipStream(stream, CompressionMode.Compress);
                }
                if (zipStream == null)
                    return new byte[] { };
                zipStream.Write(bytes, 0, bytes.Length);
                zipStream.Close();
                zipStream.Dispose();

                return stream.ToArray();
            }
        }
    }
}