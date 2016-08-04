using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using nQuant;

namespace JukeWeb.Foundry.Utilities.Utilities.Images
{
    public class ImageCompress
    {
        public static byte[] CompressJPEGImage(Image img)
        {
            // for the Quality parameter category.
            System.Drawing.Imaging.Encoder myEncoder = System.Drawing.Imaging.Encoder.Quality;
            ImageCodecInfo jgpEncoder = GetEncoder(ImageFormat.Jpeg);

            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, 75L);
            myEncoderParameters.Param[0] = myEncoderParameter;
            try
            {
                MemoryStream ms = new MemoryStream();
                img.Save(ms, jgpEncoder, myEncoderParameters); 
                return ms.ToArray();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static byte[] CompressPNGImage(string sourcePath,int alphaTransparency, int alphaFader)
        {
            var quantizer = new WuQuantizer();
            using (var bitmap = new Bitmap(sourcePath))
            {
                try
                {
                    using (var quantized = quantizer.QuantizeImage(bitmap, alphaTransparency, alphaFader))
                    {
                        MemoryStream ms = new MemoryStream();
                        quantized.Save(ms, ImageFormat.Png);
                        return ms.ToArray();
                    }
                }
                catch (QuantizationException q)
                {
                    return null;
                }
            }
        }

        private static ImageCodecInfo GetEncoder(ImageFormat imageFormat)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].FormatID == imageFormat.Guid)
                    return encoders[j];
            }
            return null;
        }
    }
}
