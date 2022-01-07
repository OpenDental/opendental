namespace OpenDental{
	partial class FormSmsTextMessaging {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSmsTextMessaging));
			this.contextMenuMessages = new System.Windows.Forms.ContextMenu();
			this.menuItemChangePat = new System.Windows.Forms.MenuItem();
			this.menuItemMarkUnread = new System.Windows.Forms.MenuItem();
			this.menuItemMarkRead = new System.Windows.Forms.MenuItem();
			this.menuItemHide = new System.Windows.Forms.MenuItem();
			this.menuItemUnhide = new System.Windows.Forms.MenuItem();
			this.menuItemBlockNumber = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItemGoToPatient = new System.Windows.Forms.MenuItem();
			this.labelPatientsForPhone = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioGroupByPhone = new System.Windows.Forms.RadioButton();
			this.radioGroupByNone = new System.Windows.Forms.RadioButton();
			this.radioGroupByPatient = new System.Windows.Forms.RadioButton();
			this.gridMessages = new OpenDental.UI.GridOD();
			this.checkRead = new System.Windows.Forms.CheckBox();
			this.checkSent = new System.Windows.Forms.CheckBox();
			this.textPatient = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textDateTo = new ODR.ValidDate();
			this.label2 = new System.Windows.Forms.Label();
			this.textDateFrom = new ODR.ValidDate();
			this.butSend = new OpenDental.UI.Button();
			this.textReply = new OpenDental.ODtextBox();
			this.smsThreadView = new OpenDental.SmsThreadView();
			this.butPatCurrent = new OpenDental.UI.Button();
			this.butPatAll = new OpenDental.UI.Button();
			this.butPatFind = new OpenDental.UI.Button();
			this.butRefresh = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.comboClinics = new OpenDental.UI.ComboBoxClinicPicker();
			this.textMsgCount = new OpenDental.ODtextBox();
			this.textCharCount = new OpenDental.ODtextBox();
			this.labelMsgCount = new System.Windows.Forms.Label();
			this.labelCharCount = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// contextMenuMessages
			// 
			this.contextMenuMessages.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemChangePat,
            this.menuItemMarkUnread,
            this.menuItemMarkRead,
            this.menuItemHide,
            this.menuItemUnhide,
            this.menuItemBlockNumber,
            this.menuItem1,
            this.menuItemGoToPatient});
			// 
			// menuItemChangePat
			// 
			this.menuItemChangePat.Index = 0;
			this.menuItemChangePat.Text = "Change Pat";
			this.menuItemChangePat.Click += new System.EventHandler(this.menuItemChangePat_Click);
			// 
			// menuItemMarkUnread
			// 
			this.menuItemMarkUnread.Index = 1;
			this.menuItemMarkUnread.Text = "Mark Unread";
			this.menuItemMarkUnread.Click += new System.EventHandler(this.menuItemMarkUnread_Click);
			// 
			// menuItemMarkRead
			// 
			this.menuItemMarkRead.Index = 2;
			this.menuItemMarkRead.Text = "Mark Read";
			this.menuItemMarkRead.Click += new System.EventHandler(this.menuItemMarkRead_Click);
			// 
			// menuItemHide
			// 
			this.menuItemHide.Index = 3;
			this.menuItemHide.Text = "Hide";
			this.menuItemHide.Click += new System.EventHandler(this.menuItemHide_Click);
			// 
			// menuItemUnhide
			// 
			this.menuItemUnhide.Index = 4;
			this.menuItemUnhide.Text = "Unhide";
			this.menuItemUnhide.Click += new System.EventHandler(this.menuItemUnhide_Click);
			// 
			// menuItemBlockNumber
			// 
			this.menuItemBlockNumber.Index = 5;
			this.menuItemBlockNumber.Text = "Block Number";
			this.menuItemBlockNumber.Click += new System.EventHandler(this.menuItemBlockNumber_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 6;
			this.menuItem1.Text = "-";
			// 
			// menuItemGoToPatient
			// 
			this.menuItemGoToPatient.Index = 7;
			this.menuItemGoToPatient.Text = "Go to Patient";
			this.menuItemGoToPatient.Click += new System.EventHandler(this.menuItemSelectPatient_Click);
			// 
			// labelPatientsForPhone
			// 
			this.labelPatientsForPhone.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelPatientsForPhone.Location = new System.Drawing.Point(902, 0);
			this.labelPatientsForPhone.Name = "labelPatientsForPhone";
			this.labelPatientsForPhone.Size = new System.Drawing.Size(165, 57);
			this.labelPatientsForPhone.TabIndex = 168;
			this.labelPatientsForPhone.Text = "labelPatientsForPhone";
			this.labelPatientsForPhone.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			this.labelPatientsForPhone.Visible = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioGroupByPhone);
			this.groupBox1.Controls.Add(this.radioGroupByNone);
			this.groupBox1.Controls.Add(this.radioGroupByPatient);
			this.groupBox1.Location = new System.Drawing.Point(551, 0);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(264, 36);
			this.groupBox1.TabIndex = 167;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Group Messages By";
			// 
			// radioGroupByPhone
			// 
			this.radioGroupByPhone.Location = new System.Drawing.Point(83, 14);
			this.radioGroupByPhone.Name = "radioGroupByPhone";
			this.radioGroupByPhone.Size = new System.Drawing.Size(110, 17);
			this.radioGroupByPhone.TabIndex = 2;
			this.radioGroupByPhone.TabStop = true;
			this.radioGroupByPhone.Text = "Phone Number";
			this.radioGroupByPhone.UseVisualStyleBackColor = true;
			// 
			// radioGroupByNone
			// 
			this.radioGroupByNone.Checked = true;
			this.radioGroupByNone.Location = new System.Drawing.Point(199, 14);
			this.radioGroupByNone.Name = "radioGroupByNone";
			this.radioGroupByNone.Size = new System.Drawing.Size(64, 17);
			this.radioGroupByNone.TabIndex = 1;
			this.radioGroupByNone.TabStop = true;
			this.radioGroupByNone.Text = "None";
			this.radioGroupByNone.UseVisualStyleBackColor = true;
			// 
			// radioGroupByPatient
			// 
			this.radioGroupByPatient.Location = new System.Drawing.Point(6, 14);
			this.radioGroupByPatient.Name = "radioGroupByPatient";
			this.radioGroupByPatient.Size = new System.Drawing.Size(74, 17);
			this.radioGroupByPatient.TabIndex = 0;
			this.radioGroupByPatient.TabStop = true;
			this.radioGroupByPatient.Text = "Patient";
			this.radioGroupByPatient.UseVisualStyleBackColor = true;
			// 
			// gridMessages
			// 
			this.gridMessages.AllowSortingByColumn = true;
			this.gridMessages.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMessages.HasMultilineHeaders = true;
			this.gridMessages.HScrollVisible = true;
			this.gridMessages.Location = new System.Drawing.Point(12, 61);
			this.gridMessages.Name = "gridMessages";
			this.gridMessages.Size = new System.Drawing.Size(803, 593);
			this.gridMessages.TabIndex = 4;
			this.gridMessages.Title = "Text Messages - Right click for options - Unread messages always shown";
			this.gridMessages.TranslationName = "FormSmsTextMessaging";
			this.gridMessages.SelectionCommitted += new System.EventHandler(this.gridMessages_SelectionCommitted);
			// 
			// checkRead
			// 
			this.checkRead.Location = new System.Drawing.Point(462, 23);
			this.checkRead.Name = "checkRead";
			this.checkRead.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkRead.Size = new System.Drawing.Size(80, 16);
			this.checkRead.TabIndex = 162;
			this.checkRead.Text = "Received";
			this.checkRead.UseVisualStyleBackColor = true;
			// 
			// checkSent
			// 
			this.checkSent.Location = new System.Drawing.Point(462, 38);
			this.checkSent.Name = "checkSent";
			this.checkSent.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkSent.Size = new System.Drawing.Size(80, 16);
			this.checkSent.TabIndex = 161;
			this.checkSent.Text = "Sent";
			this.checkSent.UseVisualStyleBackColor = true;
			// 
			// textPatient
			// 
			this.textPatient.Location = new System.Drawing.Point(233, 10);
			this.textPatient.Name = "textPatient";
			this.textPatient.ReadOnly = true;
			this.textPatient.Size = new System.Drawing.Size(216, 20);
			this.textPatient.TabIndex = 156;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(165, 14);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 13);
			this.label1.TabIndex = 155;
			this.label1.Text = "Patient";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkHidden
			// 
			this.checkHidden.Location = new System.Drawing.Point(462, 8);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkHidden.Size = new System.Drawing.Size(80, 16);
			this.checkHidden.TabIndex = 153;
			this.checkHidden.Text = "Hidden";
			this.checkHidden.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(13, 31);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(66, 21);
			this.label3.TabIndex = 10;
			this.label3.Text = "Date To";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTo
			// 
			this.textDateTo.Location = new System.Drawing.Point(80, 31);
			this.textDateTo.Name = "textDateTo";
			this.textDateTo.Size = new System.Drawing.Size(81, 20);
			this.textDateTo.TabIndex = 9;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(10, 10);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(69, 21);
			this.label2.TabIndex = 8;
			this.label2.Text = "Date From";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateFrom
			// 
			this.textDateFrom.Location = new System.Drawing.Point(80, 10);
			this.textDateFrom.Name = "textDateFrom";
			this.textDateFrom.Size = new System.Drawing.Size(81, 20);
			this.textDateFrom.TabIndex = 7;
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSend.Enabled = false;
			this.butSend.Location = new System.Drawing.Point(1024, 556);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(43, 98);
			this.butSend.TabIndex = 166;
			this.butSend.Text = "Send";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// textReply
			// 
			this.textReply.AcceptsTab = true;
			this.textReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textReply.BackColor = System.Drawing.SystemColors.Window;
			this.textReply.DetectLinksEnabled = false;
			this.textReply.DetectUrls = false;
			this.textReply.Enabled = false;
			this.textReply.Location = new System.Drawing.Point(817, 556);
			this.textReply.Name = "textReply";
			this.textReply.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textReply.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textReply.Size = new System.Drawing.Size(201, 98);
			this.textReply.TabIndex = 165;
			this.textReply.Text = "";
			// 
			// smsThreadView
			// 
			this.smsThreadView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.smsThreadView.BackColor = System.Drawing.SystemColors.Control;
			this.smsThreadView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.smsThreadView.ListSmsThreadMessages = null;
			this.smsThreadView.Location = new System.Drawing.Point(817, 61);
			this.smsThreadView.Name = "smsThreadView";
			this.smsThreadView.Size = new System.Drawing.Size(250, 493);
			this.smsThreadView.TabIndex = 164;
			// 
			// butPatCurrent
			// 
			this.butPatCurrent.Location = new System.Drawing.Point(233, 31);
			this.butPatCurrent.Name = "butPatCurrent";
			this.butPatCurrent.Size = new System.Drawing.Size(63, 24);
			this.butPatCurrent.TabIndex = 159;
			this.butPatCurrent.Text = "Current";
			this.butPatCurrent.Click += new System.EventHandler(this.butPatCurrent_Click);
			// 
			// butPatAll
			// 
			this.butPatAll.Location = new System.Drawing.Point(386, 31);
			this.butPatAll.Name = "butPatAll";
			this.butPatAll.Size = new System.Drawing.Size(63, 24);
			this.butPatAll.TabIndex = 158;
			this.butPatAll.Text = "All";
			this.butPatAll.Click += new System.EventHandler(this.butPatAll_Click);
			// 
			// butPatFind
			// 
			this.butPatFind.Location = new System.Drawing.Point(309, 31);
			this.butPatFind.Name = "butPatFind";
			this.butPatFind.Size = new System.Drawing.Size(63, 24);
			this.butPatFind.TabIndex = 157;
			this.butPatFind.Text = "Find";
			this.butPatFind.Click += new System.EventHandler(this.butPatFind_Click);
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(823, 33);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 13;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(992, 660);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// comboClinics
			// 
			this.comboClinics.IncludeAll = true;
			this.comboClinics.IncludeUnassigned = true;
			this.comboClinics.Location = new System.Drawing.Point(586, 37);
			this.comboClinics.Name = "comboClinics";
			this.comboClinics.SelectionModeMulti = true;
			this.comboClinics.Size = new System.Drawing.Size(229, 21);
			this.comboClinics.TabIndex = 169;
			// 
			// textMsgCount
			// 
			this.textMsgCount.AcceptsTab = true;
			this.textMsgCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textMsgCount.BackColor = System.Drawing.SystemColors.Control;
			this.textMsgCount.DetectLinksEnabled = false;
			this.textMsgCount.DetectUrls = false;
			this.textMsgCount.Location = new System.Drawing.Point(817, 675);
			this.textMsgCount.Name = "textMsgCount";
			this.textMsgCount.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textMsgCount.ReadOnly = true;
			this.textMsgCount.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textMsgCount.Size = new System.Drawing.Size(53, 20);
			this.textMsgCount.TabIndex = 173;
			this.textMsgCount.TabStop = false;
			this.textMsgCount.Text = "0";
			// 
			// textCharCount
			// 
			this.textCharCount.AcceptsTab = true;
			this.textCharCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textCharCount.BackColor = System.Drawing.SystemColors.Control;
			this.textCharCount.DetectLinksEnabled = false;
			this.textCharCount.DetectUrls = false;
			this.textCharCount.Location = new System.Drawing.Point(817, 655);
			this.textCharCount.Name = "textCharCount";
			this.textCharCount.QuickPasteType = OpenDentBusiness.QuickPasteType.TxtMsg;
			this.textCharCount.ReadOnly = true;
			this.textCharCount.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textCharCount.Size = new System.Drawing.Size(53, 20);
			this.textCharCount.TabIndex = 172;
			this.textCharCount.TabStop = false;
			this.textCharCount.Text = "0";
			// 
			// labelMsgCount
			// 
			this.labelMsgCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMsgCount.Location = new System.Drawing.Point(871, 678);
			this.labelMsgCount.Name = "labelMsgCount";
			this.labelMsgCount.Size = new System.Drawing.Size(115, 13);
			this.labelMsgCount.TabIndex = 171;
			this.labelMsgCount.Text = "Message Count";
			// 
			// labelCharCount
			// 
			this.labelCharCount.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCharCount.Location = new System.Drawing.Point(871, 657);
			this.labelCharCount.Name = "labelCharCount";
			this.labelCharCount.Size = new System.Drawing.Size(115, 13);
			this.labelCharCount.TabIndex = 170;
			this.labelCharCount.Text = "Character Count";
			// 
			// FormSmsTextMessaging
			// 
			this.ClientSize = new System.Drawing.Size(1079, 696);
			this.Controls.Add(this.textMsgCount);
			this.Controls.Add(this.textCharCount);
			this.Controls.Add(this.labelMsgCount);
			this.Controls.Add(this.labelCharCount);
			this.Controls.Add(this.comboClinics);
			this.Controls.Add(this.labelPatientsForPhone);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.textReply);
			this.Controls.Add(this.smsThreadView);
			this.Controls.Add(this.gridMessages);
			this.Controls.Add(this.checkRead);
			this.Controls.Add(this.checkSent);
			this.Controls.Add(this.butPatCurrent);
			this.Controls.Add(this.butPatAll);
			this.Controls.Add(this.butPatFind);
			this.Controls.Add(this.textPatient);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textDateTo);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textDateFrom);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSmsTextMessaging";
			this.Text = "Text Messaging";
			this.Load += new System.EventHandler(this.FormSmsTextMessaging_Load);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridMessages;
		private ODR.ValidDate textDateFrom;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private ODR.ValidDate textDateTo;
		private UI.Button butRefresh;
		private System.Windows.Forms.CheckBox checkHidden;
		private System.Windows.Forms.ContextMenu contextMenuMessages;
		private System.Windows.Forms.MenuItem menuItemChangePat;
		private System.Windows.Forms.MenuItem menuItemMarkUnread;
		private System.Windows.Forms.MenuItem menuItemMarkRead;
		private System.Windows.Forms.MenuItem menuItemHide;
		private System.Windows.Forms.MenuItem menuItemUnhide;
		private UI.Button butPatCurrent;
		private UI.Button butPatAll;
		private UI.Button butPatFind;
		private System.Windows.Forms.TextBox textPatient;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkSent;
		private System.Windows.Forms.CheckBox checkRead;
		private SmsThreadView smsThreadView;
		private ODtextBox textReply;
		private UI.Button butSend;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItemGoToPatient;
		private System.Windows.Forms.MenuItem menuItemBlockNumber;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioGroupByPatient;
		private System.Windows.Forms.RadioButton radioGroupByNone;
		private System.Windows.Forms.RadioButton radioGroupByPhone;
		private System.Windows.Forms.Label labelPatientsForPhone;
		private UI.ComboBoxClinicPicker comboClinics;
		private ODtextBox textMsgCount;
		private ODtextBox textCharCount;
		private System.Windows.Forms.Label labelMsgCount;
		private System.Windows.Forms.Label labelCharCount;
	}
}