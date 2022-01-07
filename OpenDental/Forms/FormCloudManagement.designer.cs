namespace OpenDental{
	partial class FormCloudManagement {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCloudManagement));
			this.butClose = new OpenDental.UI.Button();
			this.butCloseSession = new OpenDental.UI.Button();
			this.labelSessions = new System.Windows.Forms.Label();
			this.labelSlash = new System.Windows.Forms.Label();
			this.labelCurSessionNum = new System.Windows.Forms.Label();
			this.labelMaxSessionNum = new System.Windows.Forms.Label();
			this.butChangeSessionLimit = new OpenDental.UI.Button();
			this.groupSessionManagement = new OpenDental.UI.GroupBoxOD();
			this.gridActiveInstances = new OpenDental.UI.GridOD();
			this.butChangePassword = new OpenDental.UI.Button();
			this.groupStorageManagement = new OpenDental.UI.GroupBoxOD();
			this.labelAtoZSizeInfo = new System.Windows.Forms.Label();
			this.labelCurAtoZSize = new System.Windows.Forms.Label();
			this.labelAtoZSize = new System.Windows.Forms.Label();
			this.groupAllowedAddresses = new OpenDental.UI.GroupBoxOD();
			this.textAddress = new System.Windows.Forms.TextBox();
			this.labelStaticIp = new System.Windows.Forms.Label();
			this.butAddCurrent = new OpenDental.UI.Button();
			this.butDeleteAddress = new OpenDental.UI.Button();
			this.butAllowAddress = new OpenDental.UI.Button();
			this.gridAllowedAddresses = new OpenDental.UI.GridOD();
			this.groupSessionManagement.SuspendLayout();
			this.groupStorageManagement.SuspendLayout();
			this.groupAllowedAddresses.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(847, 450);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 3;
			this.butClose.Text = "&Close";
			// 
			// butCloseSession
			// 
			this.butCloseSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCloseSession.Location = new System.Drawing.Point(6, 347);
			this.butCloseSession.Name = "butCloseSession";
			this.butCloseSession.Size = new System.Drawing.Size(85, 24);
			this.butCloseSession.TabIndex = 5;
			this.butCloseSession.Text = "Close Selected";
			this.butCloseSession.UseVisualStyleBackColor = true;
			this.butCloseSession.Click += new System.EventHandler(this.butCloseSession_Click);
			// 
			// labelSessions
			// 
			this.labelSessions.Location = new System.Drawing.Point(6, 22);
			this.labelSessions.Name = "labelSessions";
			this.labelSessions.Size = new System.Drawing.Size(96, 18);
			this.labelSessions.TabIndex = 6;
			this.labelSessions.Text = "Current Sessions:";
			this.labelSessions.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSlash
			// 
			this.labelSlash.Location = new System.Drawing.Point(130, 22);
			this.labelSlash.Name = "labelSlash";
			this.labelSlash.Size = new System.Drawing.Size(10, 18);
			this.labelSlash.TabIndex = 7;
			this.labelSlash.Text = "/";
			this.labelSlash.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelCurSessionNum
			// 
			this.labelCurSessionNum.Location = new System.Drawing.Point(102, 22);
			this.labelCurSessionNum.Name = "labelCurSessionNum";
			this.labelCurSessionNum.Size = new System.Drawing.Size(30, 18);
			this.labelCurSessionNum.TabIndex = 8;
			this.labelCurSessionNum.Text = "0";
			this.labelCurSessionNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxSessionNum
			// 
			this.labelMaxSessionNum.Location = new System.Drawing.Point(138, 22);
			this.labelMaxSessionNum.Name = "labelMaxSessionNum";
			this.labelMaxSessionNum.Size = new System.Drawing.Size(30, 18);
			this.labelMaxSessionNum.TabIndex = 9;
			this.labelMaxSessionNum.Text = "0";
			this.labelMaxSessionNum.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butChangeSessionLimit
			// 
			this.butChangeSessionLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeSessionLimit.Location = new System.Drawing.Point(372, 16);
			this.butChangeSessionLimit.Name = "butChangeSessionLimit";
			this.butChangeSessionLimit.Size = new System.Drawing.Size(119, 24);
			this.butChangeSessionLimit.TabIndex = 10;
			this.butChangeSessionLimit.Text = "Change Session Limit";
			this.butChangeSessionLimit.UseVisualStyleBackColor = true;
			this.butChangeSessionLimit.Click += new System.EventHandler(this.butAddSessions_Click);
			// 
			// groupSessionManagement
			// 
			this.groupSessionManagement.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupSessionManagement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupSessionManagement.Controls.Add(this.gridActiveInstances);
			this.groupSessionManagement.Controls.Add(this.butCloseSession);
			this.groupSessionManagement.Controls.Add(this.butChangeSessionLimit);
			this.groupSessionManagement.Controls.Add(this.labelSessions);
			this.groupSessionManagement.Controls.Add(this.labelMaxSessionNum);
			this.groupSessionManagement.Controls.Add(this.labelSlash);
			this.groupSessionManagement.Controls.Add(this.labelCurSessionNum);
			this.groupSessionManagement.Location = new System.Drawing.Point(12, 12);
			this.groupSessionManagement.Name = "groupSessionManagement";
			this.groupSessionManagement.Size = new System.Drawing.Size(497, 378);
			this.groupSessionManagement.TabIndex = 11;
			this.groupSessionManagement.TabStop = false;
			this.groupSessionManagement.Text = "Session Management";
			// 
			// gridActiveInstances
			// 
			this.gridActiveInstances.AllowSortingByColumn = true;
			this.gridActiveInstances.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridActiveInstances.Location = new System.Drawing.Point(6, 46);
			this.gridActiveInstances.Name = "gridActiveInstances";
			this.gridActiveInstances.Size = new System.Drawing.Size(485, 295);
			this.gridActiveInstances.TabIndex = 4;
			// 
			// butChangePassword
			// 
			this.butChangePassword.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butChangePassword.Location = new System.Drawing.Point(12, 447);
			this.butChangePassword.Name = "butChangePassword";
			this.butChangePassword.Size = new System.Drawing.Size(102, 24);
			this.butChangePassword.TabIndex = 12;
			this.butChangePassword.Text = "Change Password";
			this.butChangePassword.UseVisualStyleBackColor = true;
			this.butChangePassword.Click += new System.EventHandler(this.butChangePassword_Click);
			// 
			// groupStorageManagement
			// 
			this.groupStorageManagement.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupStorageManagement.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupStorageManagement.Controls.Add(this.labelAtoZSizeInfo);
			this.groupStorageManagement.Controls.Add(this.labelCurAtoZSize);
			this.groupStorageManagement.Controls.Add(this.labelAtoZSize);
			this.groupStorageManagement.Location = new System.Drawing.Point(12, 396);
			this.groupStorageManagement.Name = "groupStorageManagement";
			this.groupStorageManagement.Size = new System.Drawing.Size(497, 45);
			this.groupStorageManagement.TabIndex = 13;
			this.groupStorageManagement.TabStop = false;
			this.groupStorageManagement.Text = "Storage Information";
			// 
			// labelAtoZSizeInfo
			// 
			this.labelAtoZSizeInfo.Location = new System.Drawing.Point(135, 19);
			this.labelAtoZSizeInfo.Name = "labelAtoZSizeInfo";
			this.labelAtoZSizeInfo.Size = new System.Drawing.Size(340, 18);
			this.labelAtoZSizeInfo.TabIndex = 12;
			this.labelAtoZSizeInfo.Text = "Data over the max is charged at a rate of $0.32/GB per month";
			this.labelAtoZSizeInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCurAtoZSize
			// 
			this.labelCurAtoZSize.Location = new System.Drawing.Point(76, 19);
			this.labelCurAtoZSize.Name = "labelCurAtoZSize";
			this.labelCurAtoZSize.Size = new System.Drawing.Size(56, 18);
			this.labelCurAtoZSize.TabIndex = 9;
			this.labelCurAtoZSize.Text = "0 GB";
			this.labelCurAtoZSize.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelAtoZSize
			// 
			this.labelAtoZSize.Location = new System.Drawing.Point(6, 19);
			this.labelAtoZSize.Name = "labelAtoZSize";
			this.labelAtoZSize.Size = new System.Drawing.Size(70, 18);
			this.labelAtoZSize.TabIndex = 7;
			this.labelAtoZSize.Text = "AtoZ Size:";
			this.labelAtoZSize.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupAllowedAddresses
			// 
			this.groupAllowedAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAllowedAddresses.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupAllowedAddresses.Controls.Add(this.textAddress);
			this.groupAllowedAddresses.Controls.Add(this.labelStaticIp);
			this.groupAllowedAddresses.Controls.Add(this.butAddCurrent);
			this.groupAllowedAddresses.Controls.Add(this.butDeleteAddress);
			this.groupAllowedAddresses.Controls.Add(this.butAllowAddress);
			this.groupAllowedAddresses.Controls.Add(this.gridAllowedAddresses);
			this.groupAllowedAddresses.Location = new System.Drawing.Point(515, 12);
			this.groupAllowedAddresses.Name = "groupAllowedAddresses";
			this.groupAllowedAddresses.Size = new System.Drawing.Size(407, 429);
			this.groupAllowedAddresses.TabIndex = 14;
			this.groupAllowedAddresses.TabStop = false;
			this.groupAllowedAddresses.Text = "Allowed Addresses";
			// 
			// textAddress
			// 
			this.textAddress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textAddress.Location = new System.Drawing.Point(6, 20);
			this.textAddress.Name = "textAddress";
			this.textAddress.Size = new System.Drawing.Size(294, 20);
			this.textAddress.TabIndex = 110;
			// 
			// labelStaticIp
			// 
			this.labelStaticIp.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelStaticIp.Location = new System.Drawing.Point(6, 375);
			this.labelStaticIp.Name = "labelStaticIp";
			this.labelStaticIp.Size = new System.Drawing.Size(392, 51);
			this.labelStaticIp.TabIndex = 109;
			this.labelStaticIp.Text = "Allowed addresses must be static. Check with your ISP to determine if you have a " +
    "static IP address.";
			this.labelStaticIp.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAddCurrent
			// 
			this.butAddCurrent.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddCurrent.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddCurrent.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddCurrent.Location = new System.Drawing.Point(279, 347);
			this.butAddCurrent.Name = "butAddCurrent";
			this.butAddCurrent.Size = new System.Drawing.Size(100, 24);
			this.butAddCurrent.TabIndex = 108;
			this.butAddCurrent.Text = "&Add Current";
			this.butAddCurrent.Click += new System.EventHandler(this.butAddCurrent_Click);
			// 
			// butDeleteAddress
			// 
			this.butDeleteAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDeleteAddress.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteAddress.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteAddress.Location = new System.Drawing.Point(25, 347);
			this.butDeleteAddress.Name = "butDeleteAddress";
			this.butDeleteAddress.Size = new System.Drawing.Size(100, 24);
			this.butDeleteAddress.TabIndex = 107;
			this.butDeleteAddress.Text = "&Delete";
			this.butDeleteAddress.Click += new System.EventHandler(this.butDeleteAddress_Click);
			// 
			// butAllowAddress
			// 
			this.butAllowAddress.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAllowAddress.Location = new System.Drawing.Point(306, 17);
			this.butAllowAddress.Name = "butAllowAddress";
			this.butAllowAddress.Size = new System.Drawing.Size(92, 24);
			this.butAllowAddress.TabIndex = 11;
			this.butAllowAddress.Text = "Allow Address";
			this.butAllowAddress.UseVisualStyleBackColor = true;
			this.butAllowAddress.Click += new System.EventHandler(this.butAllowAddress_Click);
			// 
			// gridAllowedAddresses
			// 
			this.gridAllowedAddresses.AllowSortingByColumn = true;
			this.gridAllowedAddresses.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAllowedAddresses.Location = new System.Drawing.Point(6, 46);
			this.gridAllowedAddresses.Name = "gridAllowedAddresses";
			this.gridAllowedAddresses.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAllowedAddresses.Size = new System.Drawing.Size(392, 295);
			this.gridAllowedAddresses.TabIndex = 4;
			// 
			// FormCloudManagement
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(934, 486);
			this.Controls.Add(this.groupAllowedAddresses);
			this.Controls.Add(this.groupStorageManagement);
			this.Controls.Add(this.butChangePassword);
			this.Controls.Add(this.groupSessionManagement);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCloudManagement";
			this.Text = "Cloud Management";
			this.Load += new System.EventHandler(this.FormCloudManagement_Load);
			this.groupSessionManagement.ResumeLayout(false);
			this.groupStorageManagement.ResumeLayout(false);
			this.groupAllowedAddresses.ResumeLayout(false);
			this.groupAllowedAddresses.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridActiveInstances;
		private UI.Button butCloseSession;
		private System.Windows.Forms.Label labelSessions;
		private System.Windows.Forms.Label labelSlash;
		private System.Windows.Forms.Label labelCurSessionNum;
		private System.Windows.Forms.Label labelMaxSessionNum;
		private UI.Button butChangeSessionLimit;
		private UI.GroupBoxOD groupSessionManagement;
		private UI.Button butChangePassword;
		private UI.GroupBoxOD groupStorageManagement;
		private System.Windows.Forms.Label labelCurAtoZSize;
		private System.Windows.Forms.Label labelAtoZSize;
		private System.Windows.Forms.Label labelAtoZSizeInfo;
		private UI.GroupBoxOD groupAllowedAddresses;
		private UI.GridOD gridAllowedAddresses;
		private UI.Button butAllowAddress;
		private System.Windows.Forms.Label labelStaticIp;
		private UI.Button butAddCurrent;
		private UI.Button butDeleteAddress;
		private System.Windows.Forms.TextBox textAddress;
	}
}