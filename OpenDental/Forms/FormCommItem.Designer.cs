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
			this.listSentOrReceived = new OpenDental.UI.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listMode = new OpenDental.UI.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateTime = new System.Windows.Forms.TextBox();
			this.listType = new OpenDental.UI.ListBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butSave = new OpenDental.UI.Button();
			this.butCommlogHist = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.timerAutoSave = new System.Windows.Forms.Timer(this.components);
			this.textDateTimeCreated = new System.Windows.Forms.TextBox();
			this.labelDateTimeCreated = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// butEditAutoNote
			// 
			this.butEditAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butEditAutoNote.Location = new System.Drawing.Point(499, 216);
			this.butEditAutoNote.Name = "butEditAutoNote";
			this.butEditAutoNote.Size = new System.Drawing.Size(82, 21);
			this.butEditAutoNote.TabIndex = 144;
			this.butEditAutoNote.Text = "Edit Auto Note";
			this.butEditAutoNote.Click += new System.EventHandler(this.butEditAutoNote_Click);
			// 
			// butClearNote
			// 
			this.butClearNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butClearNote.Location = new System.Drawing.Point(668, 216);
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
			this.butUserPrefs.Location = new System.Drawing.Point(668, 7);
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
			this.butAutoNote.Location = new System.Drawing.Point(587, 216);
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
			this.labelSavedManually.Location = new System.Drawing.Point(552, 584);
			this.labelSavedManually.Name = "labelSavedManually";
			this.labelSavedManually.Size = new System.Drawing.Size(106, 16);
			this.labelSavedManually.TabIndex = 140;
			this.labelSavedManually.Text = "Saved";
			this.labelSavedManually.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSavedManually.Visible = false;
			// 
			// butNowEnd
			// 
			this.butNowEnd.Location = new System.Drawing.Point(319, 79);
			this.butNowEnd.Name = "butNowEnd";
			this.butNowEnd.Size = new System.Drawing.Size(48, 21);
			this.butNowEnd.TabIndex = 139;
			this.butNowEnd.Text = "Now";
			this.butNowEnd.Click += new System.EventHandler(this.butNowEnd_Click);
			// 
			// textDateTimeEnd
			// 
			this.textDateTimeEnd.Location = new System.Drawing.Point(108, 79);
			this.textDateTimeEnd.Name = "textDateTimeEnd";
			this.textDateTimeEnd.Size = new System.Drawing.Size(205, 20);
			this.textDateTimeEnd.TabIndex = 138;
			// 
			// labelDateTimeEnd
			// 
			this.labelDateTimeEnd.Location = new System.Drawing.Point(27, 80);
			this.labelDateTimeEnd.Name = "labelDateTimeEnd";
			this.labelDateTimeEnd.Size = new System.Drawing.Size(81, 18);
			this.labelDateTimeEnd.TabIndex = 137;
			this.labelDateTimeEnd.Text = "End";
			this.labelDateTimeEnd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(319, 55);
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
			this.signatureBoxWrapper.Location = new System.Drawing.Point(108, 455);
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
			this.textCommlogNum.Location = new System.Drawing.Point(555, 554);
			this.textCommlogNum.Name = "textCommlogNum";
			this.textCommlogNum.ReadOnly = true;
			this.textCommlogNum.Size = new System.Drawing.Size(188, 20);
			this.textCommlogNum.TabIndex = 134;
			// 
			// labelCommlogNum
			// 
			this.labelCommlogNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCommlogNum.Location = new System.Drawing.Point(458, 556);
			this.labelCommlogNum.Name = "labelCommlogNum";
			this.labelCommlogNum.Size = new System.Drawing.Size(96, 16);
			this.labelCommlogNum.TabIndex = 133;
			this.labelCommlogNum.Text = "CommlogNum";
			this.labelCommlogNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUser
			// 
			this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textUser.Location = new System.Drawing.Point(543, 7);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(119, 20);
			this.textUser.TabIndex = 132;
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label16.Location = new System.Drawing.Point(481, 9);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(60, 16);
			this.label16.TabIndex = 131;
			this.label16.Text = "User";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPatientName
			// 
			this.textPatientName.Location = new System.Drawing.Point(108, 7);
			this.textPatientName.Name = "textPatientName";
			this.textPatientName.ReadOnly = true;
			this.textPatientName.Size = new System.Drawing.Size(205, 20);
			this.textPatientName.TabIndex = 130;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(30, 8);
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
			this.textNote.Location = new System.Drawing.Point(108, 240);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.CommLog;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(635, 209);
			this.textNote.TabIndex = 128;
			this.textNote.Text = "";
			this.textNote.TextChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// listSentOrReceived
			// 
			this.listSentOrReceived.Location = new System.Drawing.Point(380, 125);
			this.listSentOrReceived.Name = "listSentOrReceived";
			this.listSentOrReceived.Size = new System.Drawing.Size(101, 43);
			this.listSentOrReceived.TabIndex = 127;
			this.listSentOrReceived.SelectedIndexChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(384, 107);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(142, 16);
			this.label4.TabIndex = 126;
			this.label4.Text = "Sent or Received";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listMode
			// 
			this.listMode.Location = new System.Drawing.Point(242, 125);
			this.listMode.Name = "listMode";
			this.listMode.Size = new System.Drawing.Size(125, 95);
			this.listMode.TabIndex = 125;
			this.listMode.SelectedIndexChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(241, 108);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(82, 16);
			this.label3.TabIndex = 124;
			this.label3.Text = "Mode";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textDateTime
			// 
			this.textDateTime.Location = new System.Drawing.Point(108, 55);
			this.textDateTime.Name = "textDateTime";
			this.textDateTime.Size = new System.Drawing.Size(205, 20);
			this.textDateTime.TabIndex = 123;
			this.textDateTime.TextChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// listType
			// 
			this.listType.Location = new System.Drawing.Point(108, 125);
			this.listType.Name = "listType";
			this.listType.Size = new System.Drawing.Size(120, 95);
			this.listType.TabIndex = 122;
			this.listType.SelectedIndexChanged += new System.EventHandler(this.ClearSignature_Handler);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(105, 223);
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
			this.butDelete.Location = new System.Drawing.Point(12, 580);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 25);
			this.butDelete.TabIndex = 120;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(668, 580);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 25);
			this.butSave.TabIndex = 118;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// butCommlogHist
			// 
			this.butCommlogHist.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCommlogHist.Location = new System.Drawing.Point(418, 216);
			this.butCommlogHist.Name = "butCommlogHist";
			this.butCommlogHist.Size = new System.Drawing.Size(75, 21);
			this.butCommlogHist.TabIndex = 145;
			this.butCommlogHist.Text = "History";
			this.butCommlogHist.Visible = false;
			this.butCommlogHist.Click += new System.EventHandler(this.butCommlogHist_Click);
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(106, 107);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(82, 16);
			this.label6.TabIndex = 117;
			this.label6.Text = "Type";
			this.label6.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(27, 56);
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
			// textDateTimeCreated
			// 
			this.textDateTimeCreated.Location = new System.Drawing.Point(108, 31);
			this.textDateTimeCreated.Name = "textDateTimeCreated";
			this.textDateTimeCreated.ReadOnly = true;
			this.textDateTimeCreated.Size = new System.Drawing.Size(205, 20);
			this.textDateTimeCreated.TabIndex = 145;
			// 
			// labelDateTimeCreated
			// 
			this.labelDateTimeCreated.Location = new System.Drawing.Point(2, 32);
			this.labelDateTimeCreated.Name = "labelDateTimeCreated";
			this.labelDateTimeCreated.Size = new System.Drawing.Size(106, 18);
			this.labelDateTimeCreated.TabIndex = 146;
			this.labelDateTimeCreated.Text = "Date/Time Created";
			this.labelDateTimeCreated.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormCommItem
			// 
			this.ClientSize = new System.Drawing.Size(755, 617);
			this.Controls.Add(this.labelDateTimeCreated);
			this.Controls.Add(this.textDateTimeCreated);
			this.Controls.Add(this.butCommlogHist);
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
			this.Controls.Add(this.butSave);
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
		private OpenDental.UI.ListBox listSentOrReceived;
		private System.Windows.Forms.Label label4;
		private OpenDental.UI.ListBox listMode;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDateTime;
		private OpenDental.UI.ListBox listType;
		private System.Windows.Forms.Label label2;
		private UI.Button butDelete;
		private UI.Button butSave;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Timer timerAutoSave;
		private System.Windows.Forms.TextBox textDateTimeCreated;
		private System.Windows.Forms.Label labelDateTimeCreated;
		private UI.Button butCommlogHist;
	}
}