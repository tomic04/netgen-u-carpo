using System;

namespace Carpo.NetGen.Xml
{
    /// <summary>
    /// Prva grana iz Form2.vb (oko 1256-2707). Redoslijed je redoslijed If/InStr,
    /// ne enum vrijednosti iz XML-a. Tipfeler koji ne sadrzi podstring ostaje Nijedna.
    /// </summary>
    public enum NgGrana
    {
        Nijedna = 0,
        StatusStrip,
        Label,
        Group,
        Panel,
        WebBrowser,
        SplitContainer,
        Check,
        Radio,
        DxGrid,
        Grid,
        Text,
        Number,
        Search,
        Date,
        Combo,
        MultiDrop,
        Tree,
        Button,
        Help,
        PictureBox
    }

    public static class NgType
    {
        public static NgGrana Odredi(string typeRaw)
        {
            if (string.IsNullOrEmpty(typeRaw))
                return NgGrana.Nijedna;

            string t = typeRaw.ToLowerInvariant();
            if (Sadrzi(t, "statusstrip")) return NgGrana.StatusStrip;
            if (Sadrzi(t, "label")) return NgGrana.Label;
            if (Sadrzi(t, "group") || Sadrzi(t, "frame")) return NgGrana.Group;
            if (Sadrzi(t, "panel")) return NgGrana.Panel;
            if (Sadrzi(t, "webbrowser")) return NgGrana.WebBrowser;
            if (Sadrzi(t, "splitcontainer")) return NgGrana.SplitContainer;
            if (Sadrzi(t, "check")) return NgGrana.Check;
            if (Sadrzi(t, "radio")) return NgGrana.Radio;
            // dxgrid prije grid: InStr("dxgrid","grid") je istina, ali nadjen spriječi drugu granu
            if (Sadrzi(t, "dxgrid")) return NgGrana.DxGrid;
            if (Sadrzi(t, "grid")) return NgGrana.Grid;
            if (Sadrzi(t, "text")) return NgGrana.Text;
            if (Sadrzi(t, "number") || Sadrzi(t, "cur")) return NgGrana.Number;
            if (Sadrzi(t, "trazilica") || Sadrzi(t, "search")) return NgGrana.Search;
            if (Sadrzi(t, "dtpicker") || Sadrzi(t, "date")) return NgGrana.Date;
            if (Sadrzi(t, "combo")) return NgGrana.Combo;
            if (Sadrzi(t, "multidrop")) return NgGrana.MultiDrop;
            if (Sadrzi(t, "tree")) return NgGrana.Tree;
            if (Sadrzi(t, "button")) return NgGrana.Button;
            if (Sadrzi(t, "help")) return NgGrana.Help;
            if (Sadrzi(t, "picturebox")) return NgGrana.PictureBox;
            return NgGrana.Nijedna;
        }

        private static bool Sadrzi(string tekst, string dio)
        {
            return tekst.IndexOf(dio, StringComparison.Ordinal) >= 0;
        }
    }
}
