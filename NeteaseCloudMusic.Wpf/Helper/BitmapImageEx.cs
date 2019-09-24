using System.IO;
using System.Windows.Media.Imaging;

namespace NeteaseCloudMusic.Wpf.Helper
{
    public static class BitmapImageEx
    {
        public static byte[] ToBytes(this BitmapImage imageSource)
        {
            var encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(imageSource));

            using (var ms = new MemoryStream())
            {
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
    }
}
