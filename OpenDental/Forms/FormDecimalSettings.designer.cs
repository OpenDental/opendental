namespace OpenDental{
	partial class FormDecimalSettings {
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

		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDecimalSettings));
			this.textDecimal = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.checkNoShow = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textDecimal
			// 
			this.textDecimal.Location = new System.Drawing.Point(225, 77);
			this.textDecimal.Name = "textDecimal";
			this.textDecimal.ReadOnly = true;
			this.textDecimal.Size = new System.Drawing.Size(56, 20);
			this.textDecimal.TabIndex = 119;
			this.textDecimal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(53, 78);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(171, 17);
			this.label2.TabIndex = 120;
			this.label2.Text = "Number of digits after decimal";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNoShow
			// 
			this.checkNoShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoShow.Location = new System.Drawing.Point(64, 114);
			this.checkNoShow.Name = "checkNoShow";
			this.checkNoShow.Size = new System.Drawing.Size(367, 18);
			this.checkNoShow.TabIndex = 118;
			this.checkNoShow.Text = "Do not show this window on startup (this computer only)";
			this.checkNoShow.UseVisualStyleBackColor = true;
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(22, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(409, 62);
			this.label1.TabIndex = 121;
			this.label1.Text = resources.GetString("label1.Text");
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(284, 138);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 123;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(365, 138);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 122;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormDecimalSettings
			// 
			this.ClientSize = new System.Drawing.Size(452, 174);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDecimal);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkNoShow);
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormDecimalSettings";
			this.Text = "Currency Decimal Settings";
			this.Load += new System.EventHandler(this.FormDecimalSettings_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private System.Windows.Forms.TextBox textDecimal;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.CheckBox checkNoShow;
		private System.Windows.Forms.Label label1;
		private UI.Button butOK;
		private UI.Button butCancel;
	}
}
