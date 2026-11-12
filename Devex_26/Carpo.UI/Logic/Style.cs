using DevExpress.Utils;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Carpo.UI.Logic
{
    internal class Style
    {
        public enum StyleType { Undefined, FontColor, BackgroundColor, Bold, Italic, Strikeout, Regular, Underline };

        public static void SetStyle(AppearanceObject appearanceObject, string styleTypeString, string styleValue)
        {
            try
            {
                string[] styleTypeStr = styleTypeString.Split(',');

                if (styleTypeStr.Length > 0)
                {
                    List<StyleType> styleTypeList = new List<StyleType>();

                    foreach (string str in styleTypeStr)
                    {
                        StyleType styleType;
                        if (Enum.TryParse<StyleType>(str, true, out styleType))
                        {
                            styleTypeList.Add(styleType);
                        }
                    }
                    SetStyle(appearanceObject, styleTypeList, styleValue);
                }
            }
            catch
            {
            }
        }

        public static void SetStyle(AppearanceObject appearanceObject, List<StyleType> styleType, string styleValue)
        {
            if (styleType.Contains(StyleType.FontColor))
            {
                Color? color = GetColor(styleValue);
                if (color != null)
                    appearanceObject.ForeColor = (Color)color;
            }
            else if (styleType.Contains(StyleType.BackgroundColor))
            {
                Color? color = GetColor(styleValue);
                if (color != null)
                    appearanceObject.BackColor = (Color)color;
            }
            else if (styleType.Contains(StyleType.Bold) &&
                styleType.Contains(StyleType.Italic) &&
                styleType.Contains(StyleType.Strikeout))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Bold | FontStyle.Italic | FontStyle.Strikeout);
            else if (styleType.Contains(StyleType.Bold) &&
                styleType.Contains(StyleType.Italic))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Bold | FontStyle.Italic);
            else if (styleType.Contains(StyleType.Bold) &&
                styleType.Contains(StyleType.Strikeout))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Bold | FontStyle.Strikeout);
            else if (styleType.Contains(StyleType.Italic) &&
                styleType.Contains(StyleType.Strikeout))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Italic | FontStyle.Strikeout);
            else if (styleType.Contains(StyleType.Bold))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Bold);
            else if (styleType.Contains(StyleType.Italic))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Italic);
            else if (styleType.Contains(StyleType.Strikeout))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Strikeout);
            else if (styleType.Contains(StyleType.Regular))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Regular);
            else if (styleType.Contains(StyleType.Underline))
                appearanceObject.Font = new Font(appearanceObject.Font, FontStyle.Underline);
            
            //if (appearanceObject.Font.Bold)//moj test ostaviti
            //{
            //    //    appearanceObject.Font = new Font(appearanceObject.Font.FontFamily, appearanceObject.Font.Size - 1);
            //    Console.WriteLine("B: "+appearanceObject.Font.Size);
            //}
            //else
            //{
            //    Console.WriteLine(appearanceObject.Font.Size);
            //}

        }

        public static void ParseNetGenStyles(AppearanceObject appearanceObject, string value, string valueType)
        {
            value = value.ToLower();
            List<StyleType> listFormat = new List<StyleType>();
            string[] values = value.Replace(" ", "").Split(',');

            bool colorSet = false;

            foreach (string str in values)
            {
                if (str.Contains("bold"))
                    listFormat.Add(StyleType.Bold);
                else if (str.Contains("italic"))
                    listFormat.Add(StyleType.Italic);
                else if (str.Contains("strikeout"))
                    listFormat.Add(StyleType.Strikeout);
                else if (str.Contains("underline"))
                    listFormat.Add(StyleType.Underline);
                else if (colorSet == false)
                {
                    //provjeri radi li se o boji
                    Color? color = GetColor(str);
                    if (color != null)
                    {
                        colorSet = true;
                        SetStyle(appearanceObject, valueType, str);
                    }
                }
            }
            if (listFormat.Count > 0)
            {
                SetStyle(appearanceObject, listFormat, "");
            }
            listFormat = null;
        }

        public static Color? GetColor(string colorName)
        {
            try
            {
                if (!string.IsNullOrEmpty(colorName))
                    return Common.Helpers.Colors.HtmlToColor(colorName, Color.Black);
            }
            catch { }

            return null;
        }

        private static bool GetBoolByString(string styleValue)
        {
            if (styleValue.ToLower() == "true")
                return true;
            else
                return false;
        }
    }
}