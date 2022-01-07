namespace OpenDental{
	partial class FormLanguageAndRegion {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormLanguageAndRegion));
			this.textBoxDescript = new System.Windows.Forms.TextBox();
			this.textLARDB = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboLanguageAndRegion = new System.Windows.Forms.ComboBox();
			this.labelNewLAR = new System.Windows.Forms.Label();
			this.checkNoShow = new System.Windows.Forms.CheckBox();
			this.textLARLocal = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textBoxDescript
			// 
			this.textBoxDescript.BackColor = System.Drawing.SystemColors.Control;
			this.textBoxDescript.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textBoxDescript.Location = new System.Drawing.Point(36, 12);
			this.textBoxDescript.Multiline = true;
			this.textBoxDescript.Name = "textBoxDescript";
			this.textBoxDescript.Size = new System.Drawing.Size(402, 47);
			this.textBoxDescript.TabIndex = 5;
			this.textBoxDescript.Text = resources.GetString("textBoxDescript.Text");
			// 
			// textLARDB
			// 
			this.textLARDB.Location = new System.Drawing.Point(189, 90);
			this.textLARDB.Name = "textLARDB";
			this.textLARDB.ReadOnly = true;
			this.textLARDB.Size = new System.Drawing.Size(212, 20);
			this.textLARDB.TabIndex = 16;
			// 
			// label1
			// 
			this.label1.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label1.Location = new System.Drawing.Point(12, 92);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(171, 17);
			this.label1.TabIndex = 17;
			this.label1.Text = "Current Database Setting";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLanguageAndRegion
			// 
			this.comboLanguageAndRegion.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboLanguageAndRegion.Location = new System.Drawing.Point(189, 116);
			this.comboLanguageAndRegion.Name = "comboLanguageAndRegion";
			this.comboLanguageAndRegion.Size = new System.Drawing.Size(212, 21);
			this.comboLanguageAndRegion.TabIndex = 105;
			// 
			// labelNewLAR
			// 
			this.labelNewLAR.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.labelNewLAR.Location = new System.Drawing.Point(12, 117);
			this.labelNewLAR.Name = "labelNewLAR";
			this.labelNewLAR.Size = new System.Drawing.Size(171, 16);
			this.labelNewLAR.TabIndex = 106;
			this.labelNewLAR.Text = "New Database Setting";
			this.labelNewLAR.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNoShow
			// 
			this.checkNoShow.AutoSize = true;
			this.checkNoShow.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNoShow.Location = new System.Drawing.Point(85, 151);
			this.checkNoShow.Name = "checkNoShow";
			this.checkNoShow.Size = new System.Drawing.Size(294, 18);
			this.checkNoShow.TabIndex = 108;
			this.checkNoShow.Text = "Do not show this window on startup (this computer only)";
			this.checkNoShow.UseVisualStyleBackColor = true;
			// 
			// textLARLocal
			// 
			this.textLARLocal.Location = new System.Drawing.Point(189, 64);
			this.textLARLocal.Name = "textLARLocal";
			this.textLARLocal.ReadOnly = true;
			this.textLARLocal.Size = new System.Drawing.Size(212, 20);
			this.textLARLocal.TabIndex = 109;
			// 
			// label2
			// 
			this.label2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.label2.Location = new System.Drawing.Point(12, 67);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(171, 17);
			this.label2.TabIndex = 110;
			this.label2.Text = "Current Computer Setting";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(304, 177);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(385, 177);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormLanguageAndRegion
			// 
			this.ClientSize = new System.Drawing.Size(472, 213);
			this.Controls.Add(this.textLARLocal);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.checkNoShow);
			this.Controls.Add(this.comboLanguageAndRegion);
			this.Controls.Add(this.labelNewLAR);
			this.Controls.Add(this.textLARDB);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textBoxDescript);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormLanguageAndRegion";
			this.Text = "Language and Region Settings";
			this.Load += new System.EventHandler(this.FormLanguageAndRegion_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private UI.Button butCancel;
		private System.Windows.Forms.TextBox textBoxDescript;
		private System.Windows.Forms.TextBox textLARDB;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboLanguageAndRegion;
		private System.Windows.Forms.Label labelNewLAR;
		private System.Windows.Forms.CheckBox checkNoShow;
		private System.Windows.Forms.TextBox textLARLocal;
		private System.Windows.Forms.Label label2;
	}
}