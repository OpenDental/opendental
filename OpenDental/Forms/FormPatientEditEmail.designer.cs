namespace OpenDental{
	partial class FormPatientEditEmail {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatientEditEmail));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.labelCharsRemaining = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(272, 132);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(353, 132);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textEmail
			// 
			this.textEmail.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textEmail.Location = new System.Drawing.Point(12, 12);
			this.textEmail.MaxLength = 100;
			this.textEmail.MinimumSize = new System.Drawing.Size(0, 60);
			this.textEmail.Multiline = true;
			this.textEmail.Name = "textEmail";
			this.textEmail.Size = new System.Drawing.Size(416, 114);
			this.textEmail.TabIndex = 4;
			this.textEmail.TextChanged += new System.EventHandler(this.textEmail_TextChanged);
			// 
			// labelCharsRemaining
			// 
			this.labelCharsRemaining.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelCharsRemaining.Location = new System.Drawing.Point(9, 136);
			this.labelCharsRemaining.Margin = new System.Windows.Forms.Padding(0);
			this.labelCharsRemaining.Name = "labelCharsRemaining";
			this.labelCharsRemaining.Size = new System.Drawing.Size(260, 16);
			this.labelCharsRemaining.TabIndex = 6;
			this.labelCharsRemaining.Text = "Characters remaining: 100";
			this.labelCharsRemaining.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// FormPatientEditEmail
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(440, 168);
			this.Controls.Add(this.labelCharsRemaining);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatientEditEmail";
			this.Text = "Edit Email Addresses";
			this.Load += new System.EventHandler(this.FormPatientEditEmail_Load);
			this.Shown += new System.EventHandler(this.FormPatientEditEmail_Shown);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.Label labelCharsRemaining;
	}
}