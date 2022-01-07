namespace OpenDental{
	partial class FormHcpcs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormHcpcs));
			this.textCode = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butSearch = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textCode
			// 
			this.textCode.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textCode.Location = new System.Drawing.Point(246, 10);
			this.textCode.Name = "textCode";
			this.textCode.Size = new System.Drawing.Size(399, 20);
			this.textCode.TabIndex = 17;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(73, 13);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(172, 16);
			this.label1.TabIndex = 18;
			this.label1.Text = "Code(s) or Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(20, 38);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(851, 641);
			this.gridMain.TabIndex = 20;
			this.gridMain.Title = "HCPCS Codes";
			this.gridMain.TranslationName = "FormHcpcsCodes";
			this.gridMain.WrapText = false;
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butSearch
			// 
			this.butSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butSearch.Location = new System.Drawing.Point(651, 7);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(75, 24);
			this.butSearch.TabIndex = 19;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
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
			// FormHcpcs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(961, 691);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butSearch);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCode);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormHcpcs";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "HCPCS";
			this.Load += new System.EventHandler(this.FormHcpcs_Load);
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
	}
}