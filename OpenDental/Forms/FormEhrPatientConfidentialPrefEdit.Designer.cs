namespace OpenDental {
	partial class FormEhrPatientConfidentialPrefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrPatientConfidentialPrefEdit));
			this.butSave = new System.Windows.Forms.Button();
			this.comboConfidentialContact = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(267, 77);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 23);
			this.butSave.TabIndex = 9;
			this.butSave.Text = "&Save";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// comboConfidentialContact
			// 
			this.comboConfidentialContact.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConfidentialContact.FormattingEnabled = true;
			this.comboConfidentialContact.Location = new System.Drawing.Point(129, 33);
			this.comboConfidentialContact.Name = "comboConfidentialContact";
			this.comboConfidentialContact.Size = new System.Drawing.Size(213, 21);
			this.comboConfidentialContact.TabIndex = 13;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(19, 34);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 17);
			this.label3.TabIndex = 14;
			this.label3.Text = "Confidential Contact";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormEhrPatientConfidentialPrefEdit
			// 
			this.ClientSize = new System.Drawing.Size(362, 112);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboConfidentialContact);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrPatientConfidentialPrefEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Patient Confidential Pref Edit";
			this.Load += new System.EventHandler(this.FormPatientConfidentialPrefEdit_Load);
			this.ResumeLayout(false);

		}

		#endregion
		private System.Windows.Forms.Button butSave;
		private System.Windows.Forms.ComboBox comboConfidentialContact;
		private System.Windows.Forms.Label label3;
	}
}