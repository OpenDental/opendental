using System.Windows.Forms;

namespace OpenDental{
	partial class FormOpenDental{
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing){
				components?.Dispose();
				_formCreditRecurringCharges?.Dispose();
				_formCertifications?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormOpenDental));
			this.timerTimeIndic = new System.Windows.Forms.Timer(this.components);
			this.menuItem14 = new System.Windows.Forms.MenuItem();
			this.timerSignals = new System.Windows.Forms.Timer(this.components);
			this.panelSplitter = new System.Windows.Forms.Panel();
			this.menuSplitter = new System.Windows.Forms.ContextMenu();
			this.menuItemDockBottom = new System.Windows.Forms.MenuItem();
			this.menuItemDockRight = new System.Windows.Forms.MenuItem();
			this.imageListMain = new System.Windows.Forms.ImageList(this.components);
			this.menuPatient = new System.Windows.Forms.ContextMenu();
			this.menuLabel = new System.Windows.Forms.ContextMenu();
			this.menuEmail = new System.Windows.Forms.ContextMenu();
			this.menuLetter = new System.Windows.Forms.ContextMenu();
			this.panelPhoneSmall = new System.Windows.Forms.Panel();
			this.butVoiceMails = new OpenDental.UI.Button();
			this.labelFieldType = new System.Windows.Forms.Label();
			this.comboTriageCoordinator = new OpenDental.UI.ComboBoxOD();
			this.labelMsg = new System.Windows.Forms.Label();
			this.butMapPhones = new OpenDental.UI.Button();
			this.butTriage = new OpenDental.UI.Button();
			this.butBigPhones = new OpenDental.UI.Button();
			this.labelWaitTime = new System.Windows.Forms.Label();
			this.labelTriage = new System.Windows.Forms.Label();
			this.menuText = new System.Windows.Forms.ContextMenu();
			this.menuItemTextMessagesAll = new System.Windows.Forms.MenuItem();
			this.menuItemTextMessagesReceived = new System.Windows.Forms.MenuItem();
			this.menuItemTextMessagesSent = new System.Windows.Forms.MenuItem();
			this.menuTask = new System.Windows.Forms.ContextMenu();
			this.menuItemTaskNewForUser = new System.Windows.Forms.MenuItem();
			this.menuItemTaskReminders = new System.Windows.Forms.MenuItem();
			this.menuCommlog = new System.Windows.Forms.ContextMenu();
			this.menuItemCommlogPersistent = new System.Windows.Forms.MenuItem();
			this.lightSignalGrid1 = new OpenDental.UI.LightSignalGrid();
			this.splitContainerNoFlickerDashboard = new OpenDental.SplitContainerNoFlicker();
			this.menuMain = new OpenDental.UI.MenuOD();
			this.panelPhoneSmall.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlickerDashboard)).BeginInit();
			this.splitContainerNoFlickerDashboard.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerTimeIndic
			// 
			this.timerTimeIndic.Interval = 60000;
			this.timerTimeIndic.Tick += new System.EventHandler(this.timerTimeIndic_Tick);
			// 
			// menuItem14
			// 
			this.menuItem14.Index = -1;
			this.menuItem14.Text = "-";
			// 
			// timerSignals
			// 
			this.timerSignals.Tick += new System.EventHandler(this.timerSignals_Tick);
			// 
			// panelSplitter
			// 
			this.panelSplitter.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelSplitter.Cursor = System.Windows.Forms.Cursors.HSplit;
			this.panelSplitter.Location = new System.Drawing.Point(71, 568);
			this.panelSplitter.Name = "panelSplitter";
			this.panelSplitter.Size = new System.Drawing.Size(769, 7);
			this.panelSplitter.TabIndex = 50;
			this.panelSplitter.MouseDown += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseDown);
			this.panelSplitter.MouseMove += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseMove);
			this.panelSplitter.MouseUp += new System.Windows.Forms.MouseEventHandler(this.panelSplitter_MouseUp);
			// 
			// menuSplitter
			// 
			this.menuSplitter.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDockBottom,
            this.menuItemDockRight});
			// 
			// menuItemDockBottom
			// 
			this.menuItemDockBottom.Index = 0;
			this.menuItemDockBottom.Text = "Dock to Bottom";
			this.menuItemDockBottom.Click += new System.EventHandler(this.menuItemDockBottom_Click);
			// 
			// menuItemDockRight
			// 
			this.menuItemDockRight.Index = 1;
			this.menuItemDockRight.Text = "Dock to Right";
			this.menuItemDockRight.Click += new System.EventHandler(this.menuItemDockRight_Click);
			// 
			// imageListMain
			// 
			this.imageListMain.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageListMain.ImageStream")));
			this.imageListMain.TransparentColor = System.Drawing.Color.Transparent;
			this.imageListMain.Images.SetKeyName(0, "Pat.gif");
			this.imageListMain.Images.SetKeyName(1, "commlog.gif");
			this.imageListMain.Images.SetKeyName(2, "email.gif");
			this.imageListMain.Images.SetKeyName(3, "tasksNicer.gif");
			this.imageListMain.Images.SetKeyName(4, "label.gif");
			this.imageListMain.Images.SetKeyName(5, "Text.gif");
			// 
			// menuPatient
			// 
			this.menuPatient.Popup += new System.EventHandler(this.menuPatient_Popup);
			// 
			// menuLabel
			// 
			this.menuLabel.Popup += new System.EventHandler(this.menuLabel_Popup);
			// 
			// menuEmail
			// 
			this.menuEmail.Popup += new System.EventHandler(this.menuEmail_Popup);
			// 
			// menuLetter
			// 
			this.menuLetter.Popup += new System.EventHandler(this.menuLetter_Popup);
			// 
			// panelPhoneSmall
			// 
			this.panelPhoneSmall.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panelPhoneSmall.Controls.Add(this.butVoiceMails);
			this.panelPhoneSmall.Controls.Add(this.labelFieldType);
			this.panelPhoneSmall.Controls.Add(this.comboTriageCoordinator);
			this.panelPhoneSmall.Controls.Add(this.labelMsg);
			this.panelPhoneSmall.Controls.Add(this.butMapPhones);
			this.panelPhoneSmall.Controls.Add(this.butTriage);
			this.panelPhoneSmall.Controls.Add(this.butBigPhones);
			this.panelPhoneSmall.Controls.Add(this.labelWaitTime);
			this.panelPhoneSmall.Controls.Add(this.labelTriage);
			this.panelPhoneSmall.Location = new System.Drawing.Point(71, 359);
			this.panelPhoneSmall.Name = "panelPhoneSmall";
			this.panelPhoneSmall.Size = new System.Drawing.Size(213, 481);
			this.panelPhoneSmall.TabIndex = 56;
			// 
			// butVoiceMails
			// 
			this.butVoiceMails.Location = new System.Drawing.Point(1, 0);
			this.butVoiceMails.Name = "butVoiceMails";
			this.butVoiceMails.Size = new System.Drawing.Size(18, 24);
			this.butVoiceMails.TabIndex = 89;
			this.butVoiceMails.Text = "V";
			this.butVoiceMails.Click += new System.EventHandler(this.butVoiceMails_Click);
			// 
			// labelFieldType
			// 
			this.labelFieldType.Location = new System.Drawing.Point(4, 25);
			this.labelFieldType.Name = "labelFieldType";
			this.labelFieldType.Size = new System.Drawing.Size(143, 15);
			this.labelFieldType.TabIndex = 88;
			this.labelFieldType.Text = "Triage Coordinator";
			this.labelFieldType.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// comboTriageCoordinator
			// 
			this.comboTriageCoordinator.AllowScroll = false;
			this.comboTriageCoordinator.Location = new System.Drawing.Point(0, 42);
			this.comboTriageCoordinator.Name = "comboTriageCoordinator";
			this.comboTriageCoordinator.Size = new System.Drawing.Size(213, 21);
			this.comboTriageCoordinator.TabIndex = 87;
			this.comboTriageCoordinator.SelectionChangeCommitted += new System.EventHandler(this.comboTriageCoordinator_SelectionChangeCommitted);
			// 
			// labelMsg
			// 
			this.labelMsg.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelMsg.ForeColor = System.Drawing.Color.Firebrick;
			this.labelMsg.Location = new System.Drawing.Point(20, 2);
			this.labelMsg.Name = "labelMsg";
			this.labelMsg.Size = new System.Drawing.Size(35, 20);
			this.labelMsg.TabIndex = 53;
			this.labelMsg.Text = "00";
			this.labelMsg.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butMapPhones
			// 
			this.butMapPhones.Location = new System.Drawing.Point(192, 0);
			this.butMapPhones.Name = "butMapPhones";
			this.butMapPhones.Size = new System.Drawing.Size(19, 24);
			this.butMapPhones.TabIndex = 54;
			this.butMapPhones.Text = "M";
			this.butMapPhones.Click += new System.EventHandler(this.butMapPhones_Click);
			// 
			// butTriage
			// 
			this.butTriage.Location = new System.Drawing.Point(154, 0);
			this.butTriage.Name = "butTriage";
			this.butTriage.Size = new System.Drawing.Size(18, 24);
			this.butTriage.TabIndex = 52;
			this.butTriage.Text = "T";
			this.butTriage.Click += new System.EventHandler(this.butTriage_Click);
			// 
			// butBigPhones
			// 
			this.butBigPhones.Location = new System.Drawing.Point(173, 0);
			this.butBigPhones.Name = "butBigPhones";
			this.butBigPhones.Size = new System.Drawing.Size(18, 24);
			this.butBigPhones.TabIndex = 52;
			this.butBigPhones.Text = "B";
			this.butBigPhones.Click += new System.EventHandler(this.butBigPhones_Click);
			// 
			// labelWaitTime
			// 
			this.labelWaitTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelWaitTime.ForeColor = System.Drawing.Color.Black;
			this.labelWaitTime.Location = new System.Drawing.Point(123, 2);
			this.labelWaitTime.Name = "labelWaitTime";
			this.labelWaitTime.Size = new System.Drawing.Size(30, 20);
			this.labelWaitTime.TabIndex = 53;
			this.labelWaitTime.Text = "00m";
			this.labelWaitTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelTriage
			// 
			this.labelTriage.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTriage.ForeColor = System.Drawing.Color.Black;
			this.labelTriage.Location = new System.Drawing.Point(71, 2);
			this.labelTriage.Name = "labelTriage";
			this.labelTriage.Size = new System.Drawing.Size(41, 20);
			this.labelTriage.TabIndex = 53;
			this.labelTriage.Text = "T:000";
			this.labelTriage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// menuText
			// 
			this.menuText.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemTextMessagesAll,
            this.menuItemTextMessagesReceived,
            this.menuItemTextMessagesSent});
			// 
			// menuItemTextMessagesAll
			// 
			this.menuItemTextMessagesAll.Index = 0;
			this.menuItemTextMessagesAll.Text = "Text Messages All";
			this.menuItemTextMessagesAll.Click += new System.EventHandler(this.menuItemTextMessagesAll_Click);
			// 
			// menuItemTextMessagesReceived
			// 
			this.menuItemTextMessagesReceived.Index = 1;
			this.menuItemTextMessagesReceived.Text = "Text Messages Received";
			this.menuItemTextMessagesReceived.Click += new System.EventHandler(this.menuItemTextMessagesReceived_Click);
			// 
			// menuItemTextMessagesSent
			// 
			this.menuItemTextMessagesSent.Index = 2;
			this.menuItemTextMessagesSent.Text = "Text Messages Sent";
			this.menuItemTextMessagesSent.Click += new System.EventHandler(this.menuItemTextMessagesSent_Click);
			// 
			// menuTask
			// 
			this.menuTask.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemTaskNewForUser,
            this.menuItemTaskReminders});
			this.menuTask.Popup += new System.EventHandler(this.menuTask_Popup);
			// 
			// menuItemTaskNewForUser
			// 
			this.menuItemTaskNewForUser.Index = 0;
			this.menuItemTaskNewForUser.Text = "New for [User]";
			this.menuItemTaskNewForUser.Click += new System.EventHandler(this.menuItemTaskNewForUser_Click);
			// 
			// menuItemTaskReminders
			// 
			this.menuItemTaskReminders.Index = 1;
			this.menuItemTaskReminders.Text = "Reminders";
			this.menuItemTaskReminders.Click += new System.EventHandler(this.menuItemTaskReminders_Click);
			// 
			// menuCommlog
			// 
			this.menuCommlog.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemCommlogPersistent});
			// 
			// menuItemCommlogPersistent
			// 
			this.menuItemCommlogPersistent.Index = 0;
			this.menuItemCommlogPersistent.Text = "Persistent";
			this.menuItemCommlogPersistent.Click += new System.EventHandler(this.menuItemCommlogPersistent_Click);
			// 
			// lightSignalGrid1
			// 
			this.lightSignalGrid1.Location = new System.Drawing.Point(0, 489);
			this.lightSignalGrid1.Name = "lightSignalGrid1";
			this.lightSignalGrid1.Size = new System.Drawing.Size(50, 206);
			this.lightSignalGrid1.TabIndex = 20;
			this.lightSignalGrid1.Text = "lightSignalGrid1";
			this.lightSignalGrid1.ButtonClick += new OpenDental.UI.ODLightSignalGridClickEventHandler(this.lightSignalGrid1_ButtonClick);
			// 
			// splitContainerNoFlickerDashboard
			// 
			this.splitContainerNoFlickerDashboard.Location = new System.Drawing.Point(68, 52);
			this.splitContainerNoFlickerDashboard.Name = "splitContainerNoFlickerDashboard";
			this.splitContainerNoFlickerDashboard.Panel1MinSize = 50;
			this.splitContainerNoFlickerDashboard.Panel2Collapsed = true;
			this.splitContainerNoFlickerDashboard.Panel2MinSize = 50;
			this.splitContainerNoFlickerDashboard.Size = new System.Drawing.Size(915, 284);
			this.splitContainerNoFlickerDashboard.TabIndex = 57;
			this.splitContainerNoFlickerDashboard.TabStop = false;
			this.splitContainerNoFlickerDashboard.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerNoFlickerDashboard_SplitterMoved);
			// 
			// menuMain
			// 
			this.menuMain.Dock = System.Windows.Forms.DockStyle.Top;
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(1230, 24);
			this.menuMain.TabIndex = 58;
			// 
			// FormOpenDental
			// 
			this.ClientSize = new System.Drawing.Size(1230, 696);
			this.Controls.Add(this.splitContainerNoFlickerDashboard);
			this.Controls.Add(this.panelPhoneSmall);
			this.Controls.Add(this.panelSplitter);
			this.Controls.Add(this.lightSignalGrid1);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormOpenDental";
			this.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.Text = "Open Dental";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Deactivate += new System.EventHandler(this.FormOpenDental_Deactivate);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormOpenDental_FormClosing);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormOpenDental_FormClosed);
			this.Load += new System.EventHandler(this.FormOpenDental_Load);
			this.Shown += new System.EventHandler(this.FormOpenDental_Shown);
			this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FormOpenDental_KeyDown);
			this.panelPhoneSmall.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerNoFlickerDashboard)).EndInit();
			this.splitContainerNoFlickerDashboard.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Timer timerTimeIndic;
		private System.Windows.Forms.Timer timerSignals;
		private OpenDental.UI.LightSignalGrid lightSignalGrid1;
		private Panel panelSplitter;
		private ContextMenu menuSplitter;
		private MenuItem menuItemDockBottom;
		private MenuItem menuItemDockRight;
		private ImageList imageListMain;
		private ContextMenu menuPatient;
		private ContextMenu menuLabel;
		private ContextMenu menuEmail;
		private ContextMenu menuLetter;
		private UserControlTasks userControlTasks1;
		private UserControlDashboard userControlDashboard;
		private ControlAppt controlAppt;
		private ControlFamily controlFamily;
		private ControlFamilyEcw controlFamilyEcw;
		private ControlAccount controlAccount;
		private ControlTreat controlTreat;
		private ControlChart controlChart;
		private ControlImages controlImages;
		private ControlImagesJ controlImagesJ;
		private ControlManage controlManage;
		private ModuleBar moduleBar;
		private OpenDental.UI.ToolBarOD ToolBarMain;
		private UserControlPhoneSmall phoneSmall;
		private Panel panelPhoneSmall;
		private UI.Button butTriage;
		private UI.Button butBigPhones;
		private Label labelWaitTime;
		private Label labelTriage;
		private Label labelMsg;
		private UI.Button butMapPhones;
		private UI.ComboBoxOD comboTriageCoordinator;
		private Label labelFieldType;
		private MenuItem menuItem14;
		private ContextMenu menuText;
		private MenuItem menuItemTextMessagesReceived;
		private MenuItem menuItemTextMessagesSent;
		private MenuItem menuItemTextMessagesAll;
		private ContextMenu menuTask;
		private MenuItem menuItemTaskNewForUser;
		private MenuItem menuItemTaskReminders;
		private UI.Button butVoiceMails;
		private ContextMenu menuCommlog;
		private MenuItem menuItemCommlogPersistent;
		private FormWebChatTools _formWCT;
		private FormWebChatSurveys _formWebChatSurveys;
		private SplitContainerNoFlicker splitContainerNoFlickerDashboard;
		private UI.MenuOD menuMain;
	}
}
