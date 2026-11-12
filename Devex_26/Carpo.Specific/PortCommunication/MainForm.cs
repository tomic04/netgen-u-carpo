using DevExpress.XtraEditors;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace Carpo.Specific.PortCommunication
{

    public partial class MainForm : UserControl
    {
        private List<SerialPort> serialPorts = new List<SerialPort>();
        private Dictionary<string, StringBuilder> portBuffers = new Dictionary<string, StringBuilder>();
        private Dictionary<string, Dictionary<string, MemoEdit>> memoEdits = new Dictionary<string, Dictionary<string, MemoEdit>>();
        private Dictionary<string, LabelControl> statusLabels = new Dictionary<string, LabelControl>();
        private Dictionary<string, MemoEdit> rawDataEdits = new Dictionary<string, MemoEdit>(); // Dodano za sirove podatke
        private FlowLayoutPanel panel;


        public MainForm()
        {
            InitializeComponent();            
            InitializeDynamicUI();
            InitializeSerialPorts();
            this.Load += MainForm_Load;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (this.ParentForm != null)
            {
                this.ParentForm.FormClosing += ParentForm_FormClosing;
            }
        }

        private void ParentForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            CloseSerialPorts();
        }

        private void CloseSerialPorts()
        {
            foreach (var port in serialPorts)
            {
                if (port.IsOpen)
                {
                    port.Close();
                }
            }
        }

        private void InitializeDynamicUI()
        {
            panel = new FlowLayoutPanel();
            panel.Dock = DockStyle.Fill;
            panel.AutoScroll = true;
            this.Controls.Add(panel);

            var settings = SerialPortSettings.LoadFromConfig("SerialPortConfig.xml");
            int formHeight = 0;

            foreach (var setting in settings)
            {
                TableLayoutPanel controlPanel = new TableLayoutPanel();
                controlPanel.ColumnCount = 1;
                controlPanel.RowCount = setting.Patterns.Count + 3; // +3 for status label, raw data memoEdit, and optional GET button
                controlPanel.AutoSize = true;
                controlPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
                controlPanel.Width = (int)(this.Width * 0.25);
                controlPanel.Dock = DockStyle.Top;

                memoEdits[setting.PortName] = new Dictionary<string, MemoEdit>();

                LabelControl statusLabel = new LabelControl();
                statusLabel.Text = $"Status: OK - {setting.AssignedPortName}";
                statusLabel.Font = new Font(setting.LabelFontName, setting.LabelFontSize, FontStyle.Bold); // Koristi font iz konfiguracije
                statusLabel.Appearance.ForeColor = Color.FromName(setting.LabelFontColor); // Koristi boju iz konfiguracije
                statusLabel.Dock = DockStyle.Top;
                controlPanel.Controls.Add(statusLabel, 0, 0);
                statusLabels[setting.PortName] = statusLabel;

                int rowIndex = 1; // Start adding patterns from the second row

                MemoEdit rawMemoEdit = new MemoEdit();
                rawMemoEdit.Properties.ScrollBars = ScrollBars.None; // Sakrivanje scrollbar-a
                rawMemoEdit.Height = 80; // Smanjena visina
                rawMemoEdit.Width = controlPanel.Width - 10;
                rawMemoEdit.Properties.Appearance.Font = new Font(setting.DataFontName, setting.DataFontSize); // Koristi font iz konfiguracije
                rawMemoEdit.Properties.Appearance.ForeColor = Color.FromName(setting.DataFontColor); // Koristi boju iz konfiguracije
                rawMemoEdit.Visible = setting.ShowFullSignal; // Vidljivost na osnovu showfullsignal
                controlPanel.Controls.Add(rawMemoEdit, 0, rowIndex);
                rawDataEdits[setting.PortName] = rawMemoEdit;

                rowIndex++; // Move to the next row after raw data MemoEdit

                foreach (var pattern in setting.Patterns)
                {
                    LabelControl label = new LabelControl();
                    label.Text = pattern.Key;
                    label.Font = new Font(setting.LabelFontName, setting.LabelFontSize, FontStyle.Bold); // Koristi font iz konfiguracije
                    label.Appearance.ForeColor = Color.FromName(setting.LabelFontColor); // Koristi boju iz konfiguracije
                    controlPanel.Controls.Add(label, 0, rowIndex);

                    MemoEdit memoEdit = new MemoEdit();
                    memoEdit.Properties.ScrollBars = ScrollBars.None; // Sakrivanje scrollbar-a
                    memoEdit.Height = 80; // Smanjena visina
                    memoEdit.Width = controlPanel.Width - 10;
                    memoEdit.Properties.Appearance.Font = new Font(setting.DataFontName, setting.DataFontSize); // Koristi font iz konfiguracije
                    memoEdit.Properties.Appearance.ForeColor = Color.FromName(setting.DataFontColor); // Koristi boju iz konfiguracije
                    controlPanel.Controls.Add(memoEdit, 0, rowIndex + 1);

                    memoEdits[setting.PortName][pattern.Key] = memoEdit;
                    rowIndex += 2;
                }

                if (setting.GET)
                {
                    SimpleButton getButton = new SimpleButton();
                    getButton.Text = "DOHVATI";
                    getButton.Dock = DockStyle.Top;
                    getButton.Click += (sender, e) => { SendCommandToPort(setting.PortName, setting.Command); };
                    controlPanel.Controls.Add(getButton, 0, rowIndex);
                }

                panel.Controls.Add(controlPanel);
                formHeight += controlPanel.Height + 10; // Add control panel height to form height
            }

            this.Height = formHeight + 50; // Adjust form height based on content
        }

        private void InitializeSerialPorts()
        {
            var settings = SerialPortSettings.LoadFromConfig("SerialPortConfig.xml");
            foreach (var setting in settings)
            {
                var port = new SerialPort(setting.PortName, setting.BaudRate, setting.Parity, setting.DataBits, setting.StopBits);
                try
                {
                    port.Open();
                    port.DataReceived += (sender, e) => DataReceivedHandler(sender, e, setting);
                    serialPorts.Add(port);
                }
                catch (Exception ex)
                {
                    if (statusLabels.ContainsKey(setting.PortName))
                    {
                        statusLabels[setting.PortName].Text = $"Error: {ex.Message}";
                    }
                }
                portBuffers[setting.PortName] = new StringBuilder();
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e, SerialPortSettings settings)
        {
            var port = (SerialPort)sender;
            string data = port.ReadExisting();
            StringBuilder buffer = portBuffers[port.PortName];
            buffer.Append(data);

            if (buffer.ToString().Contains(settings.EndMessage))
            {
                string completeData = buffer.ToString();
                buffer.Clear();
                ResetUI(settings.PortName); // Resetuje sve MemoEdit kontrole
                if (settings.ShowFullSignal)
                {
                    UpdateRawUI(port.PortName, completeData);
                }
                if (settings.Patterns.Any())
                {
                    foreach (var pattern in settings.Patterns)
                    {
                        Match match = Regex.Match(completeData, pattern.Value);
                        if (match.Success)
                        {
                            // Pretpostavljamo da je ttt prvi broj, a zzz drugi broj
                            if (pattern.Key == "ttt")
                            {
                                string result = match.Groups[1].Value;
                                UpdatePatternUI(port.PortName, pattern.Key, result);
                            }
                            else if (pattern.Key == "zzz")
                            {
                                string result = match.Groups[2].Value;
                                UpdatePatternUI(port.PortName, pattern.Key, result);
                            }
                        }
                    }
                }
            }
        }

        private void ResetUI(string portName)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => ResetUI(portName)));
            }
            else
            {
                rawDataEdits[portName].Text = string.Empty;
                if (memoEdits.ContainsKey(portName))
                {
                    foreach (var memoEdit in memoEdits[portName].Values)
                    {
                        memoEdit.Text = string.Empty;
                    }
                }
            }
        }

        private void UpdateRawUI(string portName, string data)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdateRawUI(portName, data)));
            }
            else
            {
                rawDataEdits[portName].Text = data + Environment.NewLine;
            }
        }

        private void UpdatePatternUI(string portName, string patternKey, string data)
        {
            if (this.InvokeRequired)
            {
                this.BeginInvoke(new Action(() => UpdatePatternUI(portName, patternKey, data)));
            }
            else
            {
                if (memoEdits.ContainsKey(portName) && memoEdits[portName].ContainsKey(patternKey))
                {
                    memoEdits[portName][patternKey].Text = data + Environment.NewLine;
                }
            }
        }

        private void SendCommandToPort(string portName, string command)
        {
            var port = serialPorts.FirstOrDefault(p => p.PortName == portName);
            if (port != null && port.IsOpen)
            {
                port.WriteLine(command);
            }
        }
    }
}
