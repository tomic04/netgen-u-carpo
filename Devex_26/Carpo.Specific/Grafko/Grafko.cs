using Carpo.Specific.Grafko.Models;
using DevExpress.Diagram.Core;
using DevExpress.XtraDiagram;
using DevExpress.XtraPrinting.Native;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace Carpo.Specific.Grafko
{
    public partial class Grafko : UserControl
    {
        private int selectedId = -1;
        private int vrstaDokId = -1;
        private string tablica = "";

        private int glaId = -1;
        private string nazivTablice = "";
        private int operateriId = -1;
        private string action = "";
        private int postavljenoArgumenata = 0;
        private int potrebnoArgumenata = 3;

        public int SelectedId
        {
            get
            {
                return selectedId;
            }
            set
            {
                selectedId = value;
            }
        }

        public int VrstaDokId
        {
            get
            {
                return vrstaDokId;
            }
            set
            {
                vrstaDokId = value;
            }
        }

        public string Tablica
        {
            get
            {
                return tablica;
            }
            set
            {
                tablica = value;
            }
        }

        public int GlaID
        {
            get
            {
                return glaId;
            }
            set
            {
                glaId = value;
                if (++postavljenoArgumenata == potrebnoArgumenata)
                {
                    diagramGenerations = Graf.CrtajGraf(glaId, nazivTablice, operateriId, action);
                    drawNodes(diagramGenerations);
                }
            }
        }

        public string NazivTablice
        {
            get
            {
                return nazivTablice;
            }
            set
            {
                nazivTablice = value;
                if (++postavljenoArgumenata == potrebnoArgumenata)
                {
                    diagramGenerations = Graf.CrtajGraf(glaId, nazivTablice, operateriId, action);
                    drawNodes(diagramGenerations);
                }
            }
        }

        public int OperateriId
        {
            get
            {
                return operateriId;
            }
            set
            {
                operateriId = value;
                if (++postavljenoArgumenata == potrebnoArgumenata)
                {
                    diagramGenerations = Graf.CrtajGraf(glaId, nazivTablice, operateriId, action);
                    drawNodes(diagramGenerations);
                }
            }
        }

        public string Action
        {
            get
            {
                return action;
            }
            set
            {
                action = value;
                if (action != "" && postavljenoArgumenata == potrebnoArgumenata)
                {
                    diagramGenerations = Graf.CrtajGraf(glaId, nazivTablice, operateriId, action);
                    drawNodes(diagramGenerations);
                }
            }
        }

        private Dictionary<int, DiagramShape> diagramShapes;
        private List<NodeForDiagram> diagramGenerations;

        public Grafko()
        {
            InitializeComponent();
        }

        private void drawNodes(List<NodeForDiagram> diagramGenerations)
        {
            diagram.Items.Clear();
            try
            {
                diagramShapes = new Dictionary<int, DiagramShape>();
                foreach (var node in diagramGenerations)
                {
                    DiagramShape diagramItem = new DiagramShape();
                    diagramItem.Height = node.Height;
                    diagramItem.Width = node.Width;
                    diagramItem.Position = new DevExpress.Utils.PointFloat(node.xPosition, node.yPosition);
                    diagramItem.Shape = DevExpress.Diagram.Core.BasicShapes.Rectangle;
                    diagramItem.Content = node.Title;

                    diagram.Items.Add(diagramItem);

                    diagramShapes.Add(node.ValueId, diagramItem);

                    foreach (var parent in node.ParentIdsList)
                    {
                        DiagramConnector connector = new DiagramConnector(ConnectorType.Straight);
                        connector.BeginItem = diagramShapes.GetValueOrDefault(parent, new DiagramShape());
                        connector.EndItem = diagramItem;
                        connector.EndArrow = ArrowDescriptions.Filled90;
                        diagram.Items.Add(connector);
                    }
                }
                if (diagramGenerations.Count > 0)
                {
                    SelectedId = diagramGenerations.First().ValueId;
                    VrstaDokId = diagramGenerations.First().VrstaDokId;
                    Tablica = diagramGenerations.First().Tablica;
                }
                diagram.SelectionChanged += Diagram_SelectionChanged;
            }
            catch (Exception err)
            {
                Carpo.Common.LogWriter.LogLine("Grafko error" + err.Message);
            }
        }

        private void Diagram_SelectionChanged(object sender, DiagramSelectionChangedEventArgs e)
        {
            if (((DevExpress.XtraDiagram.DiagramControl)sender).SelectedItems.Count() > 0)
            {
                var res = ((DevExpress.XtraDiagram.DiagramControl)sender).SelectedItems[0] as DiagramConnector;
                if (res != null) return;
            }

            try
            {
                NodeForDiagram selected = ((DevExpress.XtraDiagram.DiagramControl)sender).SelectedItems.Count() > 0 ? diagramGenerations.First(n => n.Title == ((DevExpress.XtraDiagram.DiagramShape)((DevExpress.XtraDiagram.DiagramControl)sender).SelectedItems[0]).Content) : diagramGenerations.First();
                SelectedId = selected.ValueId;
                VrstaDokId = selected.VrstaDokId;
                Tablica = selected.Tablica;
            }
            catch (Exception err)
            {
                if (diagramGenerations.Count > 0)
                {
                    SelectedId = diagramGenerations.First().ValueId;
                    VrstaDokId = diagramGenerations.First().VrstaDokId;
                    Tablica = diagramGenerations.First().Tablica;
                }
                Carpo.Common.LogWriter.LogLine("Grafko error" + err.Message);
            }
        }
    }
}