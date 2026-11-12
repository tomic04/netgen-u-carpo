namespace Carpo.UI.Components
{
    partial class JTreeLookUpControl
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

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.fProperties = new DevExpress.XtraEditors.Repository.RepositoryItemTreeListLookUpEdit();
            this.fPropertiesTreeList = new DevExpress.XtraTreeList.TreeList();
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fPropertiesTreeList)).BeginInit();
            this.SuspendLayout();
            // 
            // fProperties
            // 
            this.fProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[] {
            new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)});
            this.fProperties.Name = "fProperties";
            this.fProperties.TreeList = this.fPropertiesTreeList;
            // 
            // fPropertiesTreeList
            // 
            this.fPropertiesTreeList.Location = new System.Drawing.Point(0, 0);
            this.fPropertiesTreeList.Name = "fPropertiesTreeList";
            this.fPropertiesTreeList.OptionsBehavior.EnableFiltering = true;
            this.fPropertiesTreeList.OptionsClipboard.AllowCopy = DevExpress.Utils.DefaultBoolean.True;
            this.fPropertiesTreeList.OptionsClipboard.CopyNodeHierarchy = DevExpress.Utils.DefaultBoolean.True;
            this.fPropertiesTreeList.OptionsView.ShowIndentAsRowStyle = true;
            this.fPropertiesTreeList.Size = new System.Drawing.Size(400, 200);
            this.fPropertiesTreeList.TabIndex = 0;
            ((System.ComponentModel.ISupportInitialize)(this.fProperties)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fPropertiesTreeList)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private new DevExpress.XtraEditors.Repository.RepositoryItemTreeListLookUpEdit fProperties;
        private DevExpress.XtraTreeList.TreeList fPropertiesTreeList;
    }
}
