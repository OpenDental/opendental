namespace OpenDental {
	partial class FormEhrReminders {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrReminders));
			this.label1 = new System.Windows.Forms.Label();
			this.textPreferedConfidentialContact = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butClose = new System.Windows.Forms.Button();
			this.butSend = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.gridProvided = new OpenDental.UI.GridOD();
			this.butEdit = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label1.Location = new System.Drawing.Point(17, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(213, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Confidental Communication Preference";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPreferedConfidentialContact
			// 
			this.textPreferedConfidentialContact.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textPreferedConfidentialContact.Location = new System.Drawing.Point(233, 16);
			this.textPreferedConfidentialContact.Name = "textPreferedConfidentialContact";
			this.textPreferedConfidentialContact.ReadOnly = true;
			this.textPreferedConfidentialContact.Size = new System.Drawing.Size(237, 20);
			this.textPreferedConfidentialContact.TabIndex = 1;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(15, 42);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(535, 244);
			this.gridMain.TabIndex = 3;
			this.gridMain.Title = "Reminders";
			this.gridMain.TranslationName = "TableReminders";
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(475, 585);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 4;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butSend
			// 
			this.butSend.Location = new System.Drawing.Point(248, 292);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 23);
			this.butSend.TabIndex = 5;
			this.butSend.Text = "Send";
			this.butSend.UseVisualStyleBackColor = true;
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 586);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 6;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// gridProvided
			// 
			this.gridProvided.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProvided.Location = new System.Drawing.Point(18, 321);
			this.gridProvided.Name = "gridProvided";
			this.gridProvided.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProvided.Size = new System.Drawing.Size(535, 258);
			this.gridProvided.TabIndex = 7;
			this.gridProvided.Title = "Reminders Sent";
			this.gridProvided.TranslationName = "TableRemindersSent";
			// 
			// butEdit
			// 
			this.butEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEdit.Location = new System.Drawing.Point(475, 14);
			this.butEdit.Name = "butEdit";
			this.butEdit.Size = new System.Drawing.Size(75, 23);
			this.butEdit.TabIndex = 8;
			this.butEdit.Text = "Edit";
			this.butEdit.UseVisualStyleBackColor = true;
			this.butEdit.Click += new System.EventHandler(this.butEdit_Click);
			// 
			// FormEhrReminders
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(562, 617);
			this.Controls.Add(this.butClose);
			this.Controls.Add(this.butEdit);
			this.Controls.Add(this.gridProvided);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.textPreferedConfidentialContact);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrReminders";
			this.Text = "Reminders";
			this.Load += new System.EventHandler(this.FormReminders_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textPreferedConfidentialContact;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butSend;
		private System.Windows.Forms.Button butDelete;
		private OpenDental.UI.GridOD gridProvided;
		private System.Windows.Forms.Button butEdit;
	}
}