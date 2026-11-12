using Carpo.Structure.Models.Core;
using System.Collections.Generic;

namespace Carpo.Structure.Interfaces
{
    public interface IJModelEditorBase
    {
        string Name { get; set; }
        string Caption { get; set; }
        string Type { get; set; }
        string EnabledStr { get; set; }
        string Value { get; set; }
        bool? Enabled { get; set; }
        string SaveValue { get; set; }

        string Dock { get; set; }

        //int? MaxLength { get; set; }
        //string MaxLengthStr { get; set; }
        string Format { get; set; }

        string NullText { get; set; }
        string FontColor { get; set; }
        int? FontSizeDelta { get; set; }
        string FontSizeDeltaStr { get; set; }
        bool? FontBold { get; set; }
        string FontBoldStr { get; set; }
        List<JEvent> Event { get; set; }

        //string Image { get; set; }
        //string ImageLocation { get; set; }

        //string Prefix { get; set; }
        //string Sufix { get; set; }

        //int? GroupIndex { get; set; }
        //bool? Checked { get; set; }
        //string CheckedStr { get; set; }
        //string GroupIndexStr { get; set; }
    }

    internal interface IJModelEditorCheck
    {
    }

    internal interface IJModelEditorLabel
    {
    }

    internal interface IJModelEditorButton
    {
    }
}