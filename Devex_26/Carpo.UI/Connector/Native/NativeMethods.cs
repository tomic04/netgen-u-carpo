using System;
using System.Runtime.InteropServices;

namespace Carpo.UI.Connector.Native
{
    public static class SafeNativeMethods
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr SetParent(IntPtr child, IntPtr newParent);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = false)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
    }
}