using System.Drawing;
using System.Windows.Forms;
using Carpo.NetGen.Xml;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraGrid.Views.Grid;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Vizualni defaulti kao Carpo komponente. Ne mijenja vrijednost ni SQL.
    /// </summary>
    static class CarpoIzgled
    {
        public static void Primijeni(Control kontrola, NgGrana grana)
        {
            var tekst = kontrola as TextEdit;
            if (tekst != null && !(kontrola is DateEdit) && !(kontrola is LookUpEdit))
                tekst.EnterMoveNextControl = true;
            var broj = kontrola as SpinEdit;
            if (broj != null)
                broj.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Far;
            var datum = kontrola as DateEdit;
            if (datum != null)
                Datum(datum);
            var combo = kontrola as LookUpEdit;
            if (combo != null)
                Combo(combo);
            var trazi = kontrola as ButtonEdit;
            if (trazi != null && combo == null)
            {
                trazi.Properties.NullValuePrompt = "...";
                trazi.Properties.NullValuePromptShowForEmptyValue = true;
                trazi.Properties.TextEditStyle = TextEditStyles.Standard;
            }
            var check = kontrola as CheckEdit;
            if (check != null)
            {
                check.EnterMoveNextControl = true;
                check.Properties.AllowHtmlDraw = DefaultBoolean.True;
            }
            var gumb = kontrola as SimpleButton;
            if (gumb != null)
                Gumb(gumb);
        }

        public static void Datum(DateEdit datum)
        {
            datum.EnterMoveNextControl = true;
            datum.Properties.Mask.MaskType = MaskType.DateTimeAdvancingCaret;
            datum.Properties.ShowClear = false;
            datum.Properties.VistaDisplayMode = DefaultBoolean.False;
            datum.Properties.AllowFocused = false;
            datum.Properties.AllowDropDownWhenReadOnly = DefaultBoolean.False;
            datum.Properties.AllowNullInput = DefaultBoolean.False;
        }

        public static void Combo(LookUpEdit combo)
        {
            combo.EnterMoveNextControl = true;
            combo.Properties.NullText = "...";
            combo.Properties.NullValuePrompt = "...";
            combo.Properties.NullValuePromptShowForEmptyValue = true;
            combo.Properties.ShowNullValuePromptWhenFocused = false;
            combo.Properties.TextEditStyle = TextEditStyles.Standard;
            combo.Properties.ImmediatePopup = true;
            combo.Properties.ShowHeader = false;
            combo.Properties.ShowFooter = false;
        }

        public static void Gumb(SimpleButton gumb)
        {
            gumb.Appearance.Font = new Font(gumb.Font.FontFamily, 7f, FontStyle.Bold);
            gumb.Appearance.TextOptions.WordWrap = WordWrap.Wrap;
            if (gumb.Width < 70)
                gumb.Width = 90;
        }

        public static void Pogled(GridView pogled)
        {
            pogled.OptionsView.EnableAppearanceOddRow = true;
            pogled.OptionsView.AllowHtmlDrawHeaders = true;
            pogled.Appearance.HeaderPanel.TextOptions.WordWrap = WordWrap.Wrap;
            pogled.OptionsFilter.AllowAutoFilterConditionChange = DefaultBoolean.False;
            pogled.OptionsBehavior.AllowIncrementalSearch = true;
        }
    }
}
