namespace OpenDental {
	partial class FormJobTeamUserSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobTeamUserSelect));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.checkIsTeamLead = new System.Windows.Forms.CheckBox();
			this.labelUserName = new System.Windows.Forms.Label();
			this.comboUser = new System.Windows.Forms.ComboBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(225, 112);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65, 23);
			this.butCancel.TabIndex = 3;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(154, 112);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(65, 23);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// checkIsTeamLead
			// 
			this.checkIsTeamLead.Location = new System.Drawing.Point(89, 67);
			this.checkIsTeamLead.Name = "checkIsTeamLead";
			this.checkIsTeamLead.Size = new System.Drawing.Size(113, 17);
			this.checkIsTeamLead.TabIndex = 1;
			this.checkIsTeamLead.Text = "Team Lead";
			this.checkIsTeamLead.UseVisualStyleBackColor = true;
			// 
			// labelUserName
			// 
			this.labelUserName.Location = new System.Drawing.Point(48, 37);
			this.labelUserName.Name = "labelUserName";
			this.labelUserName.Size = new System.Drawing.Size(40, 20);
			this.labelUserName.TabIndex = 249;
			this.labelUserName.Text = "User";
			this.labelUserName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboUser
			// 
			this.comboUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUser.FormattingEnabled = true;
			this.comboUser.Location = new System.Drawing.Point(89, 37);
			this.comboUser.Name = "comboUser";
			this.comboUser.Size = new System.Drawing.Size(168, 21);
			this.comboUser.TabIndex = 0;
			// 
			// FormJobTeamUserSelect
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(302, 147);
			this.Controls.Add(this.comboUser);
			this.Controls.Add(this.labelUserName);
			this.Controls.Add(this.checkIsTeamLead);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobTeamUserSelect";
			this.Text = "Select Team Member";
			this.Load += new System.EventHandler(this.FormJobTeamUserSelect_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkIsTeamLead;
		private System.Windows.Forms.Label labelUserName;
		private System.Windows.Forms.ComboBox comboUser;
	}
}