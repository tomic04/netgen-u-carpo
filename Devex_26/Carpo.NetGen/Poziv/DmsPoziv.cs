using System;
using System.Data;
using System.Globalization;
using System.Reflection;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Poziv
{
    /// <summary>
    /// DMS kartica podloge (Form1.vb 2433-2464, 8429-8700): DMSDET puni popis dokumenata,
    /// desni klik i dvoklik zovu DmsManagerV1 kasnim vezanjem (Assembly.Load), SQL ide izravno (DMS-01).
    /// </summary>
    public static class DmsPoziv
    {
        public const string Sklop = "DmsManagerV1";
        public const string Start = "DmsManagerV1.Start";
        public const string Dokumenti = "DmsManager.Db.Documents";

        /// <summary>8468: DMS_odustani.</summary>
        public static string OdustaniSql(string dokId)
        {
            return "exec dms.spSetDocumentUndoCheckOutOperater @dokId='" + (dokId ?? "").Trim() + "'";
        }

        /// <summary>8579: DMS_Obrisi.</summary>
        public static string ObrisiSql(string entityId, string dokId)
        {
            return "spNetGenDMSAzur @action='delete',@EntityName='" + (entityId ?? "").Trim() + "',@selectedids ='" + (dokId ?? "").Trim() + "'";
        }

        /// <summary>
        /// 8446-8457: "Preuzmi za potpis" samo kad CheckOutOperater nije postavljen, "Vrati potpisano" i
        /// "Odustani od potpisivanja" samo kad jest.
        /// </summary>
        public static bool Preuzet(DataRow red)
        {
            if (red == null || !red.Table.Columns.Contains("CheckOutOperater"))
                return false;
            return red["CheckOutOperater"] != DBNull.Value;
        }

        public static string Celija(DataRow red, string stupac)
        {
            if (red == null || !red.Table.Columns.Contains(stupac) || red[stupac] == DBNull.Value)
                return "";
            return (Convert.ToString(red[stupac], CultureInfo.CurrentCulture) ?? "").Trim();
        }

        /// <summary>
        /// Assembly.Load(DmsManagerV1), CreateInstance(tip), pa metoda. VB late binding pretvara
        /// argumente u tipove parametara; ovdje isto preko Convert.ChangeType. Vraća grešku ili "".
        /// </summary>
        public static string Pozovi(string tip, string metoda, params object[] argumenti)
        {
            Assembly a;
            try
            {
                a = Assembly.Load(Sklop);
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška kod Assembly.Load DMSScan : " + ex.Message);
                return ex.Message;
            }
            object o;
            try
            {
                o = a.CreateInstance(tip);
                if (o == null)
                    throw new InvalidOperationException("CreateInstance " + tip + " je vratio null");
            }
            catch (Exception ex)
            {
                NgDnevnik.Greska("Greška u  CreateInstance " + tip + " : " + ex.Message);
                return ex.Message;
            }
            try
            {
                MethodInfo m = Metoda(o.GetType(), metoda, argumenti.Length);
                if (m == null)
                    throw new MissingMethodException(tip, metoda);
                ParameterInfo[] p = m.GetParameters();
                var arg = new object[p.Length];
                for (int i = 0; i < p.Length; i++)
                    arg[i] = Pretvori(argumenti[i], p[i].ParameterType);
                NgDnevnik.Akcija("Zovem " + metoda + " u " + tip);
                m.Invoke(o, arg);
                return "";
            }
            catch (Exception ex)
            {
                Exception unutra = ex is TargetInvocationException && ex.InnerException != null ? ex.InnerException : ex;
                NgDnevnik.Greska("Greška u  " + metoda + " u " + tip + " : " + unutra.Message);
                return unutra.Message;
            }
        }

        private static MethodInfo Metoda(Type t, string ime, int broj)
        {
            MethodInfo[] sve = t.GetMethods(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < sve.Length; i++)
            {
                if (string.Equals(sve[i].Name, ime, StringComparison.OrdinalIgnoreCase) && sve[i].GetParameters().Length == broj)
                    return sve[i];
            }
            return null;
        }

        private static object Pretvori(object v, Type cilj)
        {
            if (v == null || cilj == typeof(object) || cilj.IsInstanceOfType(v))
                return v;
            Type t = Nullable.GetUnderlyingType(cilj) ?? cilj;
            return Convert.ChangeType(v, t, CultureInfo.InvariantCulture);
        }
    }
}
