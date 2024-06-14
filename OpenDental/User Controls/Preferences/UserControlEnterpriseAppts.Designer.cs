
namespace OpenDental {
	partial class UserControlEnterpriseAppts {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.checkEnterpriseHygProcUsePriProvFee = new OpenDental.UI.CheckBox();
			this.checkEnterpriseNoneApptViewDefaultDisabled = new OpenDental.UI.CheckBox();
			this.checkEnterpriseApptList = new OpenDental.UI.CheckBox();
			this.checkApptSecondaryProviderConsiderOpOnly = new OpenDental.UI.CheckBox();
			this.checkApptsRequireProc = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// checkEnterpriseHygProcUsePriProvFee
			// 
			this.checkEnterpriseHygProcUsePriProvFee.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseHygProcUsePriProvFee.Location = new System.Drawing.Point(134, 98);
			this.checkEnterpriseHygProcUsePriProvFee.Name = "checkEnterpriseHygProcUsePriProvFee";
			this.checkEnterpriseHygProcUsePriProvFee.Size = new System.Drawing.Size(320, 18);
			this.checkEnterpriseHygProcUsePriProvFee.TabIndex = 293;
			this.checkEnterpriseHygProcUsePriProvFee.Text = "Hygiene procedures use primary provider PPO fee";
			// 
			// checkEnterpriseNoneApptViewDefaultDisabled
			// 
			this.checkEnterpriseNoneApptViewDefaultDisabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseNoneApptViewDefaultDisabled.Location = new System.Drawing.Point(3, 76);
			this.checkEnterpriseNoneApptViewDefaultDisabled.Name = "checkEnterpriseNoneApptViewDefaultDisabled";
			this.checkEnterpriseNoneApptViewDefaultDisabled.Size = new System.Drawing.Size(452, 24);
			this.checkEnterpriseNoneApptViewDefaultDisabled.TabIndex = 292;
			this.checkEnterpriseNoneApptViewDefaultDisabled.Text = "Do not include \'None\' Appointment View when other views are available";
			// 
			// checkEnterpriseApptList
			// 
			this.checkEnterpriseApptList.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseApptList.Location = new System.Drawing.Point(134, 60);
			this.checkEnterpriseApptList.Name = "checkEnterpriseApptList";
			this.checkEnterpriseApptList.Size = new System.Drawing.Size(320, 18);
			this.checkEnterpriseApptList.TabIndex = 291;
			this.checkEnterpriseApptList.Text = "Enterprise Appointment Lists";
			// 
			// checkApptSecondaryProviderConsiderOpOnly
			// 
			this.checkApptSecondaryProviderConsiderOpOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptSecondaryProviderConsiderOpOnly.Location = new System.Drawing.Point(134, 42);
			this.checkApptSecondaryProviderConsiderOpOnly.Name = "checkApptSecondaryProviderConsiderOpOnly";
			this.checkApptSecondaryProviderConsiderOpOnly.Size = new System.Drawing.Size(320, 18);
			this.checkApptSecondaryProviderConsiderOpOnly.TabIndex = 290;
			this.checkApptSecondaryProviderConsiderOpOnly.Text = "Force op\'s hygiene provider as secondary provider";
			this.checkApptSecondaryProviderConsiderOpOnly.Click += new System.EventHandler(this.checkApptSecondaryProviderConsiderOpOnly_Click);
			// 
			// checkApptsRequireProc
			// 
			this.checkApptsRequireProc.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsRequireProc.Location = new System.Drawing.Point(134, 24);
			this.checkApptsRequireProc.Name = "checkApptsRequireProc";
			this.checkApptsRequireProc.Size = new System.Drawing.Size(320, 18);
			this.checkApptsRequireProc.TabIndex = 289;
			this.checkApptsRequireProc.Text = "Appointments require procedures";
			this.checkApptsRequireProc.Click += new System.EventHandler(this.checkApptsRequireProc_Click);
			// 
			// UserControlEnterpriseAppts
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkEnterpriseHygProcUsePriProvFee);
			this.Controls.Add(this.checkEnterpriseNoneApptViewDefaultDisabled);
			this.Controls.Add(this.checkEnterpriseApptList);
			this.Controls.Add(this.checkApptSecondaryProviderConsiderOpOnly);
			this.Controls.Add(this.checkApptsRequireProc);
			this.Name = "UserControlEnterpriseAppts";
			this.Size = new System.Drawing.Size(494, 660);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.CheckBox checkEnterpriseHygProcUsePriProvFee;
		private UI.CheckBox checkEnterpriseNoneApptViewDefaultDisabled;
		private UI.CheckBox checkEnterpriseApptList;
		private UI.CheckBox checkApptSecondaryProviderConsiderOpOnly;
		private UI.CheckBox checkApptsRequireProc;
	}
}
