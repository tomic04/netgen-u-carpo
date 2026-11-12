Imports Janus.Windows.GridEX
Imports System.Windows.Forms

Public Class AppearanceDialog
    Private m_Grid As GridEX
    Private mCombosLoaded As Boolean

    Private Sub LoadEnumCombos()
        If Not mCombosLoaded Then
            FillComboWithEnumValues(cboBorder, GetType(Janus.Windows.GridEX.BorderStyle))
            FillComboWithEnumValues(cboHeaderAppearance, GetType(Janus.Windows.GridEX.Appearance))
            FillComboWithEnumValues(cboButtomAppearance, GetType(Janus.Windows.GridEX.ButtonAppearance))
            FillComboWithEnumValues(cboGridlines, GetType(Janus.Windows.GridEX.GridLines))
            FillComboWithEnumValues(cboGridlineStyle, GetType(Janus.Windows.GridEX.GridLineStyle))
            mCombosLoaded = True
        End If
    End Sub
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
        If Not (m_Grid Is Nothing) Then
            LoadEnumCombos()
            If m_Grid.ThemedAreas = ThemedArea.None Then
                chkVisualStyles.Checked = False
            Else
                chkVisualStyles.Checked = True
            End If
            If m_Grid.RowHeaders = InheritableBoolean.True Then
                chkRowHeaders.Checked = True
            Else
                chkRowHeaders.Checked = False
            End If
            If m_Grid.VisualStyle = VisualStyle.Office2003 Then
                Me.chkOffice2003.Checked = True
            Else
                Me.chkOffice2003.Checked = False
            End If
            chkGroupByBox.Checked = m_Grid.GroupByBoxVisible
            chkAlternatingRows.Checked = m_Grid.AlternatingColors
            chkRecordNavigator.Checked = m_Grid.RecordNavigator
            cboBorder.SelectedItem = m_Grid.BorderStyle
            cboHeaderAppearance.SelectedItem = m_Grid.HeaderFormatStyle.Appearance
            cboButtomAppearance.SelectedItem = m_Grid.ControlStyle.ButtonAppearance
            cboGridlines.SelectedItem = m_Grid.GridLines
            cboGridlineStyle.SelectedItem = m_Grid.GridLineStyle
            CheckBox1.Checked = m_Grid.TotalRow
        End If
    End Sub
    Private Sub EnableCombos()
        Me.cboBorder.Enabled = Not (chkVisualStyles.Checked Or chkOffice2003.Checked)
        Me.cboHeaderAppearance.Enabled = Not (chkVisualStyles.Checked Or chkOffice2003.Checked)
        Me.cboButtomAppearance.Enabled = Not chkVisualStyles.Checked
    End Sub
    Private Sub FillComboWithEnumValues(ByVal combo As ComboBox, ByVal enumType As Type)
        Dim values As System.Array = System.Enum.GetValues(enumType)
        Dim styles(values.Length - 1) As Object
        values.CopyTo(styles, 0)
        combo.Items.AddRange(styles)
    End Sub

    Private Sub chkVisualStyles_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkVisualStyles.CheckedChanged
        'You can use themes only in some areas of the control
        'but in this sample we use just all or none
        If chkVisualStyles.Checked Then
            m_Grid.ThemedAreas = ThemedArea.All
        Else
            m_Grid.ThemedAreas = ThemedArea.None
        End If
        EnableCombos()


    End Sub

    Private Sub chkGroupByBox_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkGroupByBox.CheckedChanged
        m_Grid.GroupByBoxVisible = chkGroupByBox.Checked
    End Sub

    Private Sub chkRowHeaders_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRowHeaders.CheckedChanged
        If chkRowHeaders.Checked Then
            m_Grid.RowHeaders = InheritableBoolean.True
        Else
            m_Grid.RowHeaders = InheritableBoolean.False
        End If
    End Sub

    Private Sub chkAlternatingRows_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkAlternatingRows.CheckedChanged
        m_Grid.AlternatingColors = chkAlternatingRows.Checked
    End Sub

    Private Sub chkRecordNavigator_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkRecordNavigator.CheckedChanged
        m_Grid.RecordNavigator = chkRecordNavigator.Checked
    End Sub

    Private Sub cboBorder_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboBorder.SelectedIndexChanged
        m_Grid.BorderStyle = CType(cboBorder.SelectedItem, Janus.Windows.GridEX.BorderStyle)
    End Sub

    Private Sub cboHeaderAppearance_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboHeaderAppearance.SelectedIndexChanged
        m_Grid.HeaderFormatStyle.Appearance = CType(cboHeaderAppearance.SelectedItem, Janus.Windows.GridEX.Appearance)
        'changing header appearance in the dropdown too
        'm_Grid.DropDowns("Suppliers").HeaderFormatStyle.Appearance = m_Grid.HeaderFormatStyle.Appearance
    End Sub

    Private Sub cboButtomAppearance_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboButtomAppearance.SelectedIndexChanged
        m_Grid.ControlStyle.ButtonAppearance = CType(cboButtomAppearance.SelectedItem, Janus.Windows.GridEX.ButtonAppearance)
    End Sub

    Private Sub cboGridlines_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboGridlines.SelectedIndexChanged
        m_Grid.GridLines = CType(cboGridlines.SelectedItem, Janus.Windows.GridEX.GridLines)
    End Sub

    Private Sub cboGridlineStyle_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboGridlineStyle.SelectedIndexChanged
        m_Grid.GridLineStyle = CType(cboGridlineStyle.SelectedItem, Janus.Windows.GridEX.GridLineStyle)
    End Sub

    Private Sub chkOffice2003_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles chkOffice2003.CheckedChanged
        'You can use Office2003 style only in some areas of the control
        'but in this sample we use just all or none
        If chkOffice2003.Checked Then
            m_Grid.VisualStyle = VisualStyle.Office2003
        Else
            m_Grid.VisualStyle = VisualStyle.Standard
        End If
        EnableCombos()

    End Sub


    Private Sub CheckBox1_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles CheckBox1.CheckedChanged
        On Error Resume Next
        m_Grid.TotalRow = CheckBox1.Checked
    End Sub
End Class