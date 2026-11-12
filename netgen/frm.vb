
Imports System.Reflection
Imports System.Runtime.Remoting
Imports System.Windows.Forms


Public Class frm
    Public moduleId As String
    Public mainHandle As Int32

    Public WCaption As String
    Public hWnd As Int32

    Public Ikona As String
    Public Koler As String = ""

    Public ulazniID As String = ""
    Public ulaznaForma As String = ""

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

        ' On Error Resume Next
        Dim ob As NetGenDLL.Form2

        'Dim newDomain As AppDomain = AppDomain.CreateDomain(moduleId)
        'AddHandler newDomain.AssemblyResolve, AddressOf MyResolveEventHandler
        'AddHandler newDomain.UnhandledException, AddressOf MYExnHandler
        'AddHandler Application.ThreadException, AddressOf MYThreadHandler

        ''ob = newDomain.CreateInstanceAndUnwrap("NetGenDLL.dll", "NetGenDLL.Form1")
        ''ob = newDomain.CreateInstanceFromAndUnwrap("NetGenDLL.dll", "NetGenDLL.Form1")
        'ob = newDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().ToString, "NetGenDLL.Form2")

        'Run = ob

        ob.ModuleID = moduleId
        ob.MainHandle = mainHandle

        'strSQLSERVERlogin = userId
        'strSQLSERVERpassword = password

        'If strSQLSERVERlogin Is Nothing Then strSQLSERVERlogin = ""
        'If strSQLSERVERpassword Is Nothing Then strSQLSERVERpassword = ""

        ob.Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath)

        ' ob.Run(server, database, strSQLSERVERlogin, strSQLSERVERpassword, operater, clientHandle, top, left, remoteDllPath)


        WCaption = ob.WCaption
        hWnd = ob.hWnd

    End Function


    Public Function Run8(ByVal server As String, ByVal database As String, ByVal userId As String, ByVal password As String, _
      ByVal operater As Int32, ByVal clientHandle As Int32, ByVal top As Short, ByVal left As Short, ByVal remoteDllPath As String) As Object

        On Error Resume Next

        ' ### valentinotranslator4.10.2023.
        'Dim assembly As Assembly = assembly.Load("JupiterTranslator10")
        'If Err.Number = 0 Then
        '    JupiterPrevoditeljValentinoBoolean = True
        'End If

        Err.Clear()

        '  MsgBox("Miki ušo u Run8 u frm")
        'MsgBox("ušo u run8 u frm , server: " & server & ", DB:" & database)

        Dim ob As New NetGenDLL.Form2

        ' dakle , ovo je pizdarija ,
        ' ako zakomentiram imam problem sa sheringom varijabli
        ' ako odkomentiram onda imam neki drugi problem kojeg se sada en mogu sjetti ...
        ' ovo sada Jaca testira
        ' Peki je prijavo da mu sada granule na blagajni nemaju fokus nego odu iza exe-a

        ' 6.2.2024. opet odkometirao zbog J10 ... tamo se zbog mogućnosti otvranja iste granule na više firmi dešava da je konekcija uvijek od zadnje 
        ' u J10 su izbacili referencu i dižu NG preko loadassembly valjda

        ' 22.04.2024. opet komentiramo :)
        'Dim newDomain As AppDomain = AppDomain.CreateDomain(moduleId)
        'AddHandler newDomain.AssemblyResolve, AddressOf MyResolveEventHandler
        'AddHandler newDomain.UnhandledException, AddressOf MYExnHandler
        'AddHandler Application.ThreadException, AddressOf MYThreadHandler
        'ob = newDomain.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().GetName().ToString, "NetGenDLL.Form2")
        'Run8 = ob
        '
        '------------------- end of pizdarija

        ob.ModuleID = moduleId
        ob.MainHandle = mainHandle
        ob.Ikona = Ikona

        ob.ulazniID = ulazniID
        ob.ulaznaForma = ulaznaForma

        ob.Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath, Koler)

        WCaption = ob.WCaption
        hWnd = ob.hWnd

    End Function

    'Public Function Run8(ByVal server As String, ByVal database As String, ByVal userId As String, ByVal password As String, _
    '   ByVal operater As Int32, ByVal clientHandle As Int32, ByVal top As Short, ByVal left As Short, ByVal remoteDllPath As String) As Object

    '    On Error Resume Next
    '    Dim ob As New NetGenDLL.Form2

    '    Run8 = ob

    '    ob.ModuleID = moduleId
    '    ob.MainHandle = mainHandle
    '    ob.Ikona = Ikona

    '    ob.Run(server, database, userId, password, operater, clientHandle, top, left, remoteDllPath)

    '    WCaption = ob.WCaption
    '    hWnd = ob.hWnd

    'End Function


End Class

