namespace Carpo.Gen.Forms
{
    partial class Manager
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Manager));
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid.Panel1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid.Panel2)).BeginInit();
            this.splitGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).BeginInit();
            this.SuspendLayout();
            // 
            // splitGrid
            // 
            this.splitGrid.Margin = new System.Windows.Forms.Padding(3, 5, 3, 5);
            this.splitGrid.Size = new System.Drawing.Size(640, 374);
            this.splitGrid.SplitterPosition = 294;
            // 
            // ribbonControl1
            // 
            this.ribbonControl1.EmptyAreaImageOptions.ImagePadding = new System.Windows.Forms.Padding(48, 57, 48, 57);
            this.ribbonControl1.ExpandCollapseItem.Id = 0;
            this.ribbonControl1.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.ribbonControl1.OptionsMenuMinWidth = 524;
            this.ribbonControl1.Size = new System.Drawing.Size(948, 71);
            this.ribbonControl1.Toolbar.ShowCustomizeItem = false;
            // 
            // Manager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 560);
            this.IconOptions.Icon = ((System.Drawing.Icon)(resources.GetObject("Manager.IconOptions.Icon")));
            this.Margin = new System.Windows.Forms.Padding(3, 9, 3, 9);
            this.Name = "Manager";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid.Panel1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid.Panel2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitGrid)).EndInit();
            this.splitGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ribbonControl1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}

