namespace OpenDental {
	partial class FormCommitPrompt {
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
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.radioBoth = new System.Windows.Forms.RadioButton();
			this.radioInternal = new System.Windows.Forms.RadioButton();
			this.radioPublic = new System.Windows.Forms.RadioButton();
			this.textCommit = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(181, 212);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65, 23);
			this.butCancel.TabIndex = 44;
			this.butCancel.Text = "No";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(110, 212);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(65, 23);
			this.butOK.TabIndex = 43;
			this.butOK.Text = "Yes";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// radioBoth
			// 
			this.radioBoth.AutoSize = true;
			this.radioBoth.Location = new System.Drawing.Point(12, 189);
			this.radioBoth.Name = "radioBoth";
			this.radioBoth.Size = new System.Drawing.Size(47, 17);
			this.radioBoth.TabIndex = 47;
			this.radioBoth.Text = "Both";
			this.radioBoth.UseVisualStyleBackColor = true;
			// 
			// radioInternal
			// 
			this.radioInternal.AutoSize = true;
			this.radioInternal.Location = new System.Drawing.Point(12, 166);
			this.radioInternal.Name = "radioInternal";
			this.radioInternal.Size = new System.Drawing.Size(138, 17);
			this.radioInternal.TabIndex = 48;
			this.radioInternal.Text = "Commit to Internal Repo";
			this.radioInternal.UseVisualStyleBackColor = true;
			// 
			// radioPublic
			// 
			this.radioPublic.AutoSize = true;
			this.radioPublic.Checked = true;
			this.radioPublic.Location = new System.Drawing.Point(13, 143);
			this.radioPublic.Name = "radioPublic";
			this.radioPublic.Size = new System.Drawing.Size(132, 17);
			this.radioPublic.TabIndex = 49;
			this.radioPublic.TabStop = true;
			this.radioPublic.Text = "Commit to Public Repo";
			this.radioPublic.UseVisualStyleBackColor = true;
			// 
			// textCommit
			// 
			this.textCommit.Location = new System.Drawing.Point(4, 26);
			this.textCommit.Multiline = true;
			this.textCommit.Name = "textCommit";
			this.textCommit.ReadOnly = true;
			this.textCommit.Size = new System.Drawing.Size(242, 111);
			this.textCommit.TabIndex = 50;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(1, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(90, 13);
			this.label1.TabIndex = 51;
			this.label1.Text = "Commit Message:";
			// 
			// FormCommitPrompt
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(252, 247);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textCommit);
			this.Controls.Add(this.radioPublic);
			this.Controls.Add(this.radioInternal);
			this.Controls.Add(this.radioBoth);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Name = "FormCommitPrompt";
			this.Text = "Commit?";
			this.Load += new System.EventHandler(this.FormCommitPrompt_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.RadioButton radioBoth;
		private System.Windows.Forms.RadioButton radioInternal;
		private System.Windows.Forms.RadioButton radioPublic;
		private System.Windows.Forms.TextBox textCommit;
		private System.Windows.Forms.Label label1;
	}
}