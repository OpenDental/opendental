namespace OpenDental{
	partial class FormXchargeTokenTool {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormXchargeTokenTool));
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelWarning = new System.Windows.Forms.Label();
			this.butCheck = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.labelTotal = new System.Windows.Forms.Label();
			this.labelVerified = new System.Windows.Forms.Label();
			this.labelInvalid = new System.Windows.Forms.Label();
			this.textTotal = new System.Windows.Forms.TextBox();
			this.textVerified = new System.Windows.Forms.TextBox();
			this.textInvalid = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 83);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(701, 370);
			this.gridMain.TabIndex = 68;
			this.gridMain.Title = "Invalid X-Charge Tokens";
			this.gridMain.TranslationName = "FormDisplayFields";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelWarning
			// 
			this.labelWarning.Location = new System.Drawing.Point(9, 17);
			this.labelWarning.Name = "labelWarning";
			this.labelWarning.Size = new System.Drawing.Size(704, 63);
			this.labelWarning.TabIndex = 69;
			this.labelWarning.Text = resources.GetString("labelWarning.Text");
			// 
			// butCheck
			// 
			this.butCheck.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCheck.Location = new System.Drawing.Point(12, 498);
			this.butCheck.Name = "butCheck";
			this.butCheck.Size = new System.Drawing.Size(75, 24);
			this.butCheck.TabIndex = 3;
			this.butCheck.Text = "Check";
			this.butCheck.Click += new System.EventHandler(this.butCheck_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(638, 498);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// labelTotal
			// 
			this.labelTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelTotal.Location = new System.Drawing.Point(105, 461);
			this.labelTotal.Name = "labelTotal";
			this.labelTotal.Size = new System.Drawing.Size(89, 16);
			this.labelTotal.TabIndex = 70;
			this.labelTotal.Text = "Total Cards:";
			this.labelTotal.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelVerified
			// 
			this.labelVerified.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelVerified.Location = new System.Drawing.Point(105, 485);
			this.labelVerified.Name = "labelVerified";
			this.labelVerified.Size = new System.Drawing.Size(89, 16);
			this.labelVerified.TabIndex = 71;
			this.labelVerified.Text = "Verified:";
			this.labelVerified.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelInvalid
			// 
			this.labelInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelInvalid.Location = new System.Drawing.Point(105, 510);
			this.labelInvalid.Name = "labelInvalid";
			this.labelInvalid.Size = new System.Drawing.Size(89, 16);
			this.labelInvalid.TabIndex = 72;
			this.labelInvalid.Text = "Invalid:";
			this.labelInvalid.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTotal
			// 
			this.textTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTotal.Enabled = false;
			this.textTotal.Location = new System.Drawing.Point(200, 460);
			this.textTotal.Name = "textTotal";
			this.textTotal.Size = new System.Drawing.Size(64, 20);
			this.textTotal.TabIndex = 73;
			// 
			// textVerified
			// 
			this.textVerified.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textVerified.Enabled = false;
			this.textVerified.Location = new System.Drawing.Point(200, 484);
			this.textVerified.Name = "textVerified";
			this.textVerified.Size = new System.Drawing.Size(64, 20);
			this.textVerified.TabIndex = 74;
			// 
			// textInvalid
			// 
			this.textInvalid.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textInvalid.Enabled = false;
			this.textInvalid.Location = new System.Drawing.Point(200, 509);
			this.textInvalid.Name = "textInvalid";
			this.textInvalid.Size = new System.Drawing.Size(64, 20);
			this.textInvalid.TabIndex = 75;
			// 
			// FormXchargeTokenTool
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(725, 534);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.textInvalid);
			this.Controls.Add(this.textVerified);
			this.Controls.Add(this.textTotal);
			this.Controls.Add(this.labelInvalid);
			this.Controls.Add(this.labelVerified);
			this.Controls.Add(this.labelTotal);
			this.Controls.Add(this.labelWarning);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butCheck);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormXchargeTokenTool";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "X-Charge Token Tool";
			this.Load += new System.EventHandler(this.FormXchargeTokenTool_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butCheck;
		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label labelWarning;
		private System.Windows.Forms.Label labelTotal;
		private System.Windows.Forms.Label labelVerified;
		private System.Windows.Forms.Label labelInvalid;
		private System.Windows.Forms.TextBox textTotal;
		private System.Windows.Forms.TextBox textVerified;
		private System.Windows.Forms.TextBox textInvalid;
	}
}