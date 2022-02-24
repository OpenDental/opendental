namespace OpenDental{
	partial class FormSheetOutputFormat {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetOutputFormat));
			this.label1 = new System.Windows.Forms.Label();
			this.checkEmailPat = new System.Windows.Forms.CheckBox();
			this.textEmailPat = new System.Windows.Forms.TextBox();
			this.textEmail2 = new System.Windows.Forms.TextBox();
			this.checkEmail2 = new System.Windows.Forms.CheckBox();
			this.textPaperCopies = new OpenDental.ValidNum();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(78, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(134, 16);
			this.label1.TabIndex = 83;
			this.label1.Text = "Paper copies";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// checkEmailPat
			// 
			this.checkEmailPat.Location = new System.Drawing.Point(62, 65);
			this.checkEmailPat.Name = "checkEmailPat";
			this.checkEmailPat.Size = new System.Drawing.Size(314, 20);
			this.checkEmailPat.TabIndex = 85;
			this.checkEmailPat.Text = "E-mail to patient (not recommended for sensitive information):";
			this.checkEmailPat.UseVisualStyleBackColor = true;
			this.checkEmailPat.Click += new System.EventHandler(this.checkEmailPat_Click);
			// 
			// textEmailPat
			// 
			this.textEmailPat.Location = new System.Drawing.Point(81, 84);
			this.textEmailPat.Name = "textEmailPat";
			this.textEmailPat.Size = new System.Drawing.Size(295, 20);
			this.textEmailPat.TabIndex = 86;
			// 
			// textEmail2
			// 
			this.textEmail2.Location = new System.Drawing.Point(81, 140);
			this.textEmail2.Name = "textEmail2";
			this.textEmail2.Size = new System.Drawing.Size(295, 20);
			this.textEmail2.TabIndex = 88;
			// 
			// checkEmail2
			// 
			this.checkEmail2.Location = new System.Drawing.Point(62, 121);
			this.checkEmail2.Name = "checkEmail2";
			this.checkEmail2.Size = new System.Drawing.Size(314, 20);
			this.checkEmail2.TabIndex = 87;
			this.checkEmail2.Text = "E-mail to referral (not recommended for sensitive information):";
			this.checkEmail2.UseVisualStyleBackColor = true;
			this.checkEmail2.Click += new System.EventHandler(this.checkEmail2_Click);
			// 
			// textPaperCopies
			// 
			this.textPaperCopies.Location = new System.Drawing.Point(30, 24);
			this.textPaperCopies.MaxVal = 255;
			this.textPaperCopies.MinVal = 0;
			this.textPaperCopies.Name = "textPaperCopies";
			this.textPaperCopies.Size = new System.Drawing.Size(45, 20);
			this.textPaperCopies.TabIndex = 84;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(231, 196);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(343, 196);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormSheetOutputFormat
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(443, 243);
			this.Controls.Add(this.textEmail2);
			this.Controls.Add(this.checkEmail2);
			this.Controls.Add(this.textEmailPat);
			this.Controls.Add(this.checkEmailPat);
			this.Controls.Add(this.textPaperCopies);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetOutputFormat";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Sheet Output";
			this.Load += new System.EventHandler(this.FormSheetOutputFormat_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private ValidNum textPaperCopies;
		private System.Windows.Forms.CheckBox checkEmailPat;
		private System.Windows.Forms.TextBox textEmailPat;
		private System.Windows.Forms.TextBox textEmail2;
		private System.Windows.Forms.CheckBox checkEmail2;
	}
}