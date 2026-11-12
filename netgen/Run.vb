Imports System.Runtime.InteropServices
Imports System.Windows.Forms
Imports System.Text
Imports System.Drawing
Imports System.Data
Imports System.ComponentModel
Imports System.Collections.Generic
Imports System

Public Class Run
    Inherits Form

    Public Sub New()
        Me.InitializeComponent()
    End Sub

    Private Sub Form1_FormClosing(ByVal sender As Object, ByVal e As FormClosingEventArgs)
        SendMessage(DirectCast(Me.main_handle, IntPtr), &H7D0, MyBase.Handle, IntPtr.Zero)
    End Sub

    Public Sub Run(ByVal server As String, ByVal database As String, ByVal username As String, ByVal password As String, ByVal Operateriid As Integer, ByVal Parent As Integer, ByVal pTop As Short, ByVal pLeft As Short, ByVal remotedllpath As String)
        Me.Label1.Text = String.Concat(New String() {"server = ", server, ChrW(13) & ChrW(10) & "database = ", database, ChrW(13) & ChrW(10) & "username = ", username, ChrW(13) & ChrW(10) & "password = ", password, ChrW(13) & ChrW(10) & "operaterid = ", Operateriid.ToString})
        Class1.strServer = server
        Class1.strBaza = database
        Class1.OperateriID = Operateriid
        Class1.strXML = remotedllpath & "\SharedDLLs\xml\" & Mid

        Me.Label1.Text = Me.Label1.Text & vbCrLf & remotedllpath
        Me.Label1.Text = Me.Label1.Text & vbCrLf & Mid
        Class1.Form1.ID = ""
        'SetParent(MyBase.Handle.ToInt32, Parent)
        SetParent(Class1.Form1.Handle.ToInt32, Parent)
        ' SetParent(hWnd, Parent)
        'SetParent(55555, Parent)
        'MyBase.Show()
        Class1.Form1.Show()
        Close()

    End Sub

    <DllImport("user32.dll", CharSet:=CharSet.Auto)> _
    Private Shared Function SendMessage(ByVal hWnd As IntPtr, ByVal Msg As UInt32, ByVal wParam As IntPtr, ByVal lParam As IntPtr) As IntPtr
    End Function

    <DllImport("user32.dll")> _
    Private Shared Function SetParent(ByVal child As Integer, ByVal newParent As Integer) As Integer
    End Function


    ' Properties

    Public WriteOnly Property ModuleID() As String
        Set(ByVal value As String)
            Me.Mid = value
        End Set
    End Property

    Public ReadOnly Property hWnd() As Integer
        Get
            'Return MyBase.Handle.ToInt32
            Return Class1.Form1.Handle.ToInt32
        End Get
    End Property

    Public WriteOnly Property MainHandle() As Integer
        Set(ByVal value As Integer)
            Me.main_handle = value
            Globalmain_handle = value
        End Set
    End Property

    Public ReadOnly Property WCaption() As String
        Get
            Return Me.Text
        End Get
    End Property

    Private main_handle As IntPtr
    Public Mid As String
End Class

