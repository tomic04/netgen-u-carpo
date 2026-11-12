Imports System.Windows.Forms

Public Class DTPicker
    Inherits DateTimePicker

    Public OnClickSQL As String = ""
    Public OnLostFocusSQL As String = ""
    Public OnValueChangedSQL As String = ""
    Public SQL As String = ""
    Public HotKey As String = ""
    Public strCaption As String = ""

End Class
