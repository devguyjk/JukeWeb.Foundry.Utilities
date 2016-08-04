using System;
using System.Drawing;
using System.IO;
using System.Collections;
using System.Net;
using System.Windows.Forms;

namespace JukeWeb.Foundry.Utilities
{
    public class ImageConverter
    {
        public ImageConverter()
        {
        }

        public static byte[] imageToByteArray(System.Drawing.Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image byteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static byte[] GetThumbnailFromURL(string fileURL, int width, int height)
        {
            using (WebClient wc = new WebClient())
            {
                Stream strm = null;

                try
                {
                    PictureBox imageControl = new PictureBox();
                    imageControl.Height = width;
                    imageControl.Width = height;

                    strm = wc.OpenRead(fileURL);
                    var img = Image.FromStream(strm);

                    Image.GetThumbnailImageAbort myCallback =
                            new Image.GetThumbnailImageAbort(ThumbnailCallback);

                    System.Drawing.Image myThumbnail = img.GetThumbnailImage(width, height, myCallback, IntPtr.Zero);
                    return imageToByteArray(myThumbnail);
                }
                catch
                {
                    return null;
                }
                finally
                {
                    if (strm != null)
                        strm.Close();
                }

            }
        }

        public static bool ThumbnailCallback()
        {
            return false;
        }

        public static void Main()
        {

        }

    }

}