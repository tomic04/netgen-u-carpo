using System;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;

namespace Carpo.Common.Helpers
{
    public class Images
    {
        public static DevExpress.XtraEditors.ImageLocation GetDxImageLocationByString(string location)
        {
            DevExpress.XtraEditors.ImageLocation value;
            switch (location.ToLower())
            {
                case "topleft":
                    value = DevExpress.XtraEditors.ImageLocation.TopLeft;
                    break;

                case "topcenter":
                    value = DevExpress.XtraEditors.ImageLocation.TopCenter;
                    break;

                case "topright":
                    value = DevExpress.XtraEditors.ImageLocation.TopRight;
                    break;

                case "middleleft":
                    value = DevExpress.XtraEditors.ImageLocation.MiddleLeft;
                    break;

                case "middlecenter":
                    value = DevExpress.XtraEditors.ImageLocation.MiddleCenter;
                    break;

                case "middleright":
                    value = DevExpress.XtraEditors.ImageLocation.MiddleRight;
                    break;

                case "bottomleft":
                    value = DevExpress.XtraEditors.ImageLocation.BottomLeft;
                    break;

                case "bottomcenter":
                    value = DevExpress.XtraEditors.ImageLocation.BottomCenter;
                    break;

                case "bottomrightr":
                    value = DevExpress.XtraEditors.ImageLocation.BottomRight;
                    break;

                default:
                    value = DevExpress.XtraEditors.ImageLocation.Default;
                    break;
            }
            return value;
        }

        public static Icon GetFormIcon(string icon, string connStrReources)
        {
            string iconName = Path.GetFileNameWithoutExtension(icon);
            try
            {
                using (Bitmap ikonaBitmap = GetIcon(iconName, connStrReources))
                {
                    if (ikonaBitmap != null)
                    {
                        IntPtr pIcon = ikonaBitmap.GetHicon();
                        using (Icon i = Icon.FromHandle(pIcon))
                        {
                            return i;
                        }
                    }
                }
            }
            catch
            { }
            return Carpo.Common.Properties.Resources.spin;
        }

        private static Bitmap GetIcon(string iconStr, string connStrReources)
        {
            using (SqlConnection conn = new SqlConnection(connStrReources))
            using (SqlCommand cmd = new SqlCommand())
            {
                cmd.Connection = conn;
                cmd.CommandText = "SELECT Img FROM ImgIcons WHERE ImgName=@IconStr";
                cmd.Parameters.AddWithValue("@IconStr", iconStr);
                cmd.CommandType = CommandType.Text;

                if (conn.State != ConnectionState.Open) conn.Open();
                var result = cmd.ExecuteScalar();
                if (result == null)
                    return null;
                else
                {
                    return ByteArrayToImage((byte[])result);
                }
            }
        }

        public static Bitmap ByteArrayToImage(byte[] byteArrayIn)
        {
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                Image returnImage = Image.FromStream(ms);
                return new Bitmap(returnImage);
            }
        }

        //public static Image ByteArrayToImage(byte[] byteArray)
        //{
        //    using (MemoryStream ms = new MemoryStream(byteArray))
        //    {
        //        Image image = Image.FromStream(ms);
        //        return image;
        //    }
        //}

        public static byte[] ImageToByteArray(System.Drawing.Image image)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, System.Drawing.Imaging.ImageFormat.Jpeg);
                return ms.ToArray();
            }
        }

        public static Bitmap ResizeImage(Image image)
        {
            Size thumbSize = GetProportionalSize(new Size(100, 100), image.Size);

            Bitmap newImage = new Bitmap(thumbSize.Width, thumbSize.Height);

            using (Graphics gr = Graphics.FromImage(newImage))
            {
                gr.SmoothingMode = SmoothingMode.HighQuality;
                gr.InterpolationMode = InterpolationMode.HighQualityBicubic;
                gr.PixelOffsetMode = PixelOffsetMode.HighQuality;
                gr.DrawImage(image, new Rectangle(0, 0, thumbSize.Width, thumbSize.Height));
            }
            return newImage;
        }

        private static Size GetProportionalSize(Size szMax, Size szReal)
        {
            int nWidth;
            int nHeight;
            double sMaxRatio;
            double sRealRatio;

            if (szMax.Width < 1 || szMax.Height < 1 || szReal.Width < 1 || szReal.Height < 1)
                return Size.Empty;

            sMaxRatio = (double)szMax.Width / (double)szMax.Height;
            sRealRatio = (double)szReal.Width / (double)szReal.Height;

            if (sMaxRatio < sRealRatio)
            {
                nWidth = Math.Min(szMax.Width, szReal.Width);
                nHeight = (int)Math.Round(nWidth / sRealRatio);
            }
            else
            {
                nHeight = Math.Min(szMax.Height, szReal.Height);
                nWidth = (int)Math.Round(nHeight * sRealRatio);
            }

            return new Size(nWidth, nHeight);
        }
    }
}