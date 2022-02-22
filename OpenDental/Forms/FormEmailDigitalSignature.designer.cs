namespace OpenDental{
	partial class FormEmailDigitalSignature {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailDigitalSignature));
			this.butTrust = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textCertificateAuthority = new System.Windows.Forms.TextBox();
			this.textValidFrom = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textSignedBy = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textThumbprint = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textVersion = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textTrustStatus = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textTrustExplanation = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butTrust
			// 
			this.butTrust.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTrust.Location = new System.Drawing.Point(106, 316);
			this.butTrust.Name = "butTrust";
			this.butTrust.Size = new System.Drawing.Size(75, 24);
			this.butTrust.TabIndex = 3;
			this.butTrust.Text = "Trust";
			this.butTrust.Click += new System.EventHandler(this.butTrust_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(403, 316);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 141);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 20);
			this.label2.TabIndex = 5;
			this.label2.Text = "Authority:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCertificateAuthority
			// 
			this.textCertificateAuthority.Location = new System.Drawing.Point(106, 141);
			this.textCertificateAuthority.Multiline = true;
			this.textCertificateAuthority.Name = "textCertificateAuthority";
			this.textCertificateAuthority.ReadOnly = true;
			this.textCertificateAuthority.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textCertificateAuthority.Size = new System.Drawing.Size(332, 60);
			this.textCertificateAuthority.TabIndex = 6;
			// 
			// textValidFrom
			// 
			this.textValidFrom.Location = new System.Drawing.Point(106, 207);
			this.textValidFrom.Name = "textValidFrom";
			this.textValidFrom.ReadOnly = true;
			this.textValidFrom.Size = new System.Drawing.Size(332, 20);
			this.textValidFrom.TabIndex = 8;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(5, 207);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 20);
			this.label3.TabIndex = 7;
			this.label3.Text = "Valid From:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSignedBy
			// 
			this.textSignedBy.Location = new System.Drawing.Point(106, 115);
			this.textSignedBy.Name = "textSignedBy";
			this.textSignedBy.ReadOnly = true;
			this.textSignedBy.Size = new System.Drawing.Size(332, 20);
			this.textSignedBy.TabIndex = 12;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 115);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 11;
			this.label5.Text = "Signed By:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textThumbprint
			// 
			this.textThumbprint.Location = new System.Drawing.Point(106, 233);
			this.textThumbprint.Name = "textThumbprint";
			this.textThumbprint.ReadOnly = true;
			this.textThumbprint.Size = new System.Drawing.Size(332, 20);
			this.textThumbprint.TabIndex = 14;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(5, 233);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(100, 20);
			this.label6.TabIndex = 13;
			this.label6.Text = "Thumbprint:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textVersion
			// 
			this.textVersion.Location = new System.Drawing.Point(106, 256);
			this.textVersion.Name = "textVersion";
			this.textVersion.ReadOnly = true;
			this.textVersion.Size = new System.Drawing.Size(332, 20);
			this.textVersion.TabIndex = 16;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(5, 256);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 20);
			this.label7.TabIndex = 15;
			this.label7.Text = "Version:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTrustStatus
			// 
			this.textTrustStatus.Location = new System.Drawing.Point(106, 23);
			this.textTrustStatus.Name = "textTrustStatus";
			this.textTrustStatus.ReadOnly = true;
			this.textTrustStatus.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textTrustStatus.Size = new System.Drawing.Size(332, 20);
			this.textTrustStatus.TabIndex = 18;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(5, 23);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(100, 20);
			this.label4.TabIndex = 17;
			this.label4.Text = "Trust Status:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTrustExplanation
			// 
			this.textTrustExplanation.Location = new System.Drawing.Point(106, 49);
			this.textTrustExplanation.Multiline = true;
			this.textTrustExplanation.Name = "textTrustExplanation";
			this.textTrustExplanation.ReadOnly = true;
			this.textTrustExplanation.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textTrustExplanation.Size = new System.Drawing.Size(332, 60);
			this.textTrustExplanation.TabIndex = 20;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(5, 49);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(100, 20);
			this.label8.TabIndex = 19;
			this.label8.Text = "Trust Details:";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEmailDigitalSignature
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(497, 352);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.textTrustExplanation);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textTrustStatus);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textVersion);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textThumbprint);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textSignedBy);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textValidFrom);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCertificateAuthority);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butTrust);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailDigitalSignature";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Email Digital Signature";
			this.Load += new System.EventHandler(this.FormEmailDigitalSignature_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butTrust;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textCertificateAuthority;
		private System.Windows.Forms.TextBox textValidFrom;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textSignedBy;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textThumbprint;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textVersion;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textTrustStatus;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textTrustExplanation;
		private System.Windows.Forms.Label label8;
	}
}