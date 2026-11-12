Imports System.Windows.Forms

Public Class Form5
    Public cls1 As Class1
    Private Sub Form5_Disposed(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Disposed
        TextBox1.Text = ""
    End Sub

    Private Sub Form5_KeyDown(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyEventArgs) Handles Me.KeyDown
        If e.KeyCode = Keys.Escape Then
            Me.Close()
        End If
    End Sub

    Private Sub Form5_Activated(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Activated
        Dim j As Long = 0
        EditBox1.Text = ""
        Dim sss As String = ""

        On Error Resume Next

        If cls1 Is Nothing Then
            ' cls1 = New Class1
        End If

        For j = 0 To UBound(cls1.CB)
            sss = sss & j & ".  @@" & cls1.CB(j, 0).ToString & cls1.CB(j, 1).ToString & vbCrLf
        Next


        EditBox1.Text = "---------------------------" & vbCrLf & EditBox1.Text
      

        EditBox1.Text = "@@id= " & cls1.MasterID & vbCrLf & vbCrLf & EditBox1.Text
        EditBox1.Text = "@@node= " & node77 & vbCrLf & vbCrLf & EditBox1.Text
        EditBox1.Text = "XML = " & cls1.strXMLsakojimRadimzaCBlog & vbCrLf & vbCrLf & EditBox1.Text
        EditBox1.Text = "XML VERSION = " & Version1 & vbCrLf & vbCrLf & EditBox1.Text
        EditBox1.Text = "NetGen v" & My.Application.Info.Version.Major & "." & My.Application.Info.Version.Minor & "." & My.Application.Info.Version.Revision & " b " & vbCrLf & "Server=" & cls1.strServer & vbCrLf & "DB=" & cls1.strBaza & vbCrLf & " @@operateriid=" & cls1.OperateriID.ToString.Trim & vbCrLf & vbCrLf & EditBox1.Text

        EditBox1.Text = "INCLUDE FILE : " & cls1.strIncludeGlobal & vbCrLf & vbCrLf & EditBox1.Text

        ' 09.09.2026. KK - stanje JupiterLoggera. CBLog ring od 40 linija istisne
        ' poruku iz Run-a prije nego se Ctrl+F11 otvori, pa stoji ovdje.
        EditBox1.Text = "JupiterLogger : " & JupiterLog.Dijagnostika & vbCrLf & vbCrLf & EditBox1.Text

        Dim VersionInfo As Version = System.Reflection.Assembly.GetExecutingAssembly.GetName.Version
        Dim verzija As String = VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Build & "." & VersionInfo.Revision


        EditBox1.Text = "NetGen DLL v" & VersionInfo.Major & "." & VersionInfo.Minor & "." & VersionInfo.Revision & "" & vbCrLf & "Server=" & cls1.strServer & vbCrLf & "DB=" & cls1.strBaza & vbCrLf & " @@operateriid=" & cls1.OperateriID.ToString.Trim & vbCrLf & vbCrLf & EditBox1.Text

        ' EditBox1.Text = "Verzija utvrdo : 8.3.6." & vbCrLf & "-------------------------" & vbCrLf & EditBox1.Text

        EditBox1.Text = EditBox1.Text & sss

    End Sub

    Private Sub TextBox1_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox2_DoubleClick(sender As Object, e As EventArgs) Handles TextBox2.DoubleClick
        On Error Resume Next
        Clipboard.SetText(TextBox2.Text)
    End Sub

    Private Sub TextBox2_TextChanged(sender As Object, e As EventArgs) Handles TextBox2.TextChanged

    End Sub
End Class