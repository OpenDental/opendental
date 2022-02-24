namespace OpenDental{
	partial class FormAmountEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAmountEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textAmount = new System.Windows.Forms.TextBox();
			this.labelText = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(89, 57);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(170, 57);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textAmount
			// 
			this.textAmount.Location = new System.Drawing.Point(161, 21);
			this.textAmount.Name = "textAmount";
			this.textAmount.Size = new System.Drawing.Size(84, 20);
			this.textAmount.TabIndex = 0;
			// 
			// labelText
			// 
			this.labelText.Location = new System.Drawing.Point(1, 21);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(154, 21);
			this.labelText.TabIndex = 3;
			this.labelText.Text = "Amount";
			this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormAmountEdit
			// 
			this.AcceptButton = this.butOK;
			this.ClientSize = new System.Drawing.Size(257, 93);
			this.Controls.Add(this.labelText);
			this.Controls.Add(this.textAmount);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.HasHelpButton = false;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAmountEdit";
			this.Text = "Enter Amount";
			this.Load += new System.EventHandler(this.FormAmountEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textAmount;
		private System.Windows.Forms.Label labelText;
	}
}