namespace CentralManager {
	partial class FormCentralReportSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCentralReportSetup));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabReportPermissions = new System.Windows.Forms.TabPage();
			this.userControlReportSetup = new OpenDental.User_Controls.UserControlReportSetup();
			this.tabControl1.SuspendLayout();
			this.tabReportPermissions.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(388, 649);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(469, 649);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabReportPermissions);
			this.tabControl1.Location = new System.Drawing.Point(6, 4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(545, 638);
			this.tabControl1.TabIndex = 217;
			// 
			// tabReportPermissions
			// 
			this.tabReportPermissions.Controls.Add(this.userControlReportSetup);
			this.tabReportPermissions.Location = new System.Drawing.Point(4, 22);
			this.tabReportPermissions.Name = "tabReportPermissions";
			this.tabReportPermissions.Size = new System.Drawing.Size(537, 612);
			this.tabReportPermissions.TabIndex = 3;
			this.tabReportPermissions.Text = "Security Permissions";
			// 
			// userControlReportSetup
			// 
			this.userControlReportSetup.Location = new System.Drawing.Point(2, 2);
			this.userControlReportSetup.Name = "userControlReportSetup";
			this.userControlReportSetup.Size = new System.Drawing.Size(525, 607);
			this.userControlReportSetup.TabIndex = 2;
			// 
			// FormCentralReportSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(556, 681);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCentralReportSetup";
			this.Text = "Report Setup";
			this.Load += new System.EventHandler(this.FormCentralReportSetup_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabReportPermissions.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabReportPermissions;
		private OpenDental.User_Controls.UserControlReportSetup userControlReportSetup;
	}
}