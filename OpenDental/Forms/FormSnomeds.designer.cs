namespace OpenDental{
	partial class FormSnomeds {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSnomeds));
			this.butOK = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.textCode = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butSearch = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butMapICD9 = new OpenDental.UI.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.imageListInfoButton = new System.Windows.Forms.ImageList(this.components);
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(877, 625);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(877, 655);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textCode
			// 
			this.textCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCode.Location = new System.Drawing.Point(180, 10);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(412, 20);
			this.textCode.TabIndex = 17;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(5, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(172, 16);
			this.label1.TabIndex = 18;
			this.label1.Text = "Code(s) or Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butSearch
			// 
			this.butSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearch.Location = new System.Drawing.Point(598, 8);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 24);
			this.butSearch.TabIndex = 19;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(20, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(830, 641);
			this.gridMain.TabIndex = 20;
			this.gridMain.Title = "SNOMED CT Codes";
			this.gridMain.TranslationName = "FormSnomedctCodes";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			this.gridMain.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellClick);
			// 
			// butMapICD9
			// 
			this.butMapICD9.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.butMapICD9.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butMapICD9.Location = new System.Drawing.Point(6, 19);
			this.butMapICD9.Name = "butMapICD9";
			this.butMapICD9.Size = new System.Drawing.Size(103, 24);
			this.butMapICD9.TabIndex = 24;
			this.butMapICD9.Text = "Map to ICD9";
			this.butMapICD9.Click += new System.EventHandler(this.butMapToSnomed_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.butMapICD9);
			this.groupBox1.Location = new System.Drawing.Point(856, 33);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(115, 50);
			this.groupBox1.TabIndex = 25;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "One-Time Tools";
			this.groupBox1.Visible = false;
			// 
			// imageListInfoButton
			// 
			this.imageListInfoButton.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListInfoButton.ImageStream")));
			this.imageListInfoButton.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListInfoButton.Images.SetKeyName(0, "iButton_16px.png");
			// 
			// FormSnomeds
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(974, 691);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSnomeds";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "SNOMED CT";
			this.Load += new System.EventHandler(this.FormSnomeds_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textCode;
		private System.Windows.Forms.Label label1;
		private UI.Button butSearch;
		private UI.GridOD gridMain;
		private UI.Button butMapICD9;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.ImageList imageListInfoButton;
	}
}