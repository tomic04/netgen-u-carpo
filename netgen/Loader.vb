
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Windows.Forms
Imports System.Runtime.InteropServices



Public Class Loader
    Public moduleId As String
    Public strNGParams As String
    Public mainHandle As Int32

    Public WCaption As String
    Public hWnd As Int32
    Public Koler As String = ""

    Public Ikona As String

    'Dim ob As New NetGenDLL.Form1

    Public Sub ShellResize(ByVal w As Integer, ByVal h As Integer)
        'If Me.WindowState = FormWindowState.Maximized Then
        SetWindowPos(Me.hWnd, New IntPtr(-1), 0, 0, w, h, SWP_NOACTIVATE Or SWP_NOZORDER)
        ' End If
    End Sub


    <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
  Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInt32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetParent(ByVal child As Integer, ByVal newParent As Integer) As Integer
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetWindowPos(ByVal hWnd As IntPtr, _
     ByVal hWndInsertAfter As IntPtr, _
     ByVal X As Integer, _
     ByVal Y As Integer, _
     ByVal cx As Integer, _
     ByVal cy As Integer, _
    ByVal uFlags As UInteger) As Boolean
    End Function

    Public Const SWP_NOSIZE As Integer = &H1
    Public Const SWP_NOMOVE As Integer = &H2
    Public Const SWP_NOZORDER As Integer = &H4
    Public Const SWP_NOREDRAW As Integer = &H8
    Public Const SWP_NOACTIVATE As Integer = &H10
    'Public Const SWP_DRAWFRAME As Integer = &H20
    Public Const SWP_FRAMECHANGED As Integer = &H20
    Public Const SWP_SHOWWINDOW As Integer = &H40
    Public Const SWP_HIDEWINDOW As Integer = &H80
    Public Const SWP_NOCOPYBITS As Integer = &H100
    Public Const SWP_NOOWNERZORDER As Integer = &H200
    Public Const SWP_NOREPOSITION As Integer = &H200
    Public Const SWP_NOSENDCHANGING As Integer = &H400
    Public Const SWP_DEFERERASE As Integer = &H2000
    Public Const SWP_ASYNCWINDOWPOS As Integer = &H4000


    Private Sub MYExnHandler(ByVal sender As Object, ByVal e As UnhandledExceptionEventArgs)
        Dim EX As Exception
        EX = e.ExceptionObject
        MsgBox(EX.StackTrace)
    End Sub

    Private Sub MYThreadHandler(ByVal sender As Object, ByVal e As Threading.ThreadExceptionEventArgs)
        MsgBox(e.Exception.StackTrace)
    End Sub

    Function MyResolveEventHandler(ByVal sender As Object, ByVal args As ResolveEventArgs) As [Assembly]
        Console.WriteLine("Resolving...")
        Return GetType(Loader).Assembly
    End Function 'MyResolveEventHandler

    Public Function Run(ByVal server As String, ByVal database As String, ByVal userId As String, ByVal password As String, _
        ByVal operater As Int32, ByVal clientHandle As Int32, ByVal top As Short, ByVal left As Short, ByVal remoteDllPath As String) As Object

        On Error Resume Next

        Dim ob As NetGenDLL.Form1

        Dim newDomain As AppDomain = AppDomain.CreateDomain(moduleId)
        AddHandler newDomain.AssemblyResolve, AddressOf MyResolveEventHandler
        AddHandler newDomain.UnhandledException, AddressOf MYExnHandler
        AddHandler Application.ThreadException, AddressOf MYThreadHandler

        'ob = newDomain.CreateInstanceAndUnwrap("NetGenDLL.dll", "NetGenDLL.Form1")
        'ob = newDomain.CreateInstanceFromAndUnwrap("NetGenDLL.dll", "NetGenDLL.Form1")
        ob = newDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().ToString, "NetGenDLL.Form1")

        Run = ob

        ' strNGParams = ":BLAGAJNA (@id=2762219,@brojrac='111111')"
        ob.ModuleID = moduleId
        ob.strNGParams = strNGParams
        ob.MainHandle = mainHandle

        'strSQLSERVERlogin = userId
        'strSQLSERVERpassword = password

        'If strSQLSERVERlogin Is Nothing Then strSQLSERVERlogin = ""
        'If strSQLSERVERpassword Is Nothing Then strSQLSERVERpassword = ""

        'MsgBox(strSQLSERVERlogin)

        'MsgBox(strSQLSERVERpassword)

        ob.Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath)

        'ob.Run(server, database, strSQLSERVERlogin, strSQLSERVERpassword, operater, clientHandle, top, left, remoteDllPath)

        WCaption = ob.WCaption
        hWnd = ob.hWnd

    End Function
    

    Public Function Run8(ByVal server As String, ByVal database As String, ByVal userId As String, ByVal password As String, _
     ByVal operater As Int32, ByVal clientHandle As Int32, ByVal top As Short, ByVal left As Short, ByVal remoteDllPath As String) As Object




        On Error Resume Next

        '' ### valentinotranslator4.10.2023.
        'Dim assembly1 As Assembly = Assembly.Load("JupiterTranslator10")
        'If Err.Number = 0 Then
        '    JupiterPrevoditeljValentinoBoolean = True
        '    JupiterPrevoditeljValentinoType = assembly1.GetType("JupiterTranslator.Translator")
        '    Dim parameters() As Object = {"jupitertranslations", "jupiter2019", Nothing, Nothing}
        '    JupiterPrevoditeljValentinoType.InvokeMember("Init", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, Nothing, Nothing, parameters)
        '    Dim parameters2() As Object = {"en"}
        '    JupiterPrevoditeljValentinoType.InvokeMember("LoadTranslations", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, Nothing, Nothing, parameters2)
        'End If

        ' ovo je ručna inicijalizacija , nije potrebna ako se ng zove iz cc
        ' Zamislite da je YourStaticClass vaša statička klasa sa statičkom metodom
        'Module1.JupiterPrevoditelj = assembly.GetType("JupiterTranslator.Translator")
        'Dim parameters2() As Object = {"NetGen", "jupitertranslations", "jupiter2019", "", ""}
        'JupiterPrevoditelj.InvokeMember("Init", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, Nothing, Nothing, parameters2)
        'Dim parameters3() As Object = {"en"}
        'JupiterPrevoditelj.InvokeMember("LoadTranslations", BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, Nothing, Nothing, parameters3)



        'Try



        '     MsgBox("run8 u loaderu , server: " & server & ", DB:" & database)




        Dim ob As New NetGenDLL.Form1
        ' dakle , ovo je pizdarija ,
        ' ako zakomentiram imam problem sa sheringom varijabli
        ' ako odkomentiram onda imam neki drugi problem , jupiter client proguta strlice i enter evente
        ' Peki je prijavo da mu sada granule na blagajni nemaju fokus nego odu iza exe-a

        ' 6.2.2024. opet odkometirao zbog J10 ... tamo se zbog mogućnosti otvranja iste granule na više firmi dešava da je konekcija uvijek od zadnje 
        ' u J10 su izbacili referencu i dižu NG preko loadassembly valjda

        ' 22.04.2024. opet komentiramo :)
        'Dim newDomain As AppDomain = AppDomain.CreateDomain(moduleId)
        'AddHandler newDomain.AssemblyResolve, AddressOf MyResolveEventHandler
        'AddHandler newDomain.UnhandledException, AddressOf MYExnHandler
        'AddHandler Application.ThreadException, AddressOf MYThreadHandler
        'ob = newDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().ToString, "NetGenDLL.Form1")
        'Run8 = ob


        '
        '------------------- end of pizdarija

        ' MsgBox(" Dim ob As New NetGenDLL.Form1 : " & Err.Description)

        ob.ModuleID = moduleId
        ob.MainHandle = mainHandle
        ob.strNGParams = strNGParams
        ob.Ikona = Ikona



        ob.Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath, Koler)

        'MsgBox("nakon  ob.Run-a : " & Err.Description)

        WCaption = ob.WCaption
        hWnd = ob.hWnd

        'Catch ex As Exception
        'MsgBox(ex.StackTrace)
        ' End Try


    End Function

    'Public Function Run8(ByVal server As String, ByVal database As String, ByVal userId As String, ByVal password As String, _
    '   ByVal operater As Int32, ByVal clientHandle As Int32, ByVal top As Short, ByVal left As Short, ByVal remoteDllPath As String) As Object

    '    On Error Resume Next

    '    Dim ob As New NetGenDLL.Form1

    '    Run8 = ob

    '    ob.ModuleID = moduleId
    '    ob.strNGParams = strNGParams
    '    ob.MainHandle = mainHandle
    '    ob.Ikona = Ikona
    '    ' MsgBox(Ikona)

    '    ob.Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath)

    '    WCaption = ob.WCaption
    '    hWnd = ob.hWnd

    'End Function

    Public Function DajCB() As String
        DajCB = "..." ' ob.DajCB
    End Function

    Public Function DajSQL() As String
        DajSQL = "..." ' ob.DajSQL
    End Function

    Public Function DajParametre() As String
        DajParametre = "..." ' ob.DajParametre
    End Function

End Class
