namespace OpenDental{
	partial class FormFeeSchedPickAuthOntario {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormFeeSchedPickAuthOntario));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textODAMemberNumber = new System.Windows.Forms.TextBox();
			this.textODAMemberPassword = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(146,83);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75,24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(227,83);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75,24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(1,19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140,16);
			this.label1.TabIndex = 4;
			this.label1.Text = "ODA Member Number";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textODAMemberNumber
			// 
			this.textODAMemberNumber.Location = new System.Drawing.Point(141,17);
			this.textODAMemberNumber.Name = "textODAMemberNumber";
			this.textODAMemberNumber.Size = new System.Drawing.Size(161,20);
			this.textODAMemberNumber.TabIndex = 5;
			// 
			// textODAMemberPassword
			// 
			this.textODAMemberPassword.Location = new System.Drawing.Point(141,43);
			this.textODAMemberPassword.Name = "textODAMemberPassword";
			this.textODAMemberPassword.Size = new System.Drawing.Size(161,20);
			this.textODAMemberPassword.TabIndex = 7;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(1,45);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140,16);
			this.label2.TabIndex = 6;
			this.label2.Text = "Fee Guide Password";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormFeeSchedPickAuthOntario
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(327,124);
			this.Controls.Add(this.textODAMemberPassword);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textODAMemberNumber);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormFeeSchedPickAuthOntario";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Fee Schedule Authorization for Ontario";
			this.Load += new System.EventHandler(this.FormFeeSchedPickAuthOntario_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textODAMemberNumber;
		private System.Windows.Forms.TextBox textODAMemberPassword;
		private System.Windows.Forms.Label label2;
	}
}