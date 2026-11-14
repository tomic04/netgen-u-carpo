using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Carpo.NetGen.Grid
{
    /// <summary>
    /// SES-01: jedna otvorena veza po granuli, kao NetGen.
    /// NetGen otvori <c>Conn</c> jednom (Form1 1607, Module1 128 ThreadStatic) i svaki <c>Class1.ExecSql</c>
    /// (da.Fill na otvorenoj vezi) ide kroz istu SQL sesiju. Carpo je svaki upit slao kroz novu vezu iz poola;
    /// pool pri ponovnoj upotrebi pozove sp_reset_connection, koji briše privremene tablice sesije, i ##.
    /// Granule koje čitaju tablicu koju je prethodni upit napunio (<c>##temp50</c>, <c>##tponudanaplata</c>,
    /// <c>##ttnaplata</c>, <c>##tempniv</c>...) tada dobiju "Invalid object name".
    /// Ovdje: jedna otvorena veza po connection stringu (u njemu je i ime granule, APP-01).
    /// Upit s druge niti ili dok je veza zauzeta ide kroz zasebnu vezu, jer SqlConnection bez MARS-a
    /// ne smije imati dva upita odjednom.
    /// </summary>
    public static class NgVeza
    {
        private sealed class Stavka
        {
            public SqlConnection Veza;
            public bool Zauzeta;
            public int Korisnika;
        }

        private static readonly object _brava = new object();

        /// <summary>
        /// Sweep (TestShell) ovdje dobije upite dulje od <see cref="SporoMs"/> i upite koji puknu, s trajanjem.
        /// Izvan sweepa je null.
        /// </summary>
        public static Action<string> SporiUpit;

        public static int SporoMs = 300;
        private static readonly Dictionary<string, Stavka> _veze = new Dictionary<string, Stavka>(StringComparer.Ordinal);

        /// <summary>Izvrši upit kao Class1.ExecSql: kroz otvorenu vezu granule.</summary>
        public static DataTable Citaj(string spoj, string sql, int timeoutSekundi)
        {
            if (SporiUpit == null)
                return CitajBezMjere(spoj, sql, timeoutSekundi);
            var sat = System.Diagnostics.Stopwatch.StartNew();
            try
            {
                DataTable dt = CitajBezMjere(spoj, sql, timeoutSekundi);
                if (sat.ElapsedMilliseconds >= SporoMs)
                    SporiUpit("spor upit " + sat.ElapsedMilliseconds.ToString() + " ms: " + Skrati(sql));
                return dt;
            }
            catch (Exception ex)
            {
                SporiUpit("upit pukao " + sat.ElapsedMilliseconds.ToString() + " ms (" + ex.GetType().Name + " " + Skrati(ex.Message) + "): " + Skrati(sql));
                throw;
            }
        }

        private static string Skrati(string t)
        {
            t = (t ?? "").Replace("\r", " ").Replace("\n", " ").Replace("\t", " ");
            return t.Length > 160 ? t.Substring(0, 160) : t;
        }

        private static DataTable CitajBezMjere(string spoj, string sql, int timeoutSekundi)
        {
            Stavka s = Uzmi(spoj);
            if (s == null)
            {
                return CitajZasebno(spoj, sql, timeoutSekundi);
            }
            try
            {
                if (s.Veza == null || s.Veza.State != ConnectionState.Open)
                {
                    // Prvi upit ili veza je pukla (greška razine 20+ je zatvori); NetGen tada radi Close/Open (Class1 241).
                    if (s.Veza != null)
                    {
                        try { s.Veza.Dispose(); } catch { }
                    }
                    s.Veza = new SqlConnection(BezPoola(spoj));
                    s.Veza.Open();
                }
                var dt = new DataTable();
                using (var cmd = new SqlCommand(sql, s.Veza))
                using (var da = new SqlDataAdapter(cmd))
                {
                    cmd.CommandTimeout = timeoutSekundi;
                    da.Fill(dt);
                }
                return dt;
            }
            catch (SqlException ex)
            {
                // Class1.ExecSql 241-242: nakon greške NetGen radi Conn.Close() i Conn.Open() (pool, sp_reset_connection):
                // transakcija koju je procedura ostavila otvorenom se poništi, zaključavanja se puste, #temp tablice
                // nestanu. Iznimka je RAISERROR s "#" ili "+" u poruci (#OK, #close, +FOCUS...), gdje VB izađe prije
                // toga (157-200). Timeout (-2) također prekine batch usred procedure. Ovdje se veza zatvori, pa server
                // napravi rollback; sljedeći upit otvori novu sesiju (R124, PN-04).
                string poruka = ex.Message ?? "";
                bool porukaProcedure = ex.Number != -2 && (poruka.Contains("#") || poruka.Contains("+"));
                if (!porukaProcedure && s.Veza != null)
                {
                    try { s.Veza.Dispose(); } catch { }
                    s.Veza = null;
                }
                throw;
            }
            finally
            {
                lock (_brava)
                    s.Zauzeta = false;
            }
        }

        /// <summary>
        /// Zajednička veza ne ide u pool: zatvaranje tada stvarno završi sesiju (privremene tablice nestanu,
        /// otvorena transakcija se poništi), kao kad NetGen proces zatvori Conn. Iz poola bi sesija živjela dalje.
        /// </summary>
        private static string BezPoola(string spoj)
        {
            var b = new SqlConnectionStringBuilder(spoj);
            b.Pooling = false;
            return b.ConnectionString;
        }

        private static Stavka Uzmi(string spoj)
        {
            lock (_brava)
            {
                Stavka s;
                if (!_veze.TryGetValue(spoj, out s))
                {
                    s = new Stavka();
                    _veze[spoj] = s;
                }
                if (s.Zauzeta)
                    return null;
                s.Zauzeta = true;
                return s;
            }
        }

        private static DataTable CitajZasebno(string spoj, string sql, int timeoutSekundi)
        {
            var dt = new DataTable();
            using (var conn = new SqlConnection(spoj))
            using (var cmd = new SqlCommand(sql, conn))
            using (var da = new SqlDataAdapter(cmd))
            {
                cmd.CommandTimeout = timeoutSekundi;
                conn.Open();
                da.Fill(dt);
            }
            return dt;
        }

        /// <summary>Podloga se otvorila s ovim spojem.</summary>
        public static void Prijavi(string spoj)
        {
            if (string.IsNullOrEmpty(spoj))
                return;
            lock (_brava)
            {
                Stavka s;
                if (!_veze.TryGetValue(spoj, out s))
                {
                    s = new Stavka();
                    _veze[spoj] = s;
                }
                s.Korisnika++;
            }
        }

        /// <summary>
        /// Podloga se zatvorila. Kad nijedna podloga više ne koristi spoj, veza se zatvori, kao kad NetGen
        /// proces završi (privremene tablice sesije tada nestanu).
        /// </summary>
        public static void Otpusti(string spoj)
        {
            if (string.IsNullOrEmpty(spoj))
                return;
            SqlConnection zatvori = null;
            lock (_brava)
            {
                Stavka s;
                if (!_veze.TryGetValue(spoj, out s))
                    return;
                if (s.Korisnika > 0)
                    s.Korisnika--;
                if (s.Korisnika == 0 && !s.Zauzeta)
                {
                    zatvori = s.Veza;
                    _veze.Remove(spoj);
                }
            }
            if (zatvori != null)
            {
                try { zatvori.Dispose(); } catch { }
            }
        }
    }
}
