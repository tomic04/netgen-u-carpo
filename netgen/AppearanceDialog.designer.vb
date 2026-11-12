<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class AppearanceDialog
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If disposing AndAlso components IsNot Nothing Then
            components.Dispose()
        End If
        MyBase.Dispose(disposing)
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.chkRecordNavigator = New System.Windows.Forms.CheckBox
        Me.chkAlternatingRows = New System.Windows.Forms.CheckBox
        Me.chkOffice2003 = New System.Windows.Forms.CheckBox
        Me.chkGroupByBox = New System.Windows.Forms.CheckBox
        Me.chkRowHeaders = New System.Windows.Forms.CheckBox
        Me.cboGridlineStyle = New System.Windows.Forms.ComboBox
        Me.label5 = New System.Windows.Forms.Label
        Me.cboGridlines = New System.Windows.Forms.ComboBox
        Me.Gridlines = New System.Windows.Forms.Label
        Me.cboButtomAppearance = New System.Windows.Forms.ComboBox
        Me.label4 = New System.Windows.Forms.Label
        Me.label3 = New System.Windows.Forms.Label
        Me.cboHeaderAppearance = New System.Windows.Forms.ComboBox
        Me.label2 = New System.Windows.Forms.Label
        Me.chkVisualStyles = New System.Windows.Forms.CheckBox
        Me.cboBorder = New System.Windows.Forms.ComboBox
        Me.label1 = New System.Windows.Forms.Label
        Me.CheckBox1 = New System.Windows.Forms.CheckBox
        Me.SuspendLayout()
        '
        'chkRecordNavigator
        '
        Me.chkRecordNavigator.Location = New System.Drawing.Point(280, 77)
        Me.chkRecordNavigator.Name = "chkRecordNavigator"
        Me.chkRecordNavigator.Size = New System.Drawing.Size(112, 24)
        Me.chkRecordNavigator.TabIndex = 48
        Me.chkRecordNavigator.Text = "Record Navigator"
        '
        'chkAlternatingRows
        '
        Me.chkAlternatingRows.Location = New System.Drawing.Point(152, 77)
        Me.chkAlternatingRows.Name = "chkAlternatingRows"
        Me.chkAlternatingRows.Size = New System.Drawing.Size(136, 24)
        Me.chkAlternatingRows.TabIndex = 47
        Me.chkAlternatingRows.Text = "Alternating Rows"
        '
        'chkOffice2003
        '
        Me.chkOffice2003.Location = New System.Drawing.Point(4, 77)
        Me.chkOffice2003.Name = "chkOffice2003"
        Me.chkOffice2003.Size = New System.Drawing.Size(144, 24)
        Me.chkOffice2003.TabIndex = 49
        Me.chkOffice2003.Text = "VisualStyle Office2003"
        '
        'chkGroupByBox
        '
        Me.chkGroupByBox.Location = New System.Drawing.Point(156, 45)
        Me.chkGroupByBox.Name = "chkGroupByBox"
        Me.chkGroupByBox.Size = New System.Drawing.Size(112, 24)
        Me.chkGroupByBox.TabIndex = 46
        Me.chkGroupByBox.Text = "Group By Box"
        '
        'chkRowHeaders
        '
        Me.chkRowHeaders.Location = New System.Drawing.Point(280, 45)
        Me.chkRowHeaders.Name = "chkRowHeaders"
        Me.chkRowHeaders.Size = New System.Drawing.Size(104, 24)
        Me.chkRowHeaders.TabIndex = 45
        Me.chkRowHeaders.Text = "Row Headers"
        '
        'cboGridlineStyle
        '
        Me.cboGridlineStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboGridlineStyle.Location = New System.Drawing.Point(252, 221)
        Me.cboGridlineStyle.Name = "cboGridlineStyle"
        Me.cboGridlineStyle.Size = New System.Drawing.Size(140, 21)
        Me.cboGridlineStyle.TabIndex = 44
        '
        'label5
        '
        Me.label5.Location = New System.Drawing.Point(4, 225)
        Me.label5.Name = "label5"
        Me.label5.Size = New System.Drawing.Size(240, 20)
        Me.label5.TabIndex = 43
        Me.label5.Text = "Gridline Style:"
        Me.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboGridlines
        '
        Me.cboGridlines.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboGridlines.Location = New System.Drawing.Point(252, 193)
        Me.cboGridlines.Name = "cboGridlines"
        Me.cboGridlines.Size = New System.Drawing.Size(140, 21)
        Me.cboGridlines.TabIndex = 42
        '
        'Gridlines
        '
        Me.Gridlines.Location = New System.Drawing.Point(4, 197)
        Me.Gridlines.Name = "Gridlines"
        Me.Gridlines.Size = New System.Drawing.Size(240, 20)
        Me.Gridlines.TabIndex = 41
        Me.Gridlines.Text = "Gridlines:"
        Me.Gridlines.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboButtomAppearance
        '
        Me.cboButtomAppearance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboButtomAppearance.Location = New System.Drawing.Point(252, 165)
        Me.cboButtomAppearance.Name = "cboButtomAppearance"
        Me.cboButtomAppearance.Size = New System.Drawing.Size(140, 21)
        Me.cboButtomAppearance.TabIndex = 40
        '
        'label4
        '
        Me.label4.Location = New System.Drawing.Point(4, 169)
        Me.label4.Name = "label4"
        Me.label4.Size = New System.Drawing.Size(240, 20)
        Me.label4.TabIndex = 39
        Me.label4.Text = "Scrollbars and dropdown buttons appearance:"
        Me.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'label3
        '
        Me.label3.BackColor = System.Drawing.SystemColors.Info
        Me.label3.Dock = System.Windows.Forms.DockStyle.Top
        Me.label3.ForeColor = System.Drawing.SystemColors.InfoText
        Me.label3.Location = New System.Drawing.Point(0, 0)
        Me.label3.Name = "label3"
        Me.label3.Size = New System.Drawing.Size(400, 36)
        Me.label3.TabIndex = 38
        Me.label3.Tag = ""
        Me.label3.Text = "Some of the appearance settings are not applied when running on Windows XP and Vi" & _
            "sual Styles is checked."
        Me.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'cboHeaderAppearance
        '
        Me.cboHeaderAppearance.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboHeaderAppearance.Location = New System.Drawing.Point(252, 137)
        Me.cboHeaderAppearance.Name = "cboHeaderAppearance"
        Me.cboHeaderAppearance.Size = New System.Drawing.Size(140, 21)
        Me.cboHeaderAppearance.TabIndex = 37
        '
        'label2
        '
        Me.label2.Location = New System.Drawing.Point(4, 141)
        Me.label2.Name = "label2"
        Me.label2.Size = New System.Drawing.Size(240, 20)
        Me.label2.TabIndex = 36
        Me.label2.Text = "Header Appearance:"
        Me.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'chkVisualStyles
        '
        Me.chkVisualStyles.Location = New System.Drawing.Point(4, 45)
        Me.chkVisualStyles.Name = "chkVisualStyles"
        Me.chkVisualStyles.Size = New System.Drawing.Size(136, 24)
        Me.chkVisualStyles.TabIndex = 35
        Me.chkVisualStyles.Text = "Use XP Visual Styles"
        '
        'cboBorder
        '
        Me.cboBorder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cboBorder.Location = New System.Drawing.Point(252, 109)
        Me.cboBorder.Name = "cboBorder"
        Me.cboBorder.Size = New System.Drawing.Size(140, 21)
        Me.cboBorder.TabIndex = 34
        '
        'label1
        '
        Me.label1.Location = New System.Drawing.Point(4, 113)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(240, 20)
        Me.label1.TabIndex = 33
        Me.label1.Text = "Border Style:"
        Me.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft
        '
        'CheckBox1
        '
        Me.CheckBox1.AutoSize = True
        Me.CheckBox1.Location = New System.Drawing.Point(7, 261)
        Me.CheckBox1.Name = "CheckBox1"
        Me.CheckBox1.Size = New System.Drawing.Size(70, 17)
        Me.CheckBox1.TabIndex = 50
        Me.CheckBox1.Text = "Total row"
        Me.CheckBox1.UseVisualStyleBackColor = True
        '
        'AppearanceDialog
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(400, 290)
        Me.Controls.Add(Me.CheckBox1)
        Me.Controls.Add(Me.chkRecordNavigator)
        Me.Controls.Add(Me.chkAlternatingRows)
        Me.Controls.Add(Me.chkOffice2003)
        Me.Controls.Add(Me.chkGroupByBox)
        Me.Controls.Add(Me.chkRowHeaders)
        Me.Controls.Add(Me.cboGridlineStyle)
        Me.Controls.Add(Me.label5)
        Me.Controls.Add(Me.cboGridlines)
        Me.Controls.Add(Me.Gridlines)
        Me.Controls.Add(Me.cboButtomAppearance)
        Me.Controls.Add(Me.label4)
        Me.Controls.Add(Me.label3)
        Me.Controls.Add(Me.cboHeaderAppearance)
        Me.Controls.Add(Me.label2)
        Me.Controls.Add(Me.chkVisualStyles)
        Me.Controls.Add(Me.cboBorder)
        Me.Controls.Add(Me.label1)
        Me.Name = "AppearanceDialog"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "AppearanceDialog"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents chkRecordNavigator As System.Windows.Forms.CheckBox
    Friend WithEvents chkAlternatingRows As System.Windows.Forms.CheckBox
    Friend WithEvents chkOffice2003 As System.Windows.Forms.CheckBox
    Friend WithEvents chkGroupByBox As System.Windows.Forms.CheckBox
    Friend WithEvents chkRowHeaders As System.Windows.Forms.CheckBox
    Friend WithEvents cboGridlineStyle As System.Windows.Forms.ComboBox
    Friend WithEvents label5 As System.Windows.Forms.Label
    Friend WithEvents cboGridlines As System.Windows.Forms.ComboBox
    Friend WithEvents Gridlines As System.Windows.Forms.Label
    Friend WithEvents cboButtomAppearance As System.Windows.Forms.ComboBox
    Friend WithEvents label4 As System.Windows.Forms.Label
    Friend WithEvents label3 As System.Windows.Forms.Label
    Friend WithEvents cboHeaderAppearance As System.Windows.Forms.ComboBox
    Friend WithEvents label2 As System.Windows.Forms.Label
    Friend WithEvents chkVisualStyles As System.Windows.Forms.CheckBox
    Friend WithEvents cboBorder As System.Windows.Forms.ComboBox
    Friend WithEvents label1 As System.Windows.Forms.Label
    Friend WithEvents CheckBox1 As System.Windows.Forms.CheckBox
End Class
