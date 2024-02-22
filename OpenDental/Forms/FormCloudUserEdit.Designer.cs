namespace OpenDental {
	partial class FormCloudUserEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCloudUserEdit));
			this.textUserName = new System.Windows.Forms.TextBox();
			this.labelUserName = new System.Windows.Forms.Label();
			this.labelUserNameInfo = new System.Windows.Forms.Label();
			this.labelDisplayNameInfo = new System.Windows.Forms.Label();
			this.labelDisplayName = new System.Windows.Forms.Label();
			this.textDisplayName = new System.Windows.Forms.TextBox();
			this.labelLastName = new System.Windows.Forms.Label();
			this.textLastName = new System.Windows.Forms.TextBox();
			this.labelFirstName = new System.Windows.Forms.Label();
			this.textFirstName = new System.Windows.Forms.TextBox();
			this.labelEmailInfo = new System.Windows.Forms.Label();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.butSave = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.listGroups = new OpenDental.UI.ListBox();
			this.labelGroups = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// textUserName
			// 
			this.textUserName.Location = new System.Drawing.Point(96, 15);
			this.textUserName.Name = "textUserName";
			this.textUserName.Size = new System.Drawing.Size(200, 20);
			this.textUserName.TabIndex = 0;
			this.textUserName.WordWrap = false;
			// 
			// labelUserName
			// 
			this.labelUserName.Location = new System.Drawing.Point(12, 17);
			this.labelUserName.Name = "labelUserName";
			this.labelUserName.Size = new System.Drawing.Size(83, 17);
			this.labelUserName.TabIndex = 0;
			this.labelUserName.Text = "User Name";
			this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelUserNameInfo
			// 
			this.labelUserNameInfo.Location = new System.Drawing.Point(302, 12);
			this.labelUserNameInfo.Name = "labelUserNameInfo";
			this.labelUserNameInfo.Size = new System.Drawing.Size(313, 27);
			this.labelUserNameInfo.TabIndex = 0;
			this.labelUserNameInfo.Text = "Maximum length of 128 characters. Can only contain\r\nalphanumeric characters or an" +
    "y of the following: +=,.@-_";
			this.labelUserNameInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDisplayNameInfo
			// 
			this.labelDisplayNameInfo.Location = new System.Drawing.Point(302, 95);
			this.labelDisplayNameInfo.Name = "labelDisplayNameInfo";
			this.labelDisplayNameInfo.Size = new System.Drawing.Size(313, 17);
			this.labelDisplayNameInfo.TabIndex = 0;
			this.labelDisplayNameInfo.Text = "Defaults to first and last name";
			this.labelDisplayNameInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelDisplayName
			// 
			this.labelDisplayName.Location = new System.Drawing.Point(12, 95);
			this.labelDisplayName.Name = "labelDisplayName";
			this.labelDisplayName.Size = new System.Drawing.Size(83, 17);
			this.labelDisplayName.TabIndex = 0;
			this.labelDisplayName.Text = "Display Name";
			this.labelDisplayName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDisplayName
			// 
			this.textDisplayName.Location = new System.Drawing.Point(96, 93);
			this.textDisplayName.Name = "textDisplayName";
			this.textDisplayName.Size = new System.Drawing.Size(200, 20);
			this.textDisplayName.TabIndex = 3;
			this.textDisplayName.WordWrap = false;
			// 
			// labelLastName
			// 
			this.labelLastName.Location = new System.Drawing.Point(12, 69);
			this.labelLastName.Name = "labelLastName";
			this.labelLastName.Size = new System.Drawing.Size(83, 17);
			this.labelLastName.TabIndex = 0;
			this.labelLastName.Text = "Last Name";
			this.labelLastName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLastName
			// 
			this.textLastName.Location = new System.Drawing.Point(96, 67);
			this.textLastName.Name = "textLastName";
			this.textLastName.Size = new System.Drawing.Size(200, 20);
			this.textLastName.TabIndex = 2;
			this.textLastName.WordWrap = false;
			// 
			// labelFirstName
			// 
			this.labelFirstName.Location = new System.Drawing.Point(12, 43);
			this.labelFirstName.Name = "labelFirstName";
			this.labelFirstName.Size = new System.Drawing.Size(83, 17);
			this.labelFirstName.TabIndex = 0;
			this.labelFirstName.Text = "First Name";
			this.labelFirstName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFirstName
			// 
			this.textFirstName.Location = new System.Drawing.Point(96, 41);
			this.textFirstName.Name = "textFirstName";
			this.textFirstName.Size = new System.Drawing.Size(200, 20);
			this.textFirstName.TabIndex = 1;
			this.textFirstName.WordWrap = false;
			// 
			// labelEmailInfo
			// 
			this.labelEmailInfo.Location = new System.Drawing.Point(302, 121);
			this.labelEmailInfo.Name = "labelEmailInfo";
			this.labelEmailInfo.Size = new System.Drawing.Size(313, 17);
			this.labelEmailInfo.TabIndex = 0;
			this.labelEmailInfo.Text = "New users will receive an email with password setup instructions";
			this.labelEmailInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(12, 121);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(83, 17);
			this.labelEmail.TabIndex = 0;
			this.labelEmail.Text = "Email Address";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(96, 119);
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(200, 20);
			this.textEmail.TabIndex = 4;
			this.textEmail.WordWrap = false;
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butSave.Location = new System.Drawing.Point(525, 207);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(90, 24);
			this.butSave.TabIndex = 5;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 207);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// listGroups
			// 
			this.listGroups.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.listGroups.Location = new System.Drawing.Point(96, 145);
			this.listGroups.Name = "listGroups";
			this.listGroups.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listGroups.Size = new System.Drawing.Size(200, 56);
			this.listGroups.TabIndex = 7;
			this.listGroups.Text = "listGroups";
			// 
			// labelGroups
			// 
			this.labelGroups.Location = new System.Drawing.Point(12, 145);
			this.labelGroups.Name = "labelGroups";
			this.labelGroups.Size = new System.Drawing.Size(83, 17);
			this.labelGroups.TabIndex = 8;
			this.labelGroups.Text = "Groups";
			this.labelGroups.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCloudUserEdit
			// 
			this.ClientSize = new System.Drawing.Size(627, 243);
			this.Controls.Add(this.labelGroups);
			this.Controls.Add(this.listGroups);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.labelEmailInfo);
			this.Controls.Add(this.labelEmail);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.labelFirstName);
			this.Controls.Add(this.textFirstName);
			this.Controls.Add(this.labelLastName);
			this.Controls.Add(this.textLastName);
			this.Controls.Add(this.labelDisplayNameInfo);
			this.Controls.Add(this.labelDisplayName);
			this.Controls.Add(this.textDisplayName);
			this.Controls.Add(this.labelUserNameInfo);
			this.Controls.Add(this.labelUserName);
			this.Controls.Add(this.textUserName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCloudUserEdit";
			this.Text = "Edit Cloud User";
			this.Load += new System.EventHandler(this.FormCloudUserEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textUserName;
		private System.Windows.Forms.Label labelUserName;
		private System.Windows.Forms.Label labelUserNameInfo;
		private System.Windows.Forms.Label labelDisplayNameInfo;
		private System.Windows.Forms.Label labelDisplayName;
		private System.Windows.Forms.TextBox textDisplayName;
		private System.Windows.Forms.Label labelLastName;
		private System.Windows.Forms.TextBox textLastName;
		private System.Windows.Forms.Label labelFirstName;
		private System.Windows.Forms.TextBox textFirstName;
		private System.Windows.Forms.Label labelEmailInfo;
		private System.Windows.Forms.Label labelEmail;
		private System.Windows.Forms.TextBox textEmail;
		private UI.Button butSave;
		private UI.Button butDelete;
		private UI.ListBox listGroups;
		private System.Windows.Forms.Label labelGroups;
	}
}