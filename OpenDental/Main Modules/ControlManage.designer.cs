using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using CodeBase;
using OpenDental.UI;
using OpenDentalCloud;
using OpenDentBusiness;

namespace OpenDental {
	partial class ControlManage {

		#region Dispose
		///<summary></summary>
		protected override void Dispose( bool disposing ){
			if( disposing ){
				if(components != null){
					components.Dispose();
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
			this.groupBox3 = new OpenDental.UI.GroupBox();
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
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.listBoxMessages = new OpenDental.UI.ListBox();
			this.butSend = new OpenDental.UI.Button();
			this.butAck = new OpenDental.UI.Button();
			this.labelSending = new System.Windows.Forms.Label();
			this.textDays = new System.Windows.Forms.TextBox();
			this.labelDays = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxViewUser = new OpenDental.UI.ComboBox();
			this.gridMessages = new OpenDental.UI.GridOD();
			this.checkIncludeAck = new OpenDental.UI.CheckBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.listBoxExtras = new OpenDental.UI.ListBox();
			this.label4 = new System.Windows.Forms.Label();
			this.listBoxFrom = new OpenDental.UI.ListBox();
			this.label3 = new System.Windows.Forms.Label();
			this.listBoxTo = new OpenDental.UI.ListBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textMessage = new System.Windows.Forms.TextBox();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.textFilterName = new System.Windows.Forms.TextBox();
			this.labelFilterName = new System.Windows.Forms.Label();
			this.butViewSched = new OpenDental.UI.Button();
			this.butManage = new OpenDental.UI.Button();
			this.butBreaks = new OpenDental.UI.Button();
			this.gridEmp = new OpenDental.UI.GridOD();
			this.labelCurrentTime = new System.Windows.Forms.Label();
			this.listBoxStatus = new OpenDental.UI.ListBox();
			this.butClockOut = new OpenDental.UI.Button();
			this.butTimeCard = new OpenDental.UI.Button();
			this.labelTime = new System.Windows.Forms.Label();
			this.butClockIn = new OpenDental.UI.Button();
			this.butDaycare = new OpenDental.UI.Button();
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
			this.label5.Size = new System.Drawing.Size(124, 16);
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
			// butDaycare
			// 
			this.butDaycare.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDaycare.Location = new System.Drawing.Point(801, 130);
			this.butDaycare.Name = "butDaycare";
			this.butDaycare.Size = new System.Drawing.Size(79, 25);
			this.butDaycare.TabIndex = 33;
			this.butDaycare.Text = "Daycare";
			this.butDaycare.Click += new System.EventHandler(this.butDaycare_Click);
			// 
			// ControlManage
			// 
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.butDaycare);
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
		private OpenDental.UI.CheckBox checkIncludeAck;
		private UI.ComboBox comboBoxViewUser;
		private GridOD gridEmp;
		private GridOD gridMessages;
		private OpenDental.UI.GroupBox groupBox1;
		private OpenDental.UI.GroupBox groupBox2;
		private OpenDental.UI.GroupBox groupBox3;
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
		private OpenDental.UI.ListBox listBoxExtras;
		private OpenDental.UI.ListBox listBoxFrom;
		private OpenDental.UI.ListBox listBoxMessages;
		private OpenDental.UI.ListBox listBoxStatus;
		private OpenDental.UI.ListBox listBoxTo;
		private System.Windows.Forms.TextBox textDays;
		private System.Windows.Forms.TextBox textMessage;
		private System.ComponentModel.IContainer components;
		private Label labelFilterName;
		private System.Windows.Forms.TextBox textFilterName;
		private System.Windows.Forms.Timer timerUpdateTime;
		#endregion Fields - Private - Windows Forms
		private UI.Button butDaycare;
	}
}
