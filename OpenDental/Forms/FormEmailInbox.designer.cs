namespace OpenDental{
	partial class FormEmailInbox {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEmailInbox));
			this.gridInbox = new OpenDental.UI.GridOD();
			this.emailPreviewControlInbox = new OpenDental.EmailPreviewControl();
			this.imageListButtonSmall = new System.Windows.Forms.ImageList(this.components);
			this.imageListMailboxesLarge = new System.Windows.Forms.ImageList(this.components);
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.groupShowIn = new OpenDental.UI.GroupBox();
			this.listShowIn = new OpenDental.UI.ListBox();
			this.checkShowHiddenEmails = new OpenDental.UI.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboEmailAddress = new OpenDental.UI.ComboBox();
			this.butMarkUnread = new OpenDental.UI.Button();
			this.butMarkRead = new OpenDental.UI.Button();
			this.butChangePat = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCompose = new OpenDental.UI.Button();
			this.butReply = new OpenDental.UI.Button();
			this.groupSearch = new OpenDental.UI.GroupBox();
			this.textDateTo = new OpenDental.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.butClear = new OpenDental.UI.Button();
			this.butSearch = new OpenDental.UI.Button();
			this.checkSearchAttach = new OpenDental.UI.CheckBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textSearchBody = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textSearchEmail = new System.Windows.Forms.TextBox();
			this.textDateFrom = new OpenDental.ValidDate();
			this.label4 = new System.Windows.Forms.Label();
			this.butPickPat = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textSearchPat = new System.Windows.Forms.TextBox();
			this.butRefresh = new OpenDental.UI.Button();
			this.gridSent = new OpenDental.UI.GridOD();
			this.emailPreviewControlSent = new OpenDental.EmailPreviewControl();
			this.butReplyAll = new OpenDental.UI.Button();
			this.butForward = new OpenDental.UI.Button();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.groupSentMessageSource = new OpenDental.UI.GroupBox();
			this.butAutomatedNone = new OpenDental.UI.Button();
			this.butManualNone = new OpenDental.UI.Button();
			this.labelAutomatedMsgSource = new System.Windows.Forms.Label();
			this.labelManualMsgSource = new System.Windows.Forms.Label();
			this.butAutomatedAll = new OpenDental.UI.Button();
			this.butAllManual = new OpenDental.UI.Button();
			this.listAutomatedMessageSource = new OpenDental.UI.ListBox();
			this.listManualMessageSource = new OpenDental.UI.ListBox();
			this.checkShowFailedSent = new OpenDental.UI.CheckBox();
			this.tabControl = new OpenDental.UI.TabControl();
			this.tabPage3 = new OpenDental.UI.TabPage();
			this.splitContainerInbox = new OpenDental.UI.SplitContainer();
			this.splitterPanel1 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel2 = new OpenDental.UI.SplitterPanel();
			this.tabPage4 = new OpenDental.UI.TabPage();
			this.splitContainerSent = new OpenDental.UI.SplitContainer();
			this.splitterPanel3 = new OpenDental.UI.SplitterPanel();
			this.splitterPanel4 = new OpenDental.UI.SplitterPanel();
			this.groupShowIn.SuspendLayout();
			this.groupSearch.SuspendLayout();
			this.groupSentMessageSource.SuspendLayout();
			this.tabControl.SuspendLayout();
			this.tabPage3.SuspendLayout();
			this.splitContainerInbox.SuspendLayout();
			this.splitterPanel1.SuspendLayout();
			this.splitterPanel2.SuspendLayout();
			this.tabPage4.SuspendLayout();
			this.splitContainerSent.SuspendLayout();
			this.splitterPanel3.SuspendLayout();
			this.splitterPanel4.SuspendLayout();
			this.SuspendLayout();
			// 
			// gridInbox
			// 
			this.gridInbox.AllowSortingByColumn = true;
			this.gridInbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridInbox.Location = new System.Drawing.Point(0, 0);
			this.gridInbox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.gridInbox.Name = "gridInbox";
			this.gridInbox.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridInbox.Size = new System.Drawing.Size(1126, 262);
			this.gridInbox.TabIndex = 140;
			this.gridInbox.Title = "Inbox";
			this.gridInbox.TranslationName = "TableApptProcs";
			this.gridInbox.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInboxSent_CellDoubleClick);
			this.gridInbox.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInboxSent_CellClick);
			// 
			// emailPreviewControlInbox
			// 
			this.emailPreviewControlInbox.BackColor = System.Drawing.SystemColors.Control;
			this.emailPreviewControlInbox.BccAddress = "";
			this.emailPreviewControlInbox.BodyText = "";
			this.emailPreviewControlInbox.CcAddress = "";
			this.emailPreviewControlInbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.emailPreviewControlInbox.IsPreview = true;
			this.emailPreviewControlInbox.Location = new System.Drawing.Point(0, 0);
			this.emailPreviewControlInbox.Name = "emailPreviewControlInbox";
			this.emailPreviewControlInbox.Size = new System.Drawing.Size(1126, 262);
			this.emailPreviewControlInbox.Subject = "";
			this.emailPreviewControlInbox.TabIndex = 0;
			this.emailPreviewControlInbox.ToAddress = "";
			// 
			// imageListButtonSmall
			// 
			this.imageListButtonSmall.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListButtonSmall.ImageStream")));
			this.imageListButtonSmall.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListButtonSmall.Images.SetKeyName(0, "Email_Compose.png");
			this.imageListButtonSmall.Images.SetKeyName(1, "Email_Refresh.png");
			this.imageListButtonSmall.Images.SetKeyName(2, "Email_Reply.png");
			this.imageListButtonSmall.Images.SetKeyName(3, "Email_Search.png");
			this.imageListButtonSmall.Images.SetKeyName(4, "Email_ReplyAll.png");
			this.imageListButtonSmall.Images.SetKeyName(5, "Email_Forward.png");
			// 
			// imageListMailboxesLarge
			// 
			this.imageListMailboxesLarge.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMailboxesLarge.ImageStream")));
			this.imageListMailboxesLarge.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMailboxesLarge.Images.SetKeyName(0, "Email_Inbox.png");
			this.imageListMailboxesLarge.Images.SetKeyName(1, "Email_SentMsgs.png");
			// 
			// groupShowIn
			// 
			this.groupShowIn.Controls.Add(this.listShowIn);
			this.groupShowIn.Controls.Add(this.checkShowHiddenEmails);
			this.groupShowIn.Location = new System.Drawing.Point(674, 27);
			this.groupShowIn.Name = "groupShowIn";
			this.groupShowIn.Size = new System.Drawing.Size(202, 156);
			this.groupShowIn.TabIndex = 171;
			this.groupShowIn.Text = "Show Email In";
			// 
			// listShowIn
			// 
			this.listShowIn.Location = new System.Drawing.Point(6, 37);
			this.listShowIn.Name = "listShowIn";
			this.listShowIn.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listShowIn.Size = new System.Drawing.Size(183, 69);
			this.listShowIn.TabIndex = 168;
			this.listShowIn.MouseClick += new System.Windows.Forms.MouseEventHandler(this.listHideInFlags_MouseClick);
			// 
			// checkShowHiddenEmails
			// 
			this.checkShowHiddenEmails.Location = new System.Drawing.Point(6, 129);
			this.checkShowHiddenEmails.Name = "checkShowHiddenEmails";
			this.checkShowHiddenEmails.Size = new System.Drawing.Size(154, 20);
			this.checkShowHiddenEmails.TabIndex = 167;
			this.checkShowHiddenEmails.Text = "Show Hidden Emails";
			this.checkShowHiddenEmails.CheckedChanged += new System.EventHandler(this.checkShowHiddenEmails_CheckedChanged);
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label3.Location = new System.Drawing.Point(12, 56);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(229, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "View messages for address:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboEmailAddress
			// 
			this.comboEmailAddress.Location = new System.Drawing.Point(15, 73);
			this.comboEmailAddress.Name = "comboEmailAddress";
			this.comboEmailAddress.Size = new System.Drawing.Size(233, 21);
			this.comboEmailAddress.TabIndex = 1;
			this.comboEmailAddress.SelectionChangeCommitted += new System.EventHandler(this.comboEmailAddress_SelectionChangeCommitted);
			// 
			// butMarkUnread
			// 
			this.butMarkUnread.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMarkUnread.Location = new System.Drawing.Point(851, 189);
			this.butMarkUnread.Name = "butMarkUnread";
			this.butMarkUnread.Size = new System.Drawing.Size(75, 20);
			this.butMarkUnread.TabIndex = 8;
			this.butMarkUnread.Text = "Mark Unread";
			this.butMarkUnread.Click += new System.EventHandler(this.butMarkUnread_Click);
			// 
			// butMarkRead
			// 
			this.butMarkRead.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butMarkRead.Location = new System.Drawing.Point(770, 189);
			this.butMarkRead.Name = "butMarkRead";
			this.butMarkRead.Size = new System.Drawing.Size(75, 20);
			this.butMarkRead.TabIndex = 7;
			this.butMarkRead.Text = "Mark Read";
			this.butMarkRead.Click += new System.EventHandler(this.butMarkRead_Click);
			// 
			// butChangePat
			// 
			this.butChangePat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangePat.Location = new System.Drawing.Point(689, 189);
			this.butChangePat.Name = "butChangePat";
			this.butChangePat.Size = new System.Drawing.Size(75, 20);
			this.butChangePat.TabIndex = 6;
			this.butChangePat.Text = "Change Pat";
			this.butChangePat.Click += new System.EventHandler(this.butChangePat_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 767);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 9;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCompose
			// 
			this.butCompose.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCompose.ImageKey = "Email_Compose.png";
			this.butCompose.ImageList = this.imageListButtonSmall;
			this.butCompose.Location = new System.Drawing.Point(131, 189);
			this.butCompose.Name = "butCompose";
			this.butCompose.Size = new System.Drawing.Size(75, 20);
			this.butCompose.TabIndex = 4;
			this.butCompose.Text = "Compose";
			this.butCompose.Click += new System.EventHandler(this.butCompose_Click);
			// 
			// butReply
			// 
			this.butReply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReply.ImageKey = "Email_Reply.png";
			this.butReply.ImageList = this.imageListButtonSmall;
			this.butReply.Location = new System.Drawing.Point(212, 189);
			this.butReply.Name = "butReply";
			this.butReply.Size = new System.Drawing.Size(75, 20);
			this.butReply.TabIndex = 5;
			this.butReply.Text = "Reply";
			this.butReply.Click += new System.EventHandler(this.butReply_Click);
			// 
			// groupSearch
			// 
			this.groupSearch.Controls.Add(this.textDateTo);
			this.groupSearch.Controls.Add(this.label2);
			this.groupSearch.Controls.Add(this.butClear);
			this.groupSearch.Controls.Add(this.butSearch);
			this.groupSearch.Controls.Add(this.checkSearchAttach);
			this.groupSearch.Controls.Add(this.label6);
			this.groupSearch.Controls.Add(this.textSearchBody);
			this.groupSearch.Controls.Add(this.label5);
			this.groupSearch.Controls.Add(this.textSearchEmail);
			this.groupSearch.Controls.Add(this.textDateFrom);
			this.groupSearch.Controls.Add(this.label4);
			this.groupSearch.Controls.Add(this.butPickPat);
			this.groupSearch.Controls.Add(this.label1);
			this.groupSearch.Controls.Add(this.textSearchPat);
			this.groupSearch.Location = new System.Drawing.Point(253, 27);
			this.groupSearch.Name = "groupSearch";
			this.groupSearch.Size = new System.Drawing.Size(412, 156);
			this.groupSearch.TabIndex = 2;
			this.groupSearch.Text = "Search";
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(80, 67);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(70, 20);
			this.textDateTo.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label2.Location = new System.Drawing.Point(80, 50);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(80, 16);
			this.label2.TabIndex = 166;
			this.label2.Text = "Date To:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butClear
			// 
			this.butClear.Enabled = false;
			this.butClear.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butClear.Location = new System.Drawing.Point(359, 92);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(45, 20);
			this.butClear.TabIndex = 8;
			this.butClear.Text = "Clear";
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// butSearch
			// 
			this.butSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSearch.ImageKey = "Email_Search.png";
			this.butSearch.ImageList = this.imageListButtonSmall;
			this.butSearch.Location = new System.Drawing.Point(283, 92);
			this.butSearch.Name = "butSearch";
			this.butSearch.Size = new System.Drawing.Size(73, 20);
			this.butSearch.TabIndex = 7;
			this.butSearch.Text = "Search";
			this.butSearch.Click += new System.EventHandler(this.butSearch_Click);
			// 
			// checkSearchAttach
			// 
			this.checkSearchAttach.Location = new System.Drawing.Point(6, 92);
			this.checkSearchAttach.Name = "checkSearchAttach";
			this.checkSearchAttach.Size = new System.Drawing.Size(258, 20);
			this.checkSearchAttach.TabIndex = 3;
			this.checkSearchAttach.Text = "Only include messages with attachments";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(181, 50);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(147, 16);
			this.label6.TabIndex = 163;
			this.label6.Text = "Subject/Body:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSearchBody
			// 
			this.textSearchBody.Location = new System.Drawing.Point(181, 67);
			this.textSearchBody.Name = "textSearchBody";
			this.textSearchBody.Size = new System.Drawing.Size(223, 20);
			this.textSearchBody.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(181, 12);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(173, 16);
			this.label5.TabIndex = 161;
			this.label5.Text = "Email Address:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSearchEmail
			// 
			this.textSearchEmail.Location = new System.Drawing.Point(181, 29);
			this.textSearchEmail.Name = "textSearchEmail";
			this.textSearchEmail.Size = new System.Drawing.Size(223, 20);
			this.textSearchEmail.TabIndex = 2;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(6, 67);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(70, 20);
			this.textDateFrom.TabIndex = 4;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(6, 50);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(71, 16);
			this.label4.TabIndex = 158;
			this.label4.Text = "Date From:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butPickPat
			// 
			this.butPickPat.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPickPat.Location = new System.Drawing.Point(151, 29);
			this.butPickPat.Name = "butPickPat";
			this.butPickPat.Size = new System.Drawing.Size(20, 20);
			this.butPickPat.TabIndex = 1;
			this.butPickPat.Text = "...";
			this.butPickPat.Click += new System.EventHandler(this.butPickPat_Click);
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label1.Location = new System.Drawing.Point(1, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(173, 16);
			this.label1.TabIndex = 156;
			this.label1.Text = "Patient:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textSearchPat
			// 
			this.textSearchPat.Location = new System.Drawing.Point(6, 29);
			this.textSearchPat.Name = "textSearchPat";
			this.textSearchPat.ReadOnly = true;
			this.textSearchPat.Size = new System.Drawing.Size(144, 20);
			this.textSearchPat.TabIndex = 0;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butRefresh.ImageKey = "Email_Refresh.png";
			this.butRefresh.ImageList = this.imageListButtonSmall;
			this.butRefresh.Location = new System.Drawing.Point(1138, 189);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 20);
			this.butRefresh.TabIndex = 3;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// gridSent
			// 
			this.gridSent.AllowSortingByColumn = true;
			this.gridSent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridSent.Location = new System.Drawing.Point(0, 0);
			this.gridSent.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
			this.gridSent.Name = "gridSent";
			this.gridSent.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridSent.Size = new System.Drawing.Size(1126, 262);
			this.gridSent.TabIndex = 141;
			this.gridSent.Title = "Sent Messages";
			this.gridSent.TranslationName = "TableApptProcs";
			this.gridSent.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInboxSent_CellDoubleClick);
			this.gridSent.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInboxSent_CellClick);
			// 
			// emailPreviewControlSent
			// 
			this.emailPreviewControlSent.BackColor = System.Drawing.SystemColors.Control;
			this.emailPreviewControlSent.BccAddress = "";
			this.emailPreviewControlSent.BodyText = "";
			this.emailPreviewControlSent.CcAddress = "";
			this.emailPreviewControlSent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.emailPreviewControlSent.IsPreview = true;
			this.emailPreviewControlSent.Location = new System.Drawing.Point(0, 0);
			this.emailPreviewControlSent.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
			this.emailPreviewControlSent.Name = "emailPreviewControlSent";
			this.emailPreviewControlSent.Size = new System.Drawing.Size(1126, 262);
			this.emailPreviewControlSent.Subject = "";
			this.emailPreviewControlSent.TabIndex = 0;
			this.emailPreviewControlSent.ToAddress = "";
			// 
			// butReplyAll
			// 
			this.butReplyAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butReplyAll.ImageKey = "Email_ReplyAll.png";
			this.butReplyAll.ImageList = this.imageListButtonSmall;
			this.butReplyAll.Location = new System.Drawing.Point(292, 189);
			this.butReplyAll.Name = "butReplyAll";
			this.butReplyAll.Size = new System.Drawing.Size(75, 20);
			this.butReplyAll.TabIndex = 11;
			this.butReplyAll.Text = "Reply All";
			this.butReplyAll.Click += new System.EventHandler(this.butReplyAll_Click);
			// 
			// butForward
			// 
			this.butForward.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butForward.ImageKey = "Email_Forward.png";
			this.butForward.ImageList = this.imageListButtonSmall;
			this.butForward.Location = new System.Drawing.Point(372, 189);
			this.butForward.Name = "butForward";
			this.butForward.Size = new System.Drawing.Size(75, 20);
			this.butForward.TabIndex = 12;
			this.butForward.Text = "Forward";
			this.butForward.Click += new System.EventHandler(this.butForward_Click);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1230, 24);
			this.menuMain.TabIndex = 172;
			// 
			// groupSentMessageSource
			// 
			this.groupSentMessageSource.Controls.Add(this.butAutomatedNone);
			this.groupSentMessageSource.Controls.Add(this.butManualNone);
			this.groupSentMessageSource.Controls.Add(this.labelAutomatedMsgSource);
			this.groupSentMessageSource.Controls.Add(this.labelManualMsgSource);
			this.groupSentMessageSource.Controls.Add(this.butAutomatedAll);
			this.groupSentMessageSource.Controls.Add(this.butAllManual);
			this.groupSentMessageSource.Controls.Add(this.listAutomatedMessageSource);
			this.groupSentMessageSource.Controls.Add(this.listManualMessageSource);
			this.groupSentMessageSource.Controls.Add(this.checkShowFailedSent);
			this.groupSentMessageSource.Location = new System.Drawing.Point(881, 27);
			this.groupSentMessageSource.Name = "groupSentMessageSource";
			this.groupSentMessageSource.Size = new System.Drawing.Size(344, 156);
			this.groupSentMessageSource.TabIndex = 175;
			this.groupSentMessageSource.Text = "Sent Message Sources";
			// 
			// butAutomatedNone
			// 
			this.butAutomatedNone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAutomatedNone.Location = new System.Drawing.Point(264, 108);
			this.butAutomatedNone.Name = "butAutomatedNone";
			this.butAutomatedNone.Size = new System.Drawing.Size(38, 20);
			this.butAutomatedNone.TabIndex = 175;
			this.butAutomatedNone.Text = "None";
			this.butAutomatedNone.Click += new System.EventHandler(this.butAutomatedNone_Click);
			// 
			// butManualNone
			// 
			this.butManualNone.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butManualNone.Location = new System.Drawing.Point(94, 108);
			this.butManualNone.Name = "butManualNone";
			this.butManualNone.Size = new System.Drawing.Size(38, 20);
			this.butManualNone.TabIndex = 174;
			this.butManualNone.Text = "None";
			this.butManualNone.Click += new System.EventHandler(this.butManualNone_Click);
			// 
			// labelAutomatedMsgSource
			// 
			this.labelAutomatedMsgSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelAutomatedMsgSource.Location = new System.Drawing.Point(173, 18);
			this.labelAutomatedMsgSource.Name = "labelAutomatedMsgSource";
			this.labelAutomatedMsgSource.Size = new System.Drawing.Size(150, 16);
			this.labelAutomatedMsgSource.TabIndex = 173;
			this.labelAutomatedMsgSource.Text = "Automated:";
			this.labelAutomatedMsgSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelManualMsgSource
			// 
			this.labelManualMsgSource.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelManualMsgSource.Location = new System.Drawing.Point(6, 18);
			this.labelManualMsgSource.Name = "labelManualMsgSource";
			this.labelManualMsgSource.Size = new System.Drawing.Size(150, 16);
			this.labelManualMsgSource.TabIndex = 172;
			this.labelManualMsgSource.Text = "Manual:";
			this.labelManualMsgSource.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butAutomatedAll
			// 
			this.butAutomatedAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAutomatedAll.Location = new System.Drawing.Point(307, 108);
			this.butAutomatedAll.Name = "butAutomatedAll";
			this.butAutomatedAll.Size = new System.Drawing.Size(30, 20);
			this.butAutomatedAll.TabIndex = 171;
			this.butAutomatedAll.Text = "All";
			this.butAutomatedAll.Click += new System.EventHandler(this.butAutomatedAll_Click);
			// 
			// butAllManual
			// 
			this.butAllManual.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAllManual.Location = new System.Drawing.Point(137, 108);
			this.butAllManual.Name = "butAllManual";
			this.butAllManual.Size = new System.Drawing.Size(30, 20);
			this.butAllManual.TabIndex = 170;
			this.butAllManual.Text = "All";
			this.butAllManual.Click += new System.EventHandler(this.butAllManual_Click);
			// 
			// listAutomatedMessageSource
			// 
			this.listAutomatedMessageSource.Location = new System.Drawing.Point(176, 37);
			this.listAutomatedMessageSource.Name = "listAutomatedMessageSource";
			this.listAutomatedMessageSource.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listAutomatedMessageSource.Size = new System.Drawing.Size(161, 69);
			this.listAutomatedMessageSource.TabIndex = 169;
			this.listAutomatedMessageSource.MouseCaptureChanged += new System.EventHandler(this.listAutomatedMessageSource_MouseCaptureChanged);
			// 
			// listManualMessageSource
			// 
			this.listManualMessageSource.Location = new System.Drawing.Point(6, 37);
			this.listManualMessageSource.Name = "listManualMessageSource";
			this.listManualMessageSource.SelectionMode = OpenDental.UI.SelectionMode.MultiExtended;
			this.listManualMessageSource.Size = new System.Drawing.Size(161, 69);
			this.listManualMessageSource.TabIndex = 168;
			this.listManualMessageSource.MouseCaptureChanged += new System.EventHandler(this.listMessageSource_MouseCaptureChanged);
			// 
			// checkShowFailedSent
			// 
			this.checkShowFailedSent.Location = new System.Drawing.Point(6, 129);
			this.checkShowFailedSent.Name = "checkShowFailedSent";
			this.checkShowFailedSent.Size = new System.Drawing.Size(154, 20);
			this.checkShowFailedSent.TabIndex = 167;
			this.checkShowFailedSent.Text = "Show Failed Emails";
			this.checkShowFailedSent.CheckedChanged += new System.EventHandler(this.checkShowFailedSent_CheckedChanged);
			// 
			// tabControl
			// 
			this.tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.tabControl.Controls.Add(this.tabPage3);
			this.tabControl.Controls.Add(this.tabPage4);
			this.tabControl.Location = new System.Drawing.Point(15, 212);
			this.tabControl.Name = "tabControl";
			this.tabControl.Size = new System.Drawing.Size(1205, 538);
			this.tabControl.TabAlignment = OpenDental.UI.EnumTabAlignment.LeftHorizontal;
			this.tabControl.TabIndex = 176;
			this.tabControl.SelectedIndexChanged += new System.EventHandler(this.tabControl_SelectedIndexChanged);
			// 
			// tabPage3
			// 
			this.tabPage3.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage3.Controls.Add(this.splitContainerInbox);
			this.tabPage3.Location = new System.Drawing.Point(71, 2);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage3.Size = new System.Drawing.Size(1132, 534);
			this.tabPage3.TabIndex = 0;
			this.tabPage3.Text = "Inbox";
			// 
			// splitContainerInbox
			// 
			this.splitContainerInbox.Controls.Add(this.splitterPanel1);
			this.splitContainerInbox.Controls.Add(this.splitterPanel2);
			this.splitContainerInbox.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainerInbox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerInbox.Location = new System.Drawing.Point(3, 3);
			this.splitContainerInbox.Name = "splitContainerInbox";
			this.splitContainerInbox.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainerInbox.Panel1 = this.splitterPanel1;
			this.splitContainerInbox.Panel2 = this.splitterPanel2;
			this.splitContainerInbox.Size = new System.Drawing.Size(1126, 528);
			this.splitContainerInbox.SplitterDistance = 262;
			this.splitContainerInbox.TabIndex = 12;
			// 
			// splitterPanel1
			// 
			this.splitterPanel1.Controls.Add(this.gridInbox);
			this.splitterPanel1.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel1.Name = "splitterPanel1";
			this.splitterPanel1.Size = new System.Drawing.Size(1126, 262);
			this.splitterPanel1.TabIndex = 13;
			// 
			// splitterPanel2
			// 
			this.splitterPanel2.Controls.Add(this.emailPreviewControlInbox);
			this.splitterPanel2.Location = new System.Drawing.Point(0, 266);
			this.splitterPanel2.Name = "splitterPanel2";
			this.splitterPanel2.Size = new System.Drawing.Size(1126, 262);
			this.splitterPanel2.TabIndex = 14;
			// 
			// tabPage4
			// 
			this.tabPage4.BackColor = System.Drawing.SystemColors.Window;
			this.tabPage4.Controls.Add(this.splitContainerSent);
			this.tabPage4.Location = new System.Drawing.Point(71, 2);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage4.Size = new System.Drawing.Size(1132, 534);
			this.tabPage4.TabIndex = 1;
			this.tabPage4.Text = "Sent";
			// 
			// splitContainerSent
			// 
			this.splitContainerSent.Controls.Add(this.splitterPanel3);
			this.splitContainerSent.Controls.Add(this.splitterPanel4);
			this.splitContainerSent.Cursor = System.Windows.Forms.Cursors.Default;
			this.splitContainerSent.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerSent.Location = new System.Drawing.Point(3, 3);
			this.splitContainerSent.Name = "splitContainerSent";
			this.splitContainerSent.Orientation = System.Windows.Forms.Orientation.Horizontal;
			this.splitContainerSent.Panel1 = this.splitterPanel3;
			this.splitContainerSent.Panel2 = this.splitterPanel4;
			this.splitContainerSent.Size = new System.Drawing.Size(1126, 528);
			this.splitContainerSent.SplitterDistance = 262;
			this.splitContainerSent.TabIndex = 12;
			// 
			// splitterPanel3
			// 
			this.splitterPanel3.Controls.Add(this.gridSent);
			this.splitterPanel3.Location = new System.Drawing.Point(0, 0);
			this.splitterPanel3.Name = "splitterPanel3";
			this.splitterPanel3.Size = new System.Drawing.Size(1126, 262);
			this.splitterPanel3.TabIndex = 13;
			// 
			// splitterPanel4
			// 
			this.splitterPanel4.Controls.Add(this.emailPreviewControlSent);
			this.splitterPanel4.Location = new System.Drawing.Point(0, 266);
			this.splitterPanel4.Name = "splitterPanel4";
			this.splitterPanel4.Size = new System.Drawing.Size(1126, 262);
			this.splitterPanel4.TabIndex = 14;
			// 
			// FormEmailInbox
			// 
			this.ClientSize = new System.Drawing.Size(1230, 795);
			this.Controls.Add(this.tabControl);
			this.Controls.Add(this.groupSentMessageSource);
			this.Controls.Add(this.groupShowIn);
			this.Controls.Add(this.butForward);
			this.Controls.Add(this.butReplyAll);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboEmailAddress);
			this.Controls.Add(this.butMarkUnread);
			this.Controls.Add(this.butMarkRead);
			this.Controls.Add(this.butChangePat);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butCompose);
			this.Controls.Add(this.butReply);
			this.Controls.Add(this.groupSearch);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEmailInbox";
			this.Text = "Email Client";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormEmailInbox_FormClosing);
			this.Load += new System.EventHandler(this.FormEmailInbox_Load);
			this.Resize += new System.EventHandler(this.FormEmailInbox_Resize);
			this.groupShowIn.ResumeLayout(false);
			this.groupSearch.ResumeLayout(false);
			this.groupSearch.PerformLayout();
			this.groupSentMessageSource.ResumeLayout(false);
			this.tabControl.ResumeLayout(false);
			this.tabPage3.ResumeLayout(false);
			this.splitContainerInbox.ResumeLayout(false);
			this.splitterPanel1.ResumeLayout(false);
			this.splitterPanel2.ResumeLayout(false);
			this.tabPage4.ResumeLayout(false);
			this.splitContainerSent.ResumeLayout(false);
			this.splitterPanel3.ResumeLayout(false);
			this.splitterPanel4.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
    private System.Windows.Forms.Label label3;
    private OpenDental.UI.ComboBox comboEmailAddress;
    private UI.Button butMarkUnread;
    private UI.Button butMarkRead;
    private UI.Button butChangePat;
    private UI.Button butDelete;
    private UI.Button butCompose;
    private System.Windows.Forms.ImageList imageListButtonSmall;
    private UI.Button butReply;
    private OpenDental.UI.GroupBox groupSearch;
    private UI.Button butClear;
    private UI.Button butSearch;
    private OpenDental.UI.CheckBox checkSearchAttach;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.TextBox textSearchBody;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.TextBox textSearchEmail;
    private ValidDate textDateFrom;
    private System.Windows.Forms.Label label4;
    private UI.Button butPickPat;
    private System.Windows.Forms.Label label1;
    private System.Windows.Forms.TextBox textSearchPat;
    private UI.Button butRefresh;
    private UI.GridOD gridInbox;
    private EmailPreviewControl emailPreviewControlInbox;
    private UI.GridOD gridSent;
    private EmailPreviewControl emailPreviewControlSent;
    private System.Windows.Forms.ImageList imageListMailboxesLarge;
    private System.Windows.Forms.ToolTip toolTip1;
    private ValidDate textDateTo;
    private System.Windows.Forms.Label label2;
		private UI.Button butReplyAll;
		private UI.Button butForward;
		private OpenDental.UI.CheckBox checkShowHiddenEmails;
		private UI.ListBox listShowIn;
		private OpenDental.UI.GroupBox groupShowIn;
		private UI.MenuOD menuMain;
		private OpenDental.UI.GroupBox groupSentMessageSource;
		private UI.ListBox listManualMessageSource;
		private OpenDental.UI.CheckBox checkShowFailedSent;
		private UI.ListBox listAutomatedMessageSource;
		private UI.Button butAllManual;
		private UI.Button butAutomatedAll;
		private System.Windows.Forms.Label labelManualMsgSource;
		private System.Windows.Forms.Label labelAutomatedMsgSource;
		private UI.Button butAutomatedNone;
		private UI.Button butManualNone;
		private UI.TabControl tabControl;
		private UI.TabPage tabPage3;
		private UI.TabPage tabPage4;
		private UI.SplitContainer splitContainerInbox;
		private UI.SplitterPanel splitterPanel1;
		private UI.SplitterPanel splitterPanel2;
		private UI.SplitContainer splitContainerSent;
		private UI.SplitterPanel splitterPanel3;
		private UI.SplitterPanel splitterPanel4;
	}
}