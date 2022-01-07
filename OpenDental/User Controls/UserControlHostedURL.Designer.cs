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
			this.labelClinicName = new System.Windows.Forms.Label();
			this.textWebFormToLaunchNewPat = new System.Windows.Forms.TextBox();
			this.labelSchedulingURL = new System.Windows.Forms.Label();
			this.labelWebFormToLaunchNewPat = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.groupExistingPat = new OpenDental.UI.GroupBoxOD();
			this.checkExistingPatEmail = new System.Windows.Forms.CheckBox();
			this.checkExistingPatText = new System.Windows.Forms.CheckBox();
			this.groupNewPat = new OpenDental.UI.GroupBoxOD();
			this.checkNewPatEmail = new System.Windows.Forms.CheckBox();
			this.checkNewPatText = new System.Windows.Forms.CheckBox();
			this.butClearExistingPat = new OpenDental.UI.Button();
			this.butEditExistingPat = new OpenDental.UI.Button();
			this.labelWebFormToLaunchExistingPat = new System.Windows.Forms.Label();
			this.textWebFormToLaunchExistingPat = new System.Windows.Forms.TextBox();
			this.butClearNewPat = new OpenDental.UI.Button();
			this.textSchedulingURL = new System.Windows.Forms.TextBox();
			this.gridOptions = new OpenDental.UI.GridOD();
			this.butCopy = new OpenDental.UI.Button();
			this.butEditNewPat = new OpenDental.UI.Button();
			this.labelEnabled = new System.Windows.Forms.Label();
			this.butExpander = new OpenDental.UI.Button();
			this.panel1.SuspendLayout();
			this.groupExistingPat.SuspendLayout();
			this.groupNewPat.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelClinicName
			// 
			this.labelClinicName.Location = new System.Drawing.Point(28, 2);
			this.labelClinicName.Name = "labelClinicName";
			this.labelClinicName.Size = new System.Drawing.Size(287, 21);
			this.labelClinicName.TabIndex = 10;
			this.labelClinicName.Text = "Clinic Name";
			this.labelClinicName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textWebFormToLaunchNewPat
			// 
			this.textWebFormToLaunchNewPat.Location = new System.Drawing.Point(9, 135);
			this.textWebFormToLaunchNewPat.Name = "textWebFormToLaunchNewPat";
			this.textWebFormToLaunchNewPat.ReadOnly = true;
			this.textWebFormToLaunchNewPat.Size = new System.Drawing.Size(845, 20);
			this.textWebFormToLaunchNewPat.TabIndex = 8;
			// 
			// labelSchedulingURL
			// 
			this.labelSchedulingURL.Location = new System.Drawing.Point(6, 225);
			this.labelSchedulingURL.Name = "labelSchedulingURL";
			this.labelSchedulingURL.Size = new System.Drawing.Size(126, 13);
			this.labelSchedulingURL.TabIndex = 11;
			this.labelSchedulingURL.Text = "Scheduling URL";
			this.labelSchedulingURL.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelWebFormToLaunchNewPat
			// 
			this.labelWebFormToLaunchNewPat.Location = new System.Drawing.Point(6, 115);
			this.labelWebFormToLaunchNewPat.Name = "labelWebFormToLaunchNewPat";
			this.labelWebFormToLaunchNewPat.Size = new System.Drawing.Size(299, 18);
			this.labelWebFormToLaunchNewPat.TabIndex = 12;
			this.labelWebFormToLaunchNewPat.Text = "WebForm to launch after scheduling New Pat";
			this.labelWebFormToLaunchNewPat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.groupExistingPat);
			this.panel1.Controls.Add(this.groupNewPat);
			this.panel1.Controls.Add(this.butClearExistingPat);
			this.panel1.Controls.Add(this.butEditExistingPat);
			this.panel1.Controls.Add(this.labelWebFormToLaunchExistingPat);
			this.panel1.Controls.Add(this.textWebFormToLaunchExistingPat);
			this.panel1.Controls.Add(this.butClearNewPat);
			this.panel1.Controls.Add(this.textSchedulingURL);
			this.panel1.Controls.Add(this.gridOptions);
			this.panel1.Controls.Add(this.butCopy);
			this.panel1.Controls.Add(this.butEditNewPat);
			this.panel1.Controls.Add(this.labelWebFormToLaunchNewPat);
			this.panel1.Controls.Add(this.labelSchedulingURL);
			this.panel1.Controls.Add(this.textWebFormToLaunchNewPat);
			this.panel1.Location = new System.Drawing.Point(2, 25);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(983, 275);
			this.panel1.TabIndex = 9;
			// 
			// groupExistingPat
			// 
			this.groupExistingPat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupExistingPat.Controls.Add(this.checkExistingPatEmail);
			this.groupExistingPat.Controls.Add(this.checkExistingPatText);
			this.groupExistingPat.Location = new System.Drawing.Point(213, 65);
			this.groupExistingPat.Name = "groupExistingPat";
			this.groupExistingPat.Size = new System.Drawing.Size(223, 46);
			this.groupExistingPat.TabIndex = 325;
			this.groupExistingPat.TabStop = false;
			this.groupExistingPat.Text = "Web Sched Existing Patient Authentication";
			// 
			// checkExistingPatEmail
			// 
			this.checkExistingPatEmail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExistingPatEmail.Location = new System.Drawing.Point(6, 20);
			this.checkExistingPatEmail.Name = "checkExistingPatEmail";
			this.checkExistingPatEmail.Size = new System.Drawing.Size(80, 17);
			this.checkExistingPatEmail.TabIndex = 13;
			this.checkExistingPatEmail.Text = "Email";
			this.checkExistingPatEmail.UseVisualStyleBackColor = true;
			this.checkExistingPatEmail.CheckedChanged += new System.EventHandler(this.checkExistingPat_CheckedChanged);
			// 
			// checkExistingPatText
			// 
			this.checkExistingPatText.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkExistingPatText.Location = new System.Drawing.Point(131, 20);
			this.checkExistingPatText.Name = "checkExistingPatText";
			this.checkExistingPatText.Size = new System.Drawing.Size(80, 17);
			this.checkExistingPatText.TabIndex = 14;
			this.checkExistingPatText.Text = "Text";
			this.checkExistingPatText.UseVisualStyleBackColor = true;
			this.checkExistingPatText.CheckedChanged += new System.EventHandler(this.checkExistingPat_CheckedChanged);
			// 
			// groupNewPat
			// 
			this.groupNewPat.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupNewPat.Controls.Add(this.checkNewPatEmail);
			this.groupNewPat.Controls.Add(this.checkNewPatText);
			this.groupNewPat.Location = new System.Drawing.Point(1, 65);
			this.groupNewPat.Name = "groupNewPat";
			this.groupNewPat.Size = new System.Drawing.Size(206, 46);
			this.groupNewPat.TabIndex = 326;
			this.groupNewPat.TabStop = false;
			this.groupNewPat.Text = "Web Sched New Patient Authentication";
			// 
			// checkNewPatEmail
			// 
			this.checkNewPatEmail.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNewPatEmail.Location = new System.Drawing.Point(6, 20);
			this.checkNewPatEmail.Name = "checkNewPatEmail";
			this.checkNewPatEmail.Size = new System.Drawing.Size(80, 17);
			this.checkNewPatEmail.TabIndex = 324;
			this.checkNewPatEmail.Text = "Email";
			this.checkNewPatEmail.UseVisualStyleBackColor = true;
			// 
			// checkNewPatText
			// 
			this.checkNewPatText.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNewPatText.Location = new System.Drawing.Point(114, 20);
			this.checkNewPatText.Name = "checkNewPatText";
			this.checkNewPatText.Size = new System.Drawing.Size(80, 17);
			this.checkNewPatText.TabIndex = 12;
			this.checkNewPatText.Text = "Text";
			this.checkNewPatText.UseVisualStyleBackColor = true;
			// 
			// butClearExistingPat
			// 
			this.butClearExistingPat.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butClearExistingPat.Location = new System.Drawing.Point(939, 187);
			this.butClearExistingPat.Name = "butClearExistingPat";
			this.butClearExistingPat.Size = new System.Drawing.Size(27, 23);
			this.butClearExistingPat.TabIndex = 323;
			this.butClearExistingPat.UseVisualStyleBackColor = false;
			this.butClearExistingPat.Click += new System.EventHandler(this.butClearExistingPat_Click);
			// 
			// butEditExistingPat
			// 
			this.butEditExistingPat.Location = new System.Drawing.Point(860, 187);
			this.butEditExistingPat.Name = "butEditExistingPat";
			this.butEditExistingPat.Size = new System.Drawing.Size(75, 23);
			this.butEditExistingPat.TabIndex = 322;
			this.butEditExistingPat.Text = "Edit";
			this.butEditExistingPat.UseVisualStyleBackColor = true;
			this.butEditExistingPat.Click += new System.EventHandler(this.butEditExistingPat_Click);
			// 
			// labelWebFormToLaunchExistingPat
			// 
			this.labelWebFormToLaunchExistingPat.Location = new System.Drawing.Point(6, 169);
			this.labelWebFormToLaunchExistingPat.Name = "labelWebFormToLaunchExistingPat";
			this.labelWebFormToLaunchExistingPat.Size = new System.Drawing.Size(299, 18);
			this.labelWebFormToLaunchExistingPat.TabIndex = 321;
			this.labelWebFormToLaunchExistingPat.Text = "WebForm to launch after scheduling Existing Pat";
			this.labelWebFormToLaunchExistingPat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textWebFormToLaunchExistingPat
			// 
			this.textWebFormToLaunchExistingPat.Location = new System.Drawing.Point(9, 189);
			this.textWebFormToLaunchExistingPat.Name = "textWebFormToLaunchExistingPat";
			this.textWebFormToLaunchExistingPat.ReadOnly = true;
			this.textWebFormToLaunchExistingPat.Size = new System.Drawing.Size(845, 20);
			this.textWebFormToLaunchExistingPat.TabIndex = 320;
			// 
			// butClearNewPat
			// 
			this.butClearNewPat.Image = global::OpenDental.Properties.Resources.deleteX18;
			this.butClearNewPat.Location = new System.Drawing.Point(939, 133);
			this.butClearNewPat.Name = "butClearNewPat";
			this.butClearNewPat.Size = new System.Drawing.Size(27, 23);
			this.butClearNewPat.TabIndex = 15;
			this.butClearNewPat.UseVisualStyleBackColor = false;
			this.butClearNewPat.Click += new System.EventHandler(this.butClearNewPat_Click);
			// 
			// textSchedulingURL
			// 
			this.textSchedulingURL.Location = new System.Drawing.Point(9, 240);
			this.textSchedulingURL.Multiline = true;
			this.textSchedulingURL.Name = "textSchedulingURL";
			this.textSchedulingURL.ReadOnly = true;
			this.textSchedulingURL.Size = new System.Drawing.Size(845, 20);
			this.textSchedulingURL.TabIndex = 319;
			// 
			// gridOptions
			// 
			this.gridOptions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridOptions.Location = new System.Drawing.Point(0, 0);
			this.gridOptions.Name = "gridOptions";
			this.gridOptions.Size = new System.Drawing.Size(982, 59);
			this.gridOptions.TabIndex = 318;
			this.gridOptions.Title = "Options";
			this.gridOptions.TranslationName = "FormEServicesSetup";
			this.gridOptions.WrapText = false;
			this.gridOptions.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridOptions_CellClick);
			// 
			// butCopy
			// 
			this.butCopy.Location = new System.Drawing.Point(860, 238);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 23);
			this.butCopy.TabIndex = 14;
			this.butCopy.Text = "Copy";
			this.butCopy.UseVisualStyleBackColor = true;
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butEditNewPat
			// 
			this.butEditNewPat.Location = new System.Drawing.Point(860, 133);
			this.butEditNewPat.Name = "butEditNewPat";
			this.butEditNewPat.Size = new System.Drawing.Size(75, 23);
			this.butEditNewPat.TabIndex = 13;
			this.butEditNewPat.Text = "Edit";
			this.butEditNewPat.UseVisualStyleBackColor = true;
			this.butEditNewPat.Click += new System.EventHandler(this.butEditNewPat_Click);
			// 
			// labelEnabled
			// 
			this.labelEnabled.Location = new System.Drawing.Point(699, 0);
			this.labelEnabled.Name = "labelEnabled";
			this.labelEnabled.Size = new System.Drawing.Size(285, 21);
			this.labelEnabled.TabIndex = 11;
			this.labelEnabled.Text = "Enabled";
			this.labelEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butExpander
			// 
			this.butExpander.Location = new System.Drawing.Point(2, 2);
			this.butExpander.Name = "butExpander";
			this.butExpander.Size = new System.Drawing.Size(21, 21);
			this.butExpander.TabIndex = 8;
			this.butExpander.Text = "-";
			this.butExpander.UseVisualStyleBackColor = true;
			this.butExpander.Click += new System.EventHandler(this.butExpander_Click);
			// 
			// UserControlHostedURL
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.labelEnabled);
			this.Controls.Add(this.labelClinicName);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.butExpander);
			this.Name = "UserControlHostedURL";
			this.Size = new System.Drawing.Size(988, 302);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.groupExistingPat.ResumeLayout(false);
			this.groupNewPat.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private UI.Button butExpander;
		private System.Windows.Forms.Label labelClinicName;
		private System.Windows.Forms.TextBox textWebFormToLaunchNewPat;
		private System.Windows.Forms.Label labelSchedulingURL;
		private System.Windows.Forms.Label labelWebFormToLaunchNewPat;
		private UI.Button butEditNewPat;
		private UI.Button butCopy;
		private System.Windows.Forms.Panel panel1;
		private UI.GridOD gridOptions;
		private System.Windows.Forms.TextBox textSchedulingURL;
		private System.Windows.Forms.Label labelEnabled;
		private UI.Button butClearNewPat;
		private UI.Button butClearExistingPat;
		private UI.Button butEditExistingPat;
		private System.Windows.Forms.Label labelWebFormToLaunchExistingPat;
		private System.Windows.Forms.TextBox textWebFormToLaunchExistingPat;
		private System.Windows.Forms.CheckBox checkNewPatEmail;
		private System.Windows.Forms.CheckBox checkNewPatText;
		private System.Windows.Forms.CheckBox checkExistingPatEmail;
		private System.Windows.Forms.CheckBox checkExistingPatText;
		private UI.GroupBoxOD groupExistingPat;
		private UI.GroupBoxOD groupNewPat;
	}
}
