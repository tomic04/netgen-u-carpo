using Carpo.Structure.Models.Core;
using System.Collections.Generic;

namespace Carpo.Structure.Interfaces
{
    internal interface IJModelDataBase
    {
        //JData Data { get; set; }
        //List<JData> DataList { get; set; }
        List<JData> Data { get; set; }

        JFields Fields { get; set; }
        string ValueField { get; set; }
        string DisplayField { get; set; }
        string Required { get; set; }
        //string RequiredStr { get; set; }
    }

    internal interface IJModelDataDef
    {
        string DataName { get; set; }
        string DataValue { get; set; }
    }

    internal interface IJModelDataItem
    {
        string Text { get; set; }
        string Value { get; set; }
    }
}