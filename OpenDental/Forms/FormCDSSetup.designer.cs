namespace OpenDental{
	partial class FormCDSSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCDSSetup));
			this.gridMain = new OpenDental.UI.GridOD();
			this.radioGroup = new System.Windows.Forms.RadioButton();
			this.radioUser = new System.Windows.Forms.RadioButton();
			this.butOk = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.HScrollVisible = true;
			this.gridMain.Location = new System.Drawing.Point(12, 54);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(710, 433);
			this.gridMain.TabIndex = 60;
			this.gridMain.Title = "Users";
			this.gridMain.TranslationName = "TableSecurity";
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// radioGroup
			// 
			this.radioGroup.AutoCheck = false;
			this.radioGroup.Enabled = false;
			this.radioGroup.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioGroup.Location = new System.Drawing.Point(12, 30);
			this.radioGroup.Name = "radioGroup";
			this.radioGroup.Size = new System.Drawing.Size(158, 18);
			this.radioGroup.TabIndex = 62;
			this.radioGroup.Text = "by Group";
			this.radioGroup.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			// 
			// radioUser
			// 
			this.radioUser.AutoCheck = false;
			this.radioUser.Enabled = false;
			this.radioUser.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioUser.Location = new System.Drawing.Point(12, 8);
			this.radioUser.Name = "radioUser";
			this.radioUser.Size = new System.Drawing.Size(91, 22);
			this.radioUser.TabIndex = 61;
			this.radioUser.Text = "by User";
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(566, 493);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 63;
			this.butOk.Text = "Save";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(647, 493);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 64;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(462, 26);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(260, 18);
			this.label6.TabIndex = 65;
			this.label6.Text = "Click the cells to grant or revoke permissions.";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomRight;
			// 
			// FormCDSSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(734, 529);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.radioGroup);
			this.Controls.Add(this.radioUser);
			this.Controls.Add(this.gridMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCDSSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Clinical Decision Support Setup";
			this.Load += new System.EventHandler(this.FormCDSSetup_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.GridOD gridMain;
		private System.Windows.Forms.RadioButton radioGroup;
		private System.Windows.Forms.RadioButton radioUser;
		private System.Windows.Forms.Button butOk;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Label label6;
	}
}