using Carpo.Common.Helpers;
using Carpo.Core;
using Carpo.UI.Connector.Abstract;
using DevExpress.XtraEditors.Controls;
using System;

namespace Carpo.Gen
{
    public class Start : Starter
    {
        //public override string xmlPath // potreban da se može testno pozivati Carpo -> Carpo sa definiranjem putanje
        //{
        //    get;set;
        //}
        public override void RegisterForm()
        {
            var startTime = DateTime.Now;
            Localizer.Active = new Carpo.Common.Localization.XtraEditorsLocalizer(); // DODAO OGNJEN

            Carpo.UI.Forms.Jupiter.Base frm;
            if (DefinitionRoot.LogLevel != null)
                Logger.LogLevelUser = Convert.ToInt32(DefinitionRoot.LogLevel);

            if (FormType == FormTypeEnum.Manager)
                frm = new Forms.Manager(DefinitionRoot, InitForm, Logger, ConnectionString, ModuleId);
            else
            {
                if (InitForm.Template != null && InitForm.Template.ToLower().Contains("splitter"))
                    frm = new Forms.Splitter(DefinitionRoot, InitForm, Logger, ConnectionString, ModuleId);
                else if (InitForm.Template != null && InitForm.Template.ToLower().Contains("info"))
                    frm = new Forms.Info(DefinitionRoot, InitForm, Logger, ConnectionString, ModuleId);
                else if (InitForm.Template != null && InitForm.Template.ToLower().Contains("sidebar"))
                    frm = new Forms.SideBar(DefinitionRoot, InitForm, Logger, ConnectionString, ModuleId);
                else
                    frm = new Forms.Standard(DefinitionRoot, InitForm, Logger, ConnectionString, ModuleId);

                var frmJupiterEditor = frm as Carpo.UI.Forms.Base.JupiterEditor;
                frmJupiterEditor.SetLayout();
                frmJupiterEditor.SetControlAppValues(ModuleId);
                frmJupiterEditor.InitializeCustomControls();
            }
            Logger.InfoTest("RegisterForm p1", "set: layout,variable, controls", startTime);
            RegisterForm(frm);
        }
    }
}