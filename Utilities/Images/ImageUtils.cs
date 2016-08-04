using System;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Net;

namespace JukeWeb.Foundry.Utilities
{
    public class ImageUtils
    {
        public static Image AddOverlayOverImage(Image image, Color color)
        {
            Bitmap bitmap = new Bitmap(image.Width, image.Height);
            int i, j;

            for (i = 0; i < image.Width; i++)
                for (j = 0; j < image.Height; j++)
                    bitmap.SetPixel(i, j, color);
            using (Graphics g = Graphics.FromImage(image))
            {
                g.CompositingMode = CompositingMode.SourceOver;
                g.DrawImage(bitmap, new Point(0, 0));
            }
            bitmap.Dispose();

            return image;
        }

        public static Bitmap AdjustColors(Image image, int red, int green, int blue)
        {
            red = Math.Abs(red) % 256;
            green = Math.Abs(green) % 256;
            blue = Math.Abs(blue) % 256;

            Bitmap bmp = new Bitmap(image);
            for (int i = 0; i < bmp.Width; i++)
            {
                for (int j = 0; j < bmp.Height; j++)
                {
                    Color color = bmp.GetPixel(i, j);
                    if (color.A != 0)
                        bmp.SetPixel(i, j, Color.FromArgb(red, green, blue));
                }
            }

            return bmp;
        }

        public static bool ConvertImageType(Image imageIn, ImageFormat changeImageToFormat, out Image imageOut)
        {
            try
            {
                byte[] imageInBytes = ImageToByteArray(imageIn);
                byte[] imageOutBytes = null;

                ConvertImageType(imageInBytes, changeImageToFormat, out imageOutBytes);
                imageOut = ByteArrayToImage(imageOutBytes);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error calling ConvertImageType: {0}", ex.Message), ex);
            }
        }

        public static bool ConvertImageType(byte[] imageInBytes, ImageFormat changeImageToFormat, out byte[] imageOutBytes)
        {
            // has something been sent or not...
            imageOutBytes = null;

            if (imageInBytes.Length == 0)
                return false;
            try
            {
                using (Stream inImageMemStream = new MemoryStream(imageInBytes))
                {
                    using (Image imgInFile = Image.FromStream(inImageMemStream))
                    {
                        using (MemoryStream outImageMemStream = new MemoryStream())
                        {
                            imgInFile.Save(outImageMemStream, changeImageToFormat);
                            imageOutBytes = outImageMemStream.GetBuffer();
                            return true;
                        }
                    }
                }

            }
            catch (Exception ex)
            {
                throw new Exception(string.Format("Error executing ConvertImage: message:{0}", ex.Message), ex);
            }
        }

        public static byte[] GetByteArrFromFilePath(string filePath)
        {
            return ImageToByteArray(Image.FromFile(filePath));
        }

        public static Image GetImageFromFilePath(string filePath)
        {
            return Image.FromFile(filePath);
        }
        /// <summary>
        /// Return by array from an image, Default format is JPG
        /// </summary>
        /// <param name="imageIn"></param>
        /// <returns></returns>
        public static byte[] ImageToByteArray(System.Drawing.Image imageIn)
        {
            return ImageToByteArray(imageIn, ImageFormat.Jpeg);
        }


        public static byte[] ImageToByteArray(System.Drawing.Image imageIn, ImageFormat format)
        {
            byte[] retVal = null;
            if (imageIn != null)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    imageIn.Save(ms, format);
                    retVal = ms.ToArray();
                    imageIn.Dispose();
                }
            }
            return retVal;
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static Image ResizeImage(byte[] imageBytes, int? maxHeight, int newWidth)
        {
            Image fullSizeImage = ByteArrayToImage(imageBytes);
            return ResizeImage(fullSizeImage, maxHeight, newWidth, true);
        }

        public static Image ResizeImage(Image fullSizeImage, int? maxHeight, int newWidth)
        {
            return ResizeImage(fullSizeImage, maxHeight, newWidth, true);
        }

        public static Image ResizeImage(Image fullSizeImage, int? maxHeight, int newWidth, bool onlyResizeIfWider)
        {
            if (fullSizeImage.Width <= newWidth && onlyResizeIfWider)
                newWidth = fullSizeImage.Width;

            int newHeight = (int)Math.Ceiling(fullSizeImage.Height * newWidth / (double)fullSizeImage.Width);

            if (maxHeight != null && newHeight > maxHeight)
            {
                // Resize with height instead
                newWidth = (int)Math.Ceiling(fullSizeImage.Width * (int)maxHeight / (double)fullSizeImage.Height);
                newHeight = (int)maxHeight;
            }

            return fullSizeImage.GetThumbnailImage(newWidth, newHeight, null, IntPtr.Zero);
        }

        public static Image ResizeImageWithNoAspectRatio(Image fullSizeImage, int height, int width)
        {
            return fullSizeImage.GetThumbnailImage(width, height, null, IntPtr.Zero);
        }

        public void ResizeImage(string originalFile, string newFile, int newWidth, int maxHeight, bool onlyResizeIfWider)
        {
            System.Drawing.Image fullsizeImage = System.Drawing.Image.FromFile(originalFile);

            // Prevent using images internal thumbnail
            fullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);
            fullsizeImage.RotateFlip(System.Drawing.RotateFlipType.Rotate180FlipNone);

            if (onlyResizeIfWider)
            {
                if (fullsizeImage.Width <= newWidth)
                {
                    newWidth = fullsizeImage.Width;
                }
            }

            int NewHeight = fullsizeImage.Height * newWidth / fullsizeImage.Width;
            if (NewHeight > maxHeight)
            {
                // Resize with height instead
                newWidth = fullsizeImage.Width * maxHeight / fullsizeImage.Height;
                NewHeight = maxHeight;
            }

            System.Drawing.Image newImage = fullsizeImage.GetThumbnailImage(newWidth, NewHeight, null, IntPtr.Zero);

            // Clear handle to original file so that we can overwrite it if necessary
            fullsizeImage.Dispose();

            // Save resized picture
            newImage.Save(newFile);
        }

        public static Point GetCenterOfImage(Image image, int overlayHeight, int overlayWidth)
        {
            Point point = new Point();
            point.X = image.Width / 2 - overlayWidth / 2;
            point.Y = image.Height / 2 - overlayHeight / 2;

            return point;
        }

        public static Image SuperImposeImages(string canvasImagePath, string overlayPath, ImageFormat format)
        {
            return SuperImposeImages(new Bitmap(canvasImagePath), new Bitmap(overlayPath), format);
        }

        public static Image SuperImposeImages(Image canvasBase, Image overlay, ImageFormat format)
        {
            return SuperImposeImages(canvasBase, overlay, format, 0, 0);
        }

        public static Image SuperImposeImages(Image canvasBase, Image overlay, ImageFormat format, int x, int y)
        {
            using (Stream returnStream = new MemoryStream())
            {
                using (canvasBase)
                {
                    using (overlay)
                    {
                        using (var canvas = Graphics.FromImage(overlay))
                        {
                            canvas.InterpolationMode = InterpolationMode.HighQualityBicubic;
                            canvas.CompositingMode = CompositingMode.SourceOver;
                            canvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                            canvas.DrawImage(canvasBase, new Rectangle(x, y, canvasBase.Width, canvasBase.Height), new Rectangle(0, 0, canvasBase.Width, canvasBase.Height), GraphicsUnit.Pixel);
                            canvas.Save();
                        }
                        overlay.Save(returnStream, format);
                        return Image.FromStream(returnStream);
                    }
                }
            }
        }

        public static Image NewImage(int width, int height, Color color)
        {
            Bitmap bitmap = new Bitmap(width, height);
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    bitmap.SetPixel(i, j, color);

            return bitmap;
        }

        public static Image DrawTextImage(string text, Color fontColor, int fontSize, FontStyle fontStyle = FontStyle.Bold)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);

            Font objFont = new Font("Arial", fontSize, fontStyle, System.Drawing.GraphicsUnit.Pixel);
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            int intWidth = (int)objGraphics.MeasureString(text, objFont).Width;
            int intHeight = (int)objGraphics.MeasureString(text, objFont).Height;

            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));

            objGraphics = Graphics.FromImage(objBmpImage);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.DrawString(text, objFont, new SolidBrush(fontColor), 0, 0);
            objGraphics.Flush();

            return objBmpImage;
        }

        public static Image DrawTextImage(string text, Color fontColor, int fontSize, int width, FontStyle fontStyle = FontStyle.Bold)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);

            Font objFont = new Font("Arial", fontSize, fontStyle, System.Drawing.GraphicsUnit.Pixel);
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            bool trimmed = false;
            int intWidth = 0;
            do
            {
                intWidth = (int)objGraphics.MeasureString(text, objFont).Width;
                if (intWidth > width)
                {
                    text = text.Remove(text.Length - 1);
                    trimmed = true;
                }
            } while (intWidth > width);
            if (trimmed)
                text = text.Remove(text.Length - 3) + "...";
            int intHeight = (int)objGraphics.MeasureString(text, objFont).Height;

            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));
            StringFormat format = new StringFormat(StringFormat.GenericTypographic);
            format.FormatFlags = StringFormatFlags.MeasureTrailingSpaces;
            format.Trimming = StringTrimming.EllipsisWord;

            objGraphics = Graphics.FromImage(objBmpImage);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;
            objGraphics.DrawString(text, objFont, new SolidBrush(fontColor), 0, 0, format);
            objGraphics.Flush();

            return objBmpImage;
        }

        public static Image DrawGlowingTextImage(string text, Color fontColor, int fontSize, Color glowColor, int glowAmount, FontStyle fontStyle = FontStyle.Bold)
        {
            Bitmap objBmpImage = new Bitmap(1, 1);

            Font objFont = new Font("Arial", fontSize, fontStyle, System.Drawing.GraphicsUnit.Pixel);
            Graphics objGraphics = Graphics.FromImage(objBmpImage);

            int intWidth = (int)objGraphics.MeasureString(text, objFont).Width + glowAmount * 2;
            int intHeight = (int)objGraphics.MeasureString(text, objFont).Height + glowAmount * 2;

            objBmpImage = new Bitmap(objBmpImage, new Size(intWidth, intHeight));

            objGraphics = Graphics.FromImage(objBmpImage);
            objGraphics.SmoothingMode = SmoothingMode.AntiAlias;
            objGraphics.TextRenderingHint = TextRenderingHint.AntiAlias;

            for (int x = 0; x <= glowAmount; x++)
                for (int y = 0; y <= glowAmount; y++)
                    objGraphics.DrawString(text, objFont, new SolidBrush(glowColor), new PointF(x, y));
            objGraphics.DrawString(text, objFont, new SolidBrush(fontColor), glowAmount / 2, glowAmount / 2);
            
            objGraphics.Flush();

            return objBmpImage;
        }

        public static Image MergeImages(ImageMergeDirection direction, params Image[] images)
        {
            int width = 0, height = 0;

            foreach (Image image in images)
            {
                if (direction == ImageMergeDirection.TopToBottom)
                    width = (image.Width > width) ? image.Width : width;
                else
                    width += image.Width;
                if (direction == ImageMergeDirection.LeftToRight)
                    height = (image.Height > height) ? image.Height : height;
                else
                    height += image.Height;
            }

            Bitmap bitmap = new Bitmap(width, height);
            Graphics g = Graphics.FromImage(bitmap);

            int offset = 0;
            foreach (Image image in images)
            {
                if (direction == ImageMergeDirection.LeftToRight)
                {
                    g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
                    offset += image.Width;
                }
                else
                {
                    g.DrawImage(image, new Rectangle(0, offset, image.Width, image.Height));
                    offset += image.Height;
                }
            }

            return bitmap;
        }

        public static Image DrawBorder(Image original, Color borderColor, int borderWidth)
        {
            Bitmap bitmap = new Bitmap(original.Width + borderWidth * 2, original.Height + borderWidth * 2);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.Clear(borderColor);
                g.DrawImage(original, new Point(borderWidth, borderWidth));
            }

            return bitmap;
        }

        public static byte[] DrawBorderToByteArray(Image original, Color borderColor, int borderWidth)
        {
            var img = DrawBorder(original, borderColor, borderWidth);
            return ImageToByteArray(img);
        }

        public static byte[] FeatherImage(byte[] imgByte, int feather)
        {
            var img = ByteArrayToImage(imgByte);

            return null;
        }

        public static byte[] DrawBorderFromFile(string path, string borderColor, int borderWidth)
        {
            Color color = new Color();
            color = (Color)System.Drawing.ColorTranslator.FromHtml(borderColor);
            Image OriginalImage = Image.FromFile(path);
            return DrawBorderToByteArray(OriginalImage, color, borderWidth);
        }

        public static byte[] DrawBorderFromURL(string imgURL, string borderColor, int borderWidth)
        {
            Color color = new Color();
            color = (Color)System.Drawing.ColorTranslator.FromHtml(borderColor);
            Image OriginalImage = GetImageFromURL(imgURL);
            return DrawBorderToByteArray(OriginalImage, color, borderWidth);
        }

        public static byte[] CropFromUrl(string imgURL, int Width, int Height, int X, int Y, string resizeSize)
        {
            try
            {
                Image OriginalImage = GetImageFromURL(imgURL);
                if (!String.IsNullOrEmpty(resizeSize))
                {
                    string[] resizeRatio = resizeSize.Split(',');
                    OriginalImage = ResizeImageWithAspectRatio(OriginalImage, new Size(Convert.ToInt32(resizeRatio[0]), Convert.ToInt32(resizeRatio[1])), true);
                }
                return CropImageByteArray(OriginalImage, Width, Height, X, Y);
            }
            catch (Exception Ex)
            {
                throw (Ex);
            }
        }

        public static byte[] CropImageByteArray(Image image, int width, int height, int X, int Y, bool removeTransparency = false)
        {
            using (image)
            {
                using (Bitmap bmp = new Bitmap(width, height))
                {
                    bmp.SetResolution(image.HorizontalResolution, image.VerticalResolution);
                    using (Graphics graphic = Graphics.FromImage(bmp))
                    {
                        if (removeTransparency)
                            graphic.Clear(Color.White);
                        graphic.SmoothingMode = SmoothingMode.AntiAlias;
                        graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
                        graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
                        graphic.DrawImage(image, new Rectangle(0, 0, width, height), X, Y, width, height, GraphicsUnit.Pixel);
                        MemoryStream ms = new MemoryStream();
                        bmp.Save(ms, ImageFormat.Bmp);
                        return ms.GetBuffer();
                    }
                }
            }
        }

        public static Image CropImage(Image image, int width, int height, int X, int Y, bool removeTransparency = false)
        {
            var byteArr = CropImageByteArray(image, width, height, X, Y, removeTransparency);
            return ByteArrayToImage(byteArr);
        }

        public static byte[] ImgByteArrayFromFile(string path)
        {
            return ImageToByteArray(Image.FromFile(path));
        }

        public static void SaveImgByteArrayToFile(byte[] byteArr, string path)
        {
            var image = ByteArrayToImage(byteArr);
            image.Save(path);
        }

        public static byte[] GetImageByteFromURL(string url)
        {
            Image image = GetImageFromURL(url);
            if (image != null)
                return ImageToByteArray(image);
            return null;
        }

        public static Image GetImageFromURL(string url)
        {
            WebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "GET";
            WebResponse response = request.GetResponse();
            byte[] content = response.GetResponseStream().ToByteArray();
            response.Close();

            return ByteArrayToImage(content);
        }

        public static Image ResizeImageWithAspectRatio(Image image, Size size, bool preserveAspectRatio = true)
        {
            int newWidth;
            int newHeight;
            if (preserveAspectRatio)
            {
                int originalWidth = image.Width;
                int originalHeight = image.Height;
                float percentWidth = (float)size.Width / (float)originalWidth;
                float percentHeight = (float)size.Height / (float)originalHeight;
                float percent = percentHeight < percentWidth ? percentHeight : percentWidth;
                newWidth = (int)(originalWidth * percent);
                newHeight = (int)(originalHeight * percent);
            }
            else
            {
                newWidth = size.Width;
                newHeight = size.Height;
            }
            Image newImage = new Bitmap(newWidth, newHeight);
            using (Graphics graphicsHandle = Graphics.FromImage(newImage))
            {
                graphicsHandle.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphicsHandle.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }
    }
}
