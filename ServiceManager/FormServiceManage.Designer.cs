namespace ServiceManager {
	partial class FormServiceManage {
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
			this.butRefresh = new System.Windows.Forms.Button();
			this.butStop = new System.Windows.Forms.Button();
			this.butStart = new System.Windows.Forms.Button();
			this.butUninstall = new System.Windows.Forms.Button();
			this.butInstall = new System.Windows.Forms.Button();
			this.textStatus = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textPathToExe = new System.Windows.Forms.TextBox();
			this.butBrowse = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(377, 80);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 23);
			this.butRefresh.TabIndex = 4;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butStop
			// 
			this.butStop.Location = new System.Drawing.Point(359, 113);
			this.butStop.Name = "butStop";
			this.butStop.Size = new System.Drawing.Size(75, 23);
			this.butStop.TabIndex = 3;
			this.butStop.Text = "Stop";
			this.butStop.UseVisualStyleBackColor = true;
			this.butStop.Click += new System.EventHandler(this.butStop_Click);
			// 
			// butStart
			// 
			this.butStart.Location = new System.Drawing.Point(278, 113);
			this.butStart.Name = "butStart";
			this.butStart.Size = new System.Drawing.Size(75, 23);
			this.butStart.TabIndex = 2;
			this.butStart.Text = "Start";
			this.butStart.UseVisualStyleBackColor = true;
			this.butStart.Click += new System.EventHandler(this.butStart_Click);
			// 
			// butUninstall
			// 
			this.butUninstall.Location = new System.Drawing.Point(197, 113);
			this.butUninstall.Name = "butUninstall";
			this.butUninstall.Size = new System.Drawing.Size(75, 23);
			this.butUninstall.TabIndex = 1;
			this.butUninstall.Text = "Uninstall";
			this.butUninstall.UseVisualStyleBackColor = true;
			this.butUninstall.Click += new System.EventHandler(this.butUninstall_Click);
			// 
			// butInstall
			// 
			this.butInstall.Location = new System.Drawing.Point(116, 113);
			this.butInstall.Name = "butInstall";
			this.butInstall.Size = new System.Drawing.Size(75, 23);
			this.butInstall.TabIndex = 0;
			this.butInstall.Text = "Install";
			this.butInstall.UseVisualStyleBackColor = true;
			this.butInstall.Click += new System.EventHandler(this.butInstall_Click);
			// 
			// textStatus
			// 
			this.textStatus.Location = new System.Drawing.Point(156, 82);
			this.textStatus.Name = "textStatus";
			this.textStatus.ReadOnly = true;
			this.textStatus.Size = new System.Drawing.Size(217, 20);
			this.textStatus.TabIndex = 3;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(106, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(48, 18);
			this.label2.TabIndex = 2;
			this.label2.Text = "Status";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(51, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(103, 18);
			this.label1.TabIndex = 5;
			this.label1.Text = "Service Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(156, 30);
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(217, 20);
			this.textName.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 57);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(148, 18);
			this.label3.TabIndex = 7;
			this.label3.Text = "Path to service";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPathToExe
			// 
			this.textPathToExe.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textPathToExe.Location = new System.Drawing.Point(156, 56);
			this.textPathToExe.Name = "textPathToExe";
			this.textPathToExe.Size = new System.Drawing.Size(470, 20);
			this.textPathToExe.TabIndex = 8;
			// 
			// butBrowse
			// 
			this.butBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butBrowse.Location = new System.Drawing.Point(630, 55);
			this.butBrowse.Name = "butBrowse";
			this.butBrowse.Size = new System.Drawing.Size(75, 23);
			this.butBrowse.TabIndex = 9;
			this.butBrowse.Text = "Browse";
			this.butBrowse.UseVisualStyleBackColor = true;
			this.butBrowse.Click += new System.EventHandler(this.butBrowse_Click);
			// 
			// FormServiceManage
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(719, 166);
			this.Controls.Add(this.butBrowse);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textPathToExe);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.butStop);
			this.Controls.Add(this.butStart);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butUninstall);
			this.Controls.Add(this.textStatus);
			this.Controls.Add(this.butInstall);
			this.Name = "FormServiceManage";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Manage Service";
			this.Load += new System.EventHandler(this.FormServiceManager_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butRefresh;
		private System.Windows.Forms.Button butStop;
		private System.Windows.Forms.Button butStart;
		private System.Windows.Forms.Button butUninstall;
		private System.Windows.Forms.Button butInstall;
		private System.Windows.Forms.TextBox textStatus;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textPathToExe;
		private System.Windows.Forms.Button butBrowse;
	}
}