<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form6
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.groupBox1 = New System.Windows.Forms.GroupBox
        Me.numCopies = New System.Windows.Forms.NumericUpDown
        Me.label1 = New System.Windows.Forms.Label
        Me.gbOrientation = New System.Windows.Forms.GroupBox
        Me.rbLandscape = New System.Windows.Forms.RadioButton
        Me.rbPortrait = New System.Windows.Forms.RadioButton
        Me.cbPisaci = New System.Windows.Forms.ComboBox
        Me.rbPisaci = New System.Windows.Forms.RadioButton
        Me.rbPreview = New System.Windows.Forms.RadioButton
        Me.panel4 = New System.Windows.Forms.Panel
        Me.btnPrintOptions = New System.Windows.Forms.Button
        Me.btnCancel = New System.Windows.Forms.Button
        Me.btnOK = New System.Windows.Forms.Button
        Me.printDialog1 = New System.Windows.Forms.PrintDialog
        Me.groupBox1.SuspendLayout()
        CType(Me.numCopies, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.gbOrientation.SuspendLayout()
        Me.panel4.SuspendLayout()
        Me.SuspendLayout()
        '
        'groupBox1
        '
        Me.groupBox1.Controls.Add(Me.numCopies)
        Me.groupBox1.Controls.Add(Me.label1)
        Me.groupBox1.Controls.Add(Me.gbOrientation)
        Me.groupBox1.Controls.Add(Me.cbPisaci)
        Me.groupBox1.Controls.Add(Me.rbPisaci)
        Me.groupBox1.Controls.Add(Me.rbPreview)
        Me.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.groupBox1.Location = New System.Drawing.Point(0, 0)
        Me.groupBox1.Name = "groupBox1"
        Me.groupBox1.Size = New System.Drawing.Size(374, 220)
        Me.groupBox1.TabIndex = 1
        Me.groupBox1.TabStop = False
        Me.groupBox1.Text = "Pregled"
        '
        'numCopies
        '
        Me.numCopies.Location = New System.Drawing.Point(100, 129)
        Me.numCopies.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.numCopies.Name = "numCopies"
        Me.numCopies.Size = New System.Drawing.Size(44, 20)
        Me.numCopies.TabIndex = 3
        Me.numCopies.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'label1
        '
        Me.label1.AutoSize = True
        Me.label1.Location = New System.Drawing.Point(35, 136)
        Me.label1.Name = "label1"
        Me.label1.Size = New System.Drawing.Size(59, 13)
        Me.label1.TabIndex = 4
        Me.label1.Text = "Broj kopija:"
        '
        'gbOrientation
        '
        Me.gbOrientation.Controls.Add(Me.rbLandscape)
        Me.gbOrientation.Controls.Add(Me.rbPortrait)
        Me.gbOrientation.Location = New System.Drawing.Point(179, 108)
        Me.gbOrientation.Name = "gbOrientation"
        Me.gbOrientation.Size = New System.Drawing.Size(168, 46)
        Me.gbOrientation.TabIndex = 4
        Me.gbOrientation.TabStop = False
        Me.gbOrientation.Text = "Orijentacija"
        Me.gbOrientation.Visible = False
        '
        'rbLandscape
        '
        Me.rbLandscape.AutoSize = True
        Me.rbLandscape.Location = New System.Drawing.Point(74, 21)
        Me.rbLandscape.Name = "rbLandscape"
        Me.rbLandscape.Size = New System.Drawing.Size(78, 17)
        Me.rbLandscape.TabIndex = 1
        Me.rbLandscape.Text = "&Landscape"
        Me.rbLandscape.UseVisualStyleBackColor = True
        '
        'rbPortrait
        '
        Me.rbPortrait.AutoSize = True
        Me.rbPortrait.Checked = True
        Me.rbPortrait.Location = New System.Drawing.Point(10, 21)
        Me.rbPortrait.Name = "rbPortrait"
        Me.rbPortrait.Size = New System.Drawing.Size(58, 17)
        Me.rbPortrait.TabIndex = 0
        Me.rbPortrait.TabStop = True
        Me.rbPortrait.Text = "&Portrait"
        Me.rbPortrait.UseVisualStyleBackColor = True
        '
        'cbPisaci
        '
        Me.cbPisaci.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList
        Me.cbPisaci.FormattingEnabled = True
        Me.cbPisaci.Location = New System.Drawing.Point(38, 73)
        Me.cbPisaci.Name = "cbPisaci"
        Me.cbPisaci.Size = New System.Drawing.Size(310, 21)
        Me.cbPisaci.TabIndex = 2
        '
        'rbPisaci
        '
        Me.rbPisaci.AutoSize = True
        Me.rbPisaci.Location = New System.Drawing.Point(21, 50)
        Me.rbPisaci.Name = "rbPisaci"
        Me.rbPisaci.Size = New System.Drawing.Size(89, 17)
        Me.rbPisaci.TabIndex = 1
        Me.rbPisaci.Text = "&Ispis na pisač"
        Me.rbPisaci.UseVisualStyleBackColor = True
        '
        'rbPreview
        '
        Me.rbPreview.AutoSize = True
        Me.rbPreview.Checked = True
        Me.rbPreview.Location = New System.Drawing.Point(21, 27)
        Me.rbPreview.Name = "rbPreview"
        Me.rbPreview.Size = New System.Drawing.Size(106, 17)
        Me.rbPreview.TabIndex = 0
        Me.rbPreview.TabStop = True
        Me.rbPreview.Text = "Prikaz na &monitor"
        Me.rbPreview.UseVisualStyleBackColor = True
        '
        'panel4
        '
        Me.panel4.Controls.Add(Me.btnPrintOptions)
        Me.panel4.Controls.Add(Me.btnCancel)
        Me.panel4.Controls.Add(Me.btnOK)
        Me.panel4.Dock = System.Windows.Forms.DockStyle.Bottom
        Me.panel4.Location = New System.Drawing.Point(0, 181)
        Me.panel4.Name = "panel4"
        Me.panel4.Size = New System.Drawing.Size(374, 39)
        Me.panel4.TabIndex = 2
        '
        'btnPrintOptions
        '
        Me.btnPrintOptions.Enabled = False
        Me.btnPrintOptions.Location = New System.Drawing.Point(201, 8)
        Me.btnPrintOptions.Name = "btnPrintOptions"
        Me.btnPrintOptions.Size = New System.Drawing.Size(75, 23)
        Me.btnPrintOptions.TabIndex = 9
        Me.btnPrintOptions.Text = "Opcije"
        Me.btnPrintOptions.UseVisualStyleBackColor = True
        '
        'btnCancel
        '
        Me.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel
        Me.btnCancel.Location = New System.Drawing.Point(282, 8)
        Me.btnCancel.Name = "btnCancel"
        Me.btnCancel.Size = New System.Drawing.Size(75, 23)
        Me.btnCancel.TabIndex = 10
        Me.btnCancel.Text = "Odustani"
        Me.btnCancel.UseVisualStyleBackColor = True
        '
        'btnOK
        '
        Me.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK
        Me.btnOK.Location = New System.Drawing.Point(120, 8)
        Me.btnOK.Name = "btnOK"
        Me.btnOK.Size = New System.Drawing.Size(75, 23)
        Me.btnOK.TabIndex = 8
        Me.btnOK.Text = "Ispiši"
        Me.btnOK.UseVisualStyleBackColor = True
        '
        'printDialog1
        '
        Me.printDialog1.AllowCurrentPage = True
        Me.printDialog1.AllowSelection = True
        Me.printDialog1.AllowSomePages = True
        Me.printDialog1.ShowHelp = True
        Me.printDialog1.UseEXDialog = True
        '
        'Form6
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(374, 220)
        Me.Controls.Add(Me.panel4)
        Me.Controls.Add(Me.groupBox1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.MaximizeBox = False
        Me.MinimizeBox = False
        Me.Name = "Form6"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent
        Me.Text = "Opcije ispisa"
        Me.groupBox1.ResumeLayout(False)
        Me.groupBox1.PerformLayout()
        CType(Me.numCopies, System.ComponentModel.ISupportInitialize).EndInit()
        Me.gbOrientation.ResumeLayout(False)
        Me.gbOrientation.PerformLayout()
        Me.panel4.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Private WithEvents groupBox1 As System.Windows.Forms.GroupBox
    Public WithEvents numCopies As System.Windows.Forms.NumericUpDown
    Private WithEvents label1 As System.Windows.Forms.Label
    Private WithEvents gbOrientation As System.Windows.Forms.GroupBox
    Public WithEvents rbLandscape As System.Windows.Forms.RadioButton
    Public WithEvents rbPortrait As System.Windows.Forms.RadioButton
    Private WithEvents cbPisaci As System.Windows.Forms.ComboBox
    Public WithEvents rbPisaci As System.Windows.Forms.RadioButton
    Public WithEvents rbPreview As System.Windows.Forms.RadioButton
    Private WithEvents panel4 As System.Windows.Forms.Panel
    Private WithEvents btnPrintOptions As System.Windows.Forms.Button
    Private WithEvents btnCancel As System.Windows.Forms.Button
    Private WithEvents btnOK As System.Windows.Forms.Button
    Private WithEvents printDialog1 As System.Windows.Forms.PrintDialog
End Class
