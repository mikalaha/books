using System;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Books.Helpers
{
    public static class ImageHelper
    {
        static byte[] ReadResourceAsBytes(string name)
        {
            var assembly = typeof(ImageHelper).Assembly;
            using var stream = assembly.GetManifestResourceStream($"Books.Res.{name}");
            if (stream != null)
            {
                byte[] buffer = new byte[16 * 1024];
                using var ms = new MemoryStream();
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
            return Array.Empty<byte>();
        }
        public static byte[] GetRandomBookImage()
        {
            int no = Random.Shared.Next(2);
            return ReadResourceAsBytes($"book{no}.jpg");
        }
        public static ImageSource? ToImageSource(this byte[]? data)
        {
            if (data != null)
            {
                try
                {
                    MemoryStream byteStream = new(data);
                    BitmapImage image = new();
                    image.BeginInit();
                    image.StreamSource = byteStream;
                    image.EndInit();
                    return image;
                }
                catch
                {
                }
            }
            return null;
        }
    }
}