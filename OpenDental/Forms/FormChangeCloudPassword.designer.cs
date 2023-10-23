namespace OpenDental{
	partial class FormChangeCloudPassword {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormChangeCloudPassword));
			this.butChange = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textOldPass = new System.Windows.Forms.TextBox();
			this.textNewPass = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.checkShow = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(215, 146);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 24);
			this.butChange.TabIndex = 9;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(75, 9);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(335, 31);
			this.label3.TabIndex = 26;
			this.label3.Text = "This password is the one used to log into Open Dental Cloud before the program la" +
    "unches.";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textOldPass
			// 
			this.textOldPass.Location = new System.Drawing.Point(215, 66);
			this.textOldPass.Name = "textOldPass";
			this.textOldPass.Size = new System.Drawing.Size(140, 20);
			this.textOldPass.TabIndex = 0;
			this.textOldPass.UseSystemPasswordChar = true;
			// 
			// textNewPass
			// 
			this.textNewPass.Location = new System.Drawing.Point(215, 99);
			this.textNewPass.Name = "textNewPass";
			this.textNewPass.Size = new System.Drawing.Size(140, 20);
			this.textNewPass.TabIndex = 3;
			this.textNewPass.UseSystemPasswordChar = true;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(82, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(131, 17);
			this.label1.TabIndex = 105;
			this.label1.Text = "Old password";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(82, 99);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(131, 17);
			this.label4.TabIndex = 107;
			this.label4.Text = "New password";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkShow
			// 
			this.checkShow.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkShow.Location = new System.Drawing.Point(125, 123);
			this.checkShow.Name = "checkShow";
			this.checkShow.Size = new System.Drawing.Size(104, 18);
			this.checkShow.TabIndex = 6;
			this.checkShow.Text = "Show";
			this.checkShow.CheckedChanged += new System.EventHandler(this.checkShow_CheckedChanged);
			// 
			// FormChangeCloudPassword
			// 
			this.ClientSize = new System.Drawing.Size(476, 185);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.checkShow);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textNewPass);
			this.Controls.Add(this.textOldPass);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butChange);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormChangeCloudPassword";
			this.Text = "Change Office Password";
			this.Load += new System.EventHandler(this.FormChangeCloudPassword_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butChange;
		private System.Windows.Forms.Label label3;
		public System.Windows.Forms.TextBox textOldPass;
		public System.Windows.Forms.TextBox textNewPass;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.CheckBox checkShow;
	}
}