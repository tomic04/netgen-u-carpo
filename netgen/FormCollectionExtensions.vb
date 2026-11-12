Imports System.Runtime.CompilerServices
Imports System.Windows.Forms
Imports System.Linq
Imports System.Collections.Generic

Module FormCollectionExtensions

    Private checkpointForms As HashSet(Of Form)

    <Extension()>
    Public Sub SaveCheckpoint(collection As FormCollection)
        checkpointForms = New HashSet(Of Form)(Application.OpenForms.Cast(Of Form)())
    End Sub

    <Extension()>
    Public Function GetLast(collection As FormCollection, name As String) As Form
        Dim form As Form = Nothing

        Dim after As New HashSet(Of Form)(collection.Cast(Of Form)())
        Dim newForms = after.Except(checkpointForms).ToList()
        MessageBox.Show("tu sam 1")
        MessageBox.Show(newForms.Count)
        For Each f As Form In newForms
            MessageBox.Show(f.[GetType]().Namespace)
            If f.[GetType]().Namespace IsNot Nothing AndAlso f.[GetType]().Namespace.Contains(name) Then
                form = f
            End If
        Next
        MessageBox.Show("tu sam 2")
        Return form
    End Function

End Module
