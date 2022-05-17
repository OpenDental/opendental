namespace OpenDental.UI.Voice {
	partial class FormMsgBox {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
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
			this.labelText = new System.Windows.Forms.Label();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelText
			// 
			this.labelText.Location = new System.Drawing.Point(5, 3);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(267, 65);
			this.labelText.TabIndex = 0;
			this.labelText.Text = "Message text";
			this.labelText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(199, 71);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 1;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(117, 71);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 2;
			this.butOK.Text = "&OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormMsgBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(284, 104);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelText);
			this.Name = "FormMsgBox";
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.Text = "";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormMsgBox_FormClosing);
			this.Load += new System.EventHandler(this.FormMsgBox_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelText;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
	}
}