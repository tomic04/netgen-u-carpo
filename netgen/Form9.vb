Imports System.Windows.Forms
Imports System.Reflection


Public Class Form9
    Public FirstLoad As Boolean = False
    Public cls1 As Class1
    Public Template As String = ""
    Public handle5 As IntPtr = 0
    Private WithEvents timer As New Timer()

    Function GetID2() As String ' za janus
        GetID2 = "0"
        Template = ""
        Dim tNaziv As String = ""
        If GridEX1.RowCount > 0 Then
            On Error Resume Next
            Template = GridEX1.CurrentRow.Cells("Template").Value.ToString.Trim
            If Err.Number <> 0 Then
                Template = ""
            End If
        Else
            GetID2 = "0"
        End If
    End Function


    Private Sub GridEX1_Click(sender As Object, e As EventArgs) Handles GridEX1.Click
        GetID2()
        'Application.OpenForms.SaveCheckpoint()
        IspisiMePrekoSpinPrintinga(Template.ToString.Replace(":", "/"), Module1.prevParametri7.ToString)

        'For Each f As Form In Application.OpenForms.Cast(Of Form).ToList()
        '    MessageBox.Show(f.[GetType]().ToString())
        'Next
        'Dim f9val As Form = Application.OpenForms.GetLast("Spin.Printing")
        'AddHandler f9val.Closed, AddressOf f9valClose
        Me.Close()
        'MessageBox.Show("start timera!")
        'timer.Interval = 5000
        'timer.Start()
    End Sub

    'Sub f9valClose()
    '    Me.Close()
    'End Sub

    Private Sub timer_Tick(sender As Object, e As EventArgs) Handles timer.Tick
        ' Timer se okinuo nakon 5 sekundi
        'timer.Stop()
        'MessageBox.Show("Prošlo je 5 sekundi!")
        'Me.Close()
    End Sub

    Private Sub Form9_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        If FirstLoad = True Then
            'Me.Close()
        Else
            FirstLoad = True
        End If
    End Sub


    Private Sub Form9_KeyDown(sender As Object, e As Windows.Forms.KeyEventArgs) Handles MyBase.KeyDown
        If e.KeyCode = Keys.Escape Then
            Template = ""
            Me.Close()
        End If
    End Sub


    Function IspisiMePrekoSpinPrintinga(report As String, param As String) As Boolean
        Dim a As Assembly
        Dim o As Object
        Dim netgenHandle As IntPtr = IntPtr.Zero

        ' 🔹 Uzimamo handle aktivne NetGen forme ako postoji
        Try
            If Application.OpenForms.Count > 0 Then
                netgenHandle = Application.OpenForms(0).Handle
            End If
        Catch ex As Exception
            ' Ako iz nekog razloga ne uspemo, fallback: uzmi prozor po nazivu
            Try
                Dim netgenForm = Process.GetCurrentProcess().MainWindowHandle
                If netgenForm <> IntPtr.Zero Then
                    netgenHandle = netgenForm
                End If
            Catch
                ' Ignoriši
            End Try
        End Try

        ' Ako ni jedan način nije uspeo, netgenHandle ostaje 0 i sve radi kao pre
        ' -------------------------------------------------------------------------

        ' 🔹 Dinamički load DLL-a
        Try
            a = Assembly.Load("Spin.Printing.v23")
            o = a.CreateInstance("Spin.Printing.Start")
        Catch ex As Exception
            a = Assembly.Load("Spin.Printing.v9.1")
            o = a.CreateInstance("Spin.Printing.Start")
        End Try

        ' 🔹 Parametri za ispis
        Dim paramList As New List(Of String)
        If param.ToLower.Contains("@id=") Or param.ToLower.Contains("@id =") Then
            For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
                paramList.Add(param)
            Next
        Else
            For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
                paramList.Add("@id=" & Module1.arrI7(i).ToString & "," & param)
            Next
        End If

        report = report.Replace("REPX:/", "").Replace("Repx/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", "")
        report = report.Replace("/", "")

        ' 🔹 Poziv ispisa sa handle parametrom (ako postoji)
        Try
            If UBound(Module1.arrI7) > 1 Then
                If netgenHandle <> IntPtr.Zero Then
                    o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList, "", netgenHandle.ToInt32())
                Else
                    o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList)
                End If
            Else
                If netgenHandle <> IntPtr.Zero Then
                    o.Print(strGlobalSQLSERVERlogin, report, paramList(0), "", netgenHandle.ToInt32())
                Else
                    o.Print(strGlobalSQLSERVERlogin, report, paramList(0))
                End If
            End If
            IspisiMePrekoSpinPrintinga = True
        Catch ex As Exception
            IspisiMePrekoSpinPrintinga = False
        End Try

        ' 🔹 Fallback poziv ako gornji padne
        If IspisiMePrekoSpinPrintinga = False Then
            Try
                If UBound(Module1.arrI7) > 1 Then
                    o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList)
                Else
                    o.Print(strGlobalSQLSERVERlogin, report, paramList(0))
                End If
                IspisiMePrekoSpinPrintinga = True
            Catch ex As Exception
                IspisiMePrekoSpinPrintinga = False
            End Try
        End If

    End Function

    'Function IspisiMePrekoSpinPrintinga(report As String, param As String) As Boolean

    '    Dim a As Assembly ' 19.03.2024. kk
    '    Dim o As Object
    '    Try
    '        a = Assembly.Load("Spin.Printing.v23")
    '        o = a.CreateInstance("Spin.Printing.Start")
    '    Catch ex As Exception
    '        a = Assembly.Load("Spin.Printing.v9.1")
    '        o = a.CreateInstance("Spin.Printing.Start")
    '    End Try

    '    Dim paramList As New List(Of String)
    '    If param.ToLower.Contains("@id=") Or param.ToLower.Contains("@id =") Then
    '        For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
    '            paramList.Add(param)
    '        Next
    '    Else
    '        For i = LBound(Module1.arrI7) To UBound(Module1.arrI7)
    '            paramList.Add("@id=" & Module1.arrI7(i).ToString & "," & param)
    '        Next
    '    End If

    '    report = report.Replace("REPX:/", "").Replace("Repx/", "").Replace("repx:/", "").Replace("repx/", "").Replace("REPX/", "")
    '    report = report.Replace("/", "")

    '    Try
    '        If UBound(Module1.arrI7) > 1 Then
    '            o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList, "", handle5)
    '        Else
    '            o.Print(strGlobalSQLSERVERlogin, report, paramList(0), "", handle5)
    '        End If
    '        IspisiMePrekoSpinPrintinga = True
    '    Catch ex As Exception
    '        IspisiMePrekoSpinPrintinga = False
    '    End Try

    '    If IspisiMePrekoSpinPrintinga() = False Then
    '        Try
    '            If UBound(Module1.arrI7) > 1 Then
    '                o.PrintMulti(strGlobalSQLSERVERlogin, report, paramList)
    '            Else
    '                o.Print(strGlobalSQLSERVERlogin, report, paramList(0))
    '            End If
    '            IspisiMePrekoSpinPrintinga = True
    '        Catch ex As Exception
    '            IspisiMePrekoSpinPrintinga = False
    '        End Try
    '    End If

    'End Function

    Private Sub Form9_Load(sender As Object, e As EventArgs) Handles MyBase.Load

    End Sub
End Class