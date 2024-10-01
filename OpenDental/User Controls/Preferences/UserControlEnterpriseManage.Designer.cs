
namespace OpenDental {
	partial class UserControlEnterpriseManage {
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
			this.checkEnterpriseManualRefreshMainTaskLists = new OpenDental.UI.CheckBox();
			this.checkEraStrictClaimMatching = new OpenDental.UI.CheckBox();
			this.checkEraRefreshOnLoad = new OpenDental.UI.CheckBox();
			this.checkEraShowStatusAndClinic = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// checkRefresh
			// 
			this.checkEnterpriseManualRefreshMainTaskLists.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEnterpriseManualRefreshMainTaskLists.Location = new System.Drawing.Point(76, 78);
			this.checkEnterpriseManualRefreshMainTaskLists.Name = "checkRefresh";
			this.checkEnterpriseManualRefreshMainTaskLists.Size = new System.Drawing.Size(380, 18);
			this.checkEnterpriseManualRefreshMainTaskLists.TabIndex = 279;
			this.checkEnterpriseManualRefreshMainTaskLists.Text = "Tasks, \'Main\' and \'Reminders\' tabs require manual refresh";
			// 
			// checkEra835sStrictClaimMatching
			// 
			this.checkEraStrictClaimMatching.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraStrictClaimMatching.Location = new System.Drawing.Point(137, 42);
			this.checkEraStrictClaimMatching.Name = "checkEra835sStrictClaimMatching";
			this.checkEraStrictClaimMatching.Size = new System.Drawing.Size(319, 18);
			this.checkEraStrictClaimMatching.TabIndex = 277;
			this.checkEraStrictClaimMatching.Text = "ERA 835s use strict claim date matching";
			// 
			// checkEra835sRefreshOnLoad
			// 
			this.checkEraRefreshOnLoad.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraRefreshOnLoad.Location = new System.Drawing.Point(137, 24);
			this.checkEraRefreshOnLoad.Name = "checkEra835sRefreshOnLoad";
			this.checkEraRefreshOnLoad.Size = new System.Drawing.Size(319, 18);
			this.checkEraRefreshOnLoad.TabIndex = 276;
			this.checkEraRefreshOnLoad.Text = "ERA 835s window refresh data on load";
			// 
			// checkEra835sShowStatusAndClinic
			// 
			this.checkEraShowStatusAndClinic.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkEraShowStatusAndClinic.Location = new System.Drawing.Point(137, 60);
			this.checkEraShowStatusAndClinic.Name = "checkEra835sShowStatusAndClinic";
			this.checkEraShowStatusAndClinic.Size = new System.Drawing.Size(319, 18);
			this.checkEraShowStatusAndClinic.TabIndex = 278;
			this.checkEraShowStatusAndClinic.Text = "ERA 835s window show status and clinic information";
			// 
			// UserControlEnterpriseManage
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkEnterpriseManualRefreshMainTaskLists);
			this.Controls.Add(this.checkEraStrictClaimMatching);
			this.Controls.Add(this.checkEraRefreshOnLoad);
			this.Controls.Add(this.checkEraShowStatusAndClinic);
			this.Name = "UserControlEnterpriseManage";
			this.Size = new System.Drawing.Size(494, 660);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.CheckBox checkEnterpriseManualRefreshMainTaskLists;
		private UI.CheckBox checkEraStrictClaimMatching;
		private UI.CheckBox checkEraRefreshOnLoad;
		private UI.CheckBox checkEraShowStatusAndClinic;
	}
}
