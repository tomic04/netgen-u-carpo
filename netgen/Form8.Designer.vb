<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form8
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
        Me.PrintControl1 = New DevExpress.XtraPrinting.Control.PrintControl()
        Me.SuspendLayout()
        '
        'PrintControl1
        '
        Me.PrintControl1.IsMetric = True
        Me.PrintControl1.Location = New System.Drawing.Point(-6, 12)
        Me.PrintControl1.Name = "PrintControl1"
        Me.PrintControl1.Size = New System.Drawing.Size(489, 345)
        Me.PrintControl1.TabIndex = 0
        '
        'Form8
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(696, 477)
        Me.Controls.Add(Me.PrintControl1)
        Me.Name = "Form8"
        Me.Text = "Form8"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents PrintControl1 As DevExpress.XtraPrinting.Control.PrintControl
End Class
