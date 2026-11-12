
Imports System.Windows.Forms
Imports System.Drawing

Public Class GridDX
    'Inherits DataGridView

    Inherits DevExpress.XtraGrid.GridControl

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


    Public m_nodeList As Xml.XmlNodeList




End Class
