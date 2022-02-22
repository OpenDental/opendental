namespace OpenDental{
	partial class FormSecurityCentralUserEdit {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
      System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSecurityCentralUserEdit));
      this.butEdit = new OpenDental.UI.Button();
      this.butClose = new OpenDental.UI.Button();
      this.gridCentralUsers = new OpenDental.UI.GridOD();
      this.SuspendLayout();
      // 
      // butEdit
      // 
      this.butEdit.Anchor = System.Windows.Forms.AnchorStyles.Right;
      this.butEdit.Location = new System.Drawing.Point(350, 191);
      this.butEdit.Name = "butEdit";
      this.butEdit.Size = new System.Drawing.Size(75, 24);
      this.butEdit.TabIndex = 3;
      this.butEdit.Text = "Edit";
      this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
      // 
      // butClose
      // 
      this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
      this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.butClose.Location = new System.Drawing.Point(350, 371);
      this.butClose.Name = "butClose";
      this.butClose.Size = new System.Drawing.Size(75, 24);
      this.butClose.TabIndex = 2;
      this.butClose.Text = "&Close";
      this.butClose.Click += new System.EventHandler(this.butClose_Click);
      // 
      // gridCentralUsers
      // 
      this.gridCentralUsers.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
      this.gridCentralUsers.HeadersVisible = false;
      this.gridCentralUsers.Location = new System.Drawing.Point(12, 9);
      this.gridCentralUsers.Name = "gridCentralUsers";
      this.gridCentralUsers.Size = new System.Drawing.Size(327, 386);
      this.gridCentralUsers.TabIndex = 4;
      this.gridCentralUsers.Title = "Users";
      this.gridCentralUsers.TranslationName = "TableCentralUsers";
      this.gridCentralUsers.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCentralUsers_CellDoubleClick);
      // 
      // FormSecurityCentralUserEdit
      // 
      this.AcceptButton = this.butEdit;
      this.CancelButton = this.butClose;
      this.ClientSize = new System.Drawing.Size(437, 407);
      this.Controls.Add(this.gridCentralUsers);
      this.Controls.Add(this.butEdit);
      this.Controls.Add(this.butClose);
      this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
      this.Name = "FormSecurityCentralUserEdit";
      this.Text = "Central User Edit";
      this.Load += new System.EventHandler(this.FormSecurityCentralUserEdit_Load);
      this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butEdit;
    private OpenDental.UI.Button butClose;
    private UI.GridOD gridCentralUsers;
  }
}