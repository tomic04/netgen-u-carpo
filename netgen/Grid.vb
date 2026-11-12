Imports System.Windows.Forms
Imports Janus.Windows.GridEX
Imports System.Drawing

Public Class Grid
    'Inherits DataGridView

    Inherits GridEX

    Public HotKey As String = ""
    Public OnClickSQL As String = ""
    Public OnEnterKey As String = ""
    Public SQL As String = ""
    Public OnDBLClickSQL As String = ""
    Public CHECKBOXVISIBLE As String = ""
    Public TOTALROW2 As String = ""
    Public EditColumns As String = ""
    Public ONCellEditedstr As String = ""
    Public BeforeCellEditedstr As String = ""
    Public ComboColumns As String = ""
    Public strDefoltnoAktivnaKolona As String = ""
    Public zadnjiSQL As String = ""
    Public strColumnHeaderFormat As String = ""

    Public strSQL2 As String = ""
    Public strSQL3 As String = ""
    Public strSQL4 As String = ""
    Public strRelacija As String = ""

    Public strPopUp As String = ""
    Public strAfterRefresh As String = ""
    Public strCLEARCOLUMS As String = ""

    Public strPageOrientation As String = "PORTRAIT"
    Public strPageHeaderLeft As String = ""
    Public strPageHeaderRight As String = ""
    Public strPageHeaderCenter As String = ""

    Public strPageFooterLeft As String = ""
    Public strPageFooterRight As String = ""
    Public strPageFooterCenter As String = ""

    Public m_node3 As Xml.XmlNode
    Public GridovDataTejbl As DataTable
    Public c As Integer = 1
    Public strAUTOMATICSORT As String = ""
    Public strMultiGridLayOut As String = ""
    Public strFilterMode As String = ""
    Public strTranslateColumns As String = ""
    Public strRafalno As String = "false"






    Public Sub Formatiraj()
        ' custom foramat kolona 
        Dim m_node2 As Xml.XmlNode

        On Error Resume Next

        If strCLEARCOLUMS.ToLower = "true" Then
            For Each Column In RootTable.Columns
                Column.visible = False
            Next
        End If

        Dim iPosition As Integer = 0

        For Each m_node2 In m_node3.ChildNodes
            Dim tIndex As Integer = 0
            Dim tName As String = ""
            Dim tWidth As Integer = 0
            Dim tCaption As String = ""
            Dim tFormat As String = ""
            Dim tVisible As String = ""
            Dim tForeColor As String = ""
            Dim tBackColor As String = ""
            Dim tAggregateFunction As String = ""
            Dim tAlign As String = ""
            Dim tPosition As String = ""
            Dim tAutoSize As String = ""
            Dim tmaxLines As Integer = 0


            iPosition = iPosition + 1

            tIndex = m_node2.Attributes.GetNamedItem("INDEX").Value.Trim
            tName = m_node2.Attributes.GetNamedItem("NAME").Value.Trim
            tWidth = m_node2.Attributes.GetNamedItem("WIDTH").Value.Trim
            tCaption = m_node2.Attributes.GetNamedItem("CAPTION").Value.Trim
            tFormat = m_node2.Attributes.GetNamedItem("FORMAT").Value.Trim
            tVisible = m_node2.Attributes.GetNamedItem("VISIBLE").Value.Trim
            tForeColor = m_node2.Attributes.GetNamedItem("FORECOLOR").Value.Trim
            tBackColor = m_node2.Attributes.GetNamedItem("BACKCOLOR").Value.Trim
            tAggregateFunction = m_node2.Attributes.GetNamedItem("AGREGATEFUNCTION").Value.Trim
            tAlign = m_node2.Attributes.GetNamedItem("ALIGN").Value.Trim
            tPosition = m_node2.Attributes.GetNamedItem("POSITION").Value.Trim
            tAutoSize = m_node2.Attributes.GetNamedItem("AUTOSIZE").Value.Trim
            tmaxLines = m_node2.Attributes.GetNamedItem("MAXLINES").Value.Trim

            If tIndex > 0 Or tName <> "" Then
                If tWidth > 0 Then Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).width = tWidth / FakPretvorbe
                If tCaption <> "" Then Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).caption = tCaption
                If tFormat <> "" Then Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).FormatString = tFormat
                If tVisible <> "" Then Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).visible = tVisible

                If tPosition <> "" Then Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).position = tPosition

                If tmaxLines > 0 Then
                    Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).MaxLines = tmaxLines
                    Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).wordwrap = True
                End If

                If tAutoSize.ToLower = "true" Then
                    Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AutoSizeMode = ColumnAutoSizeMode.AllCellsAndHeader
                    Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AutoSize()
                End If

                If tAlign <> "" Then
                    If tAlign.ToUpper = "RIGHT" Then
                        Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).TextAlignment = 3
                    End If

                    If tAlign.ToUpper = "CENTER" Then
                        Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).TextAlignment = 2
                    End If

                    If tAlign.ToUpper = "LEFT" Then
                        Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).TextAlignment = 1
                    End If

                End If

                If tAggregateFunction <> "" Then
                    Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).AggregateFunction = tAggregateFunction
                End If

                If strCLEARCOLUMS.ToLower = "true" Then
                    iPosition = iPosition + 1
                    RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).position = iPosition
                End If

                ' me.RootTable.Columns(1).AggregateFunction =        

                If tForeColor.Trim <> "" Then
                    Select Case tForeColor.ToLower
                        Case "green"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Green
                        Case "white"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.White
                        Case "black"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Black
                        Case "darksalmon"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.DarkSalmon
                        Case "darkseagreen"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.DarkSeaGreen
                        Case "limegreen"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.LimeGreen
                        Case ("greenyellow")
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.GreenYellow
                        Case "yellow"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Yellow
                        Case "lightyellow"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.LightYellow
                        Case "red"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.forecolor = Color.Red
                    End Select
                End If

                If tBackColor.Trim <> "" Then
                    Select Case tBackColor.ToLower
                        Case "green"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Green
                        Case "white"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.White
                        Case "black"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Black
                        Case "darksalmon"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.DarkSalmon
                        Case "darkseagreen"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.DarkSeaGreen
                        Case "limegreen"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.LimeGreen
                        Case ("greenyellow")
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.GreenYellow
                        Case "yellow"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Yellow
                        Case "lightyellow"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.LightYellow
                        Case "red"
                            Me.RootTable.Columns(IIf(tIndex = 0, tName, tIndex)).CellStyle.backcolor = Color.Red
                    End Select
                End If


            End If

        Next
        '--------------------------------- end custom foramat
    End Sub

End Class
