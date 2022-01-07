namespace OpenDental {
	partial class FormDoseSpotAssignUserId {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDoseSpotAssignUserId));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textUserId = new System.Windows.Forms.TextBox();
			this.labelUserId = new System.Windows.Forms.Label();
			this.comboDoseUsers = new OpenDental.UI.ComboBoxOD();
			this.butUsertPick = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(204, 132);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(285, 132);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textUserId
			// 
			this.textUserId.Location = new System.Drawing.Point(160, 24);
			this.textUserId.Name = "textUserId";
			this.textUserId.ReadOnly = true;
			this.textUserId.Size = new System.Drawing.Size(149, 20);
			this.textUserId.TabIndex = 4;
			// 
			// labelUserId
			// 
			this.labelUserId.Location = new System.Drawing.Point(36, 24);
			this.labelUserId.Name = "labelUserId";
			this.labelUserId.Size = new System.Drawing.Size(118, 20);
			this.labelUserId.TabIndex = 5;
			this.labelUserId.Text = "DoseSpot User ID";
			this.labelUserId.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboDoseUsers
			// 
			this.comboDoseUsers.Location = new System.Drawing.Point(160, 62);
			this.comboDoseUsers.Name = "comboDoseUsers";
			this.comboDoseUsers.Size = new System.Drawing.Size(120, 21);
			this.comboDoseUsers.TabIndex = 6;
			this.comboDoseUsers.SelectionChangeCommitted += new System.EventHandler(this.comboDoseUsers_SelectionChangeCommitted);
			// 
			// butUsertPick
			// 
			this.butUsertPick.Location = new System.Drawing.Point(282, 59);
			this.butUsertPick.Name = "butUsertPick";
			this.butUsertPick.Size = new System.Drawing.Size(27, 24);
			this.butUsertPick.TabIndex = 24;
			this.butUsertPick.Text = "...";
			this.butUsertPick.Click += new System.EventHandler(this.butUserPick_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(38, 62);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 20);
			this.label1.TabIndex = 25;
			this.label1.Text = "User to Assign";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDoseSpotAssignUserId
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(372, 168);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butUsertPick);
			this.Controls.Add(this.comboDoseUsers);
			this.Controls.Add(this.labelUserId);
			this.Controls.Add(this.textUserId);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDoseSpotAssignUserId";
			this.Text = "Assign User ID";
			this.Load += new System.EventHandler(this.FormDoseSpotAssignUserId_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textUserId;
		private System.Windows.Forms.Label labelUserId;
		private UI.ComboBoxOD comboDoseUsers;
		private UI.Button butUsertPick;
		private System.Windows.Forms.Label label1;
	}
}