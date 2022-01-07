namespace OpenDental {
	partial class FormReactivationEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReactivationEdit));
			this.textDateLastContacted = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.comboStatus = new OpenDental.UI.ComboBoxOD();
			this.label9 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.textPatName = new System.Windows.Forms.TextBox();
			this.checkBoxDNC = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// textDateLastContacted
			// 
			this.textDateLastContacted.Location = new System.Drawing.Point(151, 41);
			this.textDateLastContacted.Name = "textDateLastContacted";
			this.textDateLastContacted.ReadOnly = true;
			this.textDateLastContacted.Size = new System.Drawing.Size(188, 20);
			this.textDateLastContacted.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 41);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(133, 19);
			this.label1.TabIndex = 2;
			this.label1.Text = "Date Last Contacted";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(18, 67);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(131, 19);
			this.label8.TabIndex = 8;
			this.label8.Text = "Status";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.Location = new System.Drawing.Point(151, 67);
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(188, 21);
			this.comboStatus.TabIndex = 9;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(59, 117);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(86, 82);
			this.label9.TabIndex = 11;
			this.label9.Text = "Administrative Note";
			this.label9.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(151, 119);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Recall;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(350, 99);
			this.textNote.TabIndex = 15;
			this.textNote.Text = "";
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(26, 221);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(83, 24);
			this.butDelete.TabIndex = 14;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(520, 179);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(520, 221);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 15);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(137, 19);
			this.label2.TabIndex = 16;
			this.label2.Text = "Patient Name";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatName
			// 
			this.textPatName.Location = new System.Drawing.Point(151, 15);
			this.textPatName.Name = "textPatName";
			this.textPatName.ReadOnly = true;
			this.textPatName.Size = new System.Drawing.Size(188, 20);
			this.textPatName.TabIndex = 17;
			// 
			// checkBoxDNC
			// 
			this.checkBoxDNC.Location = new System.Drawing.Point(151, 94);
			this.checkBoxDNC.Name = "checkBoxDNC";
			this.checkBoxDNC.Size = new System.Drawing.Size(167, 19);
			this.checkBoxDNC.TabIndex = 18;
			this.checkBoxDNC.Text = "Do Not Contact";
			this.checkBoxDNC.UseVisualStyleBackColor = true;
			// 
			// FormReactivationEdit
			// 
			this.ClientSize = new System.Drawing.Size(616, 270);
			this.Controls.Add(this.checkBoxDNC);
			this.Controls.Add(this.textPatName);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textDateLastContacted);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.comboStatus);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormReactivationEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Reactivation";
			this.Load += new System.EventHandler(this.FormReactivationEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion
		
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label8;
		private UI.ComboBoxOD comboStatus;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textDateLastContacted;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butDelete;
		private OpenDental.ODtextBox textNote;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPatName;
		private System.Windows.Forms.CheckBox checkBoxDNC;
	}
}
