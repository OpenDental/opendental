namespace OpenDental{
	partial class FormClaimResend {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormClaimResend));
			this.butCancel = new OpenDental.UI.Button();
			this.radioClaimOriginal = new System.Windows.Forms.RadioButton();
			this.radioClaimReplacement = new System.Windows.Forms.RadioButton();
			this.butSend = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(321,66);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// radioClaimOriginal
			// 
			this.radioClaimOriginal.Checked = true;
			this.radioClaimOriginal.Location = new System.Drawing.Point(12,12);
			this.radioClaimOriginal.Name = "radioClaimOriginal";
			this.radioClaimOriginal.Size = new System.Drawing.Size(390,18);
			this.radioClaimOriginal.TabIndex = 6;
			this.radioClaimOriginal.TabStop = true;
			this.radioClaimOriginal.Text = "The claim has not been accepted yet and I need to resend it.";
			this.radioClaimOriginal.UseVisualStyleBackColor = true;
			this.radioClaimOriginal.Click += new System.EventHandler(this.radioClaimOriginal_Click);
			// 
			// radioClaimReplacement
			// 
			this.radioClaimReplacement.Location = new System.Drawing.Point(12,36);
			this.radioClaimReplacement.Name = "radioClaimReplacement";
			this.radioClaimReplacement.Size = new System.Drawing.Size(390,18);
			this.radioClaimReplacement.TabIndex = 7;
			this.radioClaimReplacement.Text = "The claim was accepted and I need to replace it with updated information.";
			this.radioClaimReplacement.UseVisualStyleBackColor = true;
			this.radioClaimReplacement.Click += new System.EventHandler(this.radioClaimReplacement_Click);
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Image = ((System.Drawing.Image)(resources.GetObject("butSend.Image")));
			this.butSend.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSend.Location = new System.Drawing.Point(229,66);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(86,24);
			this.butSend.TabIndex = 131;
			this.butSend.Text = "&Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// FormClaimResend
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(408,102);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.radioClaimReplacement);
			this.Controls.Add(this.radioClaimOriginal);
			this.Controls.Add(this.butCancel);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormClaimResend";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Resend Claim";
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.RadioButton radioClaimOriginal;
		private System.Windows.Forms.RadioButton radioClaimReplacement;
		private UI.Button butSend;
	}
}