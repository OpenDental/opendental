namespace OpenDental{
	partial class FormPatFieldCurrencyEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPatFieldCurrencyEdit));
			this.labelName = new System.Windows.Forms.Label();
			this.textFieldCurrency = new ValidDouble();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(19, 17);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(335, 20);
			this.labelName.TabIndex = 5;
			this.labelName.Text = "Field Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textFieldCurrency
			// 
			this.textFieldCurrency.Location = new System.Drawing.Point(22, 43);
			this.textFieldCurrency.Name = "textFieldCurrency";
			this.textFieldCurrency.Size = new System.Drawing.Size(108, 20);
			this.textFieldCurrency.TabIndex = 6;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(83, 93);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(176, 93);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormPatFieldCurrencyEdit
			// 
			this.AcceptButton = this.butOK;
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(272, 140);
			this.Controls.Add(this.textFieldCurrency);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPatFieldCurrencyEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Patient Field Currency";
			this.Load += new System.EventHandler(this.FormPatFieldCurrencyEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label labelName;
		private ValidDouble textFieldCurrency;
	}
}