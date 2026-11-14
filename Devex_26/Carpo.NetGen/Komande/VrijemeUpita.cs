using System;
using System.Globalization;
using Carpo.NetGen.Model;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// COMMANDTIMEOUT (TMO-01). NetGen ima jedan strCommandTimeout (Module1 177, zadano "30") za svaki SQL
    /// kroz Class1.ExecSql (99): postavi ga PROJECT (Form1 3463, 210 granula, 120-1000000 s), a otvaranje forme
    /// s COMMANDTIMEOUT (Form2 1035, 28 formi) ga promijeni i ostane takav. Carpo je za lanac i dijalog uvijek
    /// slao 30 s, pa su dugi poslovi (IzvrsiJob 300, MAINFORM 600) pucali. Vrijednost koja nije broj ne mijenja
    /// ništa (VB greška pod On Error Resume Next).
    /// </summary>
    public static class VrijemeUpita
    {
        public const int Zadano = 30;

        /// <summary>Gornja granica za sweep i klik pregled (TestShell). 0 znači bez granice.</summary>
        public static int Najvise;

        public static int Iz(NgAttributeBag atributi, int dosad)
        {
            if (atributi == null || !atributi.Ima("COMMANDTIMEOUT"))
                return dosad;
            int n;
            string v = (atributi.Vrijednost("COMMANDTIMEOUT") ?? "").Trim();
            if (int.TryParse(v, NumberStyles.Integer, CultureInfo.InvariantCulture, out n) && n >= 0)
                return n;
            return dosad;
        }

        /// <summary>Ono što ide u SqlCommand.CommandTimeout (0 je bez roka, kao u ADO.NET).</summary>
        public static int Stvarno(int sekundi)
        {
            if (Najvise > 0 && (sekundi == 0 || sekundi > Najvise))
                return Najvise;
            return sekundi;
        }
    }
}
