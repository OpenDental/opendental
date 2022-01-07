namespace OpenDental {
	partial class FormUpdate {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormUpdate));
			this.labelVersion = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textUpdateCode = new System.Windows.Forms.TextBox();
			this.textWebsitePath = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textResult = new System.Windows.Forms.TextBox();
			this.textResult2 = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.panelClassic = new System.Windows.Forms.Panel();
			this.butCheck = new OpenDental.UI.Button();
			this.butDownload = new OpenDental.UI.Button();
			this.textConnectionMessage = new System.Windows.Forms.TextBox();
			this.groupBuild = new System.Windows.Forms.GroupBox();
			this.butDownloadMsiBuild = new OpenDental.UI.Button();
			this.textBuild = new System.Windows.Forms.TextBox();
			this.butInstallBuild = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBeta = new System.Windows.Forms.GroupBox();
			this.checkAcknowledgeBeta = new System.Windows.Forms.CheckBox();
			this.butDownloadMsiBeta = new OpenDental.UI.Button();
			this.textBeta = new System.Windows.Forms.TextBox();
			this.butInstallBeta = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.groupStable = new System.Windows.Forms.GroupBox();
			this.butDownloadMsiStable = new OpenDental.UI.Button();
			this.textStable = new System.Windows.Forms.TextBox();
			this.butInstallStable = new OpenDental.UI.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.butCheck2 = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.butShowPrev = new OpenDental.UI.Button();
			this.groupAlpha = new System.Windows.Forms.GroupBox();
			this.butDownloadMsiAlpha = new OpenDental.UI.Button();
			this.textAlpha = new System.Windows.Forms.TextBox();
			this.butInstallAlpha = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.panelClassic.SuspendLayout();
			this.groupBuild.SuspendLayout();
			this.groupBeta.SuspendLayout();
			this.groupStable.SuspendLayout();
			this.groupAlpha.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelVersion
			// 
			this.labelVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelVersion.Location = new System.Drawing.Point(74, 35);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(280, 20);
			this.labelVersion.TabIndex = 10;
			this.labelVersion.Text = "Using Version ";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(0, 0);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 23);
			this.label1.TabIndex = 0;
			// 
			// textUpdateCode
			// 
			this.textUpdateCode.Location = new System.Drawing.Point(129, 100);
			this.textUpdateCode.Name = "textUpdateCode";
			this.textUpdateCode.Size = new System.Drawing.Size(113, 20);
			this.textUpdateCode.TabIndex = 19;
			// 
			// textWebsitePath
			// 
			this.textWebsitePath.Location = new System.Drawing.Point(129, 77);
			this.textWebsitePath.Name = "textWebsitePath";
			this.textWebsitePath.Size = new System.Drawing.Size(388, 20);
			this.textWebsitePath.TabIndex = 24;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(24, 78);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 19);
			this.label3.TabIndex = 26;
			this.label3.Text = "Website Path";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textResult
			// 
			this.textResult.AcceptsReturn = true;
			this.textResult.BackColor = System.Drawing.SystemColors.Window;
			this.textResult.Location = new System.Drawing.Point(129, 156);
			this.textResult.Name = "textResult";
			this.textResult.ReadOnly = true;
			this.textResult.Size = new System.Drawing.Size(388, 20);
			this.textResult.TabIndex = 34;
			// 
			// textResult2
			// 
			this.textResult2.AcceptsReturn = true;
			this.textResult2.BackColor = System.Drawing.SystemColors.Window;
			this.textResult2.Location = new System.Drawing.Point(129, 179);
			this.textResult2.Multiline = true;
			this.textResult2.Name = "textResult2";
			this.textResult2.ReadOnly = true;
			this.textResult2.Size = new System.Drawing.Size(388, 66);
			this.textResult2.TabIndex = 35;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 100);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(120, 19);
			this.label4.TabIndex = 34;
			this.label4.Text = "Update Code";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(10, 8);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(555, 58);
			this.label6.TabIndex = 40;
			this.label6.Text = resources.GetString("label6.Text");
			// 
			// panelClassic
			// 
			this.panelClassic.Controls.Add(this.textWebsitePath);
			this.panelClassic.Controls.Add(this.textUpdateCode);
			this.panelClassic.Controls.Add(this.butCheck);
			this.panelClassic.Controls.Add(this.label3);
			this.panelClassic.Controls.Add(this.textResult);
			this.panelClassic.Controls.Add(this.label4);
			this.panelClassic.Controls.Add(this.label6);
			this.panelClassic.Controls.Add(this.textResult2);
			this.panelClassic.Controls.Add(this.butDownload);
			this.panelClassic.Location = new System.Drawing.Point(618, 35);
			this.panelClassic.Name = "panelClassic";
			this.panelClassic.Size = new System.Drawing.Size(568, 494);
			this.panelClassic.TabIndex = 48;
			this.panelClassic.Visible = false;
			// 
			// butCheck
			// 
			this.butCheck.Location = new System.Drawing.Point(129, 125);
			this.butCheck.Name = "butCheck";
			this.butCheck.Size = new System.Drawing.Size(117, 25);
			this.butCheck.TabIndex = 21;
			this.butCheck.Text = "Check for Updates";
			this.butCheck.Click += new System.EventHandler(this.butCheck_Click);
			// 
			// butDownload
			// 
			this.butDownload.Location = new System.Drawing.Point(129, 251);
			this.butDownload.Name = "butDownload";
			this.butDownload.Size = new System.Drawing.Size(83, 25);
			this.butDownload.TabIndex = 20;
			this.butDownload.Text = "Download";
			this.butDownload.Click += new System.EventHandler(this.butDownload_Click);
			// 
			// textConnectionMessage
			// 
			this.textConnectionMessage.AcceptsReturn = true;
			this.textConnectionMessage.BackColor = System.Drawing.SystemColors.Window;
			this.textConnectionMessage.Location = new System.Drawing.Point(77, 88);
			this.textConnectionMessage.Multiline = true;
			this.textConnectionMessage.Name = "textConnectionMessage";
			this.textConnectionMessage.ReadOnly = true;
			this.textConnectionMessage.Size = new System.Drawing.Size(477, 66);
			this.textConnectionMessage.TabIndex = 50;
			// 
			// groupBuild
			// 
			this.groupBuild.Controls.Add(this.butDownloadMsiBuild);
			this.groupBuild.Controls.Add(this.textBuild);
			this.groupBuild.Controls.Add(this.butInstallBuild);
			this.groupBuild.Controls.Add(this.label2);
			this.groupBuild.Location = new System.Drawing.Point(77, 162);
			this.groupBuild.Name = "groupBuild";
			this.groupBuild.Size = new System.Drawing.Size(477, 93);
			this.groupBuild.TabIndex = 51;
			this.groupBuild.TabStop = false;
			this.groupBuild.Text = "A new build is available for the current version";
			this.groupBuild.Visible = false;
			// 
			// butDownloadMsiBuild
			// 
			this.butDownloadMsiBuild.Location = new System.Drawing.Point(309, 60);
			this.butDownloadMsiBuild.Name = "butDownloadMsiBuild";
			this.butDownloadMsiBuild.Size = new System.Drawing.Size(83, 24);
			this.butDownloadMsiBuild.TabIndex = 52;
			this.butDownloadMsiBuild.Text = "Download msi";
			this.butDownloadMsiBuild.Click += new System.EventHandler(this.butDownMsiBuild_Click);
			// 
			// textBuild
			// 
			this.textBuild.AcceptsReturn = true;
			this.textBuild.BackColor = System.Drawing.SystemColors.Window;
			this.textBuild.Location = new System.Drawing.Point(9, 34);
			this.textBuild.Name = "textBuild";
			this.textBuild.ReadOnly = true;
			this.textBuild.Size = new System.Drawing.Size(462, 20);
			this.textBuild.TabIndex = 51;
			// 
			// butInstallBuild
			// 
			this.butInstallBuild.Location = new System.Drawing.Point(398, 60);
			this.butInstallBuild.Name = "butInstallBuild";
			this.butInstallBuild.Size = new System.Drawing.Size(73, 24);
			this.butInstallBuild.TabIndex = 28;
			this.butInstallBuild.Text = "Install";
			this.butInstallBuild.Click += new System.EventHandler(this.butInstallBuild_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 16);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(465, 15);
			this.label2.TabIndex = 27;
			this.label2.Text = "These are typically bug fixes.  It is strongly recommended to install any availab" +
    "le fixes.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// groupBeta
			// 
			this.groupBeta.Controls.Add(this.checkAcknowledgeBeta);
			this.groupBeta.Controls.Add(this.butDownloadMsiBeta);
			this.groupBeta.Controls.Add(this.textBeta);
			this.groupBeta.Controls.Add(this.butInstallBeta);
			this.groupBeta.Controls.Add(this.label5);
			this.groupBeta.Location = new System.Drawing.Point(77, 365);
			this.groupBeta.Name = "groupBeta";
			this.groupBeta.Size = new System.Drawing.Size(477, 104);
			this.groupBeta.TabIndex = 52;
			this.groupBeta.TabStop = false;
			this.groupBeta.Text = "A new beta version is available";
			this.groupBeta.Visible = false;
			// 
			// checkAcknowledgeBeta
			// 
			this.checkAcknowledgeBeta.Location = new System.Drawing.Point(6, 69);
			this.checkAcknowledgeBeta.Name = "checkAcknowledgeBeta";
			this.checkAcknowledgeBeta.Size = new System.Drawing.Size(238, 32);
			this.checkAcknowledgeBeta.TabIndex = 54;
			this.checkAcknowledgeBeta.Text = "I acknowledge that I am updating to the beta version at my own risk.";
			this.checkAcknowledgeBeta.UseVisualStyleBackColor = true;
			this.checkAcknowledgeBeta.CheckedChanged += new System.EventHandler(this.checkAcknowledgeBeta_CheckedChanged);
			// 
			// butDownloadMsiBeta
			// 
			this.butDownloadMsiBeta.Location = new System.Drawing.Point(309, 73);
			this.butDownloadMsiBeta.Name = "butDownloadMsiBeta";
			this.butDownloadMsiBeta.Size = new System.Drawing.Size(83, 24);
			this.butDownloadMsiBeta.TabIndex = 53;
			this.butDownloadMsiBeta.Text = "Download msi";
			this.butDownloadMsiBeta.Click += new System.EventHandler(this.butDownloadMsiBeta_Click);
			// 
			// textBeta
			// 
			this.textBeta.AcceptsReturn = true;
			this.textBeta.BackColor = System.Drawing.SystemColors.Window;
			this.textBeta.Location = new System.Drawing.Point(6, 47);
			this.textBeta.Name = "textBeta";
			this.textBeta.ReadOnly = true;
			this.textBeta.Size = new System.Drawing.Size(465, 20);
			this.textBeta.TabIndex = 51;
			// 
			// butInstallBeta
			// 
			this.butInstallBeta.Location = new System.Drawing.Point(398, 73);
			this.butInstallBeta.Name = "butInstallBeta";
			this.butInstallBeta.Size = new System.Drawing.Size(73, 24);
			this.butInstallBeta.TabIndex = 28;
			this.butInstallBeta.Text = "Install";
			this.butInstallBeta.Click += new System.EventHandler(this.butInstallBeta_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(465, 28);
			this.label5.TabIndex = 27;
			this.label5.Text = "This beta version will be very functional, but will have some bugs.  Use a beta v" +
    "ersion only if you demand the latest features.  Be sure to update regularly.";
			// 
			// groupStable
			// 
			this.groupStable.Controls.Add(this.butDownloadMsiStable);
			this.groupStable.Controls.Add(this.textStable);
			this.groupStable.Controls.Add(this.butInstallStable);
			this.groupStable.Controls.Add(this.label11);
			this.groupStable.Location = new System.Drawing.Point(77, 264);
			this.groupStable.Name = "groupStable";
			this.groupStable.Size = new System.Drawing.Size(477, 91);
			this.groupStable.TabIndex = 53;
			this.groupStable.TabStop = false;
			this.groupStable.Text = "A new stable version is available";
			this.groupStable.Visible = false;
			// 
			// butDownloadMsiStable
			// 
			this.butDownloadMsiStable.Location = new System.Drawing.Point(309, 60);
			this.butDownloadMsiStable.Name = "butDownloadMsiStable";
			this.butDownloadMsiStable.Size = new System.Drawing.Size(83, 24);
			this.butDownloadMsiStable.TabIndex = 53;
			this.butDownloadMsiStable.Text = "Download msi";
			this.butDownloadMsiStable.Click += new System.EventHandler(this.butDownloadMsiStable_Click);
			// 
			// textStable
			// 
			this.textStable.AcceptsReturn = true;
			this.textStable.BackColor = System.Drawing.SystemColors.Window;
			this.textStable.Location = new System.Drawing.Point(6, 34);
			this.textStable.Name = "textStable";
			this.textStable.ReadOnly = true;
			this.textStable.Size = new System.Drawing.Size(465, 20);
			this.textStable.TabIndex = 51;
			// 
			// butInstallStable
			// 
			this.butInstallStable.Location = new System.Drawing.Point(398, 60);
			this.butInstallStable.Name = "butInstallStable";
			this.butInstallStable.Size = new System.Drawing.Size(73, 24);
			this.butInstallStable.TabIndex = 28;
			this.butInstallStable.Text = "Install";
			this.butInstallStable.Click += new System.EventHandler(this.butInstallStable_Click);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(465, 15);
			this.label11.TabIndex = 27;
			this.label11.Text = "Will have nearly zero bugs.  Will provide many useful enhanced features.";
			this.label11.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butCheck2
			// 
			this.butCheck2.Location = new System.Drawing.Point(77, 57);
			this.butCheck2.Name = "butCheck2";
			this.butCheck2.Size = new System.Drawing.Size(117, 25);
			this.butCheck2.TabIndex = 54;
			this.butCheck2.Text = "Check for Updates";
			this.butCheck2.Visible = false;
			this.butCheck2.Click += new System.EventHandler(this.butCheckForUpdates_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(560, 546);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 25);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butShowPrev
			// 
			this.butShowPrev.Location = new System.Drawing.Point(422, 57);
			this.butShowPrev.Name = "butShowPrev";
			this.butShowPrev.Size = new System.Drawing.Size(132, 25);
			this.butShowPrev.TabIndex = 55;
			this.butShowPrev.Text = "Show Previous Versions";
			this.butShowPrev.Click += new System.EventHandler(this.butShowPrev_Click);
			// 
			// groupAlpha
			// 
			this.groupAlpha.Controls.Add(this.butDownloadMsiAlpha);
			this.groupAlpha.Controls.Add(this.textAlpha);
			this.groupAlpha.Controls.Add(this.butInstallAlpha);
			this.groupAlpha.Controls.Add(this.label7);
			this.groupAlpha.Location = new System.Drawing.Point(77, 479);
			this.groupAlpha.Name = "groupAlpha";
			this.groupAlpha.Size = new System.Drawing.Size(477, 93);
			this.groupAlpha.TabIndex = 56;
			this.groupAlpha.TabStop = false;
			this.groupAlpha.Text = "A new alpha version is available";
			this.groupAlpha.Visible = false;
			// 
			// butDownloadMsiAlpha
			// 
			this.butDownloadMsiAlpha.Location = new System.Drawing.Point(309, 60);
			this.butDownloadMsiAlpha.Name = "butDownloadMsiAlpha";
			this.butDownloadMsiAlpha.Size = new System.Drawing.Size(83, 24);
			this.butDownloadMsiAlpha.TabIndex = 52;
			this.butDownloadMsiAlpha.Text = "Download msi";
			this.butDownloadMsiAlpha.Click += new System.EventHandler(this.butDownloadMsiAlpha_Click);
			// 
			// textAlpha
			// 
			this.textAlpha.AcceptsReturn = true;
			this.textAlpha.BackColor = System.Drawing.SystemColors.Window;
			this.textAlpha.Location = new System.Drawing.Point(9, 34);
			this.textAlpha.Name = "textAlpha";
			this.textAlpha.ReadOnly = true;
			this.textAlpha.Size = new System.Drawing.Size(462, 20);
			this.textAlpha.TabIndex = 51;
			// 
			// butInstallAlpha
			// 
			this.butInstallAlpha.Location = new System.Drawing.Point(398, 60);
			this.butInstallAlpha.Name = "butInstallAlpha";
			this.butInstallAlpha.Size = new System.Drawing.Size(73, 24);
			this.butInstallAlpha.TabIndex = 28;
			this.butInstallAlpha.Text = "Install";
			this.butInstallAlpha.Click += new System.EventHandler(this.butInstallAlpha_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 16);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(465, 15);
			this.label7.TabIndex = 27;
			this.label7.Text = "There are bugs galore.  The upgrade might not even succeed. ";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(647, 24);
			this.menuMain.TabIndex = 57;
			// 
			// FormUpdate
			// 
			this.ClientSize = new System.Drawing.Size(647, 586);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.groupAlpha);
			this.Controls.Add(this.butShowPrev);
			this.Controls.Add(this.panelClassic);
			this.Controls.Add(this.butCheck2);
			this.Controls.Add(this.groupStable);
			this.Controls.Add(this.groupBeta);
			this.Controls.Add(this.groupBuild);
			this.Controls.Add(this.textConnectionMessage);
			this.Controls.Add(this.labelVersion);
			this.Controls.Add(this.menuMain);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormUpdate";
			this.Text = "Update";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormUpdate_FormClosing);
			this.Load += new System.EventHandler(this.FormUpdate_Load);
			this.panelClassic.ResumeLayout(false);
			this.panelClassic.PerformLayout();
			this.groupBuild.ResumeLayout(false);
			this.groupBuild.PerformLayout();
			this.groupBeta.ResumeLayout(false);
			this.groupBeta.PerformLayout();
			this.groupStable.ResumeLayout(false);
			this.groupStable.PerformLayout();
			this.groupAlpha.ResumeLayout(false);
			this.groupAlpha.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.Label labelVersion;
		private OpenDental.UI.Button butDownload;
		private OpenDental.UI.Button butCheck;
		private System.Windows.Forms.TextBox textUpdateCode;
		private System.Windows.Forms.TextBox textWebsitePath;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textResult;
		private System.Windows.Forms.TextBox textResult2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Panel panelClassic;
		private System.Windows.Forms.TextBox textConnectionMessage;
		private System.Windows.Forms.GroupBox groupBuild;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textBuild;
		private OpenDental.UI.Button butInstallBuild;
		private System.Windows.Forms.GroupBox groupBeta;
		private System.Windows.Forms.TextBox textBeta;
		private OpenDental.UI.Button butInstallBeta;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupStable;
		private System.Windows.Forms.TextBox textStable;
		private OpenDental.UI.Button butInstallStable;
		private System.Windows.Forms.Label label11;
		private OpenDental.UI.Button butCheck2;//OD1
		private OpenDental.UI.Button butDownloadMsiBuild;
		private OpenDental.UI.Button butDownloadMsiBeta;
		private OpenDental.UI.Button butDownloadMsiStable;
		private OpenDental.UI.Button butShowPrev;
		private System.Windows.Forms.GroupBox groupAlpha;
		private UI.Button butDownloadMsiAlpha;
		private System.Windows.Forms.TextBox textAlpha;
		private UI.Button butInstallAlpha;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkAcknowledgeBeta;
		private UI.MenuOD menuMain;
	}
}
