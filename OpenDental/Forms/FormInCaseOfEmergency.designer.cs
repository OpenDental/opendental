namespace OpenDental {
	public partial class FormInCaseOfEmergency {
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
			this.textName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textPhone = new ValidPhone();
			this.textNote = new OpenDental.ODtextBox();
			this.labelPhone = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(91, 12);
			this.textName.MaxLength = 100;
			this.textName.Name = "textName";
			this.textName.Size = new System.Drawing.Size(271, 20);
			this.textName.TabIndex = 1;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(16, 15);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(77, 14);
			this.labelName.TabIndex = 33;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 67);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(77, 14);
			this.label1.TabIndex = 39;
			this.label1.Text = "Note";
			this.label1.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(91, 38);
			this.textPhone.MaxLength = 30;
			this.textPhone.Name = "textPhone";
			this.textPhone.Size = new System.Drawing.Size(271, 20);
			this.textPhone.TabIndex = 2;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.Location = new System.Drawing.Point(91, 64);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.ContactInfo;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(271, 97);
			this.textNote.TabIndex = 0;
			this.textNote.TabStop = false;
			this.textNote.Text = "";
			// 
			// labelPhone
			// 
			this.labelPhone.Location = new System.Drawing.Point(16, 41);
			this.labelPhone.Name = "labelPhone";
			this.labelPhone.Size = new System.Drawing.Size(77, 14);
			this.labelPhone.TabIndex = 35;
			this.labelPhone.Text = "Phone";
			this.labelPhone.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(287, 187);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(368, 187);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormInCaseOfEmergency
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(455, 223);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelPhone);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Name = "FormInCaseOfEmergency";
			this.Text = "ICE - In Case of Emergency";
			this.Load += new System.EventHandler(this.FormInCaseOfEmergency_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

	}
}