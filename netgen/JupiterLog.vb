Imports System.Reflection

' -----------------------------------------------------------------------------
' JupiterLog - late-bound wrapper oko JupiterLogger.dll (Jupiter.Utility.AppLogger)
'
' Pise ERROR / INFO u JupiterLogs.dbo.LogEvent, vidljivo u JupiterLogs Vieweru
' pod Component = "NetGen".
'
' KLJUCNO: NetGen NE SMIJE pasti zbog logiranja.
'  - Sve je late-bound (Assembly.Load), nema reference u .vbproj.
'  - Ako JupiterLogger.dll ne postoji ili nesto pukne, Dostupan ostaje False
'    i svi pozivi su tihi no-op. NetGen radi kao i prije.
'  - Nikad ne zovemo ConfigureSql / SetContext / Shutdown - to je Jupiterov posao.
'    NetGen se samo prikljuci na ono sto je host vec postavio.
' -----------------------------------------------------------------------------
Public Class JupiterLog

    Private Shared tipAppLogger As Type = Nothing
    Private Shared bProbanoUcitati As Boolean = False
    Public Shared Dostupan As Boolean = False

    ' Kratak opis sto se dogodilo kod Inicijaliziraj - ide u CBLog (Ctrl+F11).
    Public Shared Dijagnostika As String = "nije ni pokusano"

    Private Const KOMPONENTA As String = "NetGen"
    Private Const MAX_PORUKA As Integer = 2000

    ' Ucitava JupiterLogger.dll i radi Init("NetGen"). Zove se jednom po procesu.
    ' Vraca True ako je logger spreman.
    Public Shared Function Inicijaliziraj(Optional ByVal granula As String = "") As Boolean
        On Error Resume Next

        If bProbanoUcitati Then
            ' vec smo probali; ako je proslo, samo osvjezi granulu
            If Dostupan And granula <> "" Then PostaviGranulu(granula)
            Return Dostupan
        End If

        bProbanoUcitati = True
        Err.Clear()

        Dim a As Assembly = Assembly.Load("JupiterLogger")
        If Err.Number <> 0 OrElse a Is Nothing Then
            Dijagnostika = "Assembly.Load(JupiterLogger) nije uspio : " & Err.Description
            Err.Clear()
            Return False
        End If

        tipAppLogger = a.GetType("Jupiter.Utility.AppLogger")
        If Err.Number <> 0 OrElse tipAppLogger Is Nothing Then
            Dijagnostika = "nema tipa Jupiter.Utility.AppLogger : " & Err.Description
            tipAppLogger = Nothing
            Err.Clear()
            Return False
        End If

        Zovi("Init", New Object() {KOMPONENTA, Nothing})
        If Err.Number <> 0 Then
            Dijagnostika = "Init(NetGen) nije uspio : " & Err.Description
            tipAppLogger = Nothing
            Err.Clear()
            Return False
        End If

        Dostupan = True

        ' SqlEnabled postavlja host (Jupiter) preko ConfigureSql. Ako je False,
        ' logger radi ali redovi se nikamo ne pisu - vazno je to vidjeti.
        Dim bSql As Object = Nothing
        bSql = tipAppLogger.InvokeMember("SqlEnabled", _
                   BindingFlags.GetProperty Or BindingFlags.Public Or BindingFlags.Static, _
                   Nothing, Nothing, Nothing)
        Dijagnostika = "OK, ucitan iz " & a.Location & " | SqlEnabled=" & CStr(bSql)
        Err.Clear()

        If granula <> "" Then PostaviGranulu(granula)

        Return True
    End Function

    ' Ime granule = ime XML-a s kojim NetGen radi (Mid).
    ' _threadGranule u AppLoggeru je ThreadStatic, pa ovo pogadja samo thread granule.
    Public Shared Sub PostaviGranulu(ByVal granula As String)
        On Error Resume Next
        If Not Dostupan Then Exit Sub
        If granula Is Nothing OrElse granula = "" Then Exit Sub
        Zovi("SetGranule", New Object() {granula})
        Err.Clear()
    End Sub

    Public Shared Sub Info(ByVal izvor As String, ByVal poruka As String)
        On Error Resume Next
        If Not Dostupan Then Exit Sub
        Zovi("LogInfo", New Object() {izvor, Sredi(poruka)})
        Err.Clear()
    End Sub

    Public Shared Sub Greska(ByVal izvor As String, ByVal poruka As String, Optional ByVal ex As Exception = Nothing)
        On Error Resume Next
        If Not Dostupan Then Exit Sub
        Zovi("LogError", New Object() {izvor, Sredi(poruka), ex})
        Err.Clear()
    End Sub

    ' -------------------------------------------------------------------------
    ' Interno
    ' -------------------------------------------------------------------------

    Private Shared Sub Zovi(ByVal metoda As String, ByVal argumenti As Object())
        On Error Resume Next
        If tipAppLogger Is Nothing Then Exit Sub
        tipAppLogger.InvokeMember(metoda, _
                                  BindingFlags.InvokeMethod Or BindingFlags.Public Or BindingFlags.Static, _
                                  Nothing, Nothing, argumenti)
    End Sub

    ' Skida lozinke iz teksta i reze predugacke poruke.
    '
    ' CBLog i Ctrl+F11 ostaju u memoriji jednog racunala, ali JupiterLogs je
    ' centralna baza s Viewerom i retencijom od 180 dana za ERROR - tamo lozinka
    ' ne smije zavrsiti. JupiterLogger.dll sam NE radi nikakav redact.
    Public Shared Function Sredi(ByVal tekst As String) As String
        On Error Resume Next

        If tekst Is Nothing Then Return ""

        Dim s As String = tekst

        s = SakrijVrijednost(s, "pwd")
        s = SakrijVrijednost(s, "password")

        If s.Length > MAX_PORUKA Then
            s = s.Substring(0, MAX_PORUKA) & " ... [skraceno, puni tekst je u Ctrl+F11]"
        End If

        Return s
    End Function

    ' Trazi "kljuc = vrijednost" (case-insensitive) i zamijeni vrijednost s ***.
    ' Vrijednost ide do ; ili " ili kraja stringa.
    Private Shared Function SakrijVrijednost(ByVal s As String, ByVal kljuc As String) As String
        On Error Resume Next

        Dim rezultat As String = s
        Dim trazi As String = kljuc.ToLower() & "="
        Dim poz As Integer = 0

        Do
            Dim i As Integer = rezultat.ToLower().IndexOf(trazi, poz)
            If i < 0 Then Exit Do

            ' pocetak vrijednosti je odmah iza "kljuc="
            Dim pocetak As Integer = i + trazi.Length
            If pocetak >= rezultat.Length Then Exit Do

            ' kraj vrijednosti: prvi ; ili " ili kraj
            Dim kraj As Integer = rezultat.Length
            Dim j As Integer
            For j = pocetak To rezultat.Length - 1
                Dim c As Char = rezultat.Chars(j)
                If c = ";"c OrElse c = """"c Then
                    kraj = j
                    Exit For
                End If
            Next

            rezultat = rezultat.Substring(0, pocetak) & "***" & rezultat.Substring(kraj)
            poz = pocetak + 3
        Loop While poz < rezultat.Length

        Return rezultat
    End Function

End Class
