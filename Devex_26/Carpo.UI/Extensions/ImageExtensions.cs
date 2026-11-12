using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.UI.Extensions
{
    internal static class ImageExtensions
    {
        public static byte[] GenerateHash(this Image image)
        {
            if (image == null) return new byte[0];
            using var ms = new MemoryStream();
            image.Save(ms, ImageFormat.Png);
            var array = ms.ToArray();

            using var sha = SHA256.Create();
            return sha.ComputeHash(array);
        }
    }
}
