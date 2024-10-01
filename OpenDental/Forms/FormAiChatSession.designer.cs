namespace OpenDental{
	partial class FormAiChatSession {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormAiChatSession));
			this.butTakeOwnership = new OpenDental.UI.Button();
			this.butEndSession = new OpenDental.UI.Button();
			this.labelOwner = new System.Windows.Forms.Label();
			this.textOwner = new System.Windows.Forms.TextBox();
			this.tabControlMain = new OpenDental.UI.TabControl();
			this.tabPageMessages = new OpenDental.UI.TabPage();
			this.butSend = new OpenDental.UI.Button();
			this.textChatMessage = new OpenDental.ODtextBox();
			this.webChatThread = new OpenDental.SmsThreadView();
			this.tabPageNotes = new OpenDental.UI.TabPage();
			this.gridODNotes = new OpenDental.UI.GridOD();
			this.label2 = new System.Windows.Forms.Label();
			this.comboVersions = new OpenDental.UI.ComboBox();
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
			// tabControlMain
			// 
			this.tabControlMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControlMain.Controls.Add(this.tabPageMessages);
			this.tabControlMain.Controls.Add(this.tabPageNotes);
			this.tabControlMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
			this.tabControlMain.Location = new System.Drawing.Point(4, 30);
			this.tabControlMain.Name = "tabControlMain";
			this.tabControlMain.Size = new System.Drawing.Size(548, 503);
			this.tabControlMain.TabIndex = 28;
			this.tabControlMain.SelectedIndexChanged += new System.EventHandler(this.tabControlMain_SelectedIndexChanged);
			// 
			// tabPageMessages
			// 
			this.tabPageMessages.Controls.Add(this.butSend);
			this.tabPageMessages.Controls.Add(this.textChatMessage);
			this.tabPageMessages.Controls.Add(this.webChatThread);
			this.tabPageMessages.Location = new System.Drawing.Point(2, 21);
			this.tabPageMessages.Name = "tabPageMessages";
			this.tabPageMessages.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageMessages.Size = new System.Drawing.Size(544, 480);
			this.tabPageMessages.TabIndex = 0;
			this.tabPageMessages.Text = "Messages";
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Location = new System.Drawing.Point(464, 427);
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
			this.textChatMessage.Location = new System.Drawing.Point(2, 427);
			this.textChatMessage.Name = "textChatMessage";
			this.textChatMessage.PlaceholderText = "Enter your message here...";
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
			this.webChatThread.Size = new System.Drawing.Size(540, 422);
			this.webChatThread.TabIndex = 6;
			// 
			// tabPageNotes
			// 
			this.tabPageNotes.Controls.Add(this.gridODNotes);
			this.tabPageNotes.Location = new System.Drawing.Point(2, 21);
			this.tabPageNotes.Name = "tabPageNotes";
			this.tabPageNotes.Padding = new System.Windows.Forms.Padding(3);
			this.tabPageNotes.Size = new System.Drawing.Size(544, 480);
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
			this.gridODNotes.Size = new System.Drawing.Size(541, 478);
			this.gridODNotes.TabIndex = 0;
			this.gridODNotes.Title = "Notes";
			this.gridODNotes.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridODNotes_CellDoubleClick);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(225, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(96, 20);
			this.label2.TabIndex = 152;
			this.label2.Text = "Version";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboVersions
			// 
			this.comboVersions.Location = new System.Drawing.Point(322, 8);
			this.comboVersions.Name = "comboVersions";
			this.comboVersions.Size = new System.Drawing.Size(150, 21);
			this.comboVersions.TabIndex = 152;
			this.comboVersions.Text = "comboBox1";
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
			// FormAiChatSession
			// 
			this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(253)))), ((int)(((byte)(254)))));
			this.ClientSize = new System.Drawing.Size(555, 575);
			this.Controls.Add(this.butAddNote);
			this.Controls.Add(this.textOwner);
			this.Controls.Add(this.labelOwner);
			this.Controls.Add(this.butEndSession);
			this.Controls.Add(this.butTakeOwnership);
			this.Controls.Add(this.tabControlMain);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.comboVersions);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormAiChatSession";
			this.Text = "AI Chat Session";
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
		private OpenDental.UI.TabControl tabControlMain;
		private OpenDental.UI.TabPage tabPageMessages;
		private UI.Button butSend;
		private ODtextBox textChatMessage;
		private SmsThreadView webChatThread;
		private OpenDental.UI.TabPage tabPageNotes;
		private UI.GridOD gridODNotes;
		private UI.Button butAddNote;
		private System.Windows.Forms.Label label2;
		private UI.ComboBox comboVersions;
	}
}