Imports System.Windows.Forms

Public Class SearchBox
    Inherits TextBox


    Public OnLostFocusSQL As String = ""
    Public strOnLeave As String = ""
    Public HotKey As String = ""
    Public strColumnCaption As String = ""
    Public strColumnWidth As String = ""
    Public SudjelujeUParametrima As Boolean = True
    Public strColumnAutoResize As String = "true"

    'Protected Overrides Sub OnValidating(ByVal e As System.ComponentModel.CancelEventArgs)
    '    Form3.OKClick = False
    '    Form3.Trazilica("in", "Partneri#partneriid#%naziv%")
    '    Form3.ShowDialog()
    'End Sub



End Class
