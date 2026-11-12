using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Xml.Linq;

public class SerialPortSettings
{
    public string PortName { get; set; }
    public int BaudRate { get; set; }
    public int DataBits { get; set; }
    public Parity Parity { get; set; }
    public StopBits StopBits { get; set; }
    public string Command { get; set; }
    public string EndMessage { get; set; }
    public bool GET { get; set; }
    public string AssignedPortName { get; set; }
    public bool ShowFullSignal { get; set; } // Dodano za kontrolu prikaza glavnog MemoEdit-a
    public string DataFontName { get; set; } // Font name for MemoEdit data
    public float DataFontSize { get; set; } // Font size for MemoEdit data
    public string DataFontColor { get; set; } // Font color for MemoEdit data
    public string LabelFontName { get; set; } // Font name for labels
    public float LabelFontSize { get; set; } // Font size for labels
    public string LabelFontColor { get; set; } // Font color for labels
    public Dictionary<string, string> Patterns { get; set; } = new Dictionary<string, string>();

    public static List<SerialPortSettings> LoadFromConfig(string configPath)
    {
        XDocument doc = XDocument.Load(configPath);
        return doc.Descendants("Port").Select(p => new SerialPortSettings
        {
            PortName = p.Element("PortName").Value,
            BaudRate = int.Parse(p.Element("BaudRate").Value),
            DataBits = int.Parse(p.Element("DataBits").Value),
            Parity = (Parity)Enum.Parse(typeof(Parity), p.Element("Parity").Value),
            StopBits = (StopBits)Enum.Parse(typeof(StopBits), p.Element("StopBits").Value),
            Command = p.Element("Command").Value,
            EndMessage = p.Element("EndMessage").Value,
            GET = (bool?)p.Element("GET") ?? false,
            AssignedPortName = p.Element("assignedPortName")?.Value,
            ShowFullSignal = (bool?)p.Element("showfullsignal") ?? false,
            DataFontName = p.Element("DataFontName")?.Value ?? "Arial",
            DataFontSize = (float?)p.Element("DataFontSize") ?? 10,
            DataFontColor = p.Element("DataFontColor")?.Value ?? "Black",
            LabelFontName = p.Element("LabelFontName")?.Value ?? "Arial",
            LabelFontSize = (float?)p.Element("LabelFontSize") ?? 12,
            LabelFontColor = p.Element("LabelFontColor")?.Value ?? "Black",
            Patterns = p.Element("Patterns")?.Elements().ToDictionary(k => k.Name.ToString(), v => v.Value) ?? new Dictionary<string, string>()
        }).ToList();
    }
}

