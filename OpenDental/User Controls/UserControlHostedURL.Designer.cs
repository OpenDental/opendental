namespace OpenDental.User_Controls {
	partial class UserControlHostedURL {
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
			this.labelWebFormToLaunchExistingPat = new System.Windows.Forms.Label();
			this.textWebFormToLaunchExistingPat = new System.Windows.Forms.TextBox();
			this.textSchedulingURL = new System.Windows.Forms.TextBox();
			this.labelWebFormToLaunchNewPat = new System.Windows.Forms.Label();
			this.labelSchedulingURL = new System.Windows.Forms.Label();
			this.textWebFormToLaunchNewPat = new System.Windows.Forms.TextBox();
			this.checkAllowChildren = new System.Windows.Forms.CheckBox();
			this.butClearExistingPat = new OpenDental.UI.Button();
			this.butEditExistingPat = new OpenDental.UI.Button();
			this.butClearNewPat = new OpenDental.UI.Button();
			this.butCopy = new OpenDental.UI.Button();
			this.butEditNewPat = new OpenDental.UI.Button();
			this.groupExistingPat = new OpenDental.UI.GroupBoxOD();
			this.checkExistingPatEmail = new System.Windows.Forms.CheckBox();
			this.checkExistingPatText = new System.Windows.Forms.CheckBox();
			this.groupNewPat = new OpenDental.UI.GroupBoxOD();
			this.checkNewPatEmail = new System.Windows.Forms.CheckBox();
			this.checkNewPatText = new System.Windows.Forms.CheckBox();
			this.groupExistingPat.SuspendLayout();
			this.groupNewPat.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelWebFormToLaunchExistingPat
			// 
			this.labelWebFormToLaunchExistingPat.Location = new System.Drawing.Point(16, 210);
			this.labelWebFormToLaunchExistingPat.Name = "labelWebFormToLaunchExistingPat";
			this.labelWebFormToLaunchExistingPat.Size = new System.Drawing.Size(299, 18);
			this.labelWebFormToLaunchExistingPat.TabIndex = 378;
			this.labelWebFormToLaunchExistingPat.Text = "WebForm to launch after scheduling Existing Pat";
			this.labelWebFormToLaunchExistingPat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textWebFormToLaunchExistingPat
			// 
			this.textWebFormToLaunchExistingPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textWebFormToLaunchExistingPat.Location = new System.Drawing.Point(19, 230);
			this.textWebFormToLaunchExistingPat.Name = "textWebFormToLaunchExistingPat";
			this.textWebFormToLaunchExistingPat.ReadOnly = true;
			this.textWebFormToLaunchExistingPat.Size = new System.Drawing.Size(615, 20);
			this.textWebFormToLaunchExistingPat.TabIndex = 377;
			// 
			// textSchedulingURL
			// 
			this.textSchedulingURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textSchedulingURL.Location = new System.Drawing.Point(19, 281);
			this.textSchedulingURL.Multiline = true;
			this.textSchedulingURL.Name = "textSchedulingURL";
			this.textSchedulingURL.ReadOnly = true;
			this.textSchedulingURL.Size = new System.Drawing.Size(615, 20);
			this.textSchedulingURL.TabIndex = 376;
			// 
			// labelWebFormToLaunchNewPat
			// 
			this.labelWebFormToLaunchNewPat.Location = new System.Drawing.Point(16, 156);
			this.labelWebFormToLaunchNewPat.Name = "labelWebFormToLaunchNewPat";
			this.labelWebFormToLaunchNewPat.Size = new System.Drawing.Size(299, 18);
			this.labelWebFormToLaunchNewPat.TabIndex = 372;
			this.labelWebFormToLaunchNewPat.Text = "WebForm to launch after scheduling New Pat";
			this.labelWebFormToLaunchNewPat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelSchedulingURL
			// 
			this.labelSchedulingURL.Location = new System.Drawing.Point(16, 266);
			this.labelSchedulingURL.Name = "labelSchedulingURL";
			this.labelSchedulingURL.Size = new System.Drawing.Size(126, 13);
			this.labelSchedulingURL.TabIndex = 371;
			this.labelSchedulingURL.Text = "Scheduling URL";
			this.labelSchedulingURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textWebFormToLaunchNewPat
			// 
			this.textWebFormToLaunchNewPat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textWebFormToLaunchNewPat.Location = new System.Drawing.Point(19, 176);
			this.textWebFormToLaunchNewPat.Name = "textWebFormToLaunchNewPat";
			this.textWebFormToLaunchNewPat.ReadOnly = true;
			this.textWebFormToLaunchNewPat.Size = new System.Drawing.Size(615, 20);
			this.textWebFormToLaunchNewPat.TabIndex = 370;
			// 
			// checkAllowChildren
			// 
			this.checkAllowChildren.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllowChildren.Location = new System.Drawing.Point(25, 3);
			this.checkAllowChildren.Name = "checkAllowChildren";
			this.checkAllowChildren.Size = new System.Drawing.Size(136, 17);
			this.checkAllowChildren.TabIndex = 367;
			this.checkAllowChildren.Text = "Allow Children";
			this.checkAllowChildren.UseVisualStyleBackColor = true;
			// 
			// butClearExistingPat
			// 
			this.butClearExistingPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearExistingPat.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butClearExistingPat.Location = new System.Drawing.Point(721, 228);
			this.butClearExistingPat.Name = "butClearExistingPat";
			this.butClearExistingPat.Size = new System.Drawing.Size(27, 23);
			this.butClearExistingPat.TabIndex = 380;
			this.butClearExistingPat.UseVisualStyleBackColor = false;
			this.butClearExistingPat.Click += new System.EventHandler(this.butClearExistingPat_Click);
			// 
			// butEditExistingPat
			// 
			this.butEditExistingPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditExistingPat.Location = new System.Drawing.Point(640, 228);
			this.butEditExistingPat.Name = "butEditExistingPat";
			this.butEditExistingPat.Size = new System.Drawing.Size(75, 23);
			this.butEditExistingPat.TabIndex = 379;
			this.butEditExistingPat.Text = "Edit";
			this.butEditExistingPat.UseVisualStyleBackColor = true;
			this.butEditExistingPat.Click += new System.EventHandler(this.butEditExistingPat_Click);
			// 
			// butClearNewPat
			// 
			this.butClearNewPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearNewPat.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butClearNewPat.Location = new System.Drawing.Point(721, 174);
			this.butClearNewPat.Name = "butClearNewPat";
			this.butClearNewPat.Size = new System.Drawing.Size(27, 23);
			this.butClearNewPat.TabIndex = 375;
			this.butClearNewPat.UseVisualStyleBackColor = false;
			this.butClearNewPat.Click += new System.EventHandler(this.butClearNewPat_Click);
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy.Location = new System.Drawing.Point(640, 279);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 23);
			this.butCopy.TabIndex = 374;
			this.butCopy.Text = "Copy";
			this.butCopy.UseVisualStyleBackColor = true;
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butEditNewPat
			// 
			this.butEditNewPat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditNewPat.Location = new System.Drawing.Point(640, 174);
			this.butEditNewPat.Name = "butEditNewPat";
			this.butEditNewPat.Size = new System.Drawing.Size(75, 23);
			this.butEditNewPat.TabIndex = 373;
			this.butEditNewPat.Text = "Edit";
			this.butEditNewPat.UseVisualStyleBackColor = true;
			this.butEditNewPat.Click += new System.EventHandler(this.butEditNewPat_Click);
			// 
			// groupExistingPat
			// 
			this.groupExistingPat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupExistingPat.Controls.Add(this.checkExistingPatEmail);
			this.groupExistingPat.Controls.Add(this.checkExistingPatText);
			this.groupExistingPat.Location = new System.Drawing.Point(19, 83);
			this.groupExistingPat.Name = "groupExistingPat";
			this.groupExistingPat.Size = new System.Drawing.Size(226, 50);
			this.groupExistingPat.TabIndex = 368;
			this.groupExistingPat.Text = "Web Sched Existing Patient Authentication";
			// 
			// checkExistingPatEmail
			// 
			this.checkExistingPatEmail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExistingPatEmail.Location = new System.Drawing.Point(6, 20);
			this.checkExistingPatEmail.Name = "checkExistingPatEmail";
			this.checkExistingPatEmail.Size = new System.Drawing.Size(60, 17);
			this.checkExistingPatEmail.TabIndex = 13;
			this.checkExistingPatEmail.Text = "Email";
			this.checkExistingPatEmail.UseVisualStyleBackColor = true;
			this.checkExistingPatEmail.CheckedChanged += new System.EventHandler(this.checkExistingPatEmail_CheckedChanged);
			// 
			// checkExistingPatText
			// 
			this.checkExistingPatText.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExistingPatText.Location = new System.Drawing.Point(114, 20);
			this.checkExistingPatText.Name = "checkExistingPatText";
			this.checkExistingPatText.Size = new System.Drawing.Size(60, 17);
			this.checkExistingPatText.TabIndex = 14;
			this.checkExistingPatText.Text = "Text";
			this.checkExistingPatText.UseVisualStyleBackColor = true;
			this.checkExistingPatText.CheckedChanged += new System.EventHandler(this.checkExistingPatText_CheckedChanged);
			// 
			// groupNewPat
			// 
			this.groupNewPat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupNewPat.Controls.Add(this.checkNewPatEmail);
			this.groupNewPat.Controls.Add(this.checkNewPatText);
			this.groupNewPat.Location = new System.Drawing.Point(19, 28);
			this.groupNewPat.Name = "groupNewPat";
			this.groupNewPat.Size = new System.Drawing.Size(226, 46);
			this.groupNewPat.TabIndex = 369;
			this.groupNewPat.Text = "Web Sched New Patient Authentication";
			// 
			// checkNewPatEmail
			// 
			this.checkNewPatEmail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNewPatEmail.Location = new System.Drawing.Point(6, 20);
			this.checkNewPatEmail.Name = "checkNewPatEmail";
			this.checkNewPatEmail.Size = new System.Drawing.Size(102, 17);
			this.checkNewPatEmail.TabIndex = 324;
			this.checkNewPatEmail.Text = "Email";
			this.checkNewPatEmail.UseVisualStyleBackColor = true;
			// 
			// checkNewPatText
			// 
			this.checkNewPatText.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNewPatText.Location = new System.Drawing.Point(114, 20);
			this.checkNewPatText.Name = "checkNewPatText";
			this.checkNewPatText.Size = new System.Drawing.Size(102, 17);
			this.checkNewPatText.TabIndex = 12;
			this.checkNewPatText.Text = "Text";
			this.checkNewPatText.UseVisualStyleBackColor = true;
			// 
			// UserControlHostedURL
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.butClearExistingPat);
			this.Controls.Add(this.butEditExistingPat);
			this.Controls.Add(this.checkAllowChildren);
			this.Controls.Add(this.labelWebFormToLaunchExistingPat);
			this.Controls.Add(this.groupNewPat);
			this.Controls.Add(this.textWebFormToLaunchExistingPat);
			this.Controls.Add(this.groupExistingPat);
			this.Controls.Add(this.butClearNewPat);
			this.Controls.Add(this.textWebFormToLaunchNewPat);
			this.Controls.Add(this.textSchedulingURL);
			this.Controls.Add(this.labelSchedulingURL);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.labelWebFormToLaunchNewPat);
			this.Controls.Add(this.butEditNewPat);
			this.Name = "UserControlHostedURL";
			this.Size = new System.Drawing.Size(756, 391);
			this.groupExistingPat.ResumeLayout(false);
			this.groupNewPat.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.Button butClearExistingPat;
		private UI.Button butEditExistingPat;
		private System.Windows.Forms.Label labelWebFormToLaunchExistingPat;
		private System.Windows.Forms.TextBox textWebFormToLaunchExistingPat;
		private UI.Button butClearNewPat;
		private System.Windows.Forms.TextBox textSchedulingURL;
		private UI.Button butCopy;
		private UI.Button butEditNewPat;
		private System.Windows.Forms.Label labelWebFormToLaunchNewPat;
		private System.Windows.Forms.Label labelSchedulingURL;
		private System.Windows.Forms.TextBox textWebFormToLaunchNewPat;
		private System.Windows.Forms.CheckBox checkAllowChildren;
		private UI.GroupBoxOD groupExistingPat;
		private System.Windows.Forms.CheckBox checkExistingPatEmail;
		private System.Windows.Forms.CheckBox checkExistingPatText;
		private UI.GroupBoxOD groupNewPat;
		private System.Windows.Forms.CheckBox checkNewPatEmail;
		private System.Windows.Forms.CheckBox checkNewPatText;
	}
}
