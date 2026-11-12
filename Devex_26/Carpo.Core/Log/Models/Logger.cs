using System;
using System.ComponentModel;

namespace Carpo.Core.Log.Models
{
    public class LoggerDef
    {
        public DateTime Time { get; set; }
        public LogLevel LogLevel { get; set; }
        public string Opis { get; set; }
        public string Text { get; set; }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public object[] Args { get; set; }
    }
}