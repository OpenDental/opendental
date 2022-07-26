using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentalCloud;
using OpenDentBusiness;

namespace OpenDental{

	///<summary></summary>
	public class ControlManage : UserControl{
		#region Fields - Public
		public FormAccounting FormAccounting;
		///<summary></summary>
		//[Category("Data"),Description("Occurs when user changes current patient, usually by clicking on the Select Patient button.")]
		//public event PatientSelectedEventHandler PatientSelected=null;
		#endregion Fields - Public

		#region Fields - Private
		private SigElementDef[] _arraySigElementDefExtras;
		private SigElementDef[] _arraySigElementDefMessages;
		private SigElementDef[] _arraySigElementDefUsers;
		private Employee _employeeCur;
		private ErrorProvider _errorProvider1=new ErrorProvider();
		private FormArManager _formArManager;
		private FormBilling _formBilling;
		private FormClaimsSend _formClaimsSend;
		private FormEmailInbox _formEmailInbox=null;
		private FormEtrans834Import _formEtrans834Import=null;
		private FormGraphEmployeeTime _formGraphEmployeeTime;
		private System.ComponentModel.IContainer iComponents;
		//private bool _initializedOnStartup;
		private List<Employee> _listEmployees=new List<Employee>();
		/// <summary>Always 1:1 with values in listStatus.Items</summary>
		private List<TimeClockStatus> _listShownTimeClockStatuses=new List<TimeClockStatus>();
		///<summary>Collection of SigMessages</summary>
		private List<SigMessage> _listSigMessages;
		private long _numPatCur;
		///<summary>Server time minus local computer time, usually +/- 1 or 2 minutes</summary>
		private TimeSpan _timeDelta;
		#endregion Fields - Private

		#region Fields - Private - Windows Forms
		private UI.Button butAccounting;
		private UI.Button butAck;
		private UI.Button butBackup;
		private UI.Button butBilling;
		private UI.Button butBreaks;
		private UI.Button butClaimPay;
		private UI.Button butClockIn;
		private UI.Button butClockOut;
		private UI.Button butDeposit;
		private UI.Button butEmailInbox;
		private UI.Button butEras;
		private UI.Button butImportInsPlans;
		private UI.Button butManage;
		private UI.Button butManageAR;
		private UI.Button butSchedule;
		private UI.Button butSend;
		private UI.Button butSendClaims;
		private UI.Button butSupply;
		private UI.Button butTasks;
		private UI.Button butTimeCard;
		private UI.Button butViewSched;
		private CheckBox checkIncludeAck;
		private ComboBox comboBoxViewUser;
		private GridOD gridEmp;
		private GridOD gridMessages;
		private OpenDental.UI.GroupBoxOD groupBox1;
		private OpenDental.UI.GroupBoxOD groupBox2;
		private OpenDental.UI.GroupBoxOD groupBox3;
		private Label label1;
		private Label label3;
		private Label label4;
		private Label label5;
		private Label label6;
		private Label label7;
		private Label labelCurrentTime;
		private Label labelDays;
		private Label labelSending;
		private Label labelTime;
		private OpenDental.UI.ListBoxOD listBoxExtras;
		private OpenDental.UI.ListBoxOD listBoxFrom;
		private OpenDental.UI.ListBoxOD listBoxMessages;
		private OpenDental.UI.ListBoxOD listBoxStatus;
		private OpenDental.UI.ListBoxOD listBoxTo;
		private TextBox textDays;
		private TextBox textMessage;
		private System.ComponentModel.IContainer components;
		private Label labelFilterName;
		private TextBox textFilterName;
		private System.Windows.Forms.Timer timerUpdateTime;
		#endregion Fields - Private - Windows Forms

		#region Constructor
		///<summary></summary>
		public ControlManage(){
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.Start);
			InitializeComponent();
			this.listBoxStatus.Click += new System.EventHandler(this.listStatus_Click);
			Logger.LogToPath("Ctor",LogPath.Startup,LogPhase.End);
		}
		#endregion Constructor

		#region Dispose
		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(iComponents != null){
					iComponents.Dispose();
				}
			}
			base.Dispose( disposing );
		}
		#endregion Dispose

		#region Component Designer generated code

		private void InitializeComponent(){
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ControlManage));
			this.timerUpdateTime = new System.Windows.Forms.Timer(this.components);
			this.butSchedule = new OpenDental.UI.Button();
			this.groupBox3 = new OpenDental.UI.GroupBoxOD();
			this.butEras = new OpenDental.UI.Button();
			this.butImportInsPlans = new OpenDental.UI.Button();
			this.butEmailInbox = new OpenDental.UI.Button();
			this.butSupply = new OpenDental.UI.Button();
			this.butClaimPay = new OpenDental.UI.Button();
			this.butBilling = new OpenDental.UI.Button();
			this.butAccounting = new OpenDental.UI.Button();
			this.butBackup = new OpenDental.UI.Button();
			this.butDeposit = new OpenDental.UI.Button();
			this.butSendClaims = new OpenDental.UI.Button();
			this.butTasks = new OpenDental.UI.Button();
			this.butManageAR = new OpenDental.UI.Button();
			this.groupBox2 = new OpenDental.UI.GroupBoxOD();
			this.listBoxMessages = new OpenDental.UI.ListBoxOD();
			this.butSend = new OpenDental.UI.Button();
			this.butAck = new OpenDental.UI.Button();
			this.labelSending = new System.Windows.Forms.Label();
			this.textDays = new System.Windows.Forms.TextBox();
			this.labelDays = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxViewUser = new System.Windows.Forms.ComboBox();
			this.gridMessages = new OpenDental.UI.GridOD();
			this.checkIncludeAck = new System.Windows.Forms.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.listBoxExtras = new OpenDental.UI.ListBoxOD();
			this.label4 = new System.Windows.Forms.Label();
			this.listBoxFrom = new OpenDental.UI.ListBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.listBoxTo = new OpenDental.UI.ListBoxOD();
			this.label1 = new System.Windows.Forms.Label();
			this.textMessage = new System.Windows.Forms.TextBox();
			this.groupBox1 = new OpenDental.UI.GroupBoxOD();
			this.textFilterName = new System.Windows.Forms.TextBox();
			this.labelFilterName = new System.Windows.Forms.Label();
			this.butViewSched = new OpenDental.UI.Button();
			this.butManage = new OpenDental.UI.Button();
			this.butBreaks = new OpenDental.UI.Button();
			this.gridEmp = new OpenDental.UI.GridOD();
			this.labelCurrentTime = new System.Windows.Forms.Label();
			this.listBoxStatus = new OpenDental.UI.ListBoxOD();
			this.butClockOut = new OpenDental.UI.Button();
			this.butTimeCard = new OpenDental.UI.Button();
			this.labelTime = new System.Windows.Forms.Label();
			this.butClockIn = new OpenDental.UI.Button();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerUpdateTime
			// 
			this.timerUpdateTime.Enabled = true;
			this.timerUpdateTime.Interval = 1000;
			this.timerUpdateTime.Tick += new System.EventHandler(this.timerUpdateTime_Tick);
			// 
			// butSchedule
			// 
			this.butSchedule.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSchedule.Location = new System.Drawing.Point(801, 99);
			this.butSchedule.Name = "butSchedule";
			this.butSchedule.Size = new System.Drawing.Size(79, 25);
			this.butSchedule.TabIndex = 32;
			this.butSchedule.Text = "Daily Graph";
			this.butSchedule.Click += new System.EventHandler(this.butSchedule_Click);
			// 
			// groupBox3
			// 
			this.groupBox3.BackColor = System.Drawing.Color.White;
			this.groupBox3.Controls.Add(this.butEras);
			this.groupBox3.Controls.Add(this.butImportInsPlans);
			this.groupBox3.Controls.Add(this.butEmailInbox);
			this.groupBox3.Controls.Add(this.butSupply);
			this.groupBox3.Controls.Add(this.butClaimPay);
			this.groupBox3.Controls.Add(this.butBilling);
			this.groupBox3.Controls.Add(this.butAccounting);
			this.groupBox3.Controls.Add(this.butBackup);
			this.groupBox3.Controls.Add(this.butDeposit);
			this.groupBox3.Controls.Add(this.butSendClaims);
			this.groupBox3.Controls.Add(this.butTasks);
			this.groupBox3.Controls.Add(this.butManageAR);
			this.groupBox3.Location = new System.Drawing.Point(17, 5);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(272, 181);
			this.groupBox3.TabIndex = 23;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Daily";
			// 
			// butEras
			// 
			this.butEras.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEras.Location = new System.Drawing.Point(148, 123);
			this.butEras.Name = "butEras";
			this.butEras.Size = new System.Drawing.Size(104, 26);
			this.butEras.TabIndex = 30;
			this.butEras.Text = "ERAs";
			this.butEras.Click += new System.EventHandler(this.butEras_Click);
			// 
			// butImportInsPlans
			// 
			this.butImportInsPlans.Location = new System.Drawing.Point(148, 149);
			this.butImportInsPlans.Name = "butImportInsPlans";
			this.butImportInsPlans.Size = new System.Drawing.Size(104, 26);
			this.butImportInsPlans.TabIndex = 29;
			this.butImportInsPlans.Text = "Import Ins Plans";
			this.butImportInsPlans.Click += new System.EventHandler(this.butImportInsPlans_Click);
			// 
			// butEmailInbox
			// 
			this.butEmailInbox.Icon = OpenDental.UI.EnumIcons.Email;
			this.butEmailInbox.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butEmailInbox.Location = new System.Drawing.Point(148, 97);
			this.butEmailInbox.Name = "butEmailInbox";
			this.butEmailInbox.Size = new System.Drawing.Size(104, 26);
			this.butEmailInbox.TabIndex = 28;
			this.butEmailInbox.Text = "Emails";
			this.butEmailInbox.Click += new System.EventHandler(this.butEmailInbox_Click);
			// 
			// butSupply
			// 
			this.butSupply.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSupply.Location = new System.Drawing.Point(16, 123);
			this.butSupply.Name = "butSupply";
			this.butSupply.Size = new System.Drawing.Size(104, 26);
			this.butSupply.TabIndex = 26;
			this.butSupply.Text = "Supply Inventory";
			this.butSupply.Click += new System.EventHandler(this.butSupply_Click);
			// 
			// butClaimPay
			// 
			this.butClaimPay.Location = new System.Drawing.Point(16, 45);
			this.butClaimPay.Name = "butClaimPay";
			this.butClaimPay.Size = new System.Drawing.Size(104, 26);
			this.butClaimPay.TabIndex = 25;
			this.butClaimPay.Text = "Batch Ins";
			this.butClaimPay.Click += new System.EventHandler(this.butClaimPay_Click);
			// 
			// butBilling
			// 
			this.butBilling.Location = new System.Drawing.Point(16, 71);
			this.butBilling.Name = "butBilling";
			this.butBilling.Size = new System.Drawing.Size(104, 26);
			this.butBilling.TabIndex = 25;
			this.butBilling.Text = "Billing";
			this.butBilling.Click += new System.EventHandler(this.butBilling_Click);
			// 
			// butAccounting
			// 
			this.butAccounting.Image = ((System.Drawing.Image)(resources.GetObject("butAccounting.Image")));
			this.butAccounting.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAccounting.Location = new System.Drawing.Point(148, 71);
			this.butAccounting.Name = "butAccounting";
			this.butAccounting.Size = new System.Drawing.Size(104, 26);
			this.butAccounting.TabIndex = 24;
			this.butAccounting.Text = "Accounting";
			this.butAccounting.Click += new System.EventHandler(this.butAccounting_Click);
			// 
			// butBackup
			// 
			this.butBackup.Image = ((System.Drawing.Image)(resources.GetObject("butBackup.Image")));
			this.butBackup.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butBackup.Location = new System.Drawing.Point(148, 45);
			this.butBackup.Name = "butBackup";
			this.butBackup.Size = new System.Drawing.Size(104, 26);
			this.butBackup.TabIndex = 22;
			this.butBackup.Text = "Backup";
			this.butBackup.Click += new System.EventHandler(this.butBackup_Click);
			// 
			// butDeposit
			// 
			this.butDeposit.Image = ((System.Drawing.Image)(resources.GetObject("butDeposit.Image")));
			this.butDeposit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeposit.Location = new System.Drawing.Point(16, 97);
			this.butDeposit.Name = "butDeposit";
			this.butDeposit.Size = new System.Drawing.Size(104, 26);
			this.butDeposit.TabIndex = 23;
			this.butDeposit.Text = "Deposits";
			this.butDeposit.Click += new System.EventHandler(this.butDeposit_Click);
			// 
			// butSendClaims
			// 
			this.butSendClaims.Image = ((System.Drawing.Image)(resources.GetObject("butSendClaims.Image")));
			this.butSendClaims.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butSendClaims.Location = new System.Drawing.Point(16, 19);
			this.butSendClaims.Name = "butSendClaims";
			this.butSendClaims.Size = new System.Drawing.Size(104, 26);
			this.butSendClaims.TabIndex = 20;
			this.butSendClaims.Text = "Send Claims";
			this.butSendClaims.Click += new System.EventHandler(this.butSendClaims_Click);
			// 
			// butTasks
			// 
			this.butTasks.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butTasks.Image = ((System.Drawing.Image)(resources.GetObject("butTasks.Image")));
			this.butTasks.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butTasks.Location = new System.Drawing.Point(148, 19);
			this.butTasks.Name = "butTasks";
			this.butTasks.Size = new System.Drawing.Size(104, 26);
			this.butTasks.TabIndex = 21;
			this.butTasks.Text = "Tasks";
			this.butTasks.Click += new System.EventHandler(this.butTasks_Click);
			// 
			// butManageAR
			// 
			this.butManageAR.AdjustImageLocation = new System.Drawing.Point(-2, 0);
			this.butManageAR.Image = global::OpenDental.Properties.Resources.TSI_Icon;
			this.butManageAR.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butManageAR.Location = new System.Drawing.Point(16, 149);
			this.butManageAR.Name = "butManageAR";
			this.butManageAR.Size = new System.Drawing.Size(104, 26);
			this.butManageAR.TabIndex = 31;
			this.butManageAR.Text = "Collections";
			this.butManageAR.Click += new System.EventHandler(this.butManageAR_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.listBoxMessages);
			this.groupBox2.Controls.Add(this.butSend);
			this.groupBox2.Controls.Add(this.butAck);
			this.groupBox2.Controls.Add(this.labelSending);
			this.groupBox2.Controls.Add(this.textDays);
			this.groupBox2.Controls.Add(this.labelDays);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.comboBoxViewUser);
			this.groupBox2.Controls.Add(this.gridMessages);
			this.groupBox2.Controls.Add(this.checkIncludeAck);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.listBoxExtras);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.listBoxFrom);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.listBoxTo);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.textMessage);
			this.groupBox2.DrawBorder = false;
			this.groupBox2.Location = new System.Drawing.Point(3, 277);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(902, 422);
			this.groupBox2.TabIndex = 19;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Messaging";
			// 
			// listBoxMessages
			// 
			this.listBoxMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxMessages.Location = new System.Drawing.Point(252, 35);
			this.listBoxMessages.Name = "listBoxMessages";
			this.listBoxMessages.Size = new System.Drawing.Size(98, 329);
			this.listBoxMessages.TabIndex = 10;
			this.listBoxMessages.Click += new System.EventHandler(this.listMessages_Click);
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSend.Location = new System.Drawing.Point(252, 392);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(98, 25);
			this.butSend.TabIndex = 15;
			this.butSend.Text = "Send Message";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// butAck
			// 
			this.butAck.Location = new System.Drawing.Point(645, 10);
			this.butAck.Name = "butAck";
			this.butAck.Size = new System.Drawing.Size(67, 22);
			this.butAck.TabIndex = 25;
			this.butAck.Text = "Ack";
			this.butAck.Click += new System.EventHandler(this.butAck_Click);
			// 
			// labelSending
			// 
			this.labelSending.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelSending.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.5F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelSending.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
			this.labelSending.Location = new System.Drawing.Point(251, 368);
			this.labelSending.Name = "labelSending";
			this.labelSending.Size = new System.Drawing.Size(100, 21);
			this.labelSending.TabIndex = 24;
			this.labelSending.Text = "Sending";
			this.labelSending.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			this.labelSending.Visible = false;
			// 
			// textDays
			// 
			this.textDays.Location = new System.Drawing.Point(594, 12);
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(45, 20);
			this.textDays.TabIndex = 19;
			this.textDays.Visible = false;
			this.textDays.TextChanged += new System.EventHandler(this.textDays_TextChanged);
			// 
			// labelDays
			// 
			this.labelDays.Location = new System.Drawing.Point(531, 14);
			this.labelDays.Name = "labelDays";
			this.labelDays.Size = new System.Drawing.Size(61, 16);
			this.labelDays.TabIndex = 18;
			this.labelDays.Text = "Days";
			this.labelDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelDays.Visible = false;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(725, 14);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(57, 16);
			this.label6.TabIndex = 17;
			this.label6.Text = "To User";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxViewUser
			// 
			this.comboBoxViewUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxViewUser.FormattingEnabled = true;
			this.comboBoxViewUser.Location = new System.Drawing.Point(783, 11);
			this.comboBoxViewUser.Name = "comboBoxViewUser";
			this.comboBoxViewUser.Size = new System.Drawing.Size(114, 21);
			this.comboBoxViewUser.TabIndex = 16;
			this.comboBoxViewUser.SelectionChangeCommitted += new System.EventHandler(this.comboViewUser_SelectionChangeCommitted);
			// 
			// gridMessages
			// 
			this.gridMessages.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMessages.Location = new System.Drawing.Point(356, 35);
			this.gridMessages.Name = "gridMessages";
			this.gridMessages.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMessages.Size = new System.Drawing.Size(540, 381);
			this.gridMessages.TabIndex = 13;
			this.gridMessages.Title = "Message History";
			this.gridMessages.TranslationName = "TableTextMessages";
			// 
			// checkIncludeAck
			// 
			this.checkIncludeAck.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeAck.Location = new System.Drawing.Point(356, 16);
			this.checkIncludeAck.Name = "checkIncludeAck";
			this.checkIncludeAck.Size = new System.Drawing.Size(173, 18);
			this.checkIncludeAck.TabIndex = 14;
			this.checkIncludeAck.Text = "Include Acknowledged";
			this.checkIncludeAck.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIncludeAck.UseVisualStyleBackColor = true;
			this.checkIncludeAck.Click += new System.EventHandler(this.checkIncludeAck_Click);
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(6, 377);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(100, 16);
			this.label7.TabIndex = 12;
			this.label7.Text = "Message";
			this.label7.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(250, 16);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 16);
			this.label5.TabIndex = 9;
			this.label5.Text = "Message (&& Send)";
			this.label5.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxExtras
			// 
			this.listBoxExtras.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxExtras.Location = new System.Drawing.Point(171, 35);
			this.listBoxExtras.Name = "listBoxExtras";
			this.listBoxExtras.Size = new System.Drawing.Size(75, 329);
			this.listBoxExtras.TabIndex = 8;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(169, 16);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(78, 16);
			this.label4.TabIndex = 7;
			this.label4.Text = "Extras";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxFrom
			// 
			this.listBoxFrom.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxFrom.Location = new System.Drawing.Point(90, 35);
			this.listBoxFrom.Name = "listBoxFrom";
			this.listBoxFrom.Size = new System.Drawing.Size(75, 329);
			this.listBoxFrom.TabIndex = 6;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(88, 16);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(78, 16);
			this.label3.TabIndex = 5;
			this.label3.Text = "From";
			this.label3.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// listBoxTo
			// 
			this.listBoxTo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.listBoxTo.Location = new System.Drawing.Point(9, 35);
			this.listBoxTo.Name = "listBoxTo";
			this.listBoxTo.Size = new System.Drawing.Size(75, 329);
			this.listBoxTo.TabIndex = 4;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(7, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(78, 16);
			this.label1.TabIndex = 3;
			this.label1.Text = "To";
			this.label1.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textMessage
			// 
			this.textMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textMessage.Location = new System.Drawing.Point(9, 396);
			this.textMessage.Name = "textMessage";
			this.textMessage.Size = new System.Drawing.Size(237, 20);
			this.textMessage.TabIndex = 1;
			// 
			// groupBox1
			// 
			this.groupBox1.BackColor = System.Drawing.Color.White;
			this.groupBox1.Controls.Add(this.textFilterName);
			this.groupBox1.Controls.Add(this.labelFilterName);
			this.groupBox1.Controls.Add(this.butViewSched);
			this.groupBox1.Controls.Add(this.butManage);
			this.groupBox1.Controls.Add(this.butBreaks);
			this.groupBox1.Controls.Add(this.gridEmp);
			this.groupBox1.Controls.Add(this.labelCurrentTime);
			this.groupBox1.Controls.Add(this.listBoxStatus);
			this.groupBox1.Controls.Add(this.butClockOut);
			this.groupBox1.Controls.Add(this.butTimeCard);
			this.groupBox1.Controls.Add(this.labelTime);
			this.groupBox1.Controls.Add(this.butClockIn);
			this.groupBox1.Location = new System.Drawing.Point(338, 5);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(458, 272);
			this.groupBox1.TabIndex = 18;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Time Clock";
			// 
			// textFilterName
			// 
			this.textFilterName.Location = new System.Drawing.Point(224, 13);
			this.textFilterName.Name = "textFilterName";
			this.textFilterName.Size = new System.Drawing.Size(100, 20);
			this.textFilterName.TabIndex = 26;
			this.textFilterName.TextChanged += new System.EventHandler(this.textFilterName_TextChanged);
			// 
			// labelFilterName
			// 
			this.labelFilterName.Location = new System.Drawing.Point(130, 12);
			this.labelFilterName.Name = "labelFilterName";
			this.labelFilterName.Size = new System.Drawing.Size(93, 20);
			this.labelFilterName.TabIndex = 25;
			this.labelFilterName.Text = "Filter by Name";
			this.labelFilterName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butViewSched
			// 
			this.butViewSched.Location = new System.Drawing.Point(340, 94);
			this.butViewSched.Name = "butViewSched";
			this.butViewSched.Size = new System.Drawing.Size(108, 25);
			this.butViewSched.TabIndex = 24;
			this.butViewSched.Text = "View Schedule";
			this.butViewSched.Click += new System.EventHandler(this.butViewSched_Click);
			// 
			// butManage
			// 
			this.butManage.Location = new System.Drawing.Point(340, 13);
			this.butManage.Name = "butManage";
			this.butManage.Size = new System.Drawing.Size(108, 25);
			this.butManage.TabIndex = 23;
			this.butManage.Text = "Manage";
			this.butManage.Click += new System.EventHandler(this.butManage_Click);
			// 
			// butBreaks
			// 
			this.butBreaks.Location = new System.Drawing.Point(340, 67);
			this.butBreaks.Name = "butBreaks";
			this.butBreaks.Size = new System.Drawing.Size(108, 25);
			this.butBreaks.TabIndex = 22;
			this.butBreaks.Text = "View Breaks";
			this.butBreaks.Click += new System.EventHandler(this.butBreaks_Click);
			// 
			// gridEmp
			// 
			this.gridEmp.Location = new System.Drawing.Point(21, 40);
			this.gridEmp.Name = "gridEmp";
			this.gridEmp.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridEmp.Size = new System.Drawing.Size(303, 220);
			this.gridEmp.TabIndex = 21;
			this.gridEmp.Title = "Employee";
			this.gridEmp.TranslationName = "TableEmpClock";
			this.gridEmp.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEmp_CellDoubleClick);
			this.gridEmp.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridEmp_CellClick);
			// 
			// labelCurrentTime
			// 
			this.labelCurrentTime.Location = new System.Drawing.Point(350, 121);
			this.labelCurrentTime.Name = "labelCurrentTime";
			this.labelCurrentTime.Size = new System.Drawing.Size(88, 17);
			this.labelCurrentTime.TabIndex = 20;
			this.labelCurrentTime.Text = "Server Time";
			this.labelCurrentTime.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// listBoxStatus
			// 
			this.listBoxStatus.Location = new System.Drawing.Point(341, 217);
			this.listBoxStatus.Name = "listBoxStatus";
			this.listBoxStatus.Size = new System.Drawing.Size(107, 43);
			this.listBoxStatus.TabIndex = 12;
			// 
			// butClockOut
			// 
			this.butClockOut.Location = new System.Drawing.Point(340, 189);
			this.butClockOut.Name = "butClockOut";
			this.butClockOut.Size = new System.Drawing.Size(108, 25);
			this.butClockOut.TabIndex = 14;
			this.butClockOut.Text = "Clock Out For:";
			this.butClockOut.Click += new System.EventHandler(this.butClockOut_Click);
			// 
			// butTimeCard
			// 
			this.butTimeCard.Location = new System.Drawing.Point(340, 40);
			this.butTimeCard.Name = "butTimeCard";
			this.butTimeCard.Size = new System.Drawing.Size(108, 25);
			this.butTimeCard.TabIndex = 16;
			this.butTimeCard.Text = "View Time Card";
			this.butTimeCard.Click += new System.EventHandler(this.butTimeCard_Click);
			// 
			// labelTime
			// 
			this.labelTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 13F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTime.Location = new System.Drawing.Point(339, 138);
			this.labelTime.Name = "labelTime";
			this.labelTime.Size = new System.Drawing.Size(109, 21);
			this.labelTime.TabIndex = 17;
			this.labelTime.Text = "12:00:00 PM";
			this.labelTime.TextAlign = System.Drawing.ContentAlignment.BottomCenter;
			// 
			// butClockIn
			// 
			this.butClockIn.Location = new System.Drawing.Point(340, 162);
			this.butClockIn.Name = "butClockIn";
			this.butClockIn.Size = new System.Drawing.Size(108, 25);
			this.butClockIn.TabIndex = 11;
			this.butClockIn.Text = "Clock In";
			this.butClockIn.Click += new System.EventHandler(this.butClockIn_Click);
			// 
			// ControlManage
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.butSchedule);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "ControlManage";
			this.Size = new System.Drawing.Size(908, 702);
			this.Load += new System.EventHandler(this.ControlManage_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		#region Methods - Event Handlers - Buttons General
		private void butAccounting_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Accounting)) {
				return;
			}
			if(FormAccounting==null || FormAccounting.IsDisposed) {
				FormAccounting=new FormAccounting();
			}
			FormAccounting.Show();
			if(FormAccounting.WindowState==FormWindowState.Minimized) {
				FormAccounting.WindowState=FormWindowState.Normal;
			}
			FormAccounting.BringToFront();
		}

		private void butBackup_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Backup)){
				return;
			}
			SecurityLogs.MakeLogEntry(Permissions.Backup,0,"FormBackup was accessed");
			using FormBackup formBackup=new FormBackup();
			formBackup.ShowDialog();
			if(formBackup.DialogResult==DialogResult.Cancel){
				return;
			}
			//ok signifies that a database was restored
			FormOpenDental.S_Contr_PatientSelected(new Patient(),false);//unload patient after restore.
			//ParentForm.Text=PrefC.GetString(PrefName.MainWindowTitle");
			DataValid.SetInvalid(true);
			ModuleSelected(_numPatCur);
		}

		private void butBilling_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Billing)) {
				return;
			}
			bool unsentStatementsExist=Statements.UnsentStatementsExist();
			if(unsentStatementsExist) {
				if(PrefC.HasClinicsEnabled) {//Using clinics.
					if(Statements.UnsentClinicStatementsExist(Clinics.ClinicNum)) {//Check if clinic has unsent bills.
						ShowBilling(Clinics.ClinicNum);//Clinic has unsent bills.  Simply show billing window.
					}
					else {//No unsent bills for clinic.  Show billing options to generate a billing list.
						ShowBillingOptions(Clinics.ClinicNum);
					}
				}
				else {//Not using clinics and has unsent bills.  Simply show billing window.
					ShowBilling(0);
				}
			}
			else {//No unsent statements exist.  Have user create a billing list.
				if(PrefC.HasClinicsEnabled) {
					ShowBillingOptions(Clinics.ClinicNum);
				}
				else {
					ShowBillingOptions(0);
				}
			}
			SecurityLogs.MakeLogEntry(Permissions.Billing,0,"");
		}

		private void butBreaks_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PayPeriods.GetCount()==0) {
				MsgBox.Show(this,"The adminstrator needs to setup pay periods first.");
				return;
			}
			using FormTimeCard formTimeCard=new FormTimeCard(_listEmployees);
			formTimeCard.EmployeeCur=_employeeCur;
			formTimeCard.IsBreaks=true;
			formTimeCard.ShowDialog();
			ModuleSelected(_numPatCur);
		}

		private void butClaimPay_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate,true) && !Security.IsAuthorized(Permissions.InsPayEdit,true)) {
				//Custom message for multiple permissions.
				MessageBox.Show(Lan.g(this,"Not authorized")+".\r\n"
					+Lan.g(this,"A user with the SecurityAdmin permission must grant you access for")+":\r\n"
					+Lan.g(this,"Insurance Payment Create or Insurance Payment Edit"));
				return;
			}
			FormClaimPayList formClaimPayList=new FormClaimPayList();
			formClaimPayList.Show();
		}

		//private void butClear_Click(object sender, System.EventArgs e) {
			//textMessage.Clear();
			//textMessage.Select();
		//}

		private void butClockIn_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PrefC.GetBool(PrefName.DockPhonePanelShow) && !Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
				//Check if the employee set their ext to 0 in the phoneempdefault table.
				if(PhoneEmpDefaults.GetByExtAndEmp(0,_employeeCur.EmployeeNum)==null) {
					MessageBox.Show("Not allowed.  Use the small phone panel or the \"Big\" phone window to clock in.\r\nIf you are trying to clock in as a \"floater\", you need to set your extension to 0 first before using this Clock In button.");
					return;
				}
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				bool[] isAuthorized=new bool[1] { false };
				if(Plugins.HookMethod(this,"ContrStaff.butClockIn_Click_ClockIn",isAuthorized,_employeeCur)) {
					if(!isAuthorized[0]) {
						throw new Exception(Lans.g(this,"You need to authenticate to clock-in"));
					}
				}
				ClockEvents.ClockIn(_employeeCur.EmployeeNum,isAtHome:false);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock out again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(this,"Processing clock event...");
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			Employee EmployeeOld=_employeeCur.Copy();
			_employeeCur.ClockStatus=Lan.g(this,"Working");
			Employees.UpdateChanged(_employeeCur, EmployeeOld, true);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				Phones.SetPhoneStatus(ClockStatusEnum.Available,Phones.GetExtensionForEmp(_employeeCur.EmployeeNum),_employeeCur.EmployeeNum);
			}
			ModuleSelected(_numPatCur);
			if(!PayPeriods.HasPayPeriodForDate(DateTime.Today)) {
				MsgBox.Show(this,"No dates exist for this pay period.  Time clock events will not display until pay periods have been created for this date range");
			}
		}

		private void butClockOut_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PrefC.GetBool(PrefName.DockPhonePanelShow) && !Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
				//Check if the employee set their ext to 0 in the phoneempdefault table.
				if(PhoneEmpDefaults.GetByExtAndEmp(0,_employeeCur.EmployeeNum)==null) {
					MessageBox.Show("Not allowed.  Use the small phone panel or the \"Big\" phone window to clock out.\r\nIf you are trying to clock out as a \"floater\", you need to set your extension to 0 first before using this Clock Out For: button.");
					return;
				}
			}
			if(listBoxStatus.SelectedIndex==-1) {
				MsgBox.Show(this,"Please select a status first.");
				return;
			}
			ProgressOD progressOD=new ProgressOD();
			progressOD.ShowCancelButton=false;//safe because this is guaranteed to be only one second, more like a fancy wait cursor
			progressOD.ActionMain=() => {
				bool[] isAuthorized=new bool[1] { false };
				if(Plugins.HookMethod(this,"ContrStaff.butClockOut_Click_ClockOut",isAuthorized,_employeeCur,_listShownTimeClockStatuses[listBoxStatus.SelectedIndex])) {
					if(!isAuthorized[0]) {
						throw new Exception(Lans.g(this,"You need to authenticate to clock-out"));
					}
				}
				ClockEvents.ClockOut(_employeeCur.EmployeeNum,_listShownTimeClockStatuses[listBoxStatus.SelectedIndex]);
				System.Threading.Thread.Sleep(1000);//Wait one second so that if they quickly clock in again, the timestamps will be far enough apart.
			};
			progressOD.StartingMessage=Lan.g(this,"Processing clock event...");
			try {
				progressOD.ShowDialogProgress();
			}
			catch(Exception ex) {
				MessageBox.Show(ex.Message);
				return;
			}
			DataValid.SetInvalid(InvalidType.PhoneEmpDefaults);
			Employee EmployeeOld=_employeeCur.Copy();
			_employeeCur.ClockStatus=Lan.g("enumTimeClockStatus",(_listShownTimeClockStatuses[listBoxStatus.SelectedIndex]).GetDescription());
			Employees.UpdateChanged(_employeeCur, EmployeeOld, true);
			ModuleSelected(_numPatCur);
			if(PrefC.GetBool(PrefName.DockPhonePanelShow)) {
				Phones.SetPhoneStatus(Phones.GetClockStatusFromEmp(_employeeCur.ClockStatus),Phones.GetExtensionForEmp(_employeeCur.EmployeeNum),_employeeCur.EmployeeNum);
			}
		}

		private void butDeposit_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.DepositSlips,DateTime.Today)) {
				return;
			}
			using FormDeposits formDeposits=new FormDeposits();
			formDeposits.ShowDialog();
		}

		private void butEmailInbox_Click(object sender,EventArgs e) {
      if(_formEmailInbox==null || _formEmailInbox.IsDisposed) {
        _formEmailInbox=null;
        _formEmailInbox=new FormEmailInbox();
        _formEmailInbox.Show();
      }
      else {
        if(_formEmailInbox.WindowState==FormWindowState.Minimized) {
          _formEmailInbox.WindowState=FormWindowState.Maximized;
        }
        _formEmailInbox.BringToFront();
      }
    }

		private void butEras_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.InsPayCreate)) {
				return;
			}
			Cursor=Cursors.WaitCursor;
			FormEtrans835s formEtrans835s=new FormEtrans835s();
			formEtrans835s.Show();//non-modal
			Cursor=Cursors.Default;
		}

		private void butImportInsPlans_Click(object sender,EventArgs e) {
			if(_formEtrans834Import!=null && _formEtrans834Import.FormEtrans834PreviewCur!=null && !_formEtrans834Import.FormEtrans834PreviewCur.IsDisposed) {
				_formEtrans834Import.FormEtrans834PreviewCur.Show();
				_formEtrans834Import.FormEtrans834PreviewCur.BringToFront();
				return;
			}
			if(_formEtrans834Import==null || _formEtrans834Import.IsDisposed) {
				_formEtrans834Import=new FormEtrans834Import();
			}
			_formEtrans834Import.Show();
			_formEtrans834Import.BringToFront();
		}

		private void butManage_Click(object sender,EventArgs e) {
			using FormTimeCardManage formTimeCardManage=new FormTimeCardManage(_listEmployees);
			formTimeCardManage.ShowDialog();
			ModuleSelected(_numPatCur);
		}

		private void butManageAR_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.Billing)) {
				return;
			}
			if(!Programs.IsEnabled(ProgramName.Transworld)) {
				try {
					Process.Start("https://opendental.com/resources/redirects/redirecttransworldsystems.html");
				}
				catch(Exception ex) {
					ex.DoNothing();
					MsgBox.Show(this,"Failed to open web browser.  Please make sure you have a default browser set and are connected to the internet and then try again.");
				}
				return;
			}
			if(_formArManager==null || _formArManager.IsDisposed) {
				while(!ValidateConnectionDetails()) {//only validate connection details if the ArManager form does not exist yet
					string msgText="An SFTP connection could not be made using the connection details "+(PrefC.HasClinicsEnabled ? "for any clinic " : "")
						+"in the enabled Transworld (TSI) program link.  Would you like to edit the Transworld program link now?";
					if(!MsgBox.Show(this,MsgBoxButtons.YesNo,msgText)) {//if user does not want to edit program link, return
						return;
					}
					using FormTransworldSetup formTransworldSetup=new FormTransworldSetup();
					if(formTransworldSetup.ShowDialog()!=DialogResult.OK) {//if user cancels edits in the setup window, return
						return;
					}
				}
				_formArManager=new FormArManager();//connections settings have been validated, create a new ArManager form
				_formArManager.FormClosed+=new FormClosedEventHandler((o,ev) => { _formArManager=null; });//So that the form can release its objects for garbage collection
			}
			_formArManager.Restore();
			_formArManager.Show();//form has a Go To option and is shown as a non-modal window so the user can view the pat account and the collection list at the same time.
			if(_formArManager!=null) {
				//When things go wrong running aging, user is prompted to load the existing account info.  If they say no, the form closes, and the closing 
				//event handler sets _formAR=null.
				_formArManager.BringToFront();
			}
		}

		private void butSchedule_Click(object sender,EventArgs e){
			//only visible at ODHQ
			DateTime date=DateTime.Today;
			if(ODBuild.IsDebug() && Environment.MachineName=="JORDANHOME") {
				//date=new DateTime(2020,3,18);
			}
			if(_formGraphEmployeeTime!=null && !_formGraphEmployeeTime.IsDisposed) {
				_formGraphEmployeeTime.Show();
				_formGraphEmployeeTime.Restore();
				return;
			}
			_formGraphEmployeeTime=new FormGraphEmployeeTime(date);
			_formGraphEmployeeTime.Show();
		}

		private void butSendClaims_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.ClaimSend)) {
				return;
			}
			if(_formClaimsSend!=null && !_formClaimsSend.IsDisposed) {//Form is open
				_formClaimsSend.Focus();//Don't open a new form.
				//We may need to close and reopen the form in the future if the window is not being brought to the front.
				//It is complicated to Close() and reopen the form, because the user might be in the middle of a task.
				return;
			}
			Cursor=Cursors.WaitCursor;
			_formClaimsSend=new FormClaimsSend();
			_formClaimsSend.FormClosed+=(s,ea) => { ODEvent.Fired-=formClaimsSend_GoToChanged; };
			ODEvent.Fired+=formClaimsSend_GoToChanged;
			_formClaimsSend.Show();//FormClaimsSend has a GoTo option and is shown as a non-modal window.
			_formClaimsSend.BringToFront();
			Cursor=Cursors.Default;
		}

		private void butSupply_Click(object sender,EventArgs e) {
			using FormSupplyInventory formSupplyInventory=new FormSupplyInventory();
			formSupplyInventory.ShowDialog();
		}

		private void butTasks_Click(object sender,EventArgs e) {
			LaunchTaskWindow(false);
			/*  //This is the old code exactly how it was before making the task window non-modal in case issues arise.
			using FormTasks FormT=new FormTasks();
			FormT.ShowDialog();
			if(FormT.GotoType==TaskObjectType.Patient){
				if(FormT.GotoKeyNum!=0){
					Patient pat=Patients.GetPat(FormT.GotoKeyNum);
					OnPatientSelected(pat);
					GotoModule.GotoAccount(0);
				}
			}
			if(FormT.GotoType==TaskObjectType.Appointment){
				if(FormT.GotoKeyNum!=0){
					Appointment apt=Appointments.GetOneApt(FormT.GotoKeyNum);
					if(apt==null){
						MsgBox.Show(this,"Appointment has been deleted, so it's not available.");
						return;
						//this could be a little better, because window has closed, but they will learn not to push that button.
					}
					DateTime dateSelected=DateTime.MinValue;
					if(apt.AptStatus==ApptStatus.Planned || apt.AptStatus==ApptStatus.UnschedList){
						//I did not add feature to put planned or unsched apt on pinboard.
						MsgBox.Show(this,"Cannot navigate to appointment.  Use the Other Appointments button.");
						//return;
					}
					else{
						dateSelected=apt.AptDateTime;
					}
					Patient pat=Patients.GetPat(apt.PatNum);
					OnPatientSelected(pat);
					GotoModule.GotoAppointment(dateSelected,apt.AptNum);
				}
			}
			*/
		}

		private void butTimeCard_Click(object sender,EventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {
				SelectEmpI(-1);
				return;
			}
			if(PayPeriods.GetCount()==0) {
				MsgBox.Show(this,"The adminstrator needs to setup pay periods first.");
				return;
			}
			using FormTimeCard formTimeCard=new FormTimeCard(_listEmployees);
			formTimeCard.EmployeeCur=_employeeCur;
			formTimeCard.ShowDialog();
			ModuleSelected(_numPatCur);
		}

		private void butViewSched_Click(object sender,EventArgs e) {
			List<long> listPreSelectedEmpNums=gridEmp.SelectedGridRows.Select(x => ((Employee)x.Tag).EmployeeNum).ToList();
			List<long> listPreSelectedProvNums=Userods.GetWhere(x => listPreSelectedEmpNums.Contains(x.EmployeeNum) && x.ProvNum!=0)
				.Select(x => x.ProvNum)
				.ToList();
			using FormSchedule formSchedule=new FormSchedule(listPreSelectedEmpNums,listPreSelectedProvNums);
			formSchedule.ShowDialog();
		}
		#endregion Methods - Event Handlers - Buttons General

		#region Methods - Event Handlers - Messaging
		private void butAck_Click(object sender,EventArgs e) {
			if(gridMessages.SelectedIndices.Length==0) {
				MsgBox.Show(this,"Please select at least one item first.");
				return;
			}
			SigMessage sigMessage;
			for(int i=gridMessages.SelectedIndices.Length-1;i>=0;i--) {//go backwards so that we can remove rows without problems.
				sigMessage=(SigMessage)gridMessages.ListGridRows[gridMessages.SelectedIndices[i]].Tag;
				if(sigMessage.AckDateTime.Year>1880) {
					continue;//totally ignore if trying to ack a previously acked signal
				}
				SigMessages.AckSigMessage(sigMessage);
				//change the grid temporarily until the next timer event.  This makes it feel more responsive.
				if(checkIncludeAck.Checked) {
					gridMessages.ListGridRows[gridMessages.SelectedIndices[i]].Cells[3].Text=sigMessage.MessageDateTime.ToShortTimeString();
				}
				else {
					try {
						gridMessages.ListGridRows.RemoveAt(gridMessages.SelectedIndices[i]);
					}
					catch {
						//do nothing
					}
				}
				Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
			}
			gridMessages.SetAll(false);
		}

		private void butSend_Click(object sender,EventArgs e) {
			if(textMessage.Text=="") {
				MsgBox.Show(this,"Please type in a message first.");
				return;
			}
			SigMessage sigMessage=new SigMessage();
			sigMessage.SigText=textMessage.Text;
			if(listBoxTo.SelectedIndex!=-1) {
				sigMessage.ToUser=_arraySigElementDefUsers[listBoxTo.SelectedIndex].SigText;
				sigMessage.SigElementDefNumUser=_arraySigElementDefUsers[listBoxTo.SelectedIndex].SigElementDefNum;
			}
			if(listBoxFrom.SelectedIndex!=-1) {
				sigMessage.FromUser=_arraySigElementDefUsers[listBoxFrom.SelectedIndex].SigText;
			}
			if(listBoxExtras.SelectedIndex!=-1) {
				sigMessage.SigElementDefNumExtra=_arraySigElementDefExtras[listBoxExtras.SelectedIndex].SigElementDefNum;
			}
			SigMessages.Insert(sigMessage);
			textMessage.Text="";
			listBoxFrom.SelectedIndex=-1;
			listBoxTo.SelectedIndex=-1;
			listBoxExtras.SelectedIndex=-1;
			listBoxMessages.SelectedIndex=-1;
			ShowSendingLabel();
			Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
		}

		private void checkIncludeAck_Click(object sender,EventArgs e) {
			if(checkIncludeAck.Checked) {
				textDays.Text="1";
				labelDays.Visible=true;
				textDays.Visible=true;
			}
			else{
				labelDays.Visible=false;
				textDays.Visible=false;
				_listSigMessages=SigMessages.GetSigMessagesSinceDateTime(DateTime.Today);//since midnight this morning.
			}
			FillMessages();
		}

		private void comboViewUser_SelectionChangeCommitted(object sender,EventArgs e) {
			FillMessages();
		}

		private void listMessages_Click(object sender,EventArgs e) {
			if(listBoxMessages.SelectedIndex==-1) {
				return;
			}
			SigMessage sigMessage=new SigMessage();
			sigMessage.SigText=textMessage.Text;
			if(listBoxTo.SelectedIndex!=-1) {
				sigMessage.ToUser=_arraySigElementDefUsers[listBoxTo.SelectedIndex].SigText;
				sigMessage.SigElementDefNumUser=_arraySigElementDefUsers[listBoxTo.SelectedIndex].SigElementDefNum;
			}
			if(listBoxFrom.SelectedIndex!=-1) {
				sigMessage.FromUser=_arraySigElementDefUsers[listBoxFrom.SelectedIndex].SigText;
				//We do not set a SigElementDefNumUser for From.
			}
			if(listBoxExtras.SelectedIndex!=-1) {
				sigMessage.SigElementDefNumExtra=_arraySigElementDefExtras[listBoxExtras.SelectedIndex].SigElementDefNum;
			}
			sigMessage.SigElementDefNumMsg=_arraySigElementDefMessages[listBoxMessages.SelectedIndex].SigElementDefNum;
			//need to do this all as a transaction, so need to do a writelock on the signal table first.
			//alternatively, we could just make sure not to retrieve any signals that were less the 300ms old.
			SigMessages.Insert(sigMessage);
			//reset the controls
			textMessage.Text="";
			listBoxFrom.SelectedIndex=-1;
			listBoxTo.SelectedIndex=-1;
			listBoxExtras.SelectedIndex=-1;
			listBoxMessages.SelectedIndex=-1;
			ShowSendingLabel();
			Signalods.SetInvalid(InvalidType.SigMessages,KeyType.SigMessage,sigMessage.SigMessageNum);
		}

		private void textDays_TextChanged(object sender,EventArgs e) {
			if(!textDays.Visible) {
				_errorProvider1.SetError(textDays,"");
				return;
			}
			try{
				int numDays=int.Parse(textDays.Text);
				_errorProvider1.SetError(textDays,"");
				_listSigMessages=SigMessages.GetSigMessagesSinceDateTime(DateTime.Today.AddDays(-numDays));
				FillMessages();
			}
			catch{
				_errorProvider1.SetError(textDays,Lan.g(this,"Invalid number.  Usually 1 or 2."));
			}
		}
		#endregion Methods - Event Handlers - Messaging

		#region Methods - Event Handlers - Other
		private void ControlManage_Load(object sender,EventArgs e) {
			if(!PrefC.IsODHQ) {
				butSchedule.Visible=false;
			}
		}

		private void formClaimsSend_GoToChanged(ODEventArgs e) {
			if(e.EventType!=ODEventType.FormClaimSend_GoTo) {
				return;
			}
			ClaimSendQueueItem claimSendQueueItem=(ClaimSendQueueItem)e.Tag;
			Patient pat=Patients.GetPat(claimSendQueueItem.PatNum);
			FormOpenDental.S_Contr_PatientSelected(pat,false);
			GotoModule.GotoClaim(claimSendQueueItem.ClaimNum);
		}

		private void gridEmp_CellClick(object sender,ODGridClickEventArgs e) {
			if(gridEmp.SelectedIndices.Length>=2) {
				SelectEmpI(-1,false);//Disable various UI elements.
				return;
			}
			if(PrefC.GetBool(PrefName.TimecardSecurityEnabled)) {
				if(Security.CurUser.EmployeeNum!=((Employee)gridEmp.ListGridRows[e.Row].Tag).EmployeeNum) {
					if(!Security.IsAuthorized(Permissions.TimecardsEditAll,true)){
						SelectEmpI(-1,false);
						return;
					}
				}
			}
			SelectEmpI(e.Row);
		}

		private void gridEmp_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(gridEmp.SelectedGridRows.Count>1) {//Just in case
				return;
			}
			if(PayPeriods.GetCount()==0) {
				MsgBox.Show(this,"The adminstrator needs to setup pay periods first.");
				return;
			}
			if(!butTimeCard.Enabled) {
				return;
			}
			using FormTimeCard formTimeCard=new FormTimeCard(_listEmployees);
			formTimeCard.EmployeeCur=(Employee)gridEmp.ListGridRows[e.Row].Tag;
			formTimeCard.ShowDialog();
			ModuleSelected(_numPatCur);
		}

		private void listStatus_Click(object sender,EventArgs e) {
			//
		}

		private void textFilterName_TextChanged(object sender,EventArgs e) {
			FillEmps(false);
		}

		private void timerUpdateTime_Tick(object sender,EventArgs e) {
			//this will happen once a second
			if(this.Visible) {
				labelTime.Text=(DateTime.Now+_timeDelta).ToLongTimeString();
			}
		}
		#endregion Methods - Event Handlers - Other

		#region Methods - Public
		///<summary>Only gets run on startup.</summary>
		public void InitializeOnStartup() {
			//if(InitializedOnStartup) {
			//	return;
			//}
			//InitializedOnStartup=true;
			//can't use Lan.F
			Lan.C(this,new Control[]
				{
				groupBox3,//groupBox3 is the 'Daily' grouping.
				groupBox2,//groupBox2 is the 'Messaging' grouping.
				label1,//label1 is the 'To' text.
				butSend,
				groupBox1,//groupBox2 is the 'Time Clock' grouping.
				labelFilterName,
				butManage,
				butTimeCard,
				labelCurrentTime,
				butClaimPay,
				butClockIn,
				butClockOut,
				butEmailInbox,
				butSendClaims,
				butBilling,
				butDeposit,
				butSupply,
				butTasks,
				butBackup,
				butAccounting,
				butBreaks,
				butViewSched,
				label3,//label3 is the 'From' text.
				label4,//label4 is the 'Extras' text.
				label5,//label5 is the 'Message (&& Send)' text.
				label7,//label7 is the 'Message' text.
				labelSending,
				checkIncludeAck,
				labelDays,
				butAck,
				label6,//label6 is the 'To User' text.
				gridEmp,
				gridMessages,
				});
			RefreshFullMessages();//after this, messages just get added to the list.
			//But if checkIncludeAck is clicked,then it does RefreshMessages again.
		}

		///<summary>Only used internally to launch the task window with the Triage task list.</summary>
		public void JumpToTriageTaskWindow() {
			LaunchTaskWindow(true);
		}

		///<summary>Used to launch the task window preloaded with a certain task list open.  isTriage is only used at OD HQ.</summary>
		public void LaunchTaskWindow(bool isTriage,UserControlTasksTab tab=UserControlTasksTab.Invalid) {
			FormTasks FormTasks=new FormTasks();
			FormTasks.Show();
			if(isTriage) {
				FormTasks.ShowTriage();
			}
			else if(tab!=UserControlTasksTab.Invalid) {
				FormTasks.TaskTab=tab;
			}
		}

		///<summary></summary>
		public void ModuleSelected(long patNum) {
			_numPatCur=patNum;
			RefreshModuleData(patNum);
			RefreshModuleScreen();
			Plugins.HookAddCode(this,"ContrStaff.ModuleSelected_end",patNum);
		}

		///<summary></summary>
		public void ModuleUnselected() {
			//this is not getting triggered yet.
			Plugins.HookAddCode(this,"ContrStaff.ModuleUnselected_end");
		}

		public void TryRefreshFormClaimSend() {
			if(_formClaimsSend!=null && !FormODBase.IsDisposedOrClosed(_formClaimsSend)) {
				_formClaimsSend.RefreshClaimsGrid();
			}
		}
		#endregion Methods - Public

		#region Methods - Public - Messaging
		///<summary>This processes timed messages coming in from the main form.  Buttons are handled in the main form, and then sent here for further display.  The list gets filtered before display.</summary>
		public void LogMsgs(List<SigMessage> listSigMessages) {
			foreach(SigMessage sigMessage in listSigMessages) {
				SigMessage sigMessageUpdate=_listSigMessages.FirstOrDefault(x => x.SigMessageNum==sigMessage.SigMessageNum);
				if(sigMessageUpdate==null) {
					_listSigMessages.Add(sigMessage.Copy());
				}
				else {//SigMessage is already in our list and we just need to update it.
					sigMessageUpdate.AckDateTime=sigMessage.AckDateTime;
				}
			}
			_listSigMessages.Sort();
			FillMessages();
		}
		#endregion Methods - Public - Messaging

		#region Methods - Private
		/// <summary>Returns a translated TimeClockStatus enum description from the given status.  Also considers PrefName.ClockEventAllowBreak to switch 'Lunch' to 'Break' for the UI.</summary>
		private string ConvertClockStatus(string status) {
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak) && status==TimeClockStatus.Lunch.GetDescription()) {
				status=TimeClockStatus.Break.GetDescription();
			}
			return Lans.g("enumTimeClockStatus",status);
		}

		private void FillEmps(bool doSelectCurUserEmployee) {
			gridEmp.BeginUpdate();
			gridEmp.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableEmpClock","Employee"),180);
			gridEmp.Columns.Add(col);
			col=new GridColumn(Lan.g("TableEmpClock","Status"),104);
			gridEmp.Columns.Add(col);
			gridEmp.ListGridRows.Clear();
			GridRow row;
			if(PrefC.HasClinicsEnabled) {
				_listEmployees=Employees.GetEmpsForClinic(Clinics.ClinicNum,false,true);
			}
			else {
				_listEmployees=Employees.GetDeepCopy(true);
			}
			foreach(Employee emp in _listEmployees) {
				if(textFilterName.Text!="" && !emp.FName.ToLower().StartsWith(textFilterName.Text.ToLower())) {
					continue;
				}
				row=new GridRow();
				row.Cells.Add(Employees.GetNameFL(emp));
				row.Cells.Add(ConvertClockStatus(emp.ClockStatus));//Translated in function.
				row.Tag=emp;
				gridEmp.ListGridRows.Add(row);
			}
			gridEmp.EndUpdate();
			listBoxStatus.Items.Clear();
			_listShownTimeClockStatuses.Clear();
			foreach(TimeClockStatus timeClockStatus in Enum.GetValues(typeof(TimeClockStatus))){
				string statusDescript=timeClockStatus.GetDescription();
				if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {
					if(timeClockStatus==TimeClockStatus.Break) {
						continue;//Skip Break option.
					}
					else if(timeClockStatus==TimeClockStatus.Lunch) {
						statusDescript=TimeClockStatus.Break.GetDescription();//Change "Lunch" to "Break", still functions as Lunch.
					}
				}
				_listShownTimeClockStatuses.Add(timeClockStatus);
				listBoxStatus.Items.Add(Lan.g("enumTimeClockStatus",statusDescript));
			}
			int index=-1;
			if(doSelectCurUserEmployee) {//Only select current user's employee when refreshing the module.
				for(int i=0;i<gridEmp.ListGridRows.Count;i++) {
					Employee employee=(Employee)gridEmp.ListGridRows[i].Tag;
					if(employee.EmployeeNum==Security.CurUser.EmployeeNum) {
						index=i;
						break;
					}
				}
			}
			SelectEmpI(index);
		}

		private void RefreshModuleData(long patNum) {
      if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
        _timeDelta=new TimeSpan(0);
      }
      else {
        _timeDelta=MiscData.GetNowDateTime()-DateTime.Now;
      }
			Employees.RefreshCache();
			//RefreshModulePatient(patNum);
		}

		///<summary>Here so it's parallel with other modules.</summary>
		//private void RefreshModulePatient(int patNum){
		//	PatCurNum=patNum;
		//	if(patNum==0){
		//		OnPatientSelected(patNum,"",false,"");
		//	}
		//	else{
		//		Patient pat=Patients.GetPat(patNum);
		//		OnPatientSelected(patNum,pat.GetNameLF(),pat.Email!="",pat.ChartNumber);
		//	}
		//}

		private void RefreshModuleScreen() {
      if(PrefC.GetBool(PrefName.LocalTimeOverridesServerTime)) {
        labelCurrentTime.Text=Lan.g(this,"Local Time");
      }
      else {
        labelCurrentTime.Text=Lan.g(this,"Server Time");
      }
			labelTime.Text=(DateTime.Now+_timeDelta).ToLongTimeString();
			textFilterName.Text="";
			FillEmps(true);
			FillMessageDefs();
			if(Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
				butManage.Enabled=true;
			}
			else {
				butManage.Enabled=false;
			}
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {//Breaks turned off, Lunch is now "Break", but maintains Lunch functionality.
				butBreaks.Visible=false;
			}
			else {
				butBreaks.Visible=true;
			}
			butImportInsPlans.Visible=true;
			if(PrefC.GetBool(PrefName.EasyHidePublicHealth)) {
				butImportInsPlans.Visible=false;//Import Ins Plans button is only visible when Public Health feature is enabled.
			}
			butManageAR.Visible=!ProgramProperties.IsAdvertisingDisabled(ProgramName.Transworld);
		}

		///<summary>-1 is also valid.</summary>
		private void SelectEmpI(int index,bool doClearGridSelection=true) {
			if(doClearGridSelection) {
				gridEmp.SetAll(false);
			}
			if(index==-1) {
				butClockIn.Enabled=false;
				butClockOut.Enabled=false;
				butTimeCard.Enabled=false;
				butBreaks.Enabled=false;
				listBoxStatus.Enabled=false;
				return;
			}
			gridEmp.SetSelected(index,true);
			_employeeCur=(Employee)gridEmp.ListGridRows[index].Tag;
			ClockEvent clockEvent=ClockEvents.GetLastEvent(_employeeCur.EmployeeNum);
			if(clockEvent==null) {//new employee.  They need to clock in.
				butClockIn.Enabled=true;
				butClockOut.Enabled=false;
				butTimeCard.Enabled=true;
				butBreaks.Enabled=true;
				listBoxStatus.SelectedIndex=_listShownTimeClockStatuses.IndexOf(TimeClockStatus.Home);
				listBoxStatus.Enabled=false;
			}
			else if(clockEvent.ClockStatus==TimeClockStatus.Break) {//only incomplete breaks will have been returned.
				//clocked out for break, but not clocked back in
				butClockIn.Enabled=true;
				butClockOut.Enabled=false;
				butTimeCard.Enabled=true;
				butBreaks.Enabled=true;
				if(PrefC.GetBool(PrefName.ClockEventAllowBreak)) {
					listBoxStatus.SelectedIndex=_listShownTimeClockStatuses.IndexOf(TimeClockStatus.Break);
				}
				else {
					//This will only happen when ClockEventAllowBreak has just changed to false, but employees are clocked out for TimeClockStatus.Break.
					//Because listStatus only contains TimeClockStatus.Home and TimeClockStatus.Lunch(displays as "Break"), we can't choose TimeClockStatus.Break.
					//Choose TimeClockStatus.Lunch which displays as "Break", and allow normal clocking in/out to handle transition into newly disabled 
					//preference statuses.
					listBoxStatus.SelectedIndex=_listShownTimeClockStatuses.IndexOf(TimeClockStatus.Lunch);
				}
				listBoxStatus.Enabled=false;
			}
			else {//normal clock in/out
				if(clockEvent.TimeDisplayed2.Year<1880) {//clocked in to work, but not clocked back out.
					butClockIn.Enabled=false;
					butClockOut.Enabled=true;
					butTimeCard.Enabled=true;
					butBreaks.Enabled=true;
					listBoxStatus.Enabled=true;
				}
				else {//clocked out for home or lunch.  Need to clock back in.
					butClockIn.Enabled=true;
					butClockOut.Enabled=false;
					butTimeCard.Enabled=true;
					butBreaks.Enabled=true;
					listBoxStatus.SelectedIndex=(int)clockEvent.ClockStatus;
					listBoxStatus.Enabled=false;
				}
			}
		}

		///<summary>Shows FormBilling and displays warning message if needed.  Pass 0 to show all clinics.  Make sure to check for unsent bills before calling this method.</summary>
		private void ShowBilling(long clinicNum,bool isHistStartMinDate=false) {
			bool hadListShowing=false;
			//Check to see if there is an instance of the billing list window already open that needs to be closed.
			//This can happen if multiple people are trying to send bills at the same time.
			if(_formBilling!=null && !_formBilling.IsDisposed) {
				hadListShowing=true;
				//It does not hurt to always close this window before loading a new instance, because the unsent bills are saved in the database and the entire purpose of FormBilling is the Go To feature.
				//Any statements that were showing in the old billing list window that we are about to close could potentially be stale and are now invalid and should not be sent.
				//Another good reason to close the window is when using clinics.  It was possible to show a different clinic billing list than the one chosen.
				for(int i=0;i<_formBilling.ListClinics.Count;i++) {
					if(_formBilling.ListClinics[i].ClinicNum!=clinicNum) {//For most users clinic nums will always be 0.
						//The old billing list was showing a different clinic.  No need to show the warning message in this scenario.
						hadListShowing=false;
					}
				}
				_formBilling.Close();
			}
			_formBilling=new FormBilling();
			_formBilling.ClinicNumInitial=clinicNum;
			_formBilling.IsHistoryStartMinDate=isHistStartMinDate;
			_formBilling.Show();//FormBilling has a Go To option and is shown as a non-modal window so the user can view the patient account and the billing list at the same time.
			_formBilling.BringToFront();
			if(hadListShowing) {
				MsgBox.Show(this,"These unsent bills must either be sent or deleted before a new list can be created.");
			}
		}

		///<summary>Shows FormBillingOptions and FormBilling if needed.  Pass 0 to show all clinics.  Make sure to check for unsent bills before calling this method.</summary>
		private void ShowBillingOptions(long clinicNum) {
			using FormBillingOptions formBillingOptions=new FormBillingOptions();
			formBillingOptions.ClinicNum=clinicNum;
			formBillingOptions.ShowDialog();
			if(formBillingOptions.DialogResult==DialogResult.OK) {
				ShowBilling(clinicNum,formBillingOptions.IsHistoryStartMinDate);
			}
		}

		private bool ValidateConnectionDetails() {
			Program progCur=Programs.GetCur(ProgramName.Transworld);
			List<long> listClinicNums=new List<long>();
			if(PrefC.HasClinicsEnabled) {
				listClinicNums=Clinics.GetAllForUserod(Security.CurUser).Select(x => x.ClinicNum).ToList();
				if(!Security.CurUser.ClinicIsRestricted) {
					listClinicNums.Add(0);
				}
			}
			else {
				listClinicNums.Add(0);
			}
			List<ProgramProperty> listProgramProperties=ProgramProperties.GetForProgram(progCur.ProgramNum);
			foreach(long clinicNum in listClinicNums) {
				List<ProgramProperty> listProgPropsForClinic=new List<ProgramProperty>();
				if(listProgramProperties.All(x => x.ClinicNum!=clinicNum)) {//if no prog props exist for the clinic, continue, clinicNum 0 will be tested once as well
					continue;
				}
				listProgPropsForClinic=listProgramProperties.FindAll(x => x.ClinicNum==clinicNum);
				string sftpAddress=listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpServerAddress")?.PropertyValue??"";
				int sftpPort;
				if(!int.TryParse(listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpServerPort")?.PropertyValue??"",out sftpPort)) {
					sftpPort=22;//default to port 22
				}
				string userName=listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpUsername")?.PropertyValue??"";
				string userPassword=CDT.Class1.TryDecrypt(listProgPropsForClinic.Find(x => x.PropertyDesc=="SftpPassword")?.PropertyValue??"");
				if(Sftp.IsConnectionValid(sftpAddress,userName,userPassword,sftpPort)) {
					return true;
				}
			}
			return false;
		}
		#endregion Methods - Private

		#region Methods - Private - Messaging
		///<summary>Gets run with each module selected.  Should be very fast.</summary>
		private void FillMessageDefs() {
			_arraySigElementDefUsers=SigElementDefs.GetSubList(SignalElementType.User);
			_arraySigElementDefExtras=SigElementDefs.GetSubList(SignalElementType.Extra);
			_arraySigElementDefMessages=SigElementDefs.GetSubList(SignalElementType.Message);
			listBoxTo.Items.Clear();
			listBoxTo.Items.AddList(_arraySigElementDefUsers,x => x.SigText);
			listBoxFrom.Items.Clear();
			listBoxFrom.Items.AddList(_arraySigElementDefUsers,x => x.SigText);
			listBoxExtras.Items.Clear();
			listBoxExtras.Items.AddList(_arraySigElementDefExtras,x => x.SigText);
			listBoxMessages.Items.Clear();
			listBoxMessages.Items.AddList(_arraySigElementDefMessages,x => x.SigText);
			comboBoxViewUser.Items.Clear();
			comboBoxViewUser.Items.Add(Lan.g(this,"all"));
			for(int i=0;i<_arraySigElementDefUsers.Length;i++) {
				comboBoxViewUser.Items.Add(_arraySigElementDefUsers[i].SigText);
			}
			comboBoxViewUser.SelectedIndex=0;
		}

		///<summary>This does not refresh any data, just fills the grid.</summary>
		private void FillMessages() {
			if(textDays.Visible && _errorProvider1.GetError(textDays) !="") {
				return;
			}
			List<long> listSelectedSigMessageNums=gridMessages.SelectedTags<SigMessage>().Select(x => x.SigMessageNum).ToList();
			gridMessages.BeginUpdate();
			gridMessages.Columns.Clear();
			GridColumn col=new GridColumn(Lan.g("TableTextMessages","To"),60);
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","From"),60);
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","Sent"),63);
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","Ack'd"),63);
			col.TextAlign=HorizontalAlignment.Center;
			gridMessages.Columns.Add(col);
			col=new GridColumn(Lan.g("TableTextMessages","Text"),274);
			gridMessages.Columns.Add(col);
			gridMessages.ListGridRows.Clear();
			GridRow row;
			string strSigText;
			foreach(SigMessage sigMessage in _listSigMessages) {
				if(checkIncludeAck.Checked) {
					if(sigMessage.AckDateTime.Year>1880//if this is acked
						&& sigMessage.AckDateTime<DateTime.Today.AddDays(1-PIn.Long(textDays.Text))) {
						continue;
					}
				}
				else {//user does not want to include acked
					if(sigMessage.AckDateTime.Year>1880) {//if this is acked
						continue;
					}
				}
				if(sigMessage.ToUser!=""//blank user always shows
					&& comboBoxViewUser.SelectedIndex!=0 //anything other than 'all'
					&& _arraySigElementDefUsers!=null//for startup
					&& _arraySigElementDefUsers[comboBoxViewUser.SelectedIndex-1].SigText!=sigMessage.ToUser)//and users don't match
				{
					continue;
				}
				row=new GridRow();
				row.Cells.Add(sigMessage.ToUser);
				row.Cells.Add(sigMessage.FromUser);
				if(sigMessage.MessageDateTime.Date==DateTime.Today) {
					row.Cells.Add(sigMessage.MessageDateTime.ToShortTimeString());
				}
				else {
					row.Cells.Add(sigMessage.MessageDateTime.ToShortDateString()+"\r\n"+sigMessage.MessageDateTime.ToShortTimeString());
				}
				if(sigMessage.AckDateTime.Year>1880) {//ok
					if(sigMessage.AckDateTime.Date==DateTime.Today) {
						row.Cells.Add(sigMessage.AckDateTime.ToShortTimeString());
					}
					else {
						row.Cells.Add(sigMessage.AckDateTime.ToShortDateString()+"\r\n"+sigMessage.AckDateTime.ToShortTimeString());
					}
				}
				else {
					row.Cells.Add("");
				}
				strSigText=sigMessage.SigText;
				SigElementDef sigElementDefExtra=SigElementDefs.GetElementDef(sigMessage.SigElementDefNumExtra);
				if(sigElementDefExtra!=null && !string.IsNullOrEmpty(sigElementDefExtra.SigText)) {
					strSigText+=(strSigText=="")?"":".  ";
					strSigText+=sigElementDefExtra.SigText;
				}
				SigElementDef sigElementDefMsg=SigElementDefs.GetElementDef(sigMessage.SigElementDefNumMsg);
				if(sigElementDefMsg!=null && !string.IsNullOrEmpty(sigElementDefMsg.SigText)) {
					strSigText+=(strSigText=="")?"":".  ";
					strSigText+=sigElementDefMsg.SigText;
				}
				row.Cells.Add(strSigText);
				row.Tag=sigMessage.Copy();
				gridMessages.ListGridRows.Add(row);
			}
			gridMessages.EndUpdate();
			for(int i=0;i<gridMessages.ListGridRows.Count;i++) {
				SigMessage sigMessage=(SigMessage)gridMessages.ListGridRows[i].Tag;
				if(listSelectedSigMessageNums.Contains(sigMessage.SigMessageNum)) {
					gridMessages.SetSelected(i,true);
				}
			}
		}

		///<summary>Gets all new data from the database for the text messages.  Not sure yet if this will also reset the lights along the left.</summary>
		private void RefreshFullMessages() {
			_listSigMessages=SigMessages.GetSigMessagesSinceDateTime(DateTime.Today);//since midnight this morning.
			FillMessages();
		}

		///<summary>Shows the sending label for 1 second.</summary>
		private void ShowSendingLabel() {
			labelSending.Visible=true;
			ODThread odThread=new ODThread((o) => {
				Thread.Sleep((int)TimeSpan.FromSeconds(1).TotalMilliseconds);
				ODException.SwallowAnyException(() => { this.Invoke(() => { labelSending.Visible=false; }); });
			});
			odThread.Start();
		}
		#endregion Methods - Private - Messaging
	}

}












