namespace OpenDental{
	partial class FormEServicesMassEmail {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEServicesMassEmail));
			this.label37 = new System.Windows.Forms.Label();
			this.menuWebSchedVerifyTextTemplate = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.insertReplacementsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.undoToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.cutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.copyToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.pasteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.selectAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.butClose = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.button1 = new OpenDental.UI.Button();
			this.checkIsSecureEnabled = new System.Windows.Forms.CheckBox();
			this.butActivate = new OpenDental.UI.Button();
			this.checkIsMassEmailEnabled = new System.Windows.Forms.CheckBox();
			this.comboClinicMassEmail = new OpenDental.UI.ComboBoxClinicPicker();
			this.webBrowser1 = new System.Windows.Forms.WebBrowser();
			this.webServiceService1 = new OpenDental.com.dentalxchange.webservices.WebServiceService();
			this.button2 = new OpenDental.UI.Button();
			this.menuWebSchedVerifyTextTemplate.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label37
			// 
			this.label37.Location = new System.Drawing.Point(0, 0);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(100, 23);
			this.label37.TabIndex = 0;
			// 
			// menuWebSchedVerifyTextTemplate
			// 
			this.menuWebSchedVerifyTextTemplate.ImageScalingSize = new System.Drawing.Size(24, 24);
			this.menuWebSchedVerifyTextTemplate.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.insertReplacementsToolStripMenuItem,
            this.undoToolStripMenuItem,
            this.cutToolStripMenuItem,
            this.copyToolStripMenuItem,
            this.pasteToolStripMenuItem,
            this.selectAllToolStripMenuItem});
			this.menuWebSchedVerifyTextTemplate.Name = "menuASAPEmailBody";
			this.menuWebSchedVerifyTextTemplate.Size = new System.Drawing.Size(137, 136);
			this.menuWebSchedVerifyTextTemplate.Text = "Insert Replacements";
			// 
			// insertReplacementsToolStripMenuItem
			// 
			this.insertReplacementsToolStripMenuItem.Name = "insertReplacementsToolStripMenuItem";
			this.insertReplacementsToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.insertReplacementsToolStripMenuItem.Text = "Insert Fields";
			// 
			// undoToolStripMenuItem
			// 
			this.undoToolStripMenuItem.Name = "undoToolStripMenuItem";
			this.undoToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.undoToolStripMenuItem.Text = "Undo";
			// 
			// cutToolStripMenuItem
			// 
			this.cutToolStripMenuItem.Name = "cutToolStripMenuItem";
			this.cutToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.cutToolStripMenuItem.Text = "Cut";
			// 
			// copyToolStripMenuItem
			// 
			this.copyToolStripMenuItem.Name = "copyToolStripMenuItem";
			this.copyToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.copyToolStripMenuItem.Text = "Copy";
			// 
			// pasteToolStripMenuItem
			// 
			this.pasteToolStripMenuItem.Name = "pasteToolStripMenuItem";
			this.pasteToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.pasteToolStripMenuItem.Text = "Paste";
			// 
			// selectAllToolStripMenuItem
			// 
			this.selectAllToolStripMenuItem.Name = "selectAllToolStripMenuItem";
			this.selectAllToolStripMenuItem.Size = new System.Drawing.Size(136, 22);
			this.selectAllToolStripMenuItem.Text = "Select All";
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(1062, 723);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 500;
			this.butClose.Text = "OK";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(1143, 722);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 501;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// splitContainer1
			// 
			this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainer1.Location = new System.Drawing.Point(2, 5);
			this.splitContainer1.Name = "splitContainer1";
			this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.button2);
			this.splitContainer1.Panel1.Controls.Add(this.button1);
			this.splitContainer1.Panel1.Controls.Add(this.checkIsSecureEnabled);
			this.splitContainer1.Panel1.Controls.Add(this.butActivate);
			this.splitContainer1.Panel1.Controls.Add(this.checkIsMassEmailEnabled);
			this.splitContainer1.Panel1.Controls.Add(this.comboClinicMassEmail);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.webBrowser1);
			this.splitContainer1.Size = new System.Drawing.Size(1227, 711);
			this.splitContainer1.SplitterDistance = 75;
			this.splitContainer1.TabIndex = 502;
			// 
			// button1
			// 
			this.button1.Location = new System.Drawing.Point(379, 24);
			this.button1.Name = "button1";
			this.button1.Size = new System.Drawing.Size(83, 24);
			this.button1.TabIndex = 315;
			this.button1.Text = "Signup Secure";
			this.button1.UseVisualStyleBackColor = true;
			this.button1.Click += new System.EventHandler(this.button1_Click);
			// 
			// checkIsSecureEnabled
			// 
			this.checkIsSecureEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIsSecureEnabled.Location = new System.Drawing.Point(1032, 53);
			this.checkIsSecureEnabled.Name = "checkIsSecureEnabled";
			this.checkIsSecureEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsSecureEnabled.Size = new System.Drawing.Size(184, 17);
			this.checkIsSecureEnabled.TabIndex = 314;
			this.checkIsSecureEnabled.Text = "Enable Secure Email";
			this.checkIsSecureEnabled.UseVisualStyleBackColor = true;
			this.checkIsSecureEnabled.Click += new System.EventHandler(this.checkIsSecureEmailEnabled_Click);
			// 
			// butActivate
			// 
			this.butActivate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butActivate.Location = new System.Drawing.Point(1097, 34);
			this.butActivate.Name = "butActivate";
			this.butActivate.Size = new System.Drawing.Size(119, 24);
			this.butActivate.TabIndex = 313;
			this.butActivate.Text = "Activate Account";
			this.butActivate.Visible = false;
			this.butActivate.Click += new System.EventHandler(this.butActivate_Click);
			// 
			// checkIsMassEmailEnabled
			// 
			this.checkIsMassEmailEnabled.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkIsMassEmailEnabled.Location = new System.Drawing.Point(1032, 34);
			this.checkIsMassEmailEnabled.Name = "checkIsMassEmailEnabled";
			this.checkIsMassEmailEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsMassEmailEnabled.Size = new System.Drawing.Size(184, 17);
			this.checkIsMassEmailEnabled.TabIndex = 312;
			this.checkIsMassEmailEnabled.Text = "Enable Mass Email";
			this.checkIsMassEmailEnabled.UseVisualStyleBackColor = true;
			this.checkIsMassEmailEnabled.Click += new System.EventHandler(this.checkIsMassEmailEnabled_Click);
			// 
			// comboClinicMassEmail
			// 
			this.comboClinicMassEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboClinicMassEmail.HqDescription = "Defaults";
			this.comboClinicMassEmail.IncludeUnassigned = true;
			this.comboClinicMassEmail.Location = new System.Drawing.Point(1002, 7);
			this.comboClinicMassEmail.Name = "comboClinicMassEmail";
			this.comboClinicMassEmail.Size = new System.Drawing.Size(214, 21);
			this.comboClinicMassEmail.TabIndex = 310;
			this.comboClinicMassEmail.SelectionChangeCommitted += new System.EventHandler(this.comboClinicPromotion_SelectionChangeCommitted);
			// 
			// webBrowser1
			// 
			this.webBrowser1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.webBrowser1.Location = new System.Drawing.Point(0, 0);
			this.webBrowser1.MinimumSize = new System.Drawing.Size(20, 20);
			this.webBrowser1.Name = "webBrowser1";
			this.webBrowser1.Size = new System.Drawing.Size(1227, 632);
			this.webBrowser1.TabIndex = 0;
			// 
			// webServiceService1
			// 
			this.webServiceService1.Credentials = null;
			this.webServiceService1.Url = "https://webservices.dentalxchange.com/dws/services/dciservice.svl";
			this.webServiceService1.UseDefaultCredentials = false;
			// 
			// button2
			// 
			this.button2.Location = new System.Drawing.Point(576, 25);
			this.button2.Name = "button2";
			this.button2.Size = new System.Drawing.Size(75, 24);
			this.button2.TabIndex = 316;
			this.button2.Text = "Reset";
			this.button2.UseVisualStyleBackColor = true;
			this.button2.Click += new System.EventHandler(this.button2_Click);
			// 
			// FormEServicesMassEmail
			// 
			this.ClientSize = new System.Drawing.Size(1230, 758);
			this.Controls.Add(this.splitContainer1);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEServicesMassEmail";
			this.Text = "eServices Hosted Email";
			this.Load += new System.EventHandler(this.FormEServicesMassEmail_Load);
			this.menuWebSchedVerifyTextTemplate.ResumeLayout(false);
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
			this.splitContainer1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butClose;
		private System.Windows.Forms.Label label37;
		private System.Windows.Forms.ContextMenuStrip menuWebSchedVerifyTextTemplate;
		private System.Windows.Forms.ToolStripMenuItem insertReplacementsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem cutToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem copyToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem pasteToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem undoToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem selectAllToolStripMenuItem;
		private UI.Button butCancel;
		private System.Windows.Forms.SplitContainer splitContainer1;
		private UI.Button butActivate;
		private System.Windows.Forms.CheckBox checkIsMassEmailEnabled;
		private UI.ComboBoxClinicPicker comboClinicMassEmail;
		private System.Windows.Forms.WebBrowser webBrowser1;
		private com.dentalxchange.webservices.WebServiceService webServiceService1;
		private UI.Button button1;
		private System.Windows.Forms.CheckBox checkIsSecureEnabled;
		private UI.Button button2;
	}
}