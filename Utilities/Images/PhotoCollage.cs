using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace JukeWeb.Foundry.Utilities
{
    public static class PhotoCollage
    {
        public static Image GenerateCollage(List<Image> images, int size)
        {
            Image returnImg;
            int numberOfImages = images.Count();
            if (numberOfImages == 1)
            {
                returnImg = images[0];
                returnImg = ImageUtils.ResizeImageWithAspectRatio(returnImg, new Size(size, size) , true);
                returnImg = ImageUtils.CropImage(returnImg, size, size, 0, 0);
                return returnImg;
            }
            else if (numberOfImages == 2)
            {
                List<Image> imageMerge = new List<Image>();
                var firstImage = ImageUtils.ResizeImageWithAspectRatio(images[0], new Size(size, size), false);
                firstImage = ImageUtils.CropImage(firstImage, size / 2, size, size / 4, 0);
                imageMerge.Add(firstImage);
                var secondImage = ImageUtils.ResizeImageWithAspectRatio(images[1], new Size(size, size), false);
                secondImage = ImageUtils.CropImage(secondImage, size / 2, size, size / 4, 0);
                imageMerge.Add(secondImage);
                returnImg = ImageUtils.MergeImages(ImageMergeDirection.LeftToRight, imageMerge.ToArray());
                return returnImg;
            }
            else if (numberOfImages > 2)
            {
                List<Image> imageMerge = new List<Image>();
                var firstImage = ImageUtils.ResizeImageWithAspectRatio(images[0], new Size(size, size), false);
                firstImage = ImageUtils.CropImage(firstImage, size / 2, size, size / 4, 0);
                
                var secondImage = ImageUtils.ResizeImageWithAspectRatio(images[1], new Size(size/2, size/2), false);
                secondImage = ImageUtils.CropImage(secondImage, size / 2, size / 2, 0, 0);
                imageMerge.Add(secondImage);

                var thirdImage = ImageUtils.ResizeImageWithAspectRatio(images[2], new Size(size/2, size/2), false);
                thirdImage = ImageUtils.CropImage(thirdImage, size / 2, size / 2, 0, 0);
                imageMerge.Add(thirdImage);

                var imageRight = ImageUtils.MergeImages(ImageMergeDirection.TopToBottom, imageMerge.ToArray());
                
                imageMerge = new List<Image>();
                imageMerge.Add(firstImage);
                imageMerge.Add(imageRight);

                returnImg = ImageUtils.MergeImages(ImageMergeDirection.LeftToRight, imageMerge.ToArray());
                return returnImg;
            }

            return null;
        }
        
        public static Image GenerateCollageFull(List<Image> images, int? frameWidth, int? frameHeight)
        {
            Image returnImg;
            List<List<Image>> imagesByRows = new List<List<Image>>();
            List<Image> imageRows = new List<Image>();

            if (images == null)
                return null;

            var rowSize = 1;
            rowSize = (int)Math.Ceiling(Math.Sqrt((float)images.Count()));

            imagesByRows = images.SplitSubList<Image>(rowSize);
            foreach (var imagesByRow in imagesByRows)
            {
                var imageLine = ImageUtils.MergeImages(ImageMergeDirection.LeftToRight, imagesByRow.ToArray());
                imageRows.Add(imageLine);
            }

            returnImg = ImageUtils.MergeImages(ImageMergeDirection.TopToBottom, imageRows.ToArray());

            return returnImg;
        }
    }
}
