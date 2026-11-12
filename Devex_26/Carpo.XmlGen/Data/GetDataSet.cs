using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Carpo.XmlGen.Data
{
    public class GetDataSet : IDisposable
    {
        public static void FillDataList(Dictionary<string, DataTable> listDataTable,
            Carpo.Structure.Models.Core.XmlGen.JDataList jDataList)
        {
            Carpo.XmlGen.Start.ErrorMethodStr = "Data.GetDataSet.FillDataList()";
            try
            {
                jDataList.Data = SortList(jDataList);

                foreach (var jData in jDataList.Data)
                {
                    if (string.IsNullOrEmpty(jData.ParentDataset))
                        AddDataTable(jData, listDataTable, null);
                    else
                        GoThroughParent(jData, listDataTable);
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static List<Carpo.Structure.Models.Core.XmlGen.JData> SortList(Carpo.Structure.Models.Core.XmlGen.JDataList jDataList)
        {
            List<Carpo.Structure.Models.Core.XmlGen.JData> lista = new List<Structure.Models.Core.XmlGen.JData>();

            foreach (var jData in jDataList.Data) // dodajem prvo one neovisne o nikom
            {
                if (string.IsNullOrEmpty(jData.ParentDataset))
                    lista.Add(jData);
            }

            Rekurzija(lista, jDataList);

            return lista;
        }

        public static void Rekurzija(List<Carpo.Structure.Models.Core.XmlGen.JData> lista, Carpo.Structure.Models.Core.XmlGen.JDataList jDataList)
        {
            foreach (var jData in jDataList.Data)
            {
                if (!lista.Contains(jData) && lista.Where(x => x.Name == jData.ParentDataset).FirstOrDefault() != null)
                    lista.Add(jData);
            }

            if (lista.Count > jDataList.Data.Count)
                Rekurzija(lista, jDataList);
        }

        public static void AddDataTable(Structure.Models.Core.XmlGen.JData jData, Dictionary<string, DataTable> listDataTable, int? index, DataRow parentRow = null)
        {
            var outParams = new List<Carpo.Data.Models.OutputParametersModel.OutParModel>();

            string sqlText = "";
            var parameters = new Dictionary<string, Object>();
            foreach (var par in jData.DataParameterList)
            {
                if (par.ValueField != null)
                {
                    string rowValue = parentRow[par.ValueField].ToString();

                    if (!string.IsNullOrEmpty(par.Type) && par.Type == "int")
                        parameters.Add(par.Name, Convert.ToInt32(rowValue));
                    else
                        parameters.Add(par.Name, rowValue);

                    continue;
                }
                if (!string.IsNullOrEmpty(par.Type) && par.Type == "int")
                    parameters.Add(par.Name, Convert.ToInt32(par.Value));
                else
                    parameters.Add(par.Name, par.Value.ToString());
            }

            var ds = Carpo.Data.Common.ExecuteQuery(Global.ConnectionString, jData.Command, jData.Type == "text" ? Carpo.Data.Common.DataType.Text : Carpo.Data.Common.DataType.StoredProcedure,
                parameters, out sqlText, Convert.ToInt32(jData.Timeout), out outParams);

            if (ds != null == ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (string.IsNullOrEmpty(jData.ParentDataset))
                    listDataTable.Add(jData.Name, ds.Tables[0]);
                else
                    listDataTable.Add((String.Format("parent[{0}]_{1}", index, jData.Name)), ds.Tables[0]);
            }
        }

        public static void GoThroughParent(Structure.Models.Core.XmlGen.JData jData, Dictionary<string, DataTable> listDataTable)
        {
            int index = 0;
            // TO DO - provjera je li parentdatatable null
            var parentDataTable = listDataTable.Where(x => x.Key == jData.ParentDataset).FirstOrDefault();

            foreach (DataRow row in parentDataTable.Value.Rows)
            {
                AddDataTable(jData, listDataTable, index, row);
                index++;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (disposedValue)
                return;
            else
            {
                disposedValue = true;

                if (this != null)
                    this.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}