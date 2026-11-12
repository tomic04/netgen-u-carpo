using System;
using Carpo.NetGen.Shell;

namespace Carpo.NetGen.Layout
{
    /// <summary>M1: prvo lijevo stablo + master grid + detail grid. M0 ne crta.</summary>
    public static class LayoutEngine
    {
        public static void Crtaj(ShellLayout raspored)
        {
            throw new NotSupportedException(
                "M1 crta obrazac " + ShellLayout.Obrazac + ". M0 ne otvara formu.");
        }
    }
}

namespace Carpo.NetGen.Commands
{
    /// <summary>M1: ONLOAD koji ne pocinje s ':' ide u grid; ':' otvara formu. Gumbi CMDxCAPTION / BUTTONS.</summary>
    public static class CommandRunner
    {
        public static void Pokreni(string naredba)
        {
            throw new NotSupportedException("M1: komande. M0 samo cuva tekst naredbe.");
        }
    }
}

namespace Carpo.NetGen.Sql
{
    /// <summary>
    /// M1: @@id i @@operateriid, preskok ako @@ preživi. SQL ostaje slobodan tekst.
    /// M0 ne otvara konekciju i ne dira bazu.
    /// </summary>
    public static class SqlBridge
    {
        public static void Exec(string sql)
        {
            throw new NotSupportedException("M1: SQL tekst. M0 ne šalje naredbu serveru.");
        }
    }
}

namespace Carpo.NetGen.Print
{
    /// <summary>M4: Spin.Printing v26 i rptDevRepx. M0 nema ispis.</summary>
    public static class PrintBridge
    {
        public static void Ispisi(string naredba)
        {
            throw new NotSupportedException("M4: ispis. M0 ne zove pisač.");
        }
    }
}
