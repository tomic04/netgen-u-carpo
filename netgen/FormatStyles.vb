Imports Janus.Windows.GridEX
Imports System.Windows.Forms

Public Class FormatStyles

    Private m_Grid As GridEX
    Public Property Grid() As GridEX
        Get
            Return m_Grid
        End Get
        Set(ByVal Value As GridEX)
            m_Grid = Value
            ReadGridProperties()
        End Set
    End Property
    Private Sub ReadGridProperties()

        optAlternatingRow.Tag = m_Grid.AlternatingRowFormatStyle
        optCardCaption.Tag = m_Grid.CardCaptionFormatStyle
        optCardHeader.Tag = m_Grid.CardColumnHeaderFormatStyle
        optGroupByBox.Tag = m_Grid.GroupByBoxFormatStyle
        optGroupByBoxInfo.Tag = m_Grid.GroupByBoxInfoFormatStyle
        optGroupRow.Tag = m_Grid.GroupRowFormatStyle
        optHeader.Tag = m_Grid.HeaderFormatStyle
        optRow.Tag = m_Grid.RowFormatStyle
        optSelectedRow.Tag = m_Grid.SelectedFormatStyle
        optControlStyle.Tag = m_Grid.ControlStyle

        optRow.Checked = True
    End Sub

    Private Sub optRow_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optRow.CheckedChanged
        Dim button As RadioButton = CType(sender, RadioButton)
        If button.Checked Then
            propertyGrid1.SelectedObject = button.Tag
        End If
    End Sub

    Private Sub optCardHeader_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optCardHeader.CheckedChanged
        optRow_CheckedChanged(sender, e)
    End Sub

    Private Sub optCardCaption_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optCardCaption.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub

    Private Sub optGroupByBoxInfo_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optGroupByBoxInfo.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub

    Private Sub optGroupByBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optGroupByBox.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub

    Private Sub optSelectedRow_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optSelectedRow.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub

    Private Sub optAlternatingRow_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optAlternatingRow.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub

    Private Sub optHeader_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optHeader.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub

    Private Sub optGroupRow_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optGroupRow.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub


    Private Sub optControlStyle_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles optControlStyle.CheckedChanged
        optRow_CheckedChanged(sender, e)

    End Sub
End Class