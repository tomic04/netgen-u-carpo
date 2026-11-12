using Carpo.Structure.Interfaces;
using Carpo.Structure.Models.Core;
using Carpo.UI.Logic;
using DevExpress.XtraBars;
using DevExpress.XtraEditors.Controls;
using System;
using System.Windows.Forms;

namespace Carpo.UI.Components.Base
{
    public class ComponentDaysEdit : ComponentBase, IDisposable
    {
        private JDaysEditControl jDaysEditControl;
        private new JDaysEdit definition;

        private readonly CalendarControl calendar = new CalendarControl();
        private readonly PopupControlContainer popUp = new PopupControlContainer();

        public ComponentDaysEdit() : base()
        {
        }

        public ComponentDaysEdit(Control control, object def) : base(control, def)
        {
            definition = def as JDaysEdit;
            jDaysEditControl = (JDaysEditControl)control;
            Init();
            SetValue(jDaysEditControl as IJEditor);
        }

        private void Init()
        {
            DateTime startTime = DateTime.Now;
            SetEvents();

            jDaysEditControl.txtDan.EnterMoveNextControl = true;

            this.popUp.Manager = jDaysEditControl.barManager1;
            this.popUp.Controls.Add(calendar);
            this.calendar.Dock = DockStyle.Fill;
            this.popUp.AutoSize = true;
            jDaysEditControl.Controls.Add(popUp);
            jDaysEditControl.cmdDatum.DropDownControl = popUp;

            if (definition.Value == null)
                jDaysEditControl.txtDan.EditValue = 0;

            if (!string.IsNullOrEmpty(definition.Caption))
                jDaysEditControl.cmdDatum.Text = definition.Caption;

            jDaysEditControl.cmdDatum.TabStop = false;
            form.Logger.InfoData("Init daysEdit ", definition.Name, startTime);
        }

        #region Events

        private void SetEvents()
        {
            calendar.DateTimeCommit += (s, e) =>
            {
                SetDays();
            };

            popUp.Popup += (s, e) =>
            {
                PopUpShow();
            };

            jDaysEditControl.txtDan.Enter += (s, e) =>
            {
                jDaysEditControl.BeginInvoke(new MethodInvoker(() =>
                {
                    jDaysEditControl.txtDan.SelectionStart = 0;
                    jDaysEditControl.txtDan.SelectionLength = jDaysEditControl.txtDan.Text.Length;
                }));
            };
        }

        private void SetDays()
        {
            DateTime baseDate = GetBaseDate();
            jDaysEditControl.txtDan.EditValue = (calendar.DateTime.Date - baseDate.Date).TotalDays;
            popUp.HidePopup();
        }

        private void PopUpShow()
        {
            DateTime baseDate = GetBaseDate();
            // Kada je basedate postavljen, pri otvaranju kalendara pozicioniraj na današnji dan (korisnik lako bira "danas")
            if (!string.IsNullOrWhiteSpace(definition.Basedate))
                calendar.DateTime = DateTime.Now.Date;
            else
                calendar.DateTime = baseDate.Date.AddDays(Convert.ToInt32(jDaysEditControl.txtDan.Text));
        }

        private DateTime GetBaseDate()
        {
            // Ako je basedate postavljen, koristi taj datum; inače koristi DateTime.Now (backward compatibility)
            if (!string.IsNullOrWhiteSpace(definition.Basedate))
            {
                try
                {
                    Parser parser = new Parser(form, jDaysEditControl, null);
                    object baseDateValue = parser.GetLogicPartValue(definition.Basedate);

                    if (baseDateValue != null)
                    {
                        if (baseDateValue is DateTime)
                            return (DateTime)baseDateValue;
                        else if (baseDateValue is string)
                        {
                            if (DateTime.TryParse(baseDateValue.ToString(), out DateTime parsedDate))
                                return parsedDate;
                        }
                    }
                }
                catch (Exception ex)
                {
                    form.Logger.Warn("DaysEdit: GetBaseDate", $"Neuspješno parsiranje basedate '{definition.Basedate}': {ex.Message}. Koristi se DateTime.Now.");
                }
            }

            // Fallback na DateTime.Now ako basedate nije postavljen ili je greška
            return DateTime.Now;
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    if (popUp != null) popUp.Dispose();
                    if (calendar != null) calendar.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ComponentDaysEdit() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support

        #endregion Events
    }
}