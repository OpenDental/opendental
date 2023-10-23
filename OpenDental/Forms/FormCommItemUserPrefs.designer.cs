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
			this.butSave = new OpenDental.UI.Button();
			this.checkCommlogPersistClearNote = new OpenDental.UI.CheckBox();
			this.checkCommlogPersistClearEndDate = new OpenDental.UI.CheckBox();
			this.checkCommlogPersistUpdateDateTimeWithNewPatient = new OpenDental.UI.CheckBox();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(401, 160);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// checkCommlogPersistClearNote
			// 
			this.checkCommlogPersistClearNote.Location = new System.Drawing.Point(48, 53);
			this.checkCommlogPersistClearNote.Name = "checkCommlogPersistClearNote";
			this.checkCommlogPersistClearNote.Size = new System.Drawing.Size(414, 17);
			this.checkCommlogPersistClearNote.TabIndex = 4;
			this.checkCommlogPersistClearNote.Text = "Clear the Note text box after creating a commlog.";
			// 
			// checkCommlogPersistClearEndDate
			// 
			this.checkCommlogPersistClearEndDate.Location = new System.Drawing.Point(48, 87);
			this.checkCommlogPersistClearEndDate.Name = "checkCommlogPersistClearEndDate";
			this.checkCommlogPersistClearEndDate.Size = new System.Drawing.Size(414, 17);
			this.checkCommlogPersistClearEndDate.TabIndex = 5;
			this.checkCommlogPersistClearEndDate.Text = "Clear the End text box after creating a commlog.";
			// 
			// checkCommlogPersistUpdateDateTimeWithNewPatient
			// 
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Location = new System.Drawing.Point(48, 121);
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Name = "checkCommlogPersistUpdateDateTimeWithNewPatient";
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Size = new System.Drawing.Size(414, 17);
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.TabIndex = 6;
			this.checkCommlogPersistUpdateDateTimeWithNewPatient.Text = "Update the Date / Time text box with NOW when the patient changes.";
			// 
			// FormCommItemUserPrefs
			// 
			this.ClientSize = new System.Drawing.Size(488, 196);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.checkCommlogPersistUpdateDateTimeWithNewPatient);
			this.Controls.Add(this.checkCommlogPersistClearEndDate);
			this.Controls.Add(this.checkCommlogPersistClearNote);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCommItemUserPrefs";
			this.Text = "Communication Item User Prefs";
			this.Load += new System.EventHandler(this.FormCommItemUserPrefs_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.CheckBox checkCommlogPersistClearNote;
		private OpenDental.UI.CheckBox checkCommlogPersistClearEndDate;
		private OpenDental.UI.CheckBox checkCommlogPersistUpdateDateTimeWithNewPatient;
	}
}