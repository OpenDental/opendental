namespace OpenDental {
	partial class FormCommItem {
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
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormCommItem));
			this.butEditAutoNote = new OpenDental.UI.Button();
			this.butClearNote = new OpenDental.UI.Button();
			this.butUserPrefs = new OpenDental.UI.Button();
			this.butAutoNote = new OpenDental.UI.Button();
			this.labelSavedManually = new System.Windows.Forms.Label();
			this.butNowEnd = new OpenDental.UI.Button();
			this.textDateTimeEnd = new System.Windows.Forms.TextBox();
			this.labelDateTimeEnd = new System.Windows.Forms.Label();
			this.butNow = new OpenDental.UI.Button();
			this.signatureBoxWrapper = new OpenDental.UI.SignatureBoxWrapper();
			this.textCommlogNum = new System.Windows.Forms.TextBox();
			this.labelCommlogNum = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.textPatientName = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textNote = new OpenDental.ODtextBox();
			this.listSentOrReceived = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.listMode = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateTime = new System.Windows.Forms.TextBox();
			this.listType = new OpenDental.UI.ListBoxOD();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.timerAutoSave = new System.Windows.Forms.Timer(this.components);
			this.SuspendLayout();
			// 
			// butEditAutoNote
			// 
			this.butEditAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditAutoNote.Location = new System.Drawing.Point(406, 192);
			this.butEditAutoNote.Name = "butEditAutoNote";
			this.butEditAutoNote.Size = new System.Drawing.Size(82, 21);
			this.butEditAutoNote.TabIndex = 144;
			this.butEditAutoNote.Text = "Edit Auto Note";
			this.butEditAutoNote.Click += new System.EventHandler(this.butEditAutoNote_Click);
			// 
			// butClearNote
			// 
			this.butClearNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearNote.Location = new System.Drawing.Point(575, 192);
			this.butClearNote.Name = "butClearNote";
			this.butClearNote.Size = new System.Drawing.Size(75, 21);
			this.butClearNote.TabIndex = 143;
			this.butClearNote.Text = "Clear";
			this.butClearNote.UseVisualStyleBackColor = true;
			this.butClearNote.Click += new System.EventHandler(this.butClearNote_Click);
			// 
			// butUserPrefs
			// 
			this.butUserPrefs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUserPrefs.Location = new System.Drawing.Point(575, 7);
			this.butUserPrefs.Name = "butUserPrefs";
			this.butUserPrefs.Size = new System.Drawing.Size(75, 21);
			this.butUserPrefs.TabIndex = 142;
			this.butUserPrefs.Text = "User Prefs";
			this.butUserPrefs.Visible = false;
			this.butUserPrefs.Click += new System.EventHandler(this.butUserPrefs_Click);
			// 
			// butAutoNote
			// 
			this.butAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAutoNote.Location = new System.Drawing.Point(494, 192);
			this.butAutoNote.Name = "butAutoNote";
			this.butAutoNote.Size = new System.Drawing.Size(75, 21);
			this.butAutoNote.TabIndex = 141;
			this.butAutoNote.Text = "Auto Note";
			this.butAutoNote.Click += new System.EventHandler(this.butAutoNote_Click);
			// 
			// labelSavedManually
			// 
			this.labelSavedManually.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelSavedManually.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelSavedManually.Location = new System.Drawing.Point(382, 561);
			this.labelSavedManually.Name = "labelSavedManually";
			this.labelSavedManually.Size = new System.Drawing.Size(106, 16);
			this.labelSavedManually.TabIndex = 140;
			this.labelSavedManually.Text = "Saved";
			this.labelSavedManually.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSavedManually.Visible = false;
			// 
			// butNowEnd
			// 
			this.butNowEnd.Location = new System.Drawing.Point(293, 55);
			this.butNowEnd.Name = "butNowEnd";
			this.butNowEnd.Size = new System.Drawing.Size(48, 21);
			this.butNowEnd.TabIndex = 139;
			this.butNowEnd.Text = "Now";
			this.butNowEnd.Click += new System.EventHandler(this.butNowEnd_Click);
			// 
			// textDateTimeEnd
			// 
			this.textDateTimeEnd.Location = new System.Drawing.Point(82, 55);
			this.textDateTimeEnd.Name = "textDateTimeEnd";
			this.textDateTimeEnd.Size = new System.Drawing.Size(205, 20);
			this.textDateTimeEnd.TabIndex = 138;
			// 
			// labelDateTimeEnd
			// 
			this.labelDateTimeEnd.Location = new System.Drawing.Point(1, 56);
			this.labelDateTimeEnd.Name = "labelDateTimeEnd";
			this.labelDateTimeEnd.Size = new System.Drawing.Size(81, 18);
			this.labelDateTimeEnd.TabIndex = 137;
			this.labelDateTimeEnd.Text = "End";
			this.labelDateTimeEnd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(293, 31);
			this.butNow.Name = "butNow";
			this.butNow.Size = new System.Drawing.Size(48, 21);
			this.butNow.TabIndex = 136;
			this.butNow.Text = "Now";
			this.butNow.Click += new System.EventHandler(this.butNow_Click);
			// 
			// signatureBoxWrapper
			// 
			this.signatureBoxWrapper.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.signatureBoxWrapper.BackColor = System.Drawing.SystemColors.ControlDark;
			this.signatureBoxWrapper.Location = new System.Drawing.Point(82, 432);
			this.signatureBoxWrapper.Name = "signatureBoxWrapper";
			this.signatureBoxWrapper.SignatureMode = OpenDental.UI.SignatureBoxWrapper.SigMode.Default;
			this.signatureBoxWrapper.Size = new System.Drawing.Size(364, 81);
			this.signatureBoxWrapper.TabIndex = 135;
			this.signatureBoxWrapper.UserSig = null;
			this.signatureBoxWrapper.SignatureChanged += new System.EventHandler(this.signatureBoxWrapper_SignatureChanged);
			// 
			// textCommlogNum
			// 
			this.textCommlogNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textCommlogNum.Location = new System.Drawing.Point(462, 531);
			this.textCommlogNum.Name = "textCommlogNum";
			this.textCommlogNum.ReadOnly = true;
			this.textCommlogNum.Size = new System.Drawing.Size(188, 20);
			this.textCommlogNum.TabIndex = 134;
			// 
			// labelCommlogNum
			// 
			this.labelCommlogNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCommlogNum.Location = new System.Drawing.Point(365, 533);
			this.labelCommlogNum.Name = "labelCommlogNum";
			this.labelCommlogNum.Size = new System.Drawing.Size(96, 16);
			this.labelCommlogNum.TabIndex = 133;
			this.labelCommlogNum.Text = "CommlogNum";
			this.labelCommlogNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUser
			// 
			this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textUser.Location = new System.Drawing.Point(450, 7);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(119, 20);
			this.textUser.TabIndex = 132;
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label16.Location = new System.Drawing.Point(388, 9);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(60, 16);
			this.label16.TabIndex = 131;
			this.label16.Text = "User";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientName
			// 
			this.textPatientName.Location = new System.Drawing.Point(82, 7);
			this.textPatientName.Name = "textPatientName";
			this.textPatientName.ReadOnly = true;
			this.textPatientName.Size = new System.Drawing.Size(205, 20);
			this.textPatientName.TabIndex = 130;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(4, 8);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(78, 18);
			this.label5.TabIndex = 129;
			this.label5.Text = "Patient";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.HasAutoNotes = true;
			this.textNote.Location = new System.Drawing.Point(82, 217);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.CommLog;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(568, 209);
			this.textNote.TabIndex = 128;
			this.textNote.Text = "";
			this.textNote.TextChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// listSentOrReceived
			// 
			this.listSentOrReceived.Location = new System.Drawing.Point(303, 98);
			this.listSentOrReceived.Name = "listSentOrReceived";
			this.listSentOrReceived.Size = new System.Drawing.Size(87, 43);
			this.listSentOrReceived.TabIndex = 127;
			this.listSentOrReceived.SelectedIndexChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(302, 80);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(142, 16);
			this.label4.TabIndex = 126;
			this.label4.Text = "Sent or Received";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listMode
			// 
			this.listMode.Location = new System.Drawing.Point(215, 98);
			this.listMode.Name = "listMode";
			this.listMode.Size = new System.Drawing.Size(73, 95);
			this.listMode.TabIndex = 125;
			this.listMode.SelectedIndexChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(214, 81);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 16);
			this.label3.TabIndex = 124;
			this.label3.Text = "Mode";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDateTime
			// 
			this.textDateTime.Location = new System.Drawing.Point(82, 31);
			this.textDateTime.Name = "textDateTime";
			this.textDateTime.Size = new System.Drawing.Size(205, 20);
			this.textDateTime.TabIndex = 123;
			this.textDateTime.TextChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(82, 98);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(120, 95);
			this.listType.TabIndex = 122;
			this.listType.SelectedIndexChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(81, 199);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(82, 16);
			this.label2.TabIndex = 121;
			this.label2.Text = "Note";
			this.label2.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 557);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 25);
			this.butDelete.TabIndex = 120;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(575, 557);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 25);
			this.butCancel.TabIndex = 119;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(494, 557);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 25);
			this.butOK.TabIndex = 118;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(80, 80);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(82, 16);
			this.label6.TabIndex = 117;
			this.label6.Text = "Type";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(1, 32);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 18);
			this.label1.TabIndex = 116;
			this.label1.Text = "Date / Time";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// timerAutoSave
			// 
			this.timerAutoSave.Interval = 10000;
			this.timerAutoSave.Tick += new System.EventHandler(this.timerAutoSave_Tick);
			// 
			// FormCommItem
			// 
			this.ClientSize = new System.Drawing.Size(662, 594);
			this.Controls.Add(this.butEditAutoNote);
			this.Controls.Add(this.butClearNote);
			this.Controls.Add(this.butUserPrefs);
			this.Controls.Add(this.butAutoNote);
			this.Controls.Add(this.labelSavedManually);
			this.Controls.Add(this.butNowEnd);
			this.Controls.Add(this.textDateTimeEnd);
			this.Controls.Add(this.labelDateTimeEnd);
			this.Controls.Add(this.butNow);
			this.Controls.Add(this.signatureBoxWrapper);
			this.Controls.Add(this.textCommlogNum);
			this.Controls.Add(this.labelCommlogNum);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.textPatientName);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.listSentOrReceived);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.listMode);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateTime);
			this.Controls.Add(this.listType);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormCommItem";
			this.Text = "Communication Item";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormCommItem_FormClosing);
			this.Load += new System.EventHandler(this.FormCommItem_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private UI.Button butEditAutoNote;
		private UI.Button butClearNote;
		private UI.Button butUserPrefs;
		private UI.Button butAutoNote;
		private System.Windows.Forms.Label labelSavedManually;
		private UI.Button butNowEnd;
		private System.Windows.Forms.TextBox textDateTimeEnd;
		private System.Windows.Forms.Label labelDateTimeEnd;
		private UI.Button butNow;
		private UI.SignatureBoxWrapper signatureBoxWrapper;
		private System.Windows.Forms.TextBox textCommlogNum;
		private System.Windows.Forms.Label labelCommlogNum;
		private System.Windows.Forms.TextBox textUser;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.TextBox textPatientName;
		private System.Windows.Forms.Label label5;
		private ODtextBox textNote;
		private OpenDental.UI.ListBoxOD listSentOrReceived;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.ListBoxOD listMode;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDateTime;
		private OpenDental.UI.ListBoxOD listType;
		private System.Windows.Forms.Label label2;
		private UI.Button butDelete;
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Timer timerAutoSave;
	}
}