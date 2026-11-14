using System;
using System.Data;
using System.Data.SqlClient;
using Carpo.NetGen.Grid;
using Carpo.NetGen.Komande;
using Carpo.NetGen.Log;

namespace Carpo.NetGen.Ulaz
{
    /// <summary>
    /// Čitanje za samostalni dijalog (ulaz frm, ULZ-02): ista pravila kao Podloga.Citaj i CitajZaDijalog.
    /// Preostali @@ ide u CB vrijednost ili null, produkcijski URL Porezne se ne šalje, upis samo uz
    /// DozvoliUpis (B-JUP). Greška SQL-a ide dijalogu (MSG-01), osim tihih (ssPermission, NetGenDBFilter...).
    /// </summary>
    internal sealed class FrmCitac
    {
        private readonly string _spoj;

        /// <summary>strCommandTimeout: Form2 ga mijenja svaka forma s COMMANDTIMEOUT (1035, TMO-01).</summary>
        public int Rok = VrijemeUpita.Zadano;

        public FrmCitac(string spoj)
        {
            _spoj = spoj ?? "";
        }

        public DataTable Citaj(string sql)
        {
            if (string.IsNullOrWhiteSpace(sql))
                return null;
            if (sql.IndexOf("@@", StringComparison.Ordinal) >= 0)
                sql = SrediPoslije.Petlja(sql, "", null, false);
            if (sql.IndexOf("cis.porezna-uprava.hr", StringComparison.OrdinalIgnoreCase) >= 0)
                return null;
            if (SqlTekst.JeZabranaPisanja(sql) && !DijalogTok.DozvoliUpis)
            {
                NgDnevnik.Akcija("nije poslano, zabrana pisanja: " + sql);
                return null;
            }
            NgDnevnik.Sql(sql);
            try
            {
                var dt = new DataTable();
                using (var conn = new SqlConnection(_spoj))
                using (var cmd = new SqlCommand(sql, conn))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = VrijemeUpita.Stvarno(Rok);
                    conn.Open();
                    da.Fill(dt);
                }
                return dt;
            }
            catch (SqlException ex)
            {
                NgDnevnik.Greska(ex.Message + " | " + sql);
                if (SqlPoruka.Tiha(ex.Message) || sql.IndexOf("NetGenDBFilter", StringComparison.OrdinalIgnoreCase) >= 0)
                    return null;
                throw new InvalidOperationException(ex.Message, ex);
            }
        }
    }
}
