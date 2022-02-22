namespace OpenDental.User_Controls.SetupWizard {
	partial class UserControlSetupWizComplete {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.labelEnd = new System.Windows.Forms.Label();
			this.labelDone = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// labelEnd
			// 
			this.labelEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelEnd.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelEnd.Location = new System.Drawing.Point(68, 180);
			this.labelEnd.Name = "labelEnd";
			this.labelEnd.Size = new System.Drawing.Size(795, 109);
			this.labelEnd.TabIndex = 1;
			this.labelEnd.Text = "text here";
			this.labelEnd.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// labelDone
			// 
			this.labelDone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelDone.Font = new System.Drawing.Font("Calibri", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDone.Location = new System.Drawing.Point(68, 37);
			this.labelDone.Name = "labelDone";
			this.labelDone.Size = new System.Drawing.Size(795, 70);
			this.labelDone.TabIndex = 0;
			this.labelDone.Text = "text here";
			this.labelDone.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// UserControlSetupWizComplete
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelEnd);
			this.Controls.Add(this.labelDone);
			this.Name = "UserControlSetupWizComplete";
			this.Size = new System.Drawing.Size(930, 530);
			this.Load += new System.EventHandler(this.UserControlSetupWizComplete_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label labelDone;
		private System.Windows.Forms.Label labelEnd;
	}
}
