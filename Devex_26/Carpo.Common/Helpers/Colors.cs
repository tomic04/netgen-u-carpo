using System;
using System.Drawing;

namespace Carpo.Common.Helpers
{
    public class Colors
    {
        public static Color ChangeColorBrightness(Color color, float correctionFactor)
        {
            float red = (float)color.R;
            float green = (float)color.G;
            float blue = (float)color.B;

            if (correctionFactor < 0)
            {
                correctionFactor = 1 + correctionFactor;
                red *= correctionFactor;
                green *= correctionFactor;
                blue *= correctionFactor;
            }
            else
            {
                red = (255 - red) * correctionFactor + red;
                green = (255 - green) * correctionFactor + green;
                blue = (255 - blue) * correctionFactor + blue;
            }

            return Color.FromArgb(color.A, (int)red, (int)green, (int)blue);
        }

        public static Color RgbStringToColor(string rgbStringColor, Color defaulColor)
        {
            string[] splitArray = rgbStringColor.Split(',');
            if (splitArray.Length == 3)
            {
                foreach (string s in splitArray)
                {
                    if (Conversion.Parse(typeof(int), s) == null)
                        return defaulColor;
                }
                return Color.FromArgb(Convert.ToInt32(splitArray[0]), Convert.ToInt32(splitArray[1]), Convert.ToInt32(splitArray[2]));
            }
            return defaulColor;
        }

        public static Color HtmlToColor(string rgbStringColor, Color defaulColor)
        {
            try
            {
                Color col = ColorTranslator.FromHtml(rgbStringColor);
                return col;
            }
            catch
            {
                return defaulColor;
            }
        }
    }
}