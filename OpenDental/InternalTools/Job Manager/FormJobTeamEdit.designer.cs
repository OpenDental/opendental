namespace OpenDental {
	partial class FormJobTeamEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormJobTeamEdit));
			this.butCancel = new OpenDental.UI.Button();
			this.butOk = new OpenDental.UI.Button();
			this.comboTeamFocus = new OpenDental.UI.ComboBox();
			this.labelTeamFocus = new System.Windows.Forms.Label();
			this.labelTeamDescription = new System.Windows.Forms.Label();
			this.textTeamDescription = new System.Windows.Forms.TextBox();
			this.labelTeamName = new System.Windows.Forms.Label();
			this.textTeamName = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(276, 117);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(65, 23);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(205, 117);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(65, 23);
			this.butOk.TabIndex = 3;
			this.butOk.Text = "OK";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOK_Click);
			// 
			// comboTeamFocus
			// 
			this.comboTeamFocus.Location = new System.Drawing.Point(90, 73);
			this.comboTeamFocus.Name = "comboTeamFocus";
			this.comboTeamFocus.Size = new System.Drawing.Size(138, 21);
			this.comboTeamFocus.TabIndex = 2;
			this.comboTeamFocus.Text = "comboTeamCategory";
			// 
			// labelTeamFocus
			// 
			this.labelTeamFocus.Location = new System.Drawing.Point(37, 73);
			this.labelTeamFocus.Name = "labelTeamFocus";
			this.labelTeamFocus.Size = new System.Drawing.Size(52, 20);
			this.labelTeamFocus.TabIndex = 252;
			this.labelTeamFocus.Text = "Focus";
			this.labelTeamFocus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTeamDescription
			// 
			this.labelTeamDescription.Location = new System.Drawing.Point(21, 52);
			this.labelTeamDescription.Name = "labelTeamDescription";
			this.labelTeamDescription.Size = new System.Drawing.Size(68, 20);
			this.labelTeamDescription.TabIndex = 250;
			this.labelTeamDescription.Text = "Description";
			this.labelTeamDescription.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTeamDescription
			// 
			this.textTeamDescription.Location = new System.Drawing.Point(90, 52);
			this.textTeamDescription.Name = "textTeamDescription";
			this.textTeamDescription.Size = new System.Drawing.Size(236, 20);
			this.textTeamDescription.TabIndex = 1;
			// 
			// labelTeamName
			// 
			this.labelTeamName.Location = new System.Drawing.Point(49, 31);
			this.labelTeamName.Name = "labelTeamName";
			this.labelTeamName.Size = new System.Drawing.Size(40, 20);
			this.labelTeamName.TabIndex = 248;
			this.labelTeamName.Text = "Name";
			this.labelTeamName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTeamName
			// 
			this.textTeamName.Location = new System.Drawing.Point(90, 31);
			this.textTeamName.Name = "textTeamName";
			this.textTeamName.Size = new System.Drawing.Size(236, 20);
			this.textTeamName.TabIndex = 0;
			// 
			// FormJobTeamEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(353, 152);
			this.Controls.Add(this.comboTeamFocus);
			this.Controls.Add(this.labelTeamFocus);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.labelTeamDescription);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.textTeamDescription);
			this.Controls.Add(this.labelTeamName);
			this.Controls.Add(this.textTeamName);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormJobTeamEdit";
			this.Text = "Team Edit";
			this.Load += new System.EventHandler(this.FormJobTeamEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOk;
		private OpenDental.UI.Button butCancel;
		private UI.ComboBox comboTeamFocus;
		private System.Windows.Forms.Label labelTeamFocus;
		private System.Windows.Forms.Label labelTeamDescription;
		private System.Windows.Forms.TextBox textTeamDescription;
		private System.Windows.Forms.Label labelTeamName;
		private System.Windows.Forms.TextBox textTeamName;
	}
}