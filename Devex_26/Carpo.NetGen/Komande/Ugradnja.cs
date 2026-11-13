using System;

namespace Carpo.NetGen.Komande
{
    /// <summary>
    /// NativeChildHost.AttachAndAlignToPanel i Loader.ShellResize.
    /// Test računa položaj. user32 se zove samo kad je kuka prazna i hwnd nije nula, na Windowsu.
    /// </summary>
    public static class Ugradnja
    {
        public const uint SWP_NOZORDER = 0x4;
        public const uint SWP_NOACTIVATE = 0x10;
        public const uint SWP_SHOWWINDOW = 0x40;
        public const int HWND_TOPMOST = -1;

        public static uint Zastavice()
        {
            return SWP_NOZORDER | SWP_NOACTIVATE | SWP_SHOWWINDOW;
        }

        public static bool Prazan(IntPtr dijete, IntPtr panel)
        {
            return dijete == IntPtr.Zero || panel == IntPtr.Zero;
        }

        /// <summary>
        /// NativeChildHost 64–79. x = lijevo panela + lijevo nakon AdjustWindowRectEx.
        /// </summary>
        public static void Polozaj(int lijevo, int vrh, int desno, int dno, int vanjskiLijevo, int vanjskiVrh, int vanjskiDesno, int vanjskiDno, out int x, out int y, out int sirina, out int visina)
        {
            x = lijevo + vanjskiLijevo;
            y = vrh + vanjskiVrh;
            sirina = vanjskiDesno - vanjskiLijevo;
            visina = vanjskiDno - vanjskiVrh;
            if (sirina == 0 && visina == 0)
            {
                sirina = desno - lijevo;
                visina = dno - vrh;
            }
        }
    }
}
