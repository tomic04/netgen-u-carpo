using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Carpo.UI.Helpers
{
    public struct TextFormat
    {
        public bool Bold;
        public bool Italic;
        public bool Underline;

        public TextFormat(bool bold, bool italic, bool underline)
        {
            Bold = bold;
            Italic = italic;
            Underline = underline;
        }

        public static readonly TextFormat None = new TextFormat(false, false, false);

        public string OpenTags() => (Bold ? "<b>" : "") + (Italic ? "<i>" : "") + (Underline ? "<u>" : "");
        public string CloseTags() => (Underline ? "</u>" : "") + (Italic ? "</i>" : "") + (Bold ? "</b>" : "");
    }
    public static class RichToPoorConverter
    {
        public static readonly Font EditorFont = new Font("Tahoma", 8.25f);
        public static void ApplyFont(Document doc, Font font)
        {
            doc.DefaultCharacterProperties.FontName = font.Name;
            doc.DefaultCharacterProperties.FontSize = font.SizeInPoints;

            CharacterProperties cp = doc.BeginUpdateCharacters(doc.Range);
            try
            {
                cp.FontName = font.Name;
                cp.FontSize = font.SizeInPoints;
            }
            finally
            {
                doc.EndUpdateCharacters(cp);
            }
        }
        public static string ToRtf(string html)
        {
            using (var server = new RichEditDocumentServer())
            {
                server.HtmlText = html ?? string.Empty;
                ApplyFont(server.Document, EditorFont);
                return server.RtfText;
            }
        }
        public static string FromRtf(string rtf)
        {
            if (string.IsNullOrEmpty(rtf)) return string.Empty;

            using (var server = new RichEditDocumentServer())
            {
                server.RtfText = rtf;
                return ToSimpleHtml(server.Document);
            }
        }
        public static string ToSimpleHtml(Document doc)
        {
            var paragraphs = new List<string>();

            foreach (Paragraph paragraph in doc.Paragraphs)
                paragraphs.Add(ParagraphToHtml(doc, paragraph));

            return string.Join("<br>", paragraphs);
        }
        private static string ParagraphToHtml(Document doc, Paragraph paragraph)
        {
            var sb = new StringBuilder();
            var current = TextFormat.None;

            int start = paragraph.Range.Start.ToInt();
            int end = paragraph.Range.End.ToInt();

            for (int pos = start; pos < end; pos++)
            {
                string ch = GetChar(doc, pos);
                if (ch == "\r" || ch == "\n") continue;

                TextFormat format = GetFormat(doc, pos);
                if (!format.Equals(current))
                {
                    sb.Append(current.CloseTags());
                    sb.Append(format.OpenTags());
                    current = format;
                }

                sb.Append(WebUtility.HtmlEncode(ch));
            }

            sb.Append(current.CloseTags());
            return sb.ToString();
        }

        private static string GetChar(Document doc, int pos)
        {
            return doc.GetText(doc.CreateRange(pos, 1));
        }

        private static TextFormat GetFormat(Document doc, int pos)
        {
            CharacterProperties cp = doc.BeginUpdateCharacters(doc.CreateRange(pos, 1));
            try
            {
                return new TextFormat(cp.Bold == true, cp.Italic == true, cp.Underline.HasValue && cp.Underline != UnderlineType.None);
            }
            finally
            {
                doc.EndUpdateCharacters(cp);
            }
        }
    }
}
