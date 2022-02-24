namespace OpenDental{
	partial class FormFHIRAssignKey {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFHIRAssignKey));
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.textKey = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(285, 133);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(204, 133);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.ButOK_Click);
			// 
			// textKey
			// 
			this.textKey.Location = new System.Drawing.Point(51, 49);
			this.textKey.Name = "textKey";
			this.textKey.Size = new System.Drawing.Size(268, 20);
			this.textKey.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(48, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(271, 27);
			this.label1.TabIndex = 5;
			this.label1.Text = "Enter API key";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(51, 76);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(271, 40);
			this.label2.TabIndex = 6;
			this.label2.Text = "The 3rd party developer requesting access to your data via API should provide you" +
    " with an API key.";
			// 
			// FormFHIRAssignKey
			// 
			this.ClientSize = new System.Drawing.Size(372, 169);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textKey);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFHIRAssignKey";
			this.Text = "Assign API Key";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private OpenDental.UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.TextBox textKey;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
	}
}