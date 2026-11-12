using Carpo.Structure.Models.Core;
using System.Collections.Generic;

namespace Carpo.UI.Connector
{
    public class SettingsMaster
    {
        public JGrid GridMaster { get; set; } = new JGrid();

        //glavni detail grid;
        //ako ima više detail gridova (unutar tab kontrole), prvi grid untar tab elementa je glavni (brojač slogova...)
        public JGrid GridDetail { get; set; } = new JGrid();

        public JTree TreeMaster { get; set; } = new JTree();
        public JComboEdit ComboEdit { get; set; } = new JComboEdit();
        public JPanel PanelButtons { get; set; } = new JPanel();
        public List<JForm> Forms { get; set; }
        public JTabs TabsDetail { get; set; }
        public JTabs TabsMaster { get; set; }
    }
}