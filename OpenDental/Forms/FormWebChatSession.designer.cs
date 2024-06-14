namespace OpenDental{
	partial class FormWebChatSession {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormWebChatSession));
			this.butTakeOwnership = new OpenDental.UI.Button();
			this.butEndSession = new OpenDental.UI.Button();
			this.labelOwner = new System.Windows.Forms.Label();
			this.textOwner = new System.Windows.Forms.TextBox();
			this.textWebChatSessionNum = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textEmail = new System.Windows.Forms.TextBox();
			this.labelEmail = new System.Windows.Forms.Label();
			this.textPhone = new OpenDental.ValidPhone();
			this.labelPhone = new System.Windows.Forms.Label();
			this.textName = new System.Windows.Forms.TextBox();
			this.labelName = new System.Windows.Forms.Label();
			this.textPractice = new System.Windows.Forms.TextBox();
			this.labelPractice = new System.Windows.Forms.Label();
			this.checkIsCustomer = new OpenDental.UI.CheckBox();
			this.textCustomer = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butSearchAndAttach = new OpenDental.UI.Button();
			this.tabControlMain = new OpenDental.UI.TabControl();
			this.tabPageMessages = new OpenDental.UI.TabPage();
			this.butAskAi = new OpenDental.UI.Button();
			this.butSend = new OpenDental.UI.Button();
			this.textChatMessage = new OpenDental.ODtextBox();
			this.webChatThread = new OpenDental.SmsThreadView();
			this.tabPageNotes = new OpenDental.UI.TabPage();
			this.gridODNotes = new OpenDental.UI.GridOD();
			this.butAttachSuggestion = new OpenDental.UI.Button();
			this.butAddNote = new OpenDental.UI.Button();
			this.tabControlMain.SuspendLayout();
			this.tabPageMessages.SuspendLayout();
			this.tabPageNotes.SuspendLayout();
			this.SuspendLayout();
			// 
			// butTakeOwnership
			// 
			this.butTakeOwnership.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butTakeOwnership.Location = new System.Drawing.Point(160, 539);
			this.butTakeOwnership.Name = "butTakeOwnership";
			this.butTakeOwnership.Size = new System.Drawing.Size(121, 26);
			this.butTakeOwnership.TabIndex = 3;
			this.butTakeOwnership.Text = "Take Ownership";
			this.butTakeOwnership.Click += new System.EventHandler(this.butTakeOwnership_Click);
			// 
			// butEndSession
			// 
			this.butEndSession.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butEndSession.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butEndSession.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEndSession.Location = new System.Drawing.Point(12, 539);
			this.butEndSession.Name = "butEndSession";
			this.butEndSession.Size = new System.Drawing.Size(114, 26);
			this.butEndSession.TabIndex = 4;
			this.butEndSession.Text = "End Session";
			this.butEndSession.Click += new System.EventHandler(this.butEndSession_Click);
			// 
			// labelOwner
			// 
			this.labelOwner.Location = new System.Drawing.Point(12, 9);
			this.labelOwner.Name = "labelOwner";
			this.labelOwner.Size = new System.Drawing.Size(66, 20);
			this.labelOwner.TabIndex = 8;
			this.labelOwner.Text = "Owner";
			this.labelOwner.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOwner
			// 
			this.textOwner.Location = new System.Drawing.Point(79, 9);
			this.textOwner.Name = "textOwner";
			this.textOwner.ReadOnly = true;
			this.textOwner.Size = new System.Drawing.Size(140, 20);
			this.textOwner.TabIndex = 9;
			// 
			// textWebChatSessionNum
			// 
			this.textWebChatSessionNum.Location = new System.Drawing.Point(322, 9);
			this.textWebChatSessionNum.Name = "textWebChatSessionNum";
			this.textWebChatSessionNum.ReadOnly = true;
			this.textWebChatSessionNum.Size = new System.Drawing.Size(150, 20);
			this.textWebChatSessionNum.TabIndex = 11;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(225, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(96, 20);
			this.label1.TabIndex = 10;
			this.label1.Text = "SessionNum";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textEmail
			// 
			this.textEmail.Location = new System.Drawing.Point(322, 30);
			this.textEmail.Name = "textEmail";
			this.textEmail.ReadOnly = true;
			this.textEmail.Size = new System.Drawing.Size(150, 20);
			this.textEmail.TabIndex = 13;
			// 
			// labelEmail
			// 
			this.labelEmail.Location = new System.Drawing.Point(225, 30);
			this.labelEmail.Name = "labelEmail";
			this.labelEmail.Size = new System.Drawing.Size(96, 20);
			this.labelEmail.TabIndex = 12;
			this.labelEmail.Text = "Email";
			this.labelEmail.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPhone
			// 
			this.textPhone.Location = new System.Drawing.Point(322, 51);
			this.textPhone.Name = "textPhone";
			this.textPhone.ReadOnly = true;
			this.textPhone.Size = new System.Drawing.Size(150, 20);
			this.textPhone.TabIndex = 15;
			// 
			// labelPhone
			// 
			this.labelPhone.Location = new System.Drawing.Point(225, 51);
			this.labelPhone.Name = "labelPhone";
			this.labelPhone.Size = new System.Drawing.Size(96, 20);
			this.labelPhone.TabIndex = 14;
			this.labelPhone.Text = "Phone";
			this.labelPhone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textName
			// 
			this.textName.Location = new System.Drawing.Point(79, 30);
			this.textName.Name = "textName";
			this.textName.ReadOnly = true;
			this.textName.Size = new System.Drawing.Size(140, 20);
			this.textName.TabIndex = 17;
			// 
			// labelName
			// 
			this.labelName.Location = new System.Drawing.Point(12, 30);
			this.labelName.Name = "labelName";
			this.labelName.Size = new System.Drawing.Size(66, 20);
			this.labelName.TabIndex = 16;
			this.labelName.Text = "Name";
			this.labelName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPractice
			// 
			this.textPractice.Location = new System.Drawing.Point(79, 51);
			this.textPractice.Name = "textPractice";
			this.textPractice.ReadOnly = true;
			this.textPractice.Size = new System.Drawing.Size(140, 20);
			this.textPractice.TabIndex = 19;
			// 
			// labelPractice
			// 
			this.labelPractice.Location = new System.Drawing.Point(12, 51);
			this.labelPractice.Name = "labelPractice";
			this.labelPractice.Size = new System.Drawing.Size(66, 20);
			this.labelPractice.TabIndex = 18;
			this.labelPractice.Text = "Practice";
			this.labelPractice.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsCustomer
			// 
			this.checkIsCustomer.Enabled = false;
			this.checkIsCustomer.Location = new System.Drawing.Point(383, 98);
			this.checkIsCustomer.Name = "checkIsCustomer";
			this.checkIsCustomer.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkIsCustomer.Size = new System.Drawing.Size(89, 20);
			this.checkIsCustomer.TabIndex = 20;
			this.checkIsCustomer.Text = "Is Customer";
			// 
			// textCustomer
			// 
			this.textCustomer.Location = new System.Drawing.Point(79, 72);
			this.textCustomer.Name = "textCustomer";
			this.textCustomer.ReadOnly = true;
			this.textCustomer.Size = new System.Drawing.Size(393, 20);
			this.textCustomer.TabIndex = 21;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 72);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(66, 20);
			this.label2.TabIndex = 22;
			this.label2.Text = "Customer";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSearchAndAttach
			// 
			this.butSearchAndAttach.Location = new System.Drawing.Point(192, 98);
			this.butSearchAndAttach.Name = "butSearchAndAttach";
			this.butSearchAndAttach.Size = new System.Drawing.Size(102, 20);
			this.butSearchAndAttach.TabIndex = 23;
			this.butSearchAndAttach.Text = "Search and Attach";
			this.butSearchAndAttach.Click += new System.EventHandler(this.butSearchAndAttach_Click);
			// 
			// tabControlMain
			// 
			this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlMain.Controls.Add(this.tabPageMessages);
			this.tabControlMain.Controls.Add(this.tabPageNotes);
			this.tabControlMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControlMain.Location = new System.Drawing.Point(4, 128);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.Size = new System.Drawing.Size(548, 405);
			this.tabControlMain.TabIndex = 28;
			this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
			// 
			// tabPageMessages
			// 
			this.tabPageMessages.Controls.Add(this.butAskAi);
			this.tabPageMessages.Controls.Add(this.butSend);
			this.tabPageMessages.Controls.Add(this.textChatMessage);
			this.tabPageMessages.Controls.Add(this.webChatThread);
			this.tabPageMessages.Location = new System.Drawing.Point(2, 21);
			this.tabPageMessages.Name = "tabPageMessages";
			this.tabPageMessages.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMessages.Size = new System.Drawing.Size(544, 382);
			this.tabPageMessages.TabIndex = 0;
			this.tabPageMessages.Text = "Messages";
			// 
			// butAskAi
			// 
			this.butAskAi.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAskAi.Location = new System.Drawing.Point(464, 356);
			this.butAskAi.Name = "butAskAi";
			this.butAskAi.Size = new System.Drawing.Size(75, 26);
			this.butAskAi.TabIndex = 10;
			this.butAskAi.Text = "Ask AI";
			this.butAskAi.Visible = false;
			this.butAskAi.Click += new System.EventHandler(this.butAskAi_Click);
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(464, 329);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 26);
			this.butSend.TabIndex = 9;
			this.butSend.Text = "Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// textChatMessage
			// 
			this.textChatMessage.AcceptsTab = true;
			this.textChatMessage.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textChatMessage.BackColor = System.Drawing.SystemColors.Window;
			this.textChatMessage.DetectLinksEnabled = false;
			this.textChatMessage.DetectUrls = false;
			this.textChatMessage.Location = new System.Drawing.Point(2, 329);
			this.textChatMessage.Name = "textChatMessage";
			this.textChatMessage.QuickPasteType = OpenDentBusiness.EnumQuickPasteType.WebChat;
			this.textChatMessage.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textChatMessage.Size = new System.Drawing.Size(454, 50);
			this.textChatMessage.TabIndex = 8;
			this.textChatMessage.Text = "";
			this.textChatMessage.KeyDown += new System.Windows.Forms.KeyEventHandler(this.textChatMessage_KeyDown);
			// 
			// webChatThread
			// 
			this.webChatThread.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.webChatThread.BackColor = System.Drawing.SystemColors.Control;
			this.webChatThread.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.webChatThread.ListSmsThreadMessages = null;
			this.webChatThread.Location = new System.Drawing.Point(2, 2);
			this.webChatThread.Name = "webChatThread";
			this.webChatThread.Size = new System.Drawing.Size(540, 324);
			this.webChatThread.TabIndex = 6;
			// 
			// tabPageNotes
			// 
			this.tabPageNotes.Controls.Add(this.gridODNotes);
			this.tabPageNotes.Location = new System.Drawing.Point(2, 21);
			this.tabPageNotes.Name = "tabPageNotes";
			this.tabPageNotes.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageNotes.Size = new System.Drawing.Size(544, 382);
			this.tabPageNotes.TabIndex = 1;
			this.tabPageNotes.Text = "Notes";
			// 
			// gridODNotes
			// 
			this.gridODNotes.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridODNotes.Location = new System.Drawing.Point(2, 1);
			this.gridODNotes.Name = "gridODNotes";
			this.gridODNotes.Size = new System.Drawing.Size(541, 380);
			this.gridODNotes.TabIndex = 0;
			this.gridODNotes.Title = "Notes";
			this.gridODNotes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridODNotes_CellDoubleClick);
			// 
			// butAttachSuggestion
			// 
			this.butAttachSuggestion.Enabled = false;
			this.butAttachSuggestion.Location = new System.Drawing.Point(79, 98);
			this.butAttachSuggestion.Name = "butAttachSuggestion";
			this.butAttachSuggestion.Size = new System.Drawing.Size(110, 20);
			this.butAttachSuggestion.TabIndex = 29;
			this.butAttachSuggestion.Text = "Attach Suggestion";
			this.butAttachSuggestion.Click += new System.EventHandler(this.butAttachSuggestion_Click);
			// 
			// butAddNote
			// 
			this.butAddNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAddNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNote.Location = new System.Drawing.Point(316, 539);
			this.butAddNote.Name = "butAddNote";
			this.butAddNote.Size = new System.Drawing.Size(75, 26);
			this.butAddNote.TabIndex = 151;
			this.butAddNote.Text = "Add";
			this.butAddNote.Click += new System.EventHandler(this.butAddNote_Click);
			// 
			// FormWebChatSession
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.ClientSize = new System.Drawing.Size(555, 575);
			this.Controls.Add(this.butAddNote);
			this.Controls.Add(this.butAttachSuggestion);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.butSearchAndAttach);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textCustomer);
			this.Controls.Add(this.checkIsCustomer);
			this.Controls.Add(this.textPractice);
			this.Controls.Add(this.labelPractice);
			this.Controls.Add(this.textName);
			this.Controls.Add(this.labelName);
			this.Controls.Add(this.textPhone);
			this.Controls.Add(this.labelPhone);
			this.Controls.Add(this.textEmail);
			this.Controls.Add(this.labelEmail);
			this.Controls.Add(this.textWebChatSessionNum);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textOwner);
			this.Controls.Add(this.labelOwner);
			this.Controls.Add(this.butEndSession);
			this.Controls.Add(this.butTakeOwnership);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormWebChatSession";
			this.Text = "Web Chat Session";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormWebChatSession_FormClosing);
			this.Load += new System.EventHandler(this.FormWebChatSession_Load);
			this.ResizeEnd += new System.EventHandler(this.FormWebChatSession_ResizeEnd);
			this.tabControlMain.ResumeLayout(false);
			this.tabPageMessages.ResumeLayout(false);
			this.tabPageNotes.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butTakeOwnership;
		private UI.Button butEndSession;
		private System.Windows.Forms.Label labelOwner;
		private System.Windows.Forms.TextBox textOwner;
		private System.Windows.Forms.TextBox textWebChatSessionNum;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textEmail;
		private System.Windows.Forms.Label labelEmail;
		private ValidPhone textPhone;
		private System.Windows.Forms.Label labelPhone;
		private System.Windows.Forms.TextBox textName;
		private System.Windows.Forms.Label labelName;
		private System.Windows.Forms.TextBox textPractice;
		private System.Windows.Forms.Label labelPractice;
		private OpenDental.UI.CheckBox checkIsCustomer;
		private System.Windows.Forms.TextBox textCustomer;
		private System.Windows.Forms.Label label2;
		private UI.Button butSearchAndAttach;
		private OpenDental.UI.TabControl tabControlMain;
		private OpenDental.UI.TabPage tabPageMessages;
		private UI.Button butSend;
		private ODtextBox textChatMessage;
		private SmsThreadView webChatThread;
		private OpenDental.UI.TabPage tabPageNotes;
		private UI.Button butAttachSuggestion;
		private UI.GridOD gridODNotes;
		private UI.Button butAddNote;
		private UI.Button butAskAi;
	}
}