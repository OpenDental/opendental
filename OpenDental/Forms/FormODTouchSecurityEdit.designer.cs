namespace OpenDental{
	partial class FormODTouchSecurityEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormODTouchSecurityEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelFrequency = new System.Windows.Forms.Label();
			this.textFrequency = new OpenDental.ValidNum();
			this.label1 = new System.Windows.Forms.Label();
			this.checkSecurityEnabled = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(112, 102);
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
			this.butCancel.Location = new System.Drawing.Point(193, 102);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelFrequency
			// 
			this.labelFrequency.Location = new System.Drawing.Point(16, 32);
			this.labelFrequency.Name = "labelFrequency";
			this.labelFrequency.Size = new System.Drawing.Size(127, 21);
			this.labelFrequency.TabIndex = 7;
			this.labelFrequency.Text = "Frequency (Minutes)";
			this.labelFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFrequency
			// 
			this.textFrequency.Location = new System.Drawing.Point(147, 32);
			this.textFrequency.Name = "textFrequency";
			this.textFrequency.Size = new System.Drawing.Size(121, 20);
			this.textFrequency.TabIndex = 8;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 55);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(252, 41);
			this.label1.TabIndex = 9;
			this.label1.Text = "Set the frequency of which clinicians are asked to validate their credentials.";
			// 
			// checkSecurityEnabled
			// 
			this.checkSecurityEnabled.Location = new System.Drawing.Point(113, 12);
			this.checkSecurityEnabled.Name = "checkSecurityEnabled";
			this.checkSecurityEnabled.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkSecurityEnabled.Size = new System.Drawing.Size(160, 17);
			this.checkSecurityEnabled.TabIndex = 10;
			this.checkSecurityEnabled.Text = "Enable Clinical Security";
			this.checkSecurityEnabled.CheckedChanged += new System.EventHandler(this.checkSecurityEnabled_CheckedChanged);
			// 
			// FormEClipboardSecurityEdit
			// 
			this.CancelButton = this.butCancel;
			this.ClientSize = new System.Drawing.Size(280, 138);
			this.Controls.Add(this.checkSecurityEnabled);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textFrequency);
			this.Controls.Add(this.labelFrequency);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEClipboardSecurityEdit";
			this.Text = "ODTouch Security";
			this.Load += new System.EventHandler(this.FormEClipboardSecurityEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelFrequency;
		private ValidNum textFrequency;
		private System.Windows.Forms.Label label1;
		private OpenDental.UI.CheckBox checkSecurityEnabled;
	}
}