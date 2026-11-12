using Carpo.Structure.Models.Core;
using Carpo.UI.Helpers;
using DevExpress.Office.Utils;
using DevExpress.Utils;
using DevExpress.Utils.Commands;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.Commands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using IRichEditControl = Carpo.Structure.Interfaces.IRichEditControl;

namespace Carpo.UI.Components.Editor
{
    [ToolboxItem(true)]
    public partial class JRichEditControl : RichEditControl, IRichEditControl
    {
        public Base.ComponentTextEdit ComponentExt { get; set; }
        private JRichEdit definition;
        private ToolStripDropDown formatBar;
        private ToolStripButton btnBold, btnItalic, btnUnderline;

        public JRichEditControl() { }

        public JRichEditControl(JRichEdit definition)
        {
            this.definition = definition;
            MinimumSize = new Size(DefaultMinimumSize.Width, 50);
            InitializeComponent();
            SetupFormatBar();

            Views.SimpleView.Padding = new DevExpress.Portable.PortablePadding(2);

            var c = Options.DocumentCapabilities;

            c.CharacterFormatting = DocumentCapability.Enabled;

            c.ParagraphFormatting = DocumentCapability.Disabled;
            c.InlinePictures = DocumentCapability.Disabled;
            c.FloatingObjects = DocumentCapability.Disabled;
            c.Tables = DocumentCapability.Disabled;
            c.Hyperlinks = DocumentCapability.Disabled;
            c.Bookmarks = DocumentCapability.Disabled;
            c.Fields = DocumentCapability.Disabled;
            c.Comments = DocumentCapability.Disabled;
            c.HeadersFooters = DocumentCapability.Disabled;
            c.Sections = DocumentCapability.Disabled;
            c.CharacterStyle = DocumentCapability.Disabled;
            c.ParagraphStyle = DocumentCapability.Disabled;
            c.Numbering.Bulleted = DocumentCapability.Disabled;
            c.Numbering.Simple = DocumentCapability.Disabled;
            c.Numbering.MultiLevel = DocumentCapability.Disabled;

            ActiveViewType = RichEditViewType.Simple;
            Options.Annotations.ShowAllAuthors = false;
            Options.DocumentSaveOptions.CurrentFormat = DocumentFormat.PlainText;
            Options.HorizontalRuler.ShowLeftIndent = false;
            Options.HorizontalRuler.ShowRightIndent = false;
            Options.HorizontalRuler.ShowTabs = false;
            Options.HorizontalRuler.Visibility = RichEditRulerVisibility.Hidden;
            Options.HorizontalScrollbar.Visibility = RichEditScrollbarVisibility.Hidden;
            Options.VerticalRuler.Visibility = RichEditRulerVisibility.Hidden;

            RichToPoorConverter.ApplyFont(Document, AppearanceObject.DefaultFont);

            KeyDown += (s, e) => formatBar.Close();
            MouseDown += (s, e) => formatBar.Close();
            Leave += (s, e) =>
            {
                formatBar.Close();
                Document.Selection = Document.CreateRange(Document.Selection.End, 0);
            };
        }
        private Point GetSelectionStartPoint(Point fallback)
        {
            var rect = GetBoundsFromPosition(Document.Selection.Start);
            if (rect.IsEmpty) return fallback;

            rect = Units.DocumentsToPixels(rect, DpiX, DpiY);
            return new Point(rect.Left, rect.Top - 2);
        }
        private void SetupFormatBar()
        {
            btnBold = CreateFormatButton("B", FontStyle.Bold, () => new ToggleFontBoldCommand(this).Execute());
            btnItalic = CreateFormatButton("I", FontStyle.Italic, () => new ToggleFontItalicCommand(this).Execute());
            btnUnderline = CreateFormatButton("U", FontStyle.Underline, () => new ToggleFontUnderlineCommand(this).Execute());

            formatBar = new ToolStripDropDown
            {
                LayoutStyle = ToolStripLayoutStyle.HorizontalStackWithOverflow,
                Padding = new Padding(2),
                AutoClose = false
            };

            formatBar.MouseLeave += (s, e) => formatBar.Close();

            formatBar.Items.AddRange([btnBold, btnItalic, btnUnderline]);

            formatBar.Closing += (s, e) =>
            {
                if (e.CloseReason == ToolStripDropDownCloseReason.ItemClicked) e.Cancel = true;
            };

            MouseUp += (s, e) =>
            {
                bool selectedWithMouse = e.Button == MouseButtons.Left && Document.Selection.Length > 0;
                if (!selectedWithMouse) return;

                UpdateFormatButtons();

                var location = Document.Selection.Length > 0 ? GetSelectionStartPoint(e.Location) : e.Location;

                formatBar.Show(this, location, ToolStripDropDownDirection.AboveRight);
            };

            Disposed += (s, e) => formatBar.Dispose();
        }
        private ToolStripButton CreateFormatButton(string text, FontStyle style, Action command)
        {
            var btn = new ToolStripButton(text)
            {
                DisplayStyle = ToolStripItemDisplayStyle.Text,
                Font = new Font(AppearanceObject.DefaultFont, style),
                AutoSize = false,
                Size = new Size(26, 24)
            };

            btn.Click += (s, e) =>
            {
                command();
                UpdateFormatButtons();
            };

            return btn;
        }
        private void UpdateFormatButtons()
        {
            btnBold.Checked = IsChecked(new ToggleFontBoldCommand(this));
            btnItalic.Checked = IsChecked(new ToggleFontItalicCommand(this));
            btnUnderline.Checked = IsChecked(new ToggleFontUnderlineCommand(this));
        }

        private static bool IsChecked(Command command)
        {
            ICommandUIState state = command.CreateDefaultCommandUIState();
            command.UpdateUIState(state);
            return state.Checked;
        }
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            ComponentExt = new Base.ComponentTextEdit(this, definition);
        }

        public void SetEvents(object ev)
        {
            if (ev is List<JEvent>)
                ComponentExt.SetEvents(ev as List<JEvent>);
            else if (ev is JEvent)
                ComponentExt.SetEvents(ev as JEvent);
        }

        public void SetNotifications(JEventAction action, object value)
        {
            ComponentExt.SetNotify(action, value);
        }

        public object GetValue(string helperStr)
        {
            string html = RichToPoorConverter.FromRtf(this.RtfText);
            return html;
        }

        public void SetValue(object value, string text)
        {
            if (value != null && value is string)
                value = value.ToString().Trim();

            this.RtfText = RichToPoorConverter.ToRtf(value.ToString());
        }

        public void SetItemCaption(object caption)
        {
            ComponentExt.SetItemCaption(caption);
        }

        public void SetEnabled(bool enabled)
        {
            this.Enabled = enabled;
        }

        public void SetVisibility(JEventAction action)
        {
            ComponentExt.SetVisibility(action);
        }

        public void RefreshValue()
        {
            ComponentExt.RefreshValue();
        }

        public void SetAttribute(JEventAction action)
        {
            ComponentExt.SetAttribute(action);
        }

        public object GetKeyCode()
        {
            return ComponentExt.GetKeycode();
        }
    }
}
