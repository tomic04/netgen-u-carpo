using Carpo.Structure.Models.Core;
using System;
using System.Data;

namespace Carpo.UI.Logic
{
    public class OperaterLock
    {
        private JForm definitionForm;
        private Forms.Base.JupiterShared form;
        public bool Locking { get; set; } = false;
        private int operaterLockValue;

        public OperaterLock(JForm definitionForm, Forms.Base.JupiterShared form)
        {
            this.definitionForm = definitionForm;
            this.form = form;
        }

        public bool CheckOperaterLock(JForm definitionForm)
        {
            bool lockOperater = false;
            string operaterName = "";

            if (definitionForm != null && (!string.IsNullOrEmpty(definitionForm.OperaterLockTable) || !string.IsNullOrEmpty(definitionForm.OperaterLockKey) || !string.IsNullOrEmpty(definitionForm.OperaterLockKeyValue)))
            {
                lockOperater = IsOperaterLocked(definitionForm, out operaterName);
            }
            if (lockOperater == true)
            {
                Common.Helpers.JMessages.Warning("Pristup dokumentu nije moguć jer je zaključan od strane drugog operatera (" + operaterName.Trim() + ")");
            }

            return lockOperater;
        }

        private bool IsOperaterLocked(JForm definitionForm, out string operaterName)
        {
            operaterName = "";
            if (definitionForm != null)
            {
                if (string.IsNullOrEmpty(definitionForm.OperaterLockTable) || string.IsNullOrEmpty(definitionForm.OperaterLockKey) || string.IsNullOrEmpty(definitionForm.OperaterLockKeyValue))
                {
                    form.Logger.Warn("method: CheckOperaterLock", "Jedan ili više atributa nisu (dobro) definirani; ");
                }
                else if (SetOperaterLockValue(definitionForm) == null)
                {
                    form.Logger.Warn("method: SetOperaterLockValue", "Neuspješno parsiranje vrijednosti; ");
                }
                else
                {
                    string sqlText = "";
                    try
                    {
                        DateTime startTime = DateTime.Now;
                        using (DataTable dt = Carpo.Data.Get.OperaterLock.OperaterLockCheck(form.ConnectionString, definitionForm.OperaterLockTable, definitionForm.OperaterLockKey, (int)SetOperaterLockValue(definitionForm), Core.PubVars.OperateriId, out sqlText).Tables[0])
                        {
                            if (!string.IsNullOrEmpty(sqlText))
                                form.Logger.InfoData("get OperaterLock", sqlText, startTime);
                            object value = dt.Rows[0][0];
                            if (value != DBNull.Value)
                            {
                                operaterName = value.ToString();
                                return true;
                            }
                            else
                                Locking = true;
                        }
                    }
                    catch (Exception ex)
                    {
                        form.Logger.Error("method: IsOperaterLocked", ex.Message, ex);
                    }
                }
            }
            return false;
        }

        private int? SetOperaterLockValue(JForm definitionForm)
        {
            if (definitionForm != null && !string.IsNullOrEmpty(definitionForm.OperaterLockKeyValue))
            {
                using (Parser parser = new Parser(form, null))
                {
                    object value = parser.GetLogicPartValue(definitionForm.OperaterLockKeyValue.Trim());
                    if (value != null)
                    {
                        operaterLockValue = Convert.ToInt32(value);
                        return Convert.ToInt32(value);
                    }
                    else
                        return null;
                }
            }
            else
                return null;
        }

        public void UnLock()
        {
            if (Locking != true)
                return;

            string sqlText = "";
            DateTime startTime = DateTime.Now;
            Carpo.Data.Set.OperaterLock.Unlock(form.ConnectionString, definitionForm.OperaterLockTable, definitionForm.OperaterLockKey, (int)operaterLockValue, out sqlText);
            if (!string.IsNullOrEmpty(sqlText))
                form.Logger.InfoData("set OperaterLock", sqlText, startTime);
        }
    }
}