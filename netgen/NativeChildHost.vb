Imports System.Runtime.InteropServices

Public Class NativeChildHost

    <StructLayout(LayoutKind.Sequential)>
    Private Structure RECT
        Public Left As Integer
        Public Top As Integer
        Public Right As Integer
        Public Bottom As Integer
    End Structure

    ' --- Win32 ---
    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetParent(hWndChild As IntPtr, hWndNewParent As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function GetClientRect(hWnd As IntPtr, ByRef lpRect As RECT) As Boolean
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function MapWindowPoints(hWndFrom As IntPtr, hWndTo As IntPtr,
                                            ByRef lpPoints As RECT, cPoints As Integer) As Integer
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function AdjustWindowRectEx(ByRef lpRect As RECT, dwStyle As UInteger,
                                               bMenu As Boolean, dwExStyle As UInteger) As Boolean
    End Function

    <DllImport("user32.dll", EntryPoint:="GetWindowLongPtr", SetLastError:=True)>
    Private Shared Function GetWindowLongPtr(hWnd As IntPtr, nIndex As Integer) As IntPtr
    End Function

    <DllImport("user32.dll", SetLastError:=True)>
    Private Shared Function SetWindowPos(hWnd As IntPtr, hWndInsertAfter As IntPtr,
                                         X As Integer, Y As Integer, cx As Integer, cy As Integer,
                                         uFlags As UInteger) As Boolean
    End Function

    ' --- Consts ---
    Private Const GWL_STYLE As Integer = -16
    Private Const GWL_EXSTYLE As Integer = -20
    Private Const SWP_NOZORDER As UInteger = &H4
    Private Const SWP_NOACTIVATE As UInteger = &H10
    Private Const SWP_SHOWWINDOW As UInteger = &H40

    ' --- Public API ---
    Public Shared Sub AttachAndAlignToPanel(childHwnd As IntPtr, panelHwnd As IntPtr)
        If childHwnd = IntPtr.Zero OrElse panelHwnd = IntPtr.Zero Then Return

        ' 1) Reparent
        SetParent(childHwnd, panelHwnd)

        ' 2) Get panel client rect in screen coords
        Dim panelClient As RECT
        GetClientRect(panelHwnd, panelClient)
        MapWindowPoints(panelHwnd, IntPtr.Zero, panelClient, 2)

        Dim desiredW As Integer = panelClient.Right - panelClient.Left
        Dim desiredH As Integer = panelClient.Bottom - panelClient.Top

        ' 3) Adjust for window borders/caption
        Dim style As Integer = GetWindowLongPtr(childHwnd, GWL_STYLE).ToInt32()
        Dim ex As Integer = GetWindowLongPtr(childHwnd, GWL_EXSTYLE).ToInt32()

        Dim outer As RECT
        outer.Left = 0 : outer.Top = 0
        outer.Right = desiredW : outer.Bottom = desiredH
        AdjustWindowRectEx(outer, CUInt(style), False, CUInt(ex))

        Dim outerW As Integer = outer.Right - outer.Left
        Dim outerH As Integer = outer.Bottom - outer.Top
        Dim x As Integer = panelClient.Left + outer.Left
        Dim y As Integer = panelClient.Top + outer.Top

        ' 4) Position
        SetWindowPos(childHwnd, IntPtr.Zero, x, y, outerW, outerH,
                     SWP_NOZORDER Or SWP_NOACTIVATE Or SWP_SHOWWINDOW)
    End Sub

End Class

