namespace OpenDental {
	partial class FormMedLabPatSelect {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMedLabPatSelect));
			this.gridLabs = new OpenDental.UI.GridOD();
			this.butOK = new System.Windows.Forms.Button();
			this.butPatSelect = new OpenDental.UI.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelExistingLab = new System.Windows.Forms.Label();
			this.gridPidInfo = new OpenDental.UI.GridOD();
			this.SuspendLayout();
			// 
			// gridLabs
			// 
			this.gridLabs.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridLabs.Location = new System.Drawing.Point(12, 163);
			this.gridLabs.Name = "gridLabs";
			this.gridLabs.Size = new System.Drawing.Size(800, 188);
			this.gridLabs.TabIndex = 5;
			this.gridLabs.Title = "Labs";
			this.gridLabs.TranslationName = "TableLabs";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(737, 357);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butPatSelect
			// 
			this.butPatSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPatSelect.Location = new System.Drawing.Point(368, 9);
			this.butPatSelect.Name = "butPatSelect";
			this.butPatSelect.Size = new System.Drawing.Size(29, 25);
			this.butPatSelect.TabIndex = 231;
			this.butPatSelect.Text = "...";
			this.butPatSelect.Click += new System.EventHandler(this.butPatSelect_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 14);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(154, 17);
			this.label5.TabIndex = 230;
			this.label5.Text = "Attached Patient";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(169, 12);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(193, 20);
			this.textName.TabIndex = 229;
			this.textName.WordWrap = false;
			// 
			// labelExistingLab
			// 
			this.labelExistingLab.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelExistingLab.ForeColor = System.Drawing.Color.Red;
			this.labelExistingLab.Location = new System.Drawing.Point(352, 360);
			this.labelExistingLab.Name = "labelExistingLab";
			this.labelExistingLab.Size = new System.Drawing.Size(386, 17);
			this.labelExistingLab.TabIndex = 250;
			this.labelExistingLab.Text = "Saving these results will update one or more existing labs.";
			this.labelExistingLab.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelExistingLab.Visible = false;
			// 
			// gridPidInfo
			// 
			this.gridPidInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridPidInfo.Location = new System.Drawing.Point(12, 40);
			this.gridPidInfo.Name = "gridPidInfo";
			this.gridPidInfo.Size = new System.Drawing.Size(800, 117);
			this.gridPidInfo.TabIndex = 251;
			this.gridPidInfo.Title = "Patient Information From Message(s)";
			this.gridPidInfo.TranslationName = "TablePatientInformation";
			// 
			// FormMedLabPatSelect
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(824, 392);
			this.Controls.Add(this.gridPidInfo);
			this.Controls.Add(this.labelExistingLab);
			this.Controls.Add(this.butPatSelect);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridLabs);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMedLabPatSelect";
			this.Text = "Lab Orders";
			this.Load += new System.EventHandler(this.FormMedLabPatSelect_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.GridOD gridLabs;
		private System.Windows.Forms.Button butOK;
		private UI.Button butPatSelect;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label labelExistingLab;
		private UI.GridOD gridPidInfo;
	}
}