using System;
using System.Data;
using System.Windows.Forms;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Model;
using Carpo.NetGen.Shell;
using Carpo.NetGen.Xml;

namespace Carpo.NetGen.Ui
{
    /// <summary>
    /// Ulaz granule. Manager samo kad OdabirForme.JeManager. Ulaz `:Forma` je dijalog.
    /// </summary>
    public static class Prozor
    {
        public static Form Otvori(string xml, string server, string baza, int operater, out string greska)
        {
            greska = null;
            var r = NetGenXmlReader.Ucitaj(xml);
            if (!r.Uspjeh || r.Projekt == null)
            {
                greska = r.Greska ?? "XML nije učitan.";
                return null;
            }
            if (OdabirForme.JeManager(r.Projekt))
            {
                var podloga = new Podloga();
                if (!podloga.Prikazi(xml, server, baza, operater))
                    greska = podloga.Text;
                return podloga;
            }
            string ulaz = OdabirForme.Ulaz(r.Projekt);
            if (ulaz.StartsWith(":", StringComparison.Ordinal))
            {
                NgForm ulazna = NadjiFormu(r.Projekt, ulaz);
                if (!DijalogTok.OtvaraProzor(ulazna))
                {
                    if (ulazna == null)
                        greska = "Nema forme " + ImeUlaza(ulaz);
                    return null;
                }
                Form dijalog = DijalogUlaza(r.Projekt, ulaz, server, baza, operater, xml);
                if (dijalog == null)
                    greska = "Nema forme " + ImeUlaza(ulaz);
                return dijalog;
            }
            var forma = new ObicnaForma();
            if (!forma.Prikazi(r.Projekt, server, baza, operater, xml))
                greska = forma.Text;
            return forma;
        }

        private static string ImeUlaza(string ulaz)
        {
            string ime = (ulaz ?? "").Trim();
            if (ime.StartsWith(":", StringComparison.Ordinal))
                ime = ime.Substring(1).Trim();
            int hash = ime.IndexOf('#');
            if (hash >= 0)
                ime = ime.Substring(0, hash).Trim();
            return ime;
        }

        private static NgForm NadjiFormu(NgProject projekt, string ulaz)
        {
            string ime = ImeUlaza(ulaz);
            if (projekt == null || projekt.Forme == null)
                return null;
            for (int i = 0; i < projekt.Forme.Count; i++)
            {
                if (string.Equals(projekt.Forme[i].Ime, ime, StringComparison.OrdinalIgnoreCase))
                    return projekt.Forme[i];
            }
            return null;
        }

        private static Form DijalogUlaza(NgProject projekt, string ulaz, string server, string baza, int operater, string xml)
        {
            NgForm trazena = NadjiFormu(projekt, ulaz);
            if (trazena == null || !DijalogTok.OtvaraProzor(trazena))
                return null;
            var d = new Dijalog();
            var kontekst = new SqlKontekst();
            kontekst.OperaterId = operater.ToString();
            kontekst.XmlName = System.IO.Path.GetFileName(xml ?? "");
            kontekst.Node = "null";
            d.Prikazi(trazena, kontekst, delegate (string sql) { return Citaj(server, baza, sql); }, false);
            return d;
        }

        private static DataTable Citaj(string server, string baza, string sql)
        {
            if (string.IsNullOrWhiteSpace(sql) || SqlTekst.JeZabranaPisanja(sql))
                return null;
            var dt = new DataTable();
            using (var conn = new System.Data.SqlClient.SqlConnection(SpinVeza.Spoj(server, baza)))
            using (var cmd = new System.Data.SqlClient.SqlCommand(sql, conn))
            using (var da = new System.Data.SqlClient.SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = 30;
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }
    }
}
