

Public Class NumerBox
    Inherits Janus.Windows.GridEX.EditControls.NumericEditBox
    'Inherits TextBox

    Public OnLostFocusSQL As String = ""
    Public HotKey As String = ""


    'Dim nonNumberEntered As Boolean

    'Protected Overrides Sub OnKeyDown(ByVal e As System.Windows.Forms.KeyEventArgs)
    '    nonNumberEntered = False

    '    If e.KeyValue = 188 Or e.KeyValue = 100 Or e.KeyValue = 190 Or e.KeyValue = 109 Or e.KeyValue = 110 Then
    '        Return
    '    End If

    '    If e.Shift = True Or e.Alt = True Or e.KeyValue = 188 Or e.KeyValue = 13 Then
    '        nonNumberEntered = True
    '        MyBase.OnKeyDown(e)
    '        Return
    '    End If

    '    If e.KeyCode < Keys.D0 Or e.KeyCode > Keys.D9 Then
    '        If e.KeyCode < Keys.NumPad0 Or e.KeyCode > Keys.NumPad9 Then
    '            If e.KeyCode <> Keys.Back Then
    '                nonNumberEntered = True
    '            End If
    '        End If
    '    End If

    'End Sub


    'Protected Overrides Sub OnKeyPress(ByVal e As System.Windows.Forms.KeyPressEventArgs)
    '    If nonNumberEntered = True Then
    '        e.Handled = True
    '    End If
    '    'MyBase.OnKeyPress(e)
    'End Sub

End Class
