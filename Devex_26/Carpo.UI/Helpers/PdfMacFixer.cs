using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Carpo.UI.Helpers
{
    internal static class PdfMacRepair
    {

        const string LATIN1 = "ISO-8859-1";
        const int CID_HEX_LENGTH = 4;
        const int SCAN_BUFFER_SIZE = 500_000;

        const string FONT_ENCODING =
            "/Encoding << /Type /Encoding /BaseEncoding /WinAnsiEncoding " +
            "/Differences [129 /dcroat 138 /Scaron 141 /Cacute 142 /Zcaron 143 /Ccaron " +
            "144 /Dcroat 154 /scaron 157 /cacute 158 /zcaron 159 /ccaron] >>";

        static readonly Dictionary<int, int> UNI_TO_BYTE = new Dictionary<int, int>
        {
            { 0x0106, 0x8D }, { 0x0107, 0x9D }, // Ć ć
            { 0x010C, 0x8F }, { 0x010D, 0x9F }, // Č č
            { 0x0110, 0x90 }, { 0x0111, 0x81 }, // Đ đ
            { 0x0160, 0x8A }, { 0x0161, 0x9A }, // Š š
            { 0x017D, 0x8E }, { 0x017E, 0x9E }, // Ž ž
        };

        const double TJ_SPACE_RELATIVE_RATIO = 0.5;
        const double TJ_SPACE_MIN_MAGNITUDE = 100.0;
        const double DEFAULT_GLYPH_WIDTH = 500;
        const int WIDTHS_FIRST_BYTE = 32;
        const int WIDTHS_LAST_BYTE = 255;

        const int FLAG_SYMBOLIC = 4;
        const int FLAG_NONSYMBOLIC = 32;

        const string IDENTITY_H_PATTERN = @"/Encoding\s*/Identity(?:#2D|-)[Hh]";
        const string TYPE0_PATTERN = @"/Subtype\s*/Type0";

        const string FONT_NAME_PATTERN = @"/(?:F|TT|T1_|C2_)\d+";
        const string FONT_REF_PATTERN = "(" + FONT_NAME_PATTERN + @")\s+(\d+)\s+0\s+R";
        const string FONT_OBJ_DICT_PATTERN = @"\b{0}\s+0\s+obj\s*<<([^>]+?)>>";

        const string TYPE0_OBJ_PATTERN = @"\b{0}\s+0\s+obj\s*<<[^>]*?/Subtype\s*/Type0[^>]*?>>\s*endobj";
        const string OBJ_DICT_FULL_PATTERN = @"\b{0}\s+0\s+obj\s*<<(.+?)>>\s*endobj";
        const string TO_UNICODE_PATTERN = @"/ToUnicode\s+(\d+)\s+0\s+R";

        const string DESCENDANT_FONTS_PATTERN =
            @"/DescendantFonts\s*(?:\[\s*(\d+)\s+0\s+R\s*\]|(\d+)\s+0\s+R)";
        const string ARRAY_OBJ_PATTERN = @"\b{0}\s+0\s+obj\s*\[\s*(\d+)\s+0\s+R\s*\]";
        const string CONTENTS_PATTERN = @"/Contents\s*(?:\[([^\]]*)\]|(\d+)\s+0\s+R)";
        const string OBJ_REF_PATTERN = @"(\d+)\s+0\s+R";
        const string FORM_XOBJECT_MARKER = "/Form";

        const string BASE_FONT_PATTERN = @"/BaseFont\s*(/\S+?)(?=\s|/|$|>)";
        const string FONT_DESCRIPTOR_REF_PATTERN = @"/FontDescriptor\s+(\d+)\s+0\s+R";
        const string FONT_FILE2_REF_PATTERN = @"/FontFile2\s+(\d+)\s+0\s+R";
        const string FLAGS_PATTERN = @"/Flags\s+(-?\d+)";
        const string W_ARRAY_HEADER_PATTERN = @"/W\s*\[";
        const string W_ARRAY_ENTRY_PATTERN = @"(\d+)\s*\[([-\d.\s]+)\]|(\d+)\s+(\d+)\s+(-?\d+(?:\.\d+)?)";

        const string STREAM_PATTERN = @"\b{0}\s+0\s+obj\s*<<(.+?)>>\s*stream\r?\n(.+?)\r?\n?endstream";
        const string STREAM_OBJ_FULL_PATTERN = @"\b{0}\s+0\s+obj\s*<<(.+?)>>\s*stream\r?\n(.+?)\r?\n?endstream\s*endobj";
        const string STREAM_DICT_PATTERN = @"<<(.+?)>>\s*stream\r?\n(.+?)\r?\n?endstream";
        const string OBJ_PATTERN = @"(\d+)\s+0\s+obj\s*(.+?)endobj";

        const string OBJ_ANYWHERE_PATTERN = @"(?<![0-9])(\d+)\s+0\s+obj\b";
        const string OBJ_HEADER_AT_PATTERN = @"\A{0}\s+0\s+obj\b";
        const int OBJ_HEADER_MAX_LENGTH = 32;
        const string STREAM_KEYWORD = "stream";
        const string END_STREAM_KEYWORD = "endstream";
        const string END_OBJ_KEYWORD = "endobj";
        const string TRAILER_DICT_PATTERN = @"trailer\s*<<(.+?)>>";

        const string FLATE_ABBREV_PATTERN = @"/Fl\b";

        const string LENGTH_ENTRY_PATTERN = @"/Length\s+\d+(?:\s+0\s+R)?";
        const string BFRANGE_PATTERN = @"beginbfrange(.*?)endbfrange";
        const string BFRANGE_ARRAY_PATTERN = @"<([0-9a-fA-F]+)>\s*<([0-9a-fA-F]+)>\s*\[(.*?)\]";
        const string BFRANGE_SCALAR_PATTERN = @"<([0-9a-fA-F]+)>\s*<([0-9a-fA-F]+)>\s*<([0-9a-fA-F]+)>(?!\s*\[)";
        const string BFCHAR_PATTERN = @"beginbfchar(.*?)endbfchar";
        const string BFCHAR_ENTRY_PATTERN = @"<([0-9a-fA-F]+)>\s*<([0-9a-fA-F]+)>";
        const string HEX_TOKEN_PATTERN = @"<([0-9a-fA-F\s]+)>";
        const string HEX_WHITESPACE_PATTERN = @"\s+";
        const string TJ_TOKEN_PATTERN = @"<([0-9a-fA-F\s]+)>|(-?\d+(?:\.\d+)?)";

        const string CONTENT_TJ_PATTERN =
            "(" + FONT_NAME_PATTERN + @")\s+[\d.]+\s+Tf|<([0-9a-fA-F\s]+)>\s*Tj|\[([^\]]*)\]\s*TJ";

        const string LITERAL_STRING_PATTERN = @"\((?:[^()\\]|\\.)*\)";
        const string TEXT_SHOW_SCAN_PATTERN =
            "(" + FONT_NAME_PATTERN + @")\s+[\d.]+\s+Tf|" +
            "(" + LITERAL_STRING_PATTERN + @")\s*(?:Tj|'|"")|" +
            @"\[([^\]]*)\]\s*TJ";

        public static bool RequiresRepair(Stream sourceStream)
        {
            sourceStream.Position = 0;
            var buffer = new byte[Math.Min(SCAN_BUFFER_SIZE, sourceStream.Length)];
            sourceStream.Read(buffer, 0, buffer.Length);
            sourceStream.Position = 0;
            return Regex.IsMatch(Encoding.GetEncoding(LATIN1).GetString(buffer),
                TYPE0_PATTERN, RegexOptions.Singleline);
        }

        public static MemoryStream Repair(Stream sourceStream)
        {
            var pdfBytes = ReadAllBytes(sourceStream);
            var pdf = Encoding.GetEncoding(LATIN1).GetString(pdfBytes);

            var fontRefs = ExtractFontRefs(pdf);
            var fontInfo = LoadType0Fonts(pdf, pdfBytes, fontRefs);

            var contentStreams = FindContentStreams(pdfBytes, ExtractContentRefs(pdf),
                out bool allTextStreamsReadable);
            var streamChains = ExtractContentChains(pdf, contentStreams)
                .Where(ChainShowsText).ToList();
            var caseAFonts = fontInfo.Cmaps.Keys.Where(n => !fontInfo.IdentityHFonts.Contains(n)).ToList();
            if (!allTextStreamsReadable || HasUnconvertibleText(streamChains, caseAFonts))
                RestrictToIdentityH(fontInfo, fontRefs);

            var replacements = new List<(int start, int end, byte[] newData)>();
            AddFontReplacements(pdf, fontRefs, fontInfo, replacements);
            AddDescriptorFlagReplacements(pdf, fontInfo, replacements);
            AddFontFile2Replacements(pdf, fontInfo, replacements);
            var cmapsForConversion = fontInfo.Cmaps
                .Where(kv => !fontInfo.IdentityHFonts.Contains(kv.Key))
                .ToDictionary(kv => kv.Key, kv => kv.Value);
            if (cmapsForConversion.Count > 0)
                AddContentStreamReplacements(streamChains, cmapsForConversion, replacements);

            if (replacements.Count == 0) return new MemoryStream(pdfBytes);

            var rebuilt = RebuildXref(ApplyReplacements(pdfBytes, replacements));
            return new MemoryStream(rebuilt ?? pdfBytes);
        }

        private static bool ChainShowsText(List<ContentStream> chain)
        {
            foreach (var cs in chain)
            {
                var text = Encoding.GetEncoding(LATIN1).GetString(cs.Decompressed);
                if (text.Contains("Tj") || text.Contains("TJ")) return true;
            }
            return false;
        }

        private static bool HasUnconvertibleText(List<List<ContentStream>> streamChains,
            List<string> caseAFonts)
        {
            foreach (var chain in streamChains)
            {
                var content = Encoding.GetEncoding(LATIN1).GetString(JoinChain(chain));
                string currentFont = null;
                foreach (Match m in Regex.Matches(content, TEXT_SHOW_SCAN_PATTERN, RegexOptions.Singleline))
                {
                    if (m.Groups[1].Success) { currentFont = m.Groups[1].Value; continue; }
                    if (currentFont == null || !caseAFonts.Contains(currentFont)) continue;
                    if (m.Groups[2].Success) return true;
                    if (m.Groups[3].Success && m.Groups[3].Value.Contains('(')) return true;
                }
            }
            return false;
        }

        private static void RestrictToIdentityH(Type0FontInfo info,
            List<(string Name, int ObjId)> fontRefs)
        {
            foreach (var name in info.Cmaps.Keys.Where(n => !info.IdentityHFonts.Contains(n)).ToList())
            {
                info.Cmaps.Remove(name);
                info.Widths.Remove(name);
                info.BaseFonts.Remove(name);
                info.FontDescriptors.Remove(name);
                info.FontFile2Ids.Remove(name);

                foreach (var (n, objId) in fontRefs)
                    if (n == name) info.Type0ObjIds.Remove(objId);
            }
        }


        private class Type0FontInfo
        {
            public Dictionary<string, Dictionary<int, int>> Cmaps = new Dictionary<string, Dictionary<int, int>>();
            public Dictionary<string, double[]> Widths = new Dictionary<string, double[]>();
            public Dictionary<string, string> BaseFonts = new Dictionary<string, string>();
            public Dictionary<string, int> FontDescriptors = new Dictionary<string, int>();
            public Dictionary<string, int> FontFile2Ids = new Dictionary<string, int>();
            public List<int> Type0ObjIds = new List<int>();
            public HashSet<string> IdentityHFonts = new HashSet<string>();
        }

        private class ContentStream
        {
            public int ObjNum;
            public string Dict;
            public byte[] Decompressed;
            public int ObjStart;
            public int ObjEnd;
        }

        private static byte[] ReadAllBytes(Stream stream)
        {
            stream.Position = 0;
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
        private static List<(string Name, int ObjId)> ExtractFontRefs(string pdf)
        {
            var refs = new List<(string, int)>();
            var seen = new HashSet<(string, int)>();
            foreach (Match m in Regex.Matches(pdf, FONT_REF_PATTERN))
            {
                var pair = (m.Groups[1].Value, int.Parse(m.Groups[2].Value));
                if (seen.Add(pair)) refs.Add(pair);
            }
            return refs;
        }

        private static Type0FontInfo LoadType0Fonts(string pdf, byte[] pdfBytes,
            List<(string Name, int ObjId)> fontRefs)
        {
            var info = new Type0FontInfo();
            foreach (var (name, objId) in fontRefs)
                LoadSingleType0Font(pdf, pdfBytes, name, objId, info);
            return info;
        }

        private static void LoadSingleType0Font(string pdf, byte[] pdfBytes,
            string fontName, int fontObjId, Type0FontInfo info)
        {
            var fontObjMatch = Regex.Match(pdf,
                string.Format(FONT_OBJ_DICT_PATTERN, fontObjId), RegexOptions.Singleline);
            if (!fontObjMatch.Success) return;

            var fontDict = fontObjMatch.Groups[1].Value;
            if (!fontDict.Contains("/Type0")) return;
            if (info.Type0ObjIds.Contains(fontObjId)) return;
            info.Type0ObjIds.Add(fontObjId);

            if (info.Cmaps.ContainsKey(fontName)) return;

            var bfMatch = Regex.Match(fontDict, BASE_FONT_PATTERN);
            if (bfMatch.Success) info.BaseFonts[fontName] = bfMatch.Groups[1].Value;

            var cidFontObjId = ResolveCidFontObjId(pdf, fontDict);
            if (cidFontObjId == null) return;
            var cidFontMatch = Regex.Match(pdf,
                string.Format(OBJ_DICT_FULL_PATTERN, cidFontObjId), RegexOptions.Singleline);
            if (!cidFontMatch.Success) return;
            var cidFontDict = cidFontMatch.Groups[1].Value;

            if (!LoadCmap(pdf, pdfBytes, fontDict, fontName, cidFontDict, info)) return;

            LinkFontDescriptorAndFile(pdf, cidFontDict, fontName, info);

            var cidWidths = ParseCidWidths(cidFontDict);
            if (cidWidths.Count > 0)
                info.Widths[fontName] = BuildByteWidths(info.Cmaps[fontName], cidWidths);
        }

        private static string ResolveCidFontObjId(string pdf, string fontDict)
        {
            var m = Regex.Match(fontDict, DESCENDANT_FONTS_PATTERN);
            if (!m.Success) return null;
            if (m.Groups[1].Success) return m.Groups[1].Value;

            var arrayMatch = Regex.Match(pdf,
                string.Format(ARRAY_OBJ_PATTERN, m.Groups[2].Value), RegexOptions.Singleline);
            return arrayMatch.Success ? arrayMatch.Groups[1].Value : null;
        }

        private static bool LoadCmap(string pdf, byte[] pdfBytes, string fontDict,
            string fontName, string cidFontDict, Type0FontInfo info)
        {
            var tuMatch = Regex.Match(fontDict, TO_UNICODE_PATTERN);
            if (tuMatch.Success)
            {
                var cmapContent = DecompressStream(pdfBytes, int.Parse(tuMatch.Groups[1].Value));
                if (cmapContent == null) return false;
                info.Cmaps[fontName] = ParseCMap(cmapContent);
                return true;
            }

            if (!Regex.IsMatch(fontDict, IDENTITY_H_PATTERN)) return false;

            var cidWidths = ParseCidWidths(cidFontDict);
            if (cidWidths.Count == 0) return false;

            var identityCmap = new Dictionary<int, int>();
            foreach (var cid in cidWidths.Keys)
                identityCmap[cid] = cid;
            info.Cmaps[fontName] = identityCmap;
            info.IdentityHFonts.Add(fontName);
            return true;
        }

        private static void LinkFontDescriptorAndFile(string pdf, string cidFontDict,
            string fontName, Type0FontInfo info)
        {
            var fdMatch = Regex.Match(cidFontDict, FONT_DESCRIPTOR_REF_PATTERN);
            if (!fdMatch.Success) return;

            var descId = int.Parse(fdMatch.Groups[1].Value);
            var ff2Id = ExtractFontFile2Id(pdf, descId);
            if (!ff2Id.HasValue) return;

            info.FontDescriptors[fontName] = descId;
            info.FontFile2Ids[fontName] = ff2Id.Value;
        }

        private static int? ExtractFontFile2Id(string pdf, int descriptorObjId)
        {
            var descMatch = Regex.Match(pdf,
                string.Format(FONT_OBJ_DICT_PATTERN, descriptorObjId), RegexOptions.Singleline);
            if (!descMatch.Success) return null;
            var ff2Match = Regex.Match(descMatch.Groups[1].Value, FONT_FILE2_REF_PATTERN);
            return ff2Match.Success ? (int?)int.Parse(ff2Match.Groups[1].Value) : null;
        }

        private static void AddFontReplacements(string pdf,
            List<(string Name, int ObjId)> fontRefs, Type0FontInfo info,
            List<(int start, int end, byte[] newData)> replacements)
        {
            var objToFontName = new Dictionary<int, string>();
            foreach (var (name, objId) in fontRefs)
                if (!objToFontName.ContainsKey(objId)) objToFontName[objId] = name;

            foreach (var fontObjId in info.Type0ObjIds)
            {
                var objMatch = Regex.Match(pdf,
                    string.Format(TYPE0_OBJ_PATTERN, fontObjId), RegexOptions.Singleline);
                if (!objMatch.Success) continue;
                if (!objToFontName.TryGetValue(fontObjId, out var fontName)) continue;


                if (info.IdentityHFonts.Contains(fontName)) continue;

                if (!info.Cmaps.ContainsKey(fontName)) continue;

                var newObj = BuildReplacementFontObj(fontObjId, fontName, objMatch.Value, info);
                replacements.Add((objMatch.Index, objMatch.Index + objMatch.Length,
                    Encoding.GetEncoding(LATIN1).GetBytes(newObj)));
            }
        }

        private static string BuildReplacementFontObj(int fontObjId, string fontName,
            string origObjText, Type0FontInfo info)
        {
            var widthsEntry = info.Widths.TryGetValue(fontName, out var byteWidths)
                ? BuildWidthsDictEntry(byteWidths) : "";

            if (CanReuseEmbeddedFont(fontName, info))
                return BuildTrueTypeWrapper(fontObjId, fontName, info, widthsEntry);

            return BuildHelveticaFallback(fontObjId, origObjText, widthsEntry);
        }

        private static bool CanReuseEmbeddedFont(string fontName, Type0FontInfo info)
            => info.BaseFonts.ContainsKey(fontName)
            && info.FontDescriptors.ContainsKey(fontName)
            && info.FontFile2Ids.ContainsKey(fontName);

        private static string BuildTrueTypeWrapper(int fontObjId, string fontName,
            Type0FontInfo info, string widthsEntry) =>
            $"{fontObjId} 0 obj\n" +
            $"<< /Type /Font /Subtype /TrueType /BaseFont {info.BaseFonts[fontName]} " +
            $"/FontDescriptor {info.FontDescriptors[fontName]} 0 R " +
            $"{FONT_ENCODING}{widthsEntry} >>\n" +
            "endobj";

        private static string BuildHelveticaFallback(int fontObjId, string origObjText, string widthsEntry)
        {
            var isBold = origObjText.Contains("Bold") || origObjText.Contains("Medium");
            var baseFont = isBold ? "/Helvetica-Bold" : "/Helvetica";
            return $"{fontObjId} 0 obj\n" +
                $"<< /Type /Font /Subtype /Type1 /BaseFont {baseFont} {FONT_ENCODING}{widthsEntry} >>\n" +
                "endobj";
        }

        private static void AddDescriptorFlagReplacements(string pdf, Type0FontInfo info,
            List<(int start, int end, byte[] newData)> replacements)
        {
            var seen = new HashSet<int>();
            foreach (var descId in info.FontDescriptors.Values)
            {
                if (!seen.Add(descId)) continue;

                var m = Regex.Match(pdf, string.Format(OBJ_DICT_FULL_PATTERN, descId), RegexOptions.Singleline);
                if (!m.Success) continue;

                var body = m.Groups[1].Value;
                var flagsMatch = Regex.Match(body, FLAGS_PATTERN);
                if (!flagsMatch.Success) continue;

                var origFlags = int.Parse(flagsMatch.Groups[1].Value);
                var newFlags = (origFlags & ~FLAG_SYMBOLIC) | FLAG_NONSYMBOLIC;
                if (newFlags == origFlags) continue;

                var newBody = body.Substring(0, flagsMatch.Index)
                    + $"/Flags {newFlags}"
                    + body.Substring(flagsMatch.Index + flagsMatch.Length);

                var newObj = $"{descId} 0 obj\n<<{newBody}>>\nendobj";
                replacements.Add((m.Index, m.Index + m.Length,
                    Encoding.GetEncoding(LATIN1).GetBytes(newObj)));
            }
        }

        private static void AddFontFile2Replacements(string pdf, Type0FontInfo info,
            List<(int start, int end, byte[] newData)> replacements)
        {
            var seen = new HashSet<int>();
            foreach (var fontName in info.FontFile2Ids.Keys)
            {
                var ff2Id = info.FontFile2Ids[fontName];
                if (!seen.Add(ff2Id)) continue;

                var injected = TryInjectCmap(pdf, ff2Id, info.Cmaps[fontName]);
                if (injected.HasValue) replacements.Add(injected.Value);
            }
        }

        private static (int start, int end, byte[] newData)? TryInjectCmap(
            string pdf, int ff2Id, Dictionary<int, int> cidToUni)
        {
            var streamMatch = Regex.Match(pdf,
                string.Format(STREAM_OBJ_FULL_PATTERN, ff2Id), RegexOptions.Singleline);
            if (!streamMatch.Success) return null;

            var rawStream = Encoding.GetEncoding(LATIN1).GetBytes(streamMatch.Groups[2].Value);
            byte[] origFontBin;
            if (IsFlate(streamMatch.Groups[1].Value))
            {
                try { origFontBin = Decompress(rawStream); }
                catch { return null; }
            }
            else origFontBin = rawStream;

            if (!TrueTypeCmapInjector.IsTrueType(origFontBin)) return null;

            var uniToGid = BuildUniToGid(cidToUni);
            if (uniToGid.Count == 0) return null;

            byte[] newFontBin;
            try { newFontBin = TrueTypeCmapInjector.WithUnicodeCmap(origFontBin, uniToGid); }
            catch { return null; }

            var newObj = BuildFontFile2Obj(ff2Id, newFontBin.Length, Compress(newFontBin));
            return (streamMatch.Index, streamMatch.Index + streamMatch.Length, newObj);
        }

        private static Dictionary<int, int> BuildUniToGid(Dictionary<int, int> cidToUni)
        {
            var uniToGid = new Dictionary<int, int>();
            foreach (var p in cidToUni)
            {
                if (p.Value == 0) continue;
                if (!uniToGid.ContainsKey(p.Value)) uniToGid[p.Value] = p.Key;
            }
            return uniToGid;
        }

        private static byte[] BuildFontFile2Obj(int objId, int decompressedLen, byte[] compressed)
        {
            var header = Encoding.GetEncoding(LATIN1).GetBytes(
                $"{objId} 0 obj\n<< /Length {compressed.Length} /Length1 {decompressedLen} /Filter /FlateDecode >>\nstream\n");
            var footer = Encoding.GetEncoding(LATIN1).GetBytes("\nendstream\nendobj");
            return header.Concat(compressed).Concat(footer).ToArray();
        }

        private static void AddContentStreamReplacements(
            List<List<ContentStream>> streamChains,
            Dictionary<string, Dictionary<int, int>> fontToCmap,
            List<(int start, int end, byte[] newData)> replacements)
        {
            foreach (var chain in streamChains)
            {
                var joined = JoinChain(chain);
                string currentFont = null;
                var converted = ConvertContentStream(joined, fontToCmap, ref currentFont);
                if (converted.SequenceEqual(joined)) continue;

                replacements.Add(BuildStreamReplacement(chain[0], converted));
                foreach (var cs in chain.Skip(1))
                    replacements.Add(BuildStreamReplacement(cs, new byte[0]));
            }
        }
        private static byte[] JoinChain(List<ContentStream> chain)
        {
            if (chain.Count == 1) return chain[0].Decompressed;
            var joined = new List<byte>();
            foreach (var cs in chain)
            {
                if (joined.Count > 0) joined.Add((byte)'\n');
                joined.AddRange(cs.Decompressed);
            }
            return joined.ToArray();
        }

        private static (int start, int end, byte[] newData) BuildStreamReplacement(
            ContentStream cs, byte[] content)
        {
            var recompressed = Compress(content);


            var newDict = Regex.Replace(cs.Dict, LENGTH_ENTRY_PATTERN, $"/Length {recompressed.Length}");
            if (!IsFlate(newDict)) newDict += " /Filter /FlateDecode";

            var header = Encoding.GetEncoding(LATIN1).GetBytes(
                $"{cs.ObjNum} 0 obj\n<<{newDict}>>\nstream\n");
            var footer = Encoding.GetEncoding(LATIN1).GetBytes("\nendstream\nendobj");

            return (cs.ObjStart, cs.ObjEnd, header.Concat(recompressed).Concat(footer).ToArray());
        }
        private static List<ContentStream> FindContentStreams(byte[] pdfBytes,
            HashSet<int> contentRefs, out bool allTextStreamsReadable)
        {
            var result = new List<ContentStream>();
            var pdf = Encoding.GetEncoding(LATIN1).GetString(pdfBytes);
            allTextStreamsReadable = true;

            foreach (Match m in Regex.Matches(pdf, OBJ_PATTERN, RegexOptions.Singleline))
            {
                var streamMatch = Regex.Match(m.Groups[2].Value, STREAM_DICT_PATTERN, RegexOptions.Singleline);
                if (!streamMatch.Success) continue;

                var dict = streamMatch.Groups[1].Value;

                if (dict.Contains("/Length1")) continue;

                if (dict.Contains("/Image")) continue;

                var objNum = int.Parse(m.Groups[1].Value);

                var drawsPage = contentRefs.Contains(objNum) || dict.Contains(FORM_XOBJECT_MARKER);

                byte[] decomp;
                if (IsFlate(dict))
                {

                    try { decomp = Decompress(Encoding.GetEncoding(LATIN1).GetBytes(streamMatch.Groups[2].Value)); }
                    catch { allTextStreamsReadable &= !drawsPage; continue; }
                }
                else if (dict.Contains("/Filter"))
                {
                    allTextStreamsReadable &= !drawsPage;
                    continue;
                }
                else decomp = Encoding.GetEncoding(LATIN1).GetBytes(streamMatch.Groups[2].Value);

                result.Add(new ContentStream
                {
                    ObjNum = objNum,
                    Dict = dict,
                    Decompressed = decomp,
                    ObjStart = m.Index,
                    ObjEnd = m.Index + m.Length
                });
            }
            return result;
        }

        private static HashSet<int> ExtractContentRefs(string pdf)
        {
            var refs = new HashSet<int>();
            foreach (Match m in Regex.Matches(pdf, CONTENTS_PATTERN))
            {
                if (m.Groups[2].Success) { refs.Add(int.Parse(m.Groups[2].Value)); continue; }
                foreach (Match r in Regex.Matches(m.Groups[1].Value, OBJ_REF_PATTERN))
                    refs.Add(int.Parse(r.Groups[1].Value));
            }
            return refs;
        }

        private static List<List<ContentStream>> ExtractContentChains(string pdf,
            List<ContentStream> contentStreams)
        {
            var byObjNum = new Dictionary<int, ContentStream>();
            foreach (var cs in contentStreams) byObjNum[cs.ObjNum] = cs;

            var chains = new List<List<ContentStream>>();
            var used = new HashSet<int>();
            foreach (Match m in Regex.Matches(pdf, CONTENTS_PATTERN))
            {
                var ids = m.Groups[2].Success
                    ? new[] { m.Groups[2].Value }
                    : Regex.Matches(m.Groups[1].Value, OBJ_REF_PATTERN)
                        .Cast<Match>().Select(r => r.Groups[1].Value).ToArray();

                var chain = new List<ContentStream>();
                foreach (var idText in ids)
                {
                    var id = int.Parse(idText);
                    if (byObjNum.TryGetValue(id, out var cs) && used.Add(id)) chain.Add(cs);
                }
                if (chain.Count > 0) chains.Add(chain);
            }

            foreach (var cs in contentStreams)
                if (used.Add(cs.ObjNum)) chains.Add(new List<ContentStream> { cs });
            return chains;
        }

        private static byte[] ConvertContentStream(byte[] contentBytes,
            Dictionary<string, Dictionary<int, int>> fontToCmap, ref string currentFont)
        {
            var content = Encoding.GetEncoding(LATIN1).GetString(contentBytes);
            var output = new StringBuilder();
            int lastPos = 0;

            foreach (Match m in Regex.Matches(content, CONTENT_TJ_PATTERN, RegexOptions.Singleline))
            {
                output.Append(content, lastPos, m.Index - lastPos);
                lastPos = m.Index + m.Length;

                if (m.Groups[1].Success)
                {
                    currentFont = m.Groups[1].Value;
                    output.Append(m.Value);
                    continue;
                }
                if (currentFont == null || !fontToCmap.TryGetValue(currentFont, out var cmap))
                {
                    output.Append(m.Value);
                    continue;
                }
                if (m.Groups[2].Success)
                {
                    output.Append('(').Append(EscapePdfString(HexToAscii(m.Groups[2].Value, cmap))).Append(") Tj");
                    continue;
                }
                AppendTjArrayAsText(m.Groups[3].Value, cmap, output);
            }

            output.Append(content, lastPos, content.Length - lastPos);
            return Encoding.GetEncoding(LATIN1).GetBytes(output.ToString());
        }

        private static void AppendTjArrayAsText(string tjArrayBody,
            Dictionary<int, int> cmap, StringBuilder output)
        {
            var tokens = Regex.Matches(tjArrayBody, TJ_TOKEN_PATTERN);
            var threshold = ComputeSpaceThreshold(tokens);

            output.Append('(');
            foreach (Match t in tokens)
            {
                if (t.Groups[1].Success)
                {
                    output.Append(EscapePdfString(HexToAscii(t.Groups[1].Value, cmap)));
                    continue;
                }
                var kern = double.Parse(t.Groups[2].Value, CultureInfo.InvariantCulture);
                if (kern < threshold) output.Append(' ');
            }
            output.Append(") Tj");
        }

        private static double ComputeSpaceThreshold(MatchCollection tokens)
        {
            double minKern = 0;
            foreach (Match t in tokens)
            {
                if (!t.Groups[2].Success) continue;
                var k = double.Parse(t.Groups[2].Value, CultureInfo.InvariantCulture);
                if (k < minKern) minKern = k;
            }
            return Math.Min(-TJ_SPACE_MIN_MAGNITUDE, minKern * TJ_SPACE_RELATIVE_RATIO);
        }

        private static string HexToAscii(string hex, Dictionary<int, int> cmap)
        {
            hex = Regex.Replace(hex, HEX_WHITESPACE_PATTERN, "");
            var sb = new StringBuilder(hex.Length / CID_HEX_LENGTH);
            for (int i = 0; i + CID_HEX_LENGTH <= hex.Length; i += CID_HEX_LENGTH)
            {
                var cid = Convert.ToInt32(hex.Substring(i, CID_HEX_LENGTH), 16);
              
                if (!cmap.TryGetValue(cid, out int uni) || uni == 0) { sb.Append(' '); continue; }
                var b = UniToByte(uni);
                sb.Append(b.HasValue ? (char)b.Value : ' ');
            }
            return sb.ToString();
        }

        private static string EscapePdfString(string s)
        {
            var sb = new StringBuilder(s.Length);
            foreach (char c in s)
            {
                if (c == '(' || c == ')' || c == '\\') sb.Append('\\');
                sb.Append(c);
            }
            return sb.ToString();
        }

        private static byte[] ApplyReplacements(byte[] pdfBytes, List<(int start, int end, byte[] newData)> replacements)
        {

            replacements.Sort((a, b) => b.start.CompareTo(a.start));
            var working = new List<byte>(pdfBytes);
            foreach (var rep in replacements)
            {
                working.RemoveRange(rep.start, rep.end - rep.start);
                working.InsertRange(rep.start, rep.newData);
            }
            return working.ToArray();
        }

        private static byte[] RebuildXref(byte[] pdfBytes)
        {
            var pdf = Encoding.GetEncoding(LATIN1).GetString(pdfBytes);
            var (objOffsets, maxObj) = ScanObjOffsets(pdf);
            if (objOffsets.Count == 0) return null;

            int bodyEnd = pdf.LastIndexOf(END_OBJ_KEYWORD, StringComparison.Ordinal);
            bodyEnd = bodyEnd < 0 ? pdfBytes.Length : bodyEnd + END_OBJ_KEYWORD.Length;

            var body = new List<byte>(pdfBytes.Take(bodyEnd));
            body.Add((byte)'\n');

            var xrefPos = body.Count;
            body.AddRange(Encoding.GetEncoding(LATIN1).GetBytes(
                BuildXrefTable(objOffsets, maxObj, BuildTrailerDict(pdf, maxObj), xrefPos)));

            var rebuilt = body.ToArray();
            return XrefOffsetsValid(rebuilt, objOffsets) ? rebuilt : null;
        }

        private static bool XrefOffsetsValid(byte[] pdfBytes, Dictionary<int, int> objOffsets)
        {
            var pdf = Encoding.GetEncoding(LATIN1).GetString(pdfBytes);
            foreach (var pair in objOffsets)
            {
                if (pair.Value >= pdf.Length) return false;

                if (!Regex.IsMatch(pdf.Substring(pair.Value,
                        Math.Min(OBJ_HEADER_MAX_LENGTH, pdf.Length - pair.Value)),
                    string.Format(OBJ_HEADER_AT_PATTERN, pair.Key))) return false;
            }
            return true;
        }
        private static (Dictionary<int, int> Offsets, int MaxObj) ScanObjOffsets(string pdf)
        {
            var offsets = new Dictionary<int, int>();
            var objRegex = new Regex(OBJ_ANYWHERE_PATTERN);
            int maxObj = 0, pos = 0;

            while (pos < pdf.Length)
            {
                var m = objRegex.Match(pdf, pos);
                if (!m.Success) break;

                var n = int.Parse(m.Groups[1].Value);
                offsets[n] = m.Index;
                if (n > maxObj) maxObj = n;
                pos = m.Index + m.Length;

                var streamStart = pdf.IndexOf(STREAM_KEYWORD, pos, StringComparison.Ordinal);
                var objEnd = pdf.IndexOf(END_OBJ_KEYWORD, pos, StringComparison.Ordinal);
                if (streamStart < 0 || (objEnd >= 0 && objEnd < streamStart)) continue;

                var streamEnd = pdf.IndexOf(END_STREAM_KEYWORD, streamStart, StringComparison.Ordinal);
                if (streamEnd > 0) pos = streamEnd + END_STREAM_KEYWORD.Length;
            }
            return (offsets, maxObj);
        }

        private static string BuildTrailerDict(string pdf, int maxObj)
        {
            var dict = Regex.Matches(pdf, TRAILER_DICT_PATTERN, RegexOptions.Singleline)
                .Cast<Match>().Select(m => m.Groups[1].Value)
                .FirstOrDefault(d => d.Contains("/Root"));
            if (dict == null) return $" /Size {maxObj + 1} /Root 3 0 R ";

            dict = Regex.Replace(dict, @"/Size\s+\d+", $"/Size {maxObj + 1}");
            dict = Regex.Replace(dict, @"/Prev\s+\d+", "");
            return dict;
        }

        private static string BuildXrefTable(Dictionary<int, int> objOffsets,
            int maxObj, string trailerDict, int xrefPos)
        {
            var sb = new StringBuilder();
            sb.Append($"xref\n0 {maxObj + 1}\n");
            sb.Append("0000000000 65535 f \n");
            for (int i = 1; i <= maxObj; i++)
            {
                if (objOffsets.TryGetValue(i, out int off))
                    sb.Append($"{off:D10} 00000 n \n");
                else
                    sb.Append("0000000000 65535 f \n");
            }
            sb.Append($"trailer\n<<{trailerDict}>>\nstartxref\n{xrefPos}\n%%EOF\n");
            return sb.ToString();
        }
        private static Dictionary<int, int> ParseCMap(string cmap)
        {
            var map = new Dictionary<int, int>();
            foreach (Match bfm in Regex.Matches(cmap, BFRANGE_PATTERN, RegexOptions.Singleline))
                ParseBfrangeBody(bfm.Groups[1].Value, map);
            foreach (Match bfm in Regex.Matches(cmap, BFCHAR_PATTERN, RegexOptions.Singleline))
                ParseBfcharBody(bfm.Groups[1].Value, map);
            return map;
        }

        private static void ParseBfrangeBody(string body, Dictionary<int, int> map)
        {
            foreach (Match m in Regex.Matches(body, BFRANGE_ARRAY_PATTERN, RegexOptions.Singleline))
            {
                var start = Convert.ToInt32(m.Groups[1].Value, 16);
                var end = Convert.ToInt32(m.Groups[2].Value, 16);
                var unicodes = Regex.Matches(m.Groups[3].Value, HEX_TOKEN_PATTERN);
                int idx = 0;
                for (int cid = start; cid <= end && idx < unicodes.Count; cid++, idx++)
                    map[cid] = Convert.ToInt32(unicodes[idx].Groups[1].Value, 16);
            }
            foreach (Match m in Regex.Matches(body, BFRANGE_SCALAR_PATTERN, RegexOptions.Singleline))
            {
                var start = Convert.ToInt32(m.Groups[1].Value, 16);
                var end = Convert.ToInt32(m.Groups[2].Value, 16);
                var startUni = Convert.ToInt32(m.Groups[3].Value, 16);
                for (int i = 0; i <= end - start; i++)
                    if (!map.ContainsKey(start + i)) map[start + i] = startUni + i;
            }
        }

        private static void ParseBfcharBody(string body, Dictionary<int, int> map)
        {
            foreach (Match m in Regex.Matches(body, BFCHAR_ENTRY_PATTERN))
                map[Convert.ToInt32(m.Groups[1].Value, 16)] = Convert.ToInt32(m.Groups[2].Value, 16);
        }

        private static Dictionary<int, double> ParseCidWidths(string cidFontDict)
        {
            var widths = new Dictionary<int, double>();
            var header = Regex.Match(cidFontDict, W_ARRAY_HEADER_PATTERN);
            if (!header.Success) return widths;

            var wContent = ExtractBracketed(cidFontDict, header.Index);
            if (wContent == null) return widths;

            foreach (Match m in Regex.Matches(wContent, W_ARRAY_ENTRY_PATTERN))
                ParseWidthEntry(m, widths);
            return widths;
        }

        private static void ParseWidthEntry(Match m, Dictionary<int, double> widths)
        {
            if (m.Groups[1].Success)
            {
                var startCid = int.Parse(m.Groups[1].Value);
                var values = m.Groups[2].Value.Split(
                    new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
                for (int i = 0; i < values.Length; i++)
                    widths[startCid + i] = double.Parse(values[i], CultureInfo.InvariantCulture);
                return;
            }
            var rangeStart = int.Parse(m.Groups[3].Value);
            var rangeEnd = int.Parse(m.Groups[4].Value);
            var rangeWidth = double.Parse(m.Groups[5].Value, CultureInfo.InvariantCulture);
            for (int cid = rangeStart; cid <= rangeEnd; cid++) widths[cid] = rangeWidth;
        }

        private static string ExtractBracketed(string source, int searchFrom)
        {
            int depth = 0, start = -1;
            for (int i = searchFrom; i < source.Length; i++)
            {
                if (source[i] == '[')
                {
                    if (depth == 0) start = i + 1;
                    depth++;
                    continue;
                }
                if (source[i] != ']') continue;
                depth--;
                if (depth == 0 && start >= 0) return source.Substring(start, i - start);
            }
            return null;
        }

        private static double[] BuildByteWidths(Dictionary<int, int> cmap, Dictionary<int, double> cidWidths)
        {
            var widths = new double[256];
            for (int i = 0; i < 256; i++) widths[i] = DEFAULT_GLYPH_WIDTH;

            foreach (var pair in cmap)
            {
                var b = UniToByte(pair.Value);
                if (b == null) continue;
                if (cidWidths.TryGetValue(pair.Key, out var w))
                    widths[b.Value] = w;
            }
            return widths;
        }

        private static int? UniToByte(int uni)
        {
            if (UNI_TO_BYTE.TryGetValue(uni, out int customByte)) return customByte;
            if (uni >= 0x20 && uni <= 0xFF) return uni;
            return null;
        }

        private static string BuildWidthsDictEntry(double[] byteWidths)
        {
            var sb = new StringBuilder($" /FirstChar {WIDTHS_FIRST_BYTE} /LastChar {WIDTHS_LAST_BYTE} /Widths [");
            for (int b = WIDTHS_FIRST_BYTE; b <= WIDTHS_LAST_BYTE; b++)
            {
                if (b > WIDTHS_FIRST_BYTE) sb.Append(' ');
                sb.Append(byteWidths[b].ToString("0.##", CultureInfo.InvariantCulture));
            }
            sb.Append(']');
            return sb.ToString();
        }

        private static bool IsFlate(string streamDict)
            => streamDict.Contains("FlateDecode") || Regex.IsMatch(streamDict, FLATE_ABBREV_PATTERN);

        private static string DecompressStream(byte[] pdfBytes, int objNum)
        {
            var pdf = Encoding.GetEncoding(LATIN1).GetString(pdfBytes);
            var m = Regex.Match(pdf, string.Format(STREAM_PATTERN, objNum), RegexOptions.Singleline);
            if (!m.Success) return null;

            var streamBytes = Encoding.GetEncoding(LATIN1).GetBytes(m.Groups[2].Value);
            try { return Encoding.GetEncoding(LATIN1).GetString(Decompress(streamBytes)); }
            catch { return m.Groups[2].Value; }
        }

        private static byte[] Decompress(byte[] data)
        {

            using (var input = new MemoryStream(data, 2, data.Length - 2))
            using (var deflate = new DeflateStream(input, CompressionMode.Decompress))
            using (var output = new MemoryStream())
            {
                deflate.CopyTo(output);
                return output.ToArray();
            }
        }

        private static byte[] Compress(byte[] data)
        {
            using (var output = new MemoryStream())
            {
                output.WriteByte(0x78); output.WriteByte(0x9C);
                using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
                    deflate.Write(data, 0, data.Length);

                var adler = Adler32(data);
                output.WriteByte((byte)((adler >> 24) & 0xFF));
                output.WriteByte((byte)((adler >> 16) & 0xFF));
                output.WriteByte((byte)((adler >> 8) & 0xFF));
                output.WriteByte((byte)(adler & 0xFF));
                return output.ToArray();
            }
        }

        private static uint Adler32(byte[] data)
        {
            const uint MOD = 65521;
            uint a = 1, b = 0;
            foreach (byte x in data)
            {
                a = (a + x) % MOD;
                b = (b + a) % MOD;
            }
            return (b << 16) | a;
        }
    }
    internal static class TrueTypeCmapInjector
    {
        const uint TT_SCALER_OPENTYPE = 0x00010000;
        const uint TT_SCALER_TRUE = 0x74727565;
        const uint HEAD_CHECKSUM_MAGIC = 0xB1B0AFBA;
        const string CMAP_TAG = "cmap";
        const string HEAD_TAG = "head";
        const int HEAD_CHECKSUM_ADJ_OFFSET = 8;
        const int OFFSET_TABLE_SIZE = 12;
        const int TABLE_RECORD_SIZE = 16;
        const int CMAP_HEADER_SIZE = 12;
        const ushort CMAP_PLATFORM_MICROSOFT = 3;
        const ushort CMAP_ENCODING_UNICODE_BMP = 1;
        const ushort CMAP_FORMAT_SEGMENT_DELTA = 4;
        const ushort CMAP_TERMINATOR_CODE = 0xFFFF;

        public static bool IsTrueType(byte[] font)
        {
            if (font == null || font.Length < 4) return false;
            var scaler = ReadU32(font, 0);
            return scaler == TT_SCALER_OPENTYPE || scaler == TT_SCALER_TRUE;
        }

        public static byte[] WithUnicodeCmap(byte[] origFont, Dictionary<int, int> uniToGid)
        {
            var tables = ReadTables(origFont);
            var cmapIdx = tables.FindIndex(t => t.Tag == CMAP_TAG);

            Dictionary<int, int> merged;
            try { merged = cmapIdx >= 0 ? ReadUnicodeBmpMappings(tables[cmapIdx].Data) : new Dictionary<int, int>(); }
            catch { merged = new Dictionary<int, int>(); }
            foreach (var p in uniToGid) merged[p.Key] = p.Value;

            var newCmap = BuildCmapTable(merged);
            if (cmapIdx >= 0) tables[cmapIdx] = new TtTable { Tag = CMAP_TAG, Data = newCmap };
            else tables.Add(new TtTable { Tag = CMAP_TAG, Data = newCmap });

            return Serialize(tables);
        }

        private static Dictionary<int, int> ReadUnicodeBmpMappings(byte[] cmap)
        {
            var map = new Dictionary<int, int>();
            int nSub = ReadU16(cmap, 2);
            for (int i = 0; i < nSub; i++)
            {
                int rec = 4 + i * 8;
                if (ReadU16(cmap, rec) != CMAP_PLATFORM_MICROSOFT) continue;
                if (ReadU16(cmap, rec + 2) != CMAP_ENCODING_UNICODE_BMP) continue;
                int sub = (int)ReadU32(cmap, rec + 4);
                if (ReadU16(cmap, sub) != CMAP_FORMAT_SEGMENT_DELTA) continue;
                ReadFormat4Subtable(cmap, sub, map);
            }
            return map;
        }

        private static void ReadFormat4Subtable(byte[] d, int sub, Dictionary<int, int> map)
        {
            int segCount = ReadU16(d, sub + 6) / 2;
            int endBase = sub + 14;
            int startBase = endBase + segCount * 2 + 2;
            int deltaBase = startBase + segCount * 2;
            int rangeBase = deltaBase + segCount * 2;
            for (int i = 0; i < segCount; i++)
            {
                int start = ReadU16(d, startBase + i * 2);
                int end = ReadU16(d, endBase + i * 2);
                if (start == CMAP_TERMINATOR_CODE) continue;
                int delta = ReadU16(d, deltaBase + i * 2);
                int rangeOff = ReadU16(d, rangeBase + i * 2);
                for (int uni = start; uni <= end; uni++)
                {
                    int gid;
                    if (rangeOff == 0) gid = (uni + delta) & 0xFFFF;
                    else
                    {

                        int gidPos = rangeBase + i * 2 + rangeOff + (uni - start) * 2;
                        if (gidPos + 1 >= d.Length) continue;
                        gid = ReadU16(d, gidPos);
                        if (gid != 0) gid = (gid + delta) & 0xFFFF;
                    }
                    if (gid != 0) map[uni] = gid;
                }
            }
        }

        private class TtTable
        {
            public string Tag;
            public byte[] Data;
        }

        private struct Segment
        {
            public int Start;
            public int End;
            public int Delta;
        }

        private static List<TtTable> ReadTables(byte[] font)
        {
            int numTables = ReadU16(font, 4);
            var tables = new List<TtTable>(numTables);
            for (int i = 0; i < numTables; i++)
            {
                int rec = OFFSET_TABLE_SIZE + i * TABLE_RECORD_SIZE;
                var tag = Encoding.ASCII.GetString(font, rec, 4);
                int offset = (int)ReadU32(font, rec + 8);
                int length = (int)ReadU32(font, rec + 12);
                var data = new byte[length];
                Array.Copy(font, offset, data, 0, length);
                tables.Add(new TtTable { Tag = tag, Data = data });
            }
            return tables;
        }

        private static byte[] Serialize(List<TtTable> tables)
        {

            tables.Sort((a, b) => string.CompareOrdinal(a.Tag, b.Tag));

            var offsets = ComputeTableOffsets(tables, out int totalSize);
            var font = new byte[totalSize];

            WriteOffsetTable(font, tables.Count);
            WriteTableRecordsAndData(font, tables, offsets);
            FixHeadChecksumAdjustment(font, tables, offsets);
            return font;
        }

        private static int[] ComputeTableOffsets(List<TtTable> tables, out int totalSize)
        {
            int n = tables.Count;
            int cursor = OFFSET_TABLE_SIZE + TABLE_RECORD_SIZE * n;
            var offsets = new int[n];
            for (int i = 0; i < n; i++)
            {
                offsets[i] = cursor;
                cursor += tables[i].Data.Length;
                cursor = (cursor + 3) & ~3;
            }
            totalSize = cursor;
            return offsets;
        }

        private static void WriteOffsetTable(byte[] font, int numTables)
        {
            WriteU32(font, 0, TT_SCALER_OPENTYPE);
            WriteU16(font, 4, (ushort)numTables);

            int entrySelector = (int)Math.Floor(Math.Log(numTables, 2));
            int searchRange = (1 << entrySelector) * 16;
            int rangeShift = numTables * 16 - searchRange;
            WriteU16(font, 6, (ushort)searchRange);
            WriteU16(font, 8, (ushort)entrySelector);
            WriteU16(font, 10, (ushort)rangeShift);
        }

        private static void WriteTableRecordsAndData(byte[] font, List<TtTable> tables, int[] offsets)
        {
            for (int i = 0; i < tables.Count; i++)
            {
                int rec = OFFSET_TABLE_SIZE + i * TABLE_RECORD_SIZE;
                var tag = tables[i].Tag.PadRight(4).Substring(0, 4);
                Array.Copy(Encoding.ASCII.GetBytes(tag), 0, font, rec, 4);
                WriteU32(font, rec + 4, ComputeChecksum(tables[i].Data));
                WriteU32(font, rec + 8, (uint)offsets[i]);
                WriteU32(font, rec + 12, (uint)tables[i].Data.Length);
                Array.Copy(tables[i].Data, 0, font, offsets[i], tables[i].Data.Length);
            }
        }

        private static void FixHeadChecksumAdjustment(byte[] font, List<TtTable> tables, int[] offsets)
        {
            int headIdx = tables.FindIndex(t => t.Tag == HEAD_TAG);
            if (headIdx < 0) return;

            int csAdjOff = offsets[headIdx] + HEAD_CHECKSUM_ADJ_OFFSET;
            WriteU32(font, csAdjOff, 0);
            WriteU32(font, csAdjOff, HEAD_CHECKSUM_MAGIC - ComputeChecksum(font));
        }

        private static byte[] BuildCmapTable(Dictionary<int, int> uniToGid)
        {
            var subtable = BuildFormat4Subtable(uniToGid);
            var data = new List<byte>(CMAP_HEADER_SIZE + subtable.Length);
            AppendU16(data, 0);
            AppendU16(data, 1);
            AppendU16(data, CMAP_PLATFORM_MICROSOFT);
            AppendU16(data, CMAP_ENCODING_UNICODE_BMP);
            AppendU32(data, CMAP_HEADER_SIZE);
            data.AddRange(subtable);
            return data.ToArray();
        }

        private static byte[] BuildFormat4Subtable(Dictionary<int, int> uniToGid)
        {
            var sorted = uniToGid.Where(p => p.Key >= 0 && p.Key < CMAP_TERMINATOR_CODE)
                .OrderBy(p => p.Key).ToList();

            var segments = BuildSegments(sorted);
            segments.Add(new Segment { Start = CMAP_TERMINATOR_CODE, End = CMAP_TERMINATOR_CODE, Delta = 1 });

            int segCount = segments.Count;
            int subtableLength = 16 + 8 * segCount;
            int entrySelector = (int)Math.Floor(Math.Log(segCount, 2));
            int searchRange = (1 << entrySelector) * 2;
            int rangeShift = 2 * segCount - searchRange;

            var bytes = new List<byte>(subtableLength);
            AppendU16(bytes, CMAP_FORMAT_SEGMENT_DELTA);
            AppendU16(bytes, (ushort)subtableLength);
            AppendU16(bytes, 0);
            AppendU16(bytes, (ushort)(2 * segCount));
            AppendU16(bytes, (ushort)searchRange);
            AppendU16(bytes, (ushort)entrySelector);
            AppendU16(bytes, (ushort)rangeShift);

            foreach (var s in segments) AppendU16(bytes, (ushort)s.End);
            AppendU16(bytes, 0);
            foreach (var s in segments) AppendU16(bytes, (ushort)s.Start);
            foreach (var s in segments) AppendU16(bytes, (ushort)s.Delta);
            foreach (var _ in segments) AppendU16(bytes, 0);
            return bytes.ToArray();
        }

        private static List<Segment> BuildSegments(List<KeyValuePair<int, int>> sorted)
        {
            var segments = new List<Segment>();
            int i = 0;
            while (i < sorted.Count)
            {
                int start = sorted[i].Key;
                int delta = (sorted[i].Value - start) & 0xFFFF;
                int end = start;
                int j = i + 1;
                while (j < sorted.Count
                    && sorted[j].Key == sorted[j - 1].Key + 1
                    && ((sorted[j].Value - sorted[j].Key) & 0xFFFF) == delta)
                {
                    end = sorted[j].Key;
                    j++;
                }
                segments.Add(new Segment { Start = start, End = end, Delta = delta });
                i = j;
            }
            return segments;
        }

        private static uint ComputeChecksum(byte[] data)
        {
            uint sum = 0;
            int aligned = data.Length & ~3;
            for (int i = 0; i < aligned; i += 4)
            {
                sum += ((uint)data[i] << 24) | ((uint)data[i + 1] << 16)
                    | ((uint)data[i + 2] << 8) | data[i + 3];
            }
            int rem = data.Length - aligned;
            if (rem == 0) return sum;

            uint word = 0;
            for (int i = 0; i < rem; i++)
                word |= (uint)data[aligned + i] << (24 - i * 8);
            return sum + word;
        }

        private static uint ReadU32(byte[] d, int o) =>
            ((uint)d[o] << 24) | ((uint)d[o + 1] << 16) | ((uint)d[o + 2] << 8) | d[o + 3];

        private static ushort ReadU16(byte[] d, int o) =>
            (ushort)(((uint)d[o] << 8) | d[o + 1]);

        private static void WriteU32(byte[] d, int o, uint v)
        {
            d[o] = (byte)(v >> 24);
            d[o + 1] = (byte)(v >> 16);
            d[o + 2] = (byte)(v >> 8);
            d[o + 3] = (byte)v;
        }

        private static void WriteU16(byte[] d, int o, ushort v)
        {
            d[o] = (byte)(v >> 8);
            d[o + 1] = (byte)v;
        }

        private static void AppendU16(List<byte> list, ushort v)
        {
            list.Add((byte)(v >> 8));
            list.Add((byte)v);
        }

        private static void AppendU32(List<byte> list, uint v)
        {
            list.Add((byte)(v >> 24));
            list.Add((byte)(v >> 16));
            list.Add((byte)(v >> 8));
            list.Add((byte)v);
        }
    }
}
