namespace OpenDental{
	partial class FormCommItemUserPrefs {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCommItemUserPrefs));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkCommlogPersistClearNote = new System.Windows.Forms.CheckBox();
			this.checkCommlogPersistClearEndDate = new System.Windows.Forms.CheckBox();
			this.checkCommlogPersistUpdateDateTimeWithNewPatient = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(320, 183);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(401, 183);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkCommlogPersistClearNote
			// 
			this.checkCommlogPersistClearNote.Location = new System.Drawing.Point(48, 53);
			this.checkCommlogPersistClearNote.Name = "checkCommlogPersistClearNote";
			this.checkCommlogPersistClearNote.Size = new System.Drawing.Size(414, 17);
			this.checkCommlogPersistClearNote.TabIndex = 4;
			this.checkCommlogPersistClearNote.Text = "Clear the Note text box after creating a commlog.";
			this.checkCommlogPersistClearNote.UseVisualStyleBackColor = true;
			// 
			// checkCommlogPersistClearEndDate
			// 
			this.checkCommlogPersistClearEndDate.Location = new System.Drawing.Point(48, 87);
			this.checkCommlogPersistClearEndDate.Name = "checkCommlogPersistClearEndDate";
			this.checkCommlogPersistClearEndDate.Size = new System.Drawing.Size(414, 17);
			this.checkCommlogPersistClearEndDate.TabIndex = 5;
			this.checkCommlogPersistClearEndDate.Text = "Clear the End text box after creating a commlog.";
			this.checkCommlogPersistClearEndDate.UseVisualStyleBackColor = true;
			// 
			// checkCommlogPersistUpdateDateTimeWithNewPatient
			// 
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Location = new System.Drawing.Point(48, 121);
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Name = "checkCommlogPersistUpdateDateTimeWithNewPatient";
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Size = new System.Drawing.Size(414, 17);
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.TabIndex = 6;
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Text = "Update the Date / Time text box with NOW when the patient changes.";
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.UseVisualStyleBackColor = true;
			// 
			// FormCommItemUserPrefs
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(488, 219);
			this.Controls.Add(this.checkCommlogPersistUpdateDateTimeWithNewPatient);
			this.Controls.Add(this.checkCommlogPersistClearEndDate);
			this.Controls.Add(this.checkCommlogPersistClearNote);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCommItemUserPrefs";
			this.Text = "Communication Item User Prefs";
			this.Load += new System.EventHandler(this.FormCommItemUserPrefs_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkCommlogPersistClearNote;
		private System.Windows.Forms.CheckBox checkCommlogPersistClearEndDate;
		private System.Windows.Forms.CheckBox checkCommlogPersistUpdateDateTimeWithNewPatient;
	}
}