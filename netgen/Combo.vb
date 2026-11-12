Imports System.Windows.Forms

Public Class Combo
    Inherits ComboBox

    Public OnClickSQL As String = ""
    Public OnLostFocusSQL As String = ""
    Public SQL As String = ""
    Public AllowEmpty As Boolean = False
    Public HotKey As String = ""
    Public strCaption As String = ""

    Public SelectedIndexMOJ As Integer = 0
    Public SelectedTextMOJ As String = ""

    Public ZaPrevodjenje As String = ""

End Class
