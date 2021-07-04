namespace SignalStrength.Wpf.Extensions
{
    using System.Drawing;
    using System.IO;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;

    public static class ImageExtension
    {
        public static ImageSource ConvertBitmapToImageSource(this Bitmap bitmap)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, System.Drawing.Imaging.ImageFormat.Bmp);
                stream.Position = 0;
                var bmpImg = new BitmapImage();
                bmpImg.BeginInit();
                bmpImg.StreamSource = stream;
                bmpImg.CacheOption = BitmapCacheOption.OnLoad;
                bmpImg.EndInit();
                return bmpImg;
            }
        }
    }
}