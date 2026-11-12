using Carpo.Core;
using Carpo.Structure.Models.Core;
using Carpo.Structure.Models.Core.Base;
using System.Data;

namespace Carpo.UI.Logic.Data
{
    public class DataDefinition
    {
        /****start - varijable potrebne prilikom serijalizacije xml-a****/
        public JRoot DefinitionRoot { get; set; }
        public Logger Log { get; set; }
        /****end****/

        private Forms.Base.JupiterShared form;
        private JModelEditorDataBase definition;
        private string dataName = null;

        public DataDefinition(Forms.Base.JupiterShared form, JModelEditorDataBase definition)
        {
            this.definition = definition;
            SetParameters(form, definition.DataName);
        }

        public DataDefinition(Forms.Base.JupiterShared form, string dataName)
        {
            SetParameters(form, dataName);
        }

        private void SetParameters(Forms.Base.JupiterShared form, string dataName)
        {
            this.form = form;
            this.dataName = dataName;
        }

        public DataSet GetDataSet(string infoOpis)
        {
            bool stopActions = false;
            return GetDataSet(infoOpis, out stopActions);
        }

        public DataSet GetDataSet(string infoOpis, out bool stopActions)
        {
            //****za potrebe partsql-a - prilikom serializacije nema još aktivirane forme
            var connStr = "";
            if (form != null)
                connStr = form.ConnectionString;
            else
                connStr = Carpo.Data.Connection.ConnectionString;
            //****

            Logic.Data.Core data = new Logic.Data.Core()
            { ConnectionString = connStr, ParameterControlValue = "", CurrentForm = form };

            //****za potrebe partsql-a
            if (DefinitionRoot != null)
                data.DefinitionRoot = DefinitionRoot;
            if (Log != null)
                data.Log = Log;
            //****

            JData jDataDef = null;
            if (definition != null && definition.Data != null && definition.Data.Count > 0)
                jDataDef = data.FindJData(definition.Data);
            else if (!string.IsNullOrEmpty(dataName))
                jDataDef = data.GetDataByName(dataName);

            DataSet dataSet = data.GetDataSet(jDataDef, infoOpis, out stopActions);
            return dataSet;
        }

        public JData GetJData()
        {
            string dataName = "";
            dataName = definition.DataName;

            Logic.Data.Core data = new Logic.Data.Core()
            { ConnectionString = form.ConnectionString, ParameterControlValue = "", CurrentForm = form };

            JData jDataDef = null;
            if (definition.Data != null && definition.Data.Count > 0)
                jDataDef = data.FindJData(definition.Data);
            else if (!string.IsNullOrEmpty(dataName))
                jDataDef = data.GetDataByName(dataName);

            return jDataDef;
        }
    }
}