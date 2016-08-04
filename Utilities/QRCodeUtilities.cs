using System;
using System.Linq;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using Gma.QrCodeNet.Encoding;
using Gma.QrCodeNet.Encoding.Windows.Render;

namespace JukeWeb.Foundry.Utilities
{
    public class QRCodeUtilities
    {
        public const int DEFAULT_SIZE = 5;
        public static Brush DEFAULT_DARK_COLOR = Brushes.Black;
        public static Brush DEFAULT_LIGHT_COLOR = Brushes.White;
        public const QuietZoneModules DEFAULT_QUIET_ZONES = QuietZoneModules.Two;
        public static ImageFormat DEFAULT_IMAGEFORMAT = ImageFormat.Jpeg;

        #region Get QR Barcode Bytes

        public static byte[] GetQRCodeImageBytes(int size, Brush lightColor, Brush darkColor, QuietZoneModules quietZones, string value, ImageFormat imageType)
        {
            try
            {
                QrEncoder qrEncoder = new QrEncoder(ErrorCorrectionLevel.H);
                QrCode qrCode = qrEncoder.Encode(value);
                GraphicsRenderer renderer = new GraphicsRenderer(new FixedModuleSize(size, quietZones), darkColor, lightColor);
                using (MemoryStream stream = new MemoryStream())
                {
                    renderer.WriteToStream(qrCode.Matrix, imageType, stream);
                    return stream.ToArray();
                }
            }
            catch (Exception ex)
            {
               throw new Exception(string.Format("Error in foundry QRCodeUtilities, GetQRCodeImageBytes. Message: {0}", ex.Message), ex);
            }
        }

        public static byte[] GetQRCodeImageBytes(string value)
        {
            return GetQRCodeImageBytes(DEFAULT_SIZE, DEFAULT_LIGHT_COLOR, DEFAULT_DARK_COLOR, DEFAULT_QUIET_ZONES, value, DEFAULT_IMAGEFORMAT);
        }

        public static byte[] GetQRCodeImageBytes(string value, ImageFormat format)
        {
            return GetQRCodeImageBytes(DEFAULT_SIZE, DEFAULT_LIGHT_COLOR, DEFAULT_DARK_COLOR, DEFAULT_QUIET_ZONES, value, format);
        }

        public static byte[] GetQRCodeImageBytes(string value, ImageFormat format, int size)
        {
            return GetQRCodeImageBytes(size, DEFAULT_LIGHT_COLOR, DEFAULT_DARK_COLOR, DEFAULT_QUIET_ZONES, value, format);
        }

        #endregion

        #region Get QR Barcode Image

        public static Image GetQRCodeImage(int size, Brush lightColor, Brush darkColor, QuietZoneModules quietZones, string value, ImageFormat imageType)
        {
            byte[] imageBytes = GetQRCodeImageBytes(size, lightColor, darkColor, quietZones, value, imageType);
            return ImageUtils.ByteArrayToImage(imageBytes);
        }

        public static Image GetQRCodeImage(string value)
        {
            return ImageUtils.ByteArrayToImage(GetQRCodeImageBytes(value));
        }

        public static Image GetQRCodeImage(string value, ImageFormat format, int size)
        {
            return ImageUtils.ByteArrayToImage(GetQRCodeImageBytes(value, format, size));
        }

        #endregion
    }
}
