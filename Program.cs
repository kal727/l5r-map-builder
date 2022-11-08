
using System.Drawing;

var baseFilePath = "../../../images/";
var baseUriPath = "http://talesofrokugan.com/map/6/";

var gridLength = 64;
var imageHeight = 256;
var imageWidth = 256;

var relevantTileXStart = 12;
var relevantTileXEnd = 51;

var relevantTileYStart = 2;
var relevantTileYEnd = 61;

for (var x = relevantTileXStart; x <= relevantTileXEnd; x++)
{
    for (var y = relevantTileYStart; y <= relevantTileYEnd; y++)
    {
        var imageFilename = $"{x}-{y}.png";
        var filePath = $"{baseFilePath}/image-tiles/{imageFilename}";

        if (File.Exists(filePath))
        {
            Console.WriteLine("File exists");
        }
        else
        {
            // file does not exis
            var uri = $"{baseUriPath}{x}/{y}.png";

            using var fileStream = File.OpenWrite(filePath);
            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.GetAsync(uri);
                var stream = await response.Content.ReadAsStreamAsync();
                await stream.CopyToAsync(fileStream);
                Console.WriteLine("Download Completed");
            }

            await fileStream.FlushAsync();
        }
    }
}

//Generate Completed Images
var completedImagePath = $"{baseFilePath}full-image.jpeg";
if (!File.Exists(completedImagePath))
{
    var completedImage = GenerateFinalImage();
    completedImage.Save(completedImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
}

Bitmap GenerateFinalImage()
{
    //Create a bitmap to hold the combined image
    var finalWidth = imageWidth * (relevantTileXEnd - relevantTileXStart + 1);
    var finalHeight = imageHeight * (relevantTileYEnd - relevantTileYStart + 1);
    var finalImage = new Bitmap(finalWidth, finalHeight);
    var images = new List<Bitmap>();

    try
    {

        //Get a graphics object from the image so we can draw on it
        using Graphics g = Graphics.FromImage(finalImage);
        //Set background color
        g.Clear(Color.Black);

        var horizontalOffset = 0;
        for (var x = relevantTileXStart; x <= relevantTileXEnd; x++)
        {
            var verticalOffset = 0;

            for (var y = relevantTileYStart; y <= relevantTileYEnd; y++)
            {
                var imagePath = $"{baseFilePath}image-tiles/{x}-{y}.png";
                using var image = new Bitmap(imagePath);
                images.Add(image);

                g.DrawImage(image, new Rectangle(horizontalOffset, verticalOffset, image.Width, image.Height));
                verticalOffset += imageHeight;
            }
            horizontalOffset += imageWidth;
        }

        return finalImage;
    }
    catch (Exception ex)
    {
        if (finalImage != null)
            finalImage.Dispose();

        throw ex;
    }
    finally
    {
        //clean up memory
        foreach (Bitmap image in images)
        {
            image.Dispose();
        }
    }
}


// //Generate Image Strips
// for (var x = relevantTileXStart; x <= relevantTileXEnd; x++)
// {
//     var imageStripFilename = $"image-strip-{x}.jpeg";
//     var imageStripFilePath = $"{baseFilePath}image-strips/{imageStripFilename}";

//     if (!File.Exists(imageStripFilePath))
//     {
//         var imageFilePathsInStrip = new List<string>();
//         for (var y = relevantTileYStart; y <= relevantTileYEnd; y++)
//         {
//             var imageTilePath = $"{baseFilePath}image-tiles/{x}-{y}.png";
//             imageFilePathsInStrip.Add(imageTilePath);
//         }
//         var imageStrip = CombineBitmap(imageFilePathsInStrip, true);
//         imageStrip.Save(imageStripFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
//     }
// }

// //Generated finished image with strip
// var completedImageFilePath = $"{baseFilePath}full-image-striped.jpeg";
// if (!File.Exists(completedImageFilePath))
// {
//     var imageStripFilePaths = new List<string>();
//     for (var x = relevantTileXStart; x <= relevantTileXEnd; x++)
//     {
//         imageStripFilePaths.Add($"{baseFilePath}image-strips/image-strip-{x}.jpeg");
//     }
//     var completedImage = CombineBitmap(imageStripFilePaths, false);
//     completedImage.Save(completedImageFilePath, System.Drawing.Imaging.ImageFormat.Jpeg);
// }

// Bitmap CombineBitmap(List<string> files, bool offsetByHeight)
// {
//     //read all images into memory
//     var images = new List<Bitmap>();
//     Bitmap finalImage = null;

//     try
//     {
//         int width = 0;
//         int height = 0;

//         foreach (string image in files)
//         {
//             //create a Bitmap from the file and add it to the list
//             Bitmap bitmap = new Bitmap(image);

//             //update the size of the final bitmap
//             if (offsetByHeight)
//             {
//                 width = bitmap.Width;
//                 height += bitmap.Height;
//             }
//             else
//             {
//                 width += bitmap.Width;
//                 height = bitmap.Height;
//             }

//             images.Add(bitmap);
//         }

//         //create a bitmap to hold the combined image
//         finalImage = new Bitmap(width, height);

//         //get a graphics object from the image so we can draw on it
//         using (Graphics g = Graphics.FromImage(finalImage))
//         {
//             //set background color
//             g.Clear(Color.Black);

//             //go through each image and draw it on the final image
//             int offset = 0;
//             foreach (Bitmap image in images)
//             {
//                 if (offsetByHeight)
//                 {
//                     g.DrawImage(image,
//                       new Rectangle(0, offset, image.Width, image.Height));
//                     offset += image.Height;
//                 }
//                 else
//                 {
//                     g.DrawImage(image, new Rectangle(offset, 0, image.Width, image.Height));
//                     offset += image.Width;
//                 }

//             }
//         }

//         return finalImage;
//     }
//     catch (Exception ex)
//     {
//         if (finalImage != null)
//             finalImage.Dispose();

//         throw ex;
//     }
//     finally
//     {
//         //clean up memory
//         foreach (Bitmap image in images)
//         {
//             image.Dispose();
//         }
//     }
// }