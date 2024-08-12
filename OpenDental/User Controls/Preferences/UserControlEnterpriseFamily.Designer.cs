
namespace OpenDental {
	partial class UserControlEnterpriseFamily {
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
			this.checkShowFeeSchedGroups = new OpenDental.UI.CheckBox();
			this.groupClaimSnapshot = new OpenDental.UI.GroupBox();
			this.textClaimSnapshotRunTime = new System.Windows.Forms.TextBox();
			this.comboClaimSnapshotTriggerType = new OpenDental.UI.ComboBox();
			this.labelClaimSnapshotTrigger = new System.Windows.Forms.Label();
			this.labelClaimSnapshotRunTime = new System.Windows.Forms.Label();
			this.checkShowFeaturePatientClone = new OpenDental.UI.CheckBox();
			this.checkShowFeatureSuperfamilies = new OpenDental.UI.CheckBox();
			this.checkClaimSnapshotEnabled = new OpenDental.UI.CheckBox();
			this.checkCloneCreateSuperFamily = new OpenDental.UI.CheckBox();
			this.groupClaimSnapshot.SuspendLayout();
			this.SuspendLayout();
			// 
			// checkShowFeeSchedGroups
			// 
			this.checkShowFeeSchedGroups.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFeeSchedGroups.Location = new System.Drawing.Point(135, 96);
			this.checkShowFeeSchedGroups.Name = "checkShowFeeSchedGroups";
			this.checkShowFeeSchedGroups.Size = new System.Drawing.Size(319, 18);
			this.checkShowFeeSchedGroups.TabIndex = 287;
			this.checkShowFeeSchedGroups.Text = "Show Fee Schedule Groups";
			// 
			// groupClaimSnapshot
			// 
			this.groupClaimSnapshot.Controls.Add(this.textClaimSnapshotRunTime);
			this.groupClaimSnapshot.Controls.Add(this.comboClaimSnapshotTriggerType);
			this.groupClaimSnapshot.Controls.Add(this.labelClaimSnapshotTrigger);
			this.groupClaimSnapshot.Controls.Add(this.labelClaimSnapshotRunTime);
			this.groupClaimSnapshot.Location = new System.Drawing.Point(158, 120);
			this.groupClaimSnapshot.Name = "groupClaimSnapshot";
			this.groupClaimSnapshot.Size = new System.Drawing.Size(296, 73);
			this.groupClaimSnapshot.TabIndex = 286;
			this.groupClaimSnapshot.Text = "Claim Snapshot";
			// 
			// textClaimSnapshotRunTime
			// 
			this.textClaimSnapshotRunTime.Location = new System.Drawing.Point(194, 42);
			this.textClaimSnapshotRunTime.Name = "textClaimSnapshotRunTime";
			this.textClaimSnapshotRunTime.Size = new System.Drawing.Size(96, 20);
			this.textClaimSnapshotRunTime.TabIndex = 270;
			this.textClaimSnapshotRunTime.Validating += new System.ComponentModel.CancelEventHandler(this.textClaimSnapShotRunTime_Validating);
			// 
			// comboClaimSnapshotTriggerType
			// 
			this.comboClaimSnapshotTriggerType.Location = new System.Drawing.Point(118, 17);
			this.comboClaimSnapshotTriggerType.Name = "comboClaimSnapshotTriggerType";
			this.comboClaimSnapshotTriggerType.Size = new System.Drawing.Size(172, 21);
			this.comboClaimSnapshotTriggerType.TabIndex = 269;
			this.comboClaimSnapshotTriggerType.SelectionChangeCommitted += new System.EventHandler(this.comboClaimSnapshotTriggerType_ChangeCommitted);
			// 
			// labelClaimSnapshotTrigger
			// 
			this.labelClaimSnapshotTrigger.Location = new System.Drawing.Point(17, 18);
			this.labelClaimSnapshotTrigger.Name = "labelClaimSnapshotTrigger";
			this.labelClaimSnapshotTrigger.Size = new System.Drawing.Size(100, 18);
			this.labelClaimSnapshotTrigger.TabIndex = 272;
			this.labelClaimSnapshotTrigger.Text = "Snapshot Trigger";
			this.labelClaimSnapshotTrigger.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelClaimSnapshotRunTime
			// 
			this.labelClaimSnapshotRunTime.Location = new System.Drawing.Point(93, 43);
			this.labelClaimSnapshotRunTime.Name = "labelClaimSnapshotRunTime";
			this.labelClaimSnapshotRunTime.Size = new System.Drawing.Size(100, 18);
			this.labelClaimSnapshotRunTime.TabIndex = 271;
			this.labelClaimSnapshotRunTime.Text = "Service Run Time";
			this.labelClaimSnapshotRunTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShowFeaturePatientClone
			// 
			this.checkShowFeaturePatientClone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFeaturePatientClone.Location = new System.Drawing.Point(135, 42);
			this.checkShowFeaturePatientClone.Name = "checkShowFeaturePatientClone";
			this.checkShowFeaturePatientClone.Size = new System.Drawing.Size(319, 18);
			this.checkShowFeaturePatientClone.TabIndex = 285;
			this.checkShowFeaturePatientClone.Text = "Patient Clone";
			this.checkShowFeaturePatientClone.Click += new System.EventHandler(this.checkShowFeaturePatientClone_Click);
			// 
			// checkShowFeatureSuperfamilies
			// 
			this.checkShowFeatureSuperfamilies.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShowFeatureSuperfamilies.Location = new System.Drawing.Point(135, 24);
			this.checkShowFeatureSuperfamilies.Name = "checkShowFeatureSuperfamilies";
			this.checkShowFeatureSuperfamilies.Size = new System.Drawing.Size(319, 18);
			this.checkShowFeatureSuperfamilies.TabIndex = 284;
			this.checkShowFeatureSuperfamilies.Text = "Super Families";
			this.checkShowFeatureSuperfamilies.Click += new System.EventHandler(this.checkShowFeatureSuperFamilies_Click);
			// 
			// checkClaimSnapshotEnabled
			// 
			this.checkClaimSnapshotEnabled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkClaimSnapshotEnabled.Enabled = false;
			this.checkClaimSnapshotEnabled.Location = new System.Drawing.Point(135, 78);
			this.checkClaimSnapshotEnabled.Name = "checkClaimSnapshotEnabled";
			this.checkClaimSnapshotEnabled.Size = new System.Drawing.Size(319, 18);
			this.checkClaimSnapshotEnabled.TabIndex = 282;
			this.checkClaimSnapshotEnabled.Text = "Claim Snapshot Enabled";
			// 
			// checkCloneCreateSuperFamily
			// 
			this.checkCloneCreateSuperFamily.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkCloneCreateSuperFamily.Location = new System.Drawing.Point(77, 60);
			this.checkCloneCreateSuperFamily.Name = "checkCloneCreateSuperFamily";
			this.checkCloneCreateSuperFamily.Size = new System.Drawing.Size(377, 18);
			this.checkCloneCreateSuperFamily.TabIndex = 283;
			this.checkCloneCreateSuperFamily.Text = "New patient clones use super family instead of regular family";
			this.checkCloneCreateSuperFamily.Click += new System.EventHandler(this.checkCloneCreateSuperFamily_Click);
			// 
			// UserControlEnterpriseFamily
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.checkShowFeeSchedGroups);
			this.Controls.Add(this.groupClaimSnapshot);
			this.Controls.Add(this.checkShowFeaturePatientClone);
			this.Controls.Add(this.checkShowFeatureSuperfamilies);
			this.Controls.Add(this.checkClaimSnapshotEnabled);
			this.Controls.Add(this.checkCloneCreateSuperFamily);
			this.Name = "UserControlEnterpriseFamily";
			this.Size = new System.Drawing.Size(494, 660);
			this.groupClaimSnapshot.ResumeLayout(false);
			this.groupClaimSnapshot.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private UI.CheckBox checkShowFeeSchedGroups;
		private UI.GroupBox groupClaimSnapshot;
		private System.Windows.Forms.TextBox textClaimSnapshotRunTime;
		private UI.ComboBox comboClaimSnapshotTriggerType;
		private System.Windows.Forms.Label labelClaimSnapshotTrigger;
		private System.Windows.Forms.Label labelClaimSnapshotRunTime;
		private UI.CheckBox checkShowFeaturePatientClone;
		private UI.CheckBox checkShowFeatureSuperfamilies;
		private UI.CheckBox checkClaimSnapshotEnabled;
		private UI.CheckBox checkCloneCreateSuperFamily;
	}
}
