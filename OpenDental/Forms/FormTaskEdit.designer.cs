using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormTaskEdit {
		private System.ComponentModel.IContainer components=null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				if(components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTaskEdit));
			this.timePickerReminder = new System.Windows.Forms.DateTimePicker();
			this.butCreateJob = new OpenDental.UI.Button();
			this.label6 = new System.Windows.Forms.Label();
			this.labelJobs = new System.Windows.Forms.Label();
			this.panelRepeating = new OpenDental.UI.PanelOD();
			this.labelDateTask = new System.Windows.Forms.Label();
			this.labelDateType = new System.Windows.Forms.Label();
			this.checkFromNum = new System.Windows.Forms.CheckBox();
			this.textDateTask = new OpenDental.ValidDate();
			this.comboDateType = new System.Windows.Forms.ComboBox();
			this.labelDateAdvice = new System.Windows.Forms.Label();
			this.groupReminder = new System.Windows.Forms.GroupBox();
			this.datePickerReminder = new System.Windows.Forms.DateTimePicker();
			this.panelReminderFrequency = new OpenDental.UI.PanelOD();
			this.labelRemindFrequency = new System.Windows.Forms.Label();
			this.textReminderRepeatFrequency = new OpenDental.ValidNum();
			this.label2 = new System.Windows.Forms.Label();
			this.labelReminderRepeat = new System.Windows.Forms.Label();
			this.comboReminderRepeat = new System.Windows.Forms.ComboBox();
			this.panelReminderDays = new OpenDental.UI.PanelOD();
			this.labelReminderRepeatDayKey = new System.Windows.Forms.Label();
			this.labelReminderRepeatDays = new System.Windows.Forms.Label();
			this.checkReminderRepeatMonday = new System.Windows.Forms.CheckBox();
			this.checkReminderRepeatSunday = new System.Windows.Forms.CheckBox();
			this.checkReminderRepeatTuesday = new System.Windows.Forms.CheckBox();
			this.checkReminderRepeatSaturday = new System.Windows.Forms.CheckBox();
			this.checkReminderRepeatWednesday = new System.Windows.Forms.CheckBox();
			this.checkReminderRepeatFriday = new System.Windows.Forms.CheckBox();
			this.checkReminderRepeatThursday = new System.Windows.Forms.CheckBox();
			this.comboJobs = new System.Windows.Forms.ComboBox();
			this.butAudit = new OpenDental.UI.Button();
			this.butColor = new System.Windows.Forms.Button();
			this.label8 = new System.Windows.Forms.Label();
			this.comboTaskPriorities = new System.Windows.Forms.ComboBox();
			this.textBox1 = new System.Windows.Forms.TextBox();
			this.textTaskNum = new System.Windows.Forms.TextBox();
			this.labelTaskNum = new System.Windows.Forms.Label();
			this.checkDone = new System.Windows.Forms.CheckBox();
			this.labelDoneAffectsAll = new System.Windows.Forms.Label();
			this.checkNew = new System.Windows.Forms.CheckBox();
			this.butChangeUser = new OpenDental.UI.Button();
			this.butAddNote = new OpenDental.UI.Button();
			this.textTaskList = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.butSend = new OpenDental.UI.Button();
			this.labelReply = new System.Windows.Forms.Label();
			this.butReply = new OpenDental.UI.Button();
			this.butNowFinished = new OpenDental.UI.Button();
			this.textDateTimeFinished = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textUser = new System.Windows.Forms.TextBox();
			this.label16 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.butNow = new OpenDental.UI.Button();
			this.textDateTimeEntry = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.panelObject = new System.Windows.Forms.Panel();
			this.textObjectDesc = new System.Windows.Forms.TextBox();
			this.labelObjectDesc = new System.Windows.Forms.Label();
			this.butGoto = new OpenDental.UI.Button();
			this.butChange = new OpenDental.UI.Button();
			this.listObjectType = new OpenDental.UI.ListBoxOD();
			this.butCopy = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.splitContainerDescriptNote = new OpenDental.SplitContainerNoFlicker();
			this.textDescriptOverride = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butEditAutoNote = new OpenDental.UI.Button();
			this.butAutoNote = new OpenDental.UI.Button();
			this.butBlue = new OpenDental.UI.Button();
			this.butRed = new OpenDental.UI.Button();
			this.textDescript = new OpenDental.ODtextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.labelTaskChanged = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.textBoxDateTimeCreated = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.panelRepeating.SuspendLayout();
			this.groupReminder.SuspendLayout();
			this.panelReminderFrequency.SuspendLayout();
			this.panelReminderDays.SuspendLayout();
			this.panelObject.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerDescriptNote)).BeginInit();
			this.splitContainerDescriptNote.Panel1.SuspendLayout();
			this.splitContainerDescriptNote.Panel2.SuspendLayout();
			this.splitContainerDescriptNote.SuspendLayout();
			this.SuspendLayout();
			// 
			// timePickerReminder
			// 
			this.timePickerReminder.CustomFormat = "hh:mm:ss tt";
			this.timePickerReminder.Format = System.Windows.Forms.DateTimePickerFormat.Time;
			this.timePickerReminder.Location = new System.Drawing.Point(150, 36);
			this.timePickerReminder.Name = "timePickerReminder";
			this.timePickerReminder.ShowUpDown = true;
			this.timePickerReminder.Size = new System.Drawing.Size(89, 20);
			this.timePickerReminder.TabIndex = 4;
			this.timePickerReminder.Visible = false;
			// 
			// butCreateJob
			// 
			this.butCreateJob.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCreateJob.Location = new System.Drawing.Point(702, 567);
			this.butCreateJob.Name = "butCreateJob";
			this.butCreateJob.Size = new System.Drawing.Size(75, 24);
			this.butCreateJob.TabIndex = 172;
			this.butCreateJob.Text = "Create Job";
			this.butCreateJob.Visible = false;
			this.butCreateJob.Click += new System.EventHandler(this.butCreateJob_Click);
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(312, 564);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(116, 19);
			this.label6.TabIndex = 14;
			this.label6.Text = "Object Type";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelJobs
			// 
			this.labelJobs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelJobs.Location = new System.Drawing.Point(382, 540);
			this.labelJobs.Name = "labelJobs";
			this.labelJobs.Size = new System.Drawing.Size(47, 19);
			this.labelJobs.TabIndex = 162;
			this.labelJobs.Text = "Jobs";
			this.labelJobs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelJobs.Visible = false;
			// 
			// panelRepeating
			// 
			this.panelRepeating.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelRepeating.Controls.Add(this.labelDateTask);
			this.panelRepeating.Controls.Add(this.labelDateType);
			this.panelRepeating.Controls.Add(this.checkFromNum);
			this.panelRepeating.Controls.Add(this.textDateTask);
			this.panelRepeating.Controls.Add(this.comboDateType);
			this.panelRepeating.Controls.Add(this.labelDateAdvice);
			this.panelRepeating.Location = new System.Drawing.Point(12, 535);
			this.panelRepeating.Name = "panelRepeating";
			this.panelRepeating.Size = new System.Drawing.Size(383, 75);
			this.panelRepeating.TabIndex = 170;
			// 
			// labelDateTask
			// 
			this.labelDateTask.Location = new System.Drawing.Point(1, 1);
			this.labelDateTask.Name = "labelDateTask";
			this.labelDateTask.Size = new System.Drawing.Size(102, 19);
			this.labelDateTask.TabIndex = 4;
			this.labelDateTask.Text = "Date";
			this.labelDateTask.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDateType
			// 
			this.labelDateType.Location = new System.Drawing.Point(1, 27);
			this.labelDateType.Name = "labelDateType";
			this.labelDateType.Size = new System.Drawing.Size(102, 19);
			this.labelDateType.TabIndex = 7;
			this.labelDateType.Text = "Date Type";
			this.labelDateType.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkFromNum
			// 
			this.checkFromNum.CheckAlign = System.Drawing.ContentAlignment.TopRight;
			this.checkFromNum.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkFromNum.Location = new System.Drawing.Point(1, 53);
			this.checkFromNum.Name = "checkFromNum";
			this.checkFromNum.Size = new System.Drawing.Size(116, 18);
			this.checkFromNum.TabIndex = 3;
			this.checkFromNum.Text = "Is From Repeating";
			this.checkFromNum.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// textDateTask
			// 
			this.textDateTask.Location = new System.Drawing.Point(105, 1);
			this.textDateTask.Name = "textDateTask";
			this.textDateTask.Size = new System.Drawing.Size(87, 20);
			this.textDateTask.TabIndex = 2;
			// 
			// comboDateType
			// 
			this.comboDateType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDateType.FormattingEnabled = true;
			this.comboDateType.Location = new System.Drawing.Point(105, 27);
			this.comboDateType.Name = "comboDateType";
			this.comboDateType.Size = new System.Drawing.Size(145, 21);
			this.comboDateType.TabIndex = 148;
			// 
			// labelDateAdvice
			// 
			this.labelDateAdvice.Location = new System.Drawing.Point(193, -1);
			this.labelDateAdvice.Name = "labelDateAdvice";
			this.labelDateAdvice.Size = new System.Drawing.Size(185, 32);
			this.labelDateAdvice.TabIndex = 6;
			this.labelDateAdvice.Text = "Leave blank unless you want this task to show on a dated list";
			// 
			// groupReminder
			// 
			this.groupReminder.Controls.Add(this.datePickerReminder);
			this.groupReminder.Controls.Add(this.timePickerReminder);
			this.groupReminder.Controls.Add(this.panelReminderFrequency);
			this.groupReminder.Controls.Add(this.labelReminderRepeat);
			this.groupReminder.Controls.Add(this.comboReminderRepeat);
			this.groupReminder.Controls.Add(this.panelReminderDays);
			this.groupReminder.Location = new System.Drawing.Point(336, 1);
			this.groupReminder.Name = "groupReminder";
			this.groupReminder.Size = new System.Drawing.Size(242, 84);
			this.groupReminder.TabIndex = 169;
			this.groupReminder.TabStop = false;
			this.groupReminder.Text = "Reminder";
			this.groupReminder.Visible = false;
			// 
			// datePickerReminder
			// 
			this.datePickerReminder.CustomFormat = "MMMM  dd,   yyyy";
			this.datePickerReminder.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
			this.datePickerReminder.Location = new System.Drawing.Point(4, 36);
			this.datePickerReminder.Name = "datePickerReminder";
			this.datePickerReminder.Size = new System.Drawing.Size(144, 20);
			this.datePickerReminder.TabIndex = 2;
			this.datePickerReminder.Visible = false;
			// 
			// panelReminderFrequency
			// 
			this.panelReminderFrequency.Controls.Add(this.labelRemindFrequency);
			this.panelReminderFrequency.Controls.Add(this.textReminderRepeatFrequency);
			this.panelReminderFrequency.Controls.Add(this.label2);
			this.panelReminderFrequency.Location = new System.Drawing.Point(1, 34);
			this.panelReminderFrequency.Name = "panelReminderFrequency";
			this.panelReminderFrequency.Size = new System.Drawing.Size(240, 22);
			this.panelReminderFrequency.TabIndex = 2;
			this.panelReminderFrequency.TabStop = true;
			// 
			// labelRemindFrequency
			// 
			this.labelRemindFrequency.Location = new System.Drawing.Point(157, 1);
			this.labelRemindFrequency.Name = "labelRemindFrequency";
			this.labelRemindFrequency.Size = new System.Drawing.Size(80, 20);
			this.labelRemindFrequency.TabIndex = 0;
			this.labelRemindFrequency.Text = "Days";
			this.labelRemindFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textReminderRepeatFrequency
			// 
			this.textReminderRepeatFrequency.Location = new System.Drawing.Point(92, 1);
			this.textReminderRepeatFrequency.MaxVal = 999999999;
			this.textReminderRepeatFrequency.MinVal = 1;
			this.textReminderRepeatFrequency.Name = "textReminderRepeatFrequency";
			this.textReminderRepeatFrequency.ShowZero = false;
			this.textReminderRepeatFrequency.Size = new System.Drawing.Size(50, 20);
			this.textReminderRepeatFrequency.TabIndex = 1;
			this.textReminderRepeatFrequency.KeyUp += new System.Windows.Forms.KeyEventHandler(this.textReminderRepeatFrequency_KeyUp);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(1, 1);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(90, 20);
			this.label2.TabIndex = 0;
			this.label2.Text = "Every";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelReminderRepeat
			// 
			this.labelReminderRepeat.Location = new System.Drawing.Point(2, 13);
			this.labelReminderRepeat.Name = "labelReminderRepeat";
			this.labelReminderRepeat.Size = new System.Drawing.Size(90, 21);
			this.labelReminderRepeat.TabIndex = 0;
			this.labelReminderRepeat.Text = "Type";
			this.labelReminderRepeat.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboReminderRepeat
			// 
			this.comboReminderRepeat.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboReminderRepeat.FormattingEnabled = true;
			this.comboReminderRepeat.Location = new System.Drawing.Point(93, 12);
			this.comboReminderRepeat.Name = "comboReminderRepeat";
			this.comboReminderRepeat.Size = new System.Drawing.Size(145, 21);
			this.comboReminderRepeat.TabIndex = 1;
			this.comboReminderRepeat.SelectedIndexChanged += new System.EventHandler(this.comboReminderRepeat_SelectedIndexChanged);
			// 
			// panelReminderDays
			// 
			this.panelReminderDays.Controls.Add(this.labelReminderRepeatDayKey);
			this.panelReminderDays.Controls.Add(this.labelReminderRepeatDays);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatMonday);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatSunday);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatTuesday);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatSaturday);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatWednesday);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatFriday);
			this.panelReminderDays.Controls.Add(this.checkReminderRepeatThursday);
			this.panelReminderDays.Location = new System.Drawing.Point(1, 55);
			this.panelReminderDays.Name = "panelReminderDays";
			this.panelReminderDays.Size = new System.Drawing.Size(240, 28);
			this.panelReminderDays.TabIndex = 3;
			this.panelReminderDays.TabStop = true;
			// 
			// labelReminderRepeatDayKey
			// 
			this.labelReminderRepeatDayKey.Location = new System.Drawing.Point(91, 15);
			this.labelReminderRepeatDayKey.Name = "labelReminderRepeatDayKey";
			this.labelReminderRepeatDayKey.Size = new System.Drawing.Size(109, 11);
			this.labelReminderRepeatDayKey.TabIndex = 0;
			this.labelReminderRepeatDayKey.Text = "M  T  W  R  F  S  U";
			// 
			// labelReminderRepeatDays
			// 
			this.labelReminderRepeatDays.Location = new System.Drawing.Point(18, 1);
			this.labelReminderRepeatDays.Name = "labelReminderRepeatDays";
			this.labelReminderRepeatDays.Size = new System.Drawing.Size(73, 17);
			this.labelReminderRepeatDays.TabIndex = 0;
			this.labelReminderRepeatDays.Text = "Days";
			this.labelReminderRepeatDays.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatMonday
			// 
			this.checkReminderRepeatMonday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatMonday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatMonday.Location = new System.Drawing.Point(92, 0);
			this.checkReminderRepeatMonday.Name = "checkReminderRepeatMonday";
			this.checkReminderRepeatMonday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatMonday.TabIndex = 1;
			this.checkReminderRepeatMonday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatSunday
			// 
			this.checkReminderRepeatSunday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatSunday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatSunday.Location = new System.Drawing.Point(176, 0);
			this.checkReminderRepeatSunday.Name = "checkReminderRepeatSunday";
			this.checkReminderRepeatSunday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatSunday.TabIndex = 7;
			this.checkReminderRepeatSunday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatTuesday
			// 
			this.checkReminderRepeatTuesday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatTuesday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatTuesday.Location = new System.Drawing.Point(106, 0);
			this.checkReminderRepeatTuesday.Name = "checkReminderRepeatTuesday";
			this.checkReminderRepeatTuesday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatTuesday.TabIndex = 2;
			this.checkReminderRepeatTuesday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatSaturday
			// 
			this.checkReminderRepeatSaturday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatSaturday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatSaturday.Location = new System.Drawing.Point(162, 0);
			this.checkReminderRepeatSaturday.Name = "checkReminderRepeatSaturday";
			this.checkReminderRepeatSaturday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatSaturday.TabIndex = 6;
			this.checkReminderRepeatSaturday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatWednesday
			// 
			this.checkReminderRepeatWednesday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatWednesday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatWednesday.Location = new System.Drawing.Point(120, 0);
			this.checkReminderRepeatWednesday.Name = "checkReminderRepeatWednesday";
			this.checkReminderRepeatWednesday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatWednesday.TabIndex = 3;
			this.checkReminderRepeatWednesday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatFriday
			// 
			this.checkReminderRepeatFriday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatFriday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatFriday.Location = new System.Drawing.Point(148, 0);
			this.checkReminderRepeatFriday.Name = "checkReminderRepeatFriday";
			this.checkReminderRepeatFriday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatFriday.TabIndex = 5;
			this.checkReminderRepeatFriday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkReminderRepeatThursday
			// 
			this.checkReminderRepeatThursday.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkReminderRepeatThursday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReminderRepeatThursday.Location = new System.Drawing.Point(134, 0);
			this.checkReminderRepeatThursday.Name = "checkReminderRepeatThursday";
			this.checkReminderRepeatThursday.Size = new System.Drawing.Size(13, 17);
			this.checkReminderRepeatThursday.TabIndex = 4;
			this.checkReminderRepeatThursday.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboJobs
			// 
			this.comboJobs.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.comboJobs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboJobs.FormattingEnabled = true;
			this.comboJobs.Location = new System.Drawing.Point(431, 540);
			this.comboJobs.Name = "comboJobs";
			this.comboJobs.Size = new System.Drawing.Size(346, 21);
			this.comboJobs.TabIndex = 163;
			this.comboJobs.Visible = false;
			this.comboJobs.SelectedIndexChanged += new System.EventHandler(this.comboJobs_SelectedIndexChanged);
			// 
			// butAudit
			// 
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAudit.Location = new System.Drawing.Point(887, 59);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(61, 24);
			this.butAudit.TabIndex = 160;
			this.butAudit.Text = "History";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// butColor
			// 
			this.butColor.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butColor.Enabled = false;
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.butColor.Location = new System.Drawing.Point(857, 61);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(24, 21);
			this.butColor.TabIndex = 159;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label8.Location = new System.Drawing.Point(625, 61);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(94, 21);
			this.label8.TabIndex = 158;
			this.label8.Text = "Task Priority";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTaskPriorities
			// 
			this.comboTaskPriorities.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboTaskPriorities.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboTaskPriorities.FormattingEnabled = true;
			this.comboTaskPriorities.Location = new System.Drawing.Point(720, 61);
			this.comboTaskPriorities.Name = "comboTaskPriorities";
			this.comboTaskPriorities.Size = new System.Drawing.Size(134, 21);
			this.comboTaskPriorities.TabIndex = 157;
			this.comboTaskPriorities.SelectedIndexChanged += new System.EventHandler(this.comboTaskPriorities_SelectedIndexChanged);
			this.comboTaskPriorities.SelectionChangeCommitted += new System.EventHandler(this.comboTaskPriorities_SelectionChangeCommitted);
			// 
			// textBox1
			// 
			this.textBox1.Location = new System.Drawing.Point(454, -72);
			this.textBox1.Name = "textBox1";
			this.textBox1.ReadOnly = true;
			this.textBox1.Size = new System.Drawing.Size(54, 20);
			this.textBox1.TabIndex = 134;
			this.textBox1.Visible = false;
			// 
			// textTaskNum
			// 
			this.textTaskNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textTaskNum.Location = new System.Drawing.Point(894, 617);
			this.textTaskNum.Name = "textTaskNum";
			this.textTaskNum.ReadOnly = true;
			this.textTaskNum.Size = new System.Drawing.Size(54, 20);
			this.textTaskNum.TabIndex = 134;
			// 
			// labelTaskNum
			// 
			this.labelTaskNum.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTaskNum.Location = new System.Drawing.Point(819, 618);
			this.labelTaskNum.Name = "labelTaskNum";
			this.labelTaskNum.Size = new System.Drawing.Size(73, 16);
			this.labelTaskNum.TabIndex = 133;
			this.labelTaskNum.Text = "TaskNum";
			this.labelTaskNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkDone
			// 
			this.checkDone.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDone.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkDone.Location = new System.Drawing.Point(134, 3);
			this.checkDone.Name = "checkDone";
			this.checkDone.Size = new System.Drawing.Size(82, 17);
			this.checkDone.TabIndex = 153;
			this.checkDone.Text = "Done";
			this.checkDone.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkDone.Click += new System.EventHandler(this.checkDone_Click);
			// 
			// labelDoneAffectsAll
			// 
			this.labelDoneAffectsAll.Location = new System.Drawing.Point(217, 3);
			this.labelDoneAffectsAll.Name = "labelDoneAffectsAll";
			this.labelDoneAffectsAll.Size = new System.Drawing.Size(110, 17);
			this.labelDoneAffectsAll.TabIndex = 154;
			this.labelDoneAffectsAll.Text = "(affects all users)";
			this.labelDoneAffectsAll.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// checkNew
			// 
			this.checkNew.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNew.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNew.Location = new System.Drawing.Point(12, 3);
			this.checkNew.Name = "checkNew";
			this.checkNew.Size = new System.Drawing.Size(109, 17);
			this.checkNew.TabIndex = 152;
			this.checkNew.Text = "New";
			this.checkNew.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkNew.Click += new System.EventHandler(this.checkNew_Click);
			// 
			// butChangeUser
			// 
			this.butChangeUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeUser.Location = new System.Drawing.Point(857, 14);
			this.butChangeUser.Name = "butChangeUser";
			this.butChangeUser.Size = new System.Drawing.Size(24, 22);
			this.butChangeUser.TabIndex = 151;
			this.butChangeUser.Text = "...";
			this.butChangeUser.Click += new System.EventHandler(this.butChangeUser_Click);
			// 
			// butAddNote
			// 
			this.butAddNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddNote.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAddNote.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddNote.Location = new System.Drawing.Point(873, 540);
			this.butAddNote.Name = "butAddNote";
			this.butAddNote.Size = new System.Drawing.Size(75, 24);
			this.butAddNote.TabIndex = 150;
			this.butAddNote.Text = "Add";
			this.butAddNote.Click += new System.EventHandler(this.butAddNote_Click);
			// 
			// textTaskList
			// 
			this.textTaskList.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textTaskList.Location = new System.Drawing.Point(720, 39);
			this.textTaskList.Name = "textTaskList";
			this.textTaskList.ReadOnly = true;
			this.textTaskList.Size = new System.Drawing.Size(134, 20);
			this.textTaskList.TabIndex = 146;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label10.Location = new System.Drawing.Point(625, 39);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(94, 20);
			this.label10.TabIndex = 147;
			this.label10.Text = "Task List";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butSend
			// 
			this.butSend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSend.Location = new System.Drawing.Point(478, 649);
			this.butSend.Name = "butSend";
			this.butSend.Size = new System.Drawing.Size(75, 24);
			this.butSend.TabIndex = 142;
			this.butSend.Text = "&Send To...";
			this.butSend.Click += new System.EventHandler(this.butSend_Click);
			// 
			// labelReply
			// 
			this.labelReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelReply.Location = new System.Drawing.Point(312, 652);
			this.labelReply.Name = "labelReply";
			this.labelReply.Size = new System.Drawing.Size(162, 19);
			this.labelReply.TabIndex = 141;
			this.labelReply.Text = "(Send to author)";
			this.labelReply.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butReply
			// 
			this.butReply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butReply.Location = new System.Drawing.Point(233, 649);
			this.butReply.Name = "butReply";
			this.butReply.Size = new System.Drawing.Size(75, 24);
			this.butReply.TabIndex = 140;
			this.butReply.Text = "Reply";
			this.butReply.Click += new System.EventHandler(this.butReply_Click);
			// 
			// butNowFinished
			// 
			this.butNowFinished.Location = new System.Drawing.Point(264, 65);
			this.butNowFinished.Name = "butNowFinished";
			this.butNowFinished.Size = new System.Drawing.Size(62, 19);
			this.butNowFinished.TabIndex = 132;
			this.butNowFinished.Text = "Now";
			this.butNowFinished.Click += new System.EventHandler(this.butNowFinished_Click);
			// 
			// textDateTimeFinished
			// 
			this.textDateTimeFinished.Location = new System.Drawing.Point(107, 65);
			this.textDateTimeFinished.Name = "textDateTimeFinished";
			this.textDateTimeFinished.Size = new System.Drawing.Size(151, 20);
			this.textDateTimeFinished.TabIndex = 131;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(1, 64);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(105, 20);
			this.label7.TabIndex = 130;
			this.label7.Text = "Date/Time Finished";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textUser
			// 
			this.textUser.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textUser.Location = new System.Drawing.Point(720, 16);
			this.textUser.Name = "textUser";
			this.textUser.ReadOnly = true;
			this.textUser.Size = new System.Drawing.Size(134, 20);
			this.textUser.TabIndex = 0;
			// 
			// label16
			// 
			this.label16.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label16.Location = new System.Drawing.Point(625, 16);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(94, 20);
			this.label16.TabIndex = 125;
			this.label16.Text = "From User";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(21, 649);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(80, 24);
			this.butDelete.TabIndex = 124;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butNow
			// 
			this.butNow.Location = new System.Drawing.Point(264, 43);
			this.butNow.Name = "butNow";
			this.butNow.Size = new System.Drawing.Size(62, 19);
			this.butNow.TabIndex = 19;
			this.butNow.Text = "Now";
			this.butNow.Click += new System.EventHandler(this.butNow_Click);
			// 
			// textDateTimeEntry
			// 
			this.textDateTimeEntry.Location = new System.Drawing.Point(107, 43);
			this.textDateTimeEntry.Name = "textDateTimeEntry";
			this.textDateTimeEntry.Size = new System.Drawing.Size(151, 20);
			this.textDateTimeEntry.TabIndex = 18;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(1, 42);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(105, 20);
			this.label5.TabIndex = 17;
			this.label5.Text = "Date/Time Task";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panelObject
			// 
			this.panelObject.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.panelObject.Controls.Add(this.textObjectDesc);
			this.panelObject.Controls.Add(this.labelObjectDesc);
			this.panelObject.Controls.Add(this.butGoto);
			this.panelObject.Controls.Add(this.butChange);
			this.panelObject.Location = new System.Drawing.Point(3, 611);
			this.panelObject.Name = "panelObject";
			this.panelObject.Size = new System.Drawing.Size(590, 34);
			this.panelObject.TabIndex = 15;
			// 
			// textObjectDesc
			// 
			this.textObjectDesc.BackColor = System.Drawing.SystemColors.Window;
			this.textObjectDesc.Location = new System.Drawing.Point(103, 0);
			this.textObjectDesc.Multiline = true;
			this.textObjectDesc.Name = "textObjectDesc";
			this.textObjectDesc.ReadOnly = true;
			this.textObjectDesc.Size = new System.Drawing.Size(302, 34);
			this.textObjectDesc.TabIndex = 0;
			this.textObjectDesc.Text = "line 1\r\nline 2";
			// 
			// labelObjectDesc
			// 
			this.labelObjectDesc.Location = new System.Drawing.Point(26, 1);
			this.labelObjectDesc.Name = "labelObjectDesc";
			this.labelObjectDesc.Size = new System.Drawing.Size(77, 19);
			this.labelObjectDesc.TabIndex = 8;
			this.labelObjectDesc.Text = "ObjectDesc";
			this.labelObjectDesc.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butGoto
			// 
			this.butGoto.Location = new System.Drawing.Point(501, 5);
			this.butGoto.Name = "butGoto";
			this.butGoto.Size = new System.Drawing.Size(75, 24);
			this.butGoto.TabIndex = 12;
			this.butGoto.Text = "Go To";
			this.butGoto.Click += new System.EventHandler(this.butGoto_Click);
			// 
			// butChange
			// 
			this.butChange.Location = new System.Drawing.Point(418, 5);
			this.butChange.Name = "butChange";
			this.butChange.Size = new System.Drawing.Size(75, 24);
			this.butChange.TabIndex = 10;
			this.butChange.Text = "Change";
			this.butChange.Click += new System.EventHandler(this.butChange_Click);
			// 
			// listObjectType
			// 
			this.listObjectType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.listObjectType.Location = new System.Drawing.Point(431, 565);
			this.listObjectType.Name = "listObjectType";
			this.listObjectType.Size = new System.Drawing.Size(120, 43);
			this.listObjectType.TabIndex = 13;
			this.listObjectType.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listObjectType_MouseDown);
			// 
			// butCopy
			// 
			this.butCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCopy.Image = global::OpenDental.Properties.Resources.Copy;
			this.butCopy.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCopy.Location = new System.Drawing.Point(791, 540);
			this.butCopy.Name = "butCopy";
			this.butCopy.Size = new System.Drawing.Size(75, 24);
			this.butCopy.TabIndex = 4;
			this.butCopy.Text = "Copy";
			this.butCopy.Click += new System.EventHandler(this.butCopy_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(791, 649);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(873, 649);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// splitContainerDescriptNote
			// 
			this.splitContainerDescriptNote.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.splitContainerDescriptNote.Location = new System.Drawing.Point(12, 85);
			this.splitContainerDescriptNote.Name = "splitContainerDescriptNote";
			this.splitContainerDescriptNote.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerDescriptNote.Panel1
			// 
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.textDescriptOverride);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.label4);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.butEditAutoNote);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.butAutoNote);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.butBlue);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.butRed);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.textDescript);
			this.splitContainerDescriptNote.Panel1.Controls.Add(this.label1);
			this.splitContainerDescriptNote.Panel1MinSize = 106;
			// 
			// splitContainerDescriptNote.Panel2
			// 
			this.splitContainerDescriptNote.Panel2.Controls.Add(this.gridMain);
			this.splitContainerDescriptNote.Panel2MinSize = 50;
			this.splitContainerDescriptNote.Size = new System.Drawing.Size(936, 450);
			this.splitContainerDescriptNote.SplitterDistance = 126;
			this.splitContainerDescriptNote.TabIndex = 171;
			this.splitContainerDescriptNote.SplitterMoved += new System.Windows.Forms.SplitterEventHandler(this.splitContainerNoFlicker1_SplitterMoved);
			// 
			// textDescriptOverride
			// 
			this.textDescriptOverride.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescriptOverride.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textDescriptOverride.Location = new System.Drawing.Point(92, 103);
			this.textDescriptOverride.Name = "textDescriptOverride";
			this.textDescriptOverride.Size = new System.Drawing.Size(841, 20);
			this.textDescriptOverride.TabIndex = 177;
			// 
			// label4
			// 
			this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label4.Location = new System.Drawing.Point(8, 105);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(84, 18);
			this.label4.TabIndex = 159;
			this.label4.Text = "Short Descript";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butEditAutoNote
			// 
			this.butEditAutoNote.Location = new System.Drawing.Point(4, 54);
			this.butEditAutoNote.Name = "butEditAutoNote";
			this.butEditAutoNote.Size = new System.Drawing.Size(85, 24);
			this.butEditAutoNote.TabIndex = 158;
			this.butEditAutoNote.Text = "Edit Auto Note";
			this.butEditAutoNote.UseVisualStyleBackColor = true;
			this.butEditAutoNote.Click += new System.EventHandler(this.ButEditAutoNote_Click);
			// 
			// butAutoNote
			// 
			this.butAutoNote.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAutoNote.Location = new System.Drawing.Point(4, 81);
			this.butAutoNote.Name = "butAutoNote";
			this.butAutoNote.Size = new System.Drawing.Size(85, 22);
			this.butAutoNote.TabIndex = 157;
			this.butAutoNote.Text = "Auto Note";
			this.butAutoNote.Click += new System.EventHandler(this.butAutoNote_Click);
			// 
			// butBlue
			// 
			this.butBlue.Location = new System.Drawing.Point(47, 27);
			this.butBlue.Name = "butBlue";
			this.butBlue.Size = new System.Drawing.Size(42, 24);
			this.butBlue.TabIndex = 156;
			this.butBlue.Text = "Blue";
			this.butBlue.Visible = false;
			this.butBlue.Click += new System.EventHandler(this.butBlue_Click);
			// 
			// butRed
			// 
			this.butRed.Location = new System.Drawing.Point(4, 27);
			this.butRed.Name = "butRed";
			this.butRed.Size = new System.Drawing.Size(42, 24);
			this.butRed.TabIndex = 155;
			this.butRed.Text = "Red";
			this.butRed.Visible = false;
			this.butRed.Click += new System.EventHandler(this.butRed_Click);
			// 
			// textDescript
			// 
			this.textDescript.AcceptsTab = true;
			this.textDescript.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDescript.BackColor = System.Drawing.SystemColors.Window;
			this.textDescript.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.textDescript.DetectLinksEnabled = false;
			this.textDescript.DetectUrls = false;
			this.textDescript.DoShowPatNumLinks = true;
			this.textDescript.DoShowTaskNumLinks = true;
			this.textDescript.HasAutoNotes = true;
			this.textDescript.Location = new System.Drawing.Point(92, 5);
			this.textDescript.Name = "textDescript";
			this.textDescript.QuickPasteType = OpenDentBusiness.QuickPasteType.Task;
			this.textDescript.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textDescript.Size = new System.Drawing.Size(841, 97);
			this.textDescript.TabIndex = 1;
			this.textDescript.Text = "";
			this.textDescript.TextChanged += new System.EventHandler(this.textDescript_TextChanged);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(9, 4);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(84, 20);
			this.label1.TabIndex = 2;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.DoShowPatNumLinks = true;
			this.gridMain.DoShowTaskNumLinks = true;
			this.gridMain.Location = new System.Drawing.Point(0, 3);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(933, 313);
			this.gridMain.TabIndex = 149;
			this.gridMain.Title = "Notes";
			this.gridMain.TranslationName = "FormTaskEdit";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// labelTaskChanged
			// 
			this.labelTaskChanged.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTaskChanged.ForeColor = System.Drawing.Color.Red;
			this.labelTaskChanged.Location = new System.Drawing.Point(599, 624);
			this.labelTaskChanged.Name = "labelTaskChanged";
			this.labelTaskChanged.Size = new System.Drawing.Size(185, 23);
			this.labelTaskChanged.TabIndex = 173;
			this.labelTaskChanged.Text = "The task has been changed ";
			this.labelTaskChanged.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			this.labelTaskChanged.Visible = false;
			// 
			// butRefresh
			// 
			this.butRefresh.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butRefresh.Location = new System.Drawing.Point(648, 649);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(75, 24);
			this.butRefresh.TabIndex = 174;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Visible = false;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// textBoxDateTimeCreated
			// 
			this.textBoxDateTimeCreated.Location = new System.Drawing.Point(107, 21);
			this.textBoxDateTimeCreated.Name = "textBoxDateTimeCreated";
			this.textBoxDateTimeCreated.ReadOnly = true;
			this.textBoxDateTimeCreated.Size = new System.Drawing.Size(151, 20);
			this.textBoxDateTimeCreated.TabIndex = 175;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(1, 21);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(105, 20);
			this.label3.TabIndex = 176;
			this.label3.Text = "Date/Time Created";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormTaskEdit
			// 
			this.ClientSize = new System.Drawing.Size(974, 676);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textBoxDateTimeCreated);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.labelTaskChanged);
			this.Controls.Add(this.butCreateJob);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.labelJobs);
			this.Controls.Add(this.panelRepeating);
			this.Controls.Add(this.groupReminder);
			this.Controls.Add(this.comboJobs);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.butColor);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.comboTaskPriorities);
			this.Controls.Add(this.textBox1);
			this.Controls.Add(this.textTaskNum);
			this.Controls.Add(this.labelTaskNum);
			this.Controls.Add(this.checkDone);
			this.Controls.Add(this.labelDoneAffectsAll);
			this.Controls.Add(this.checkNew);
			this.Controls.Add(this.butChangeUser);
			this.Controls.Add(this.butAddNote);
			this.Controls.Add(this.textTaskList);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.butSend);
			this.Controls.Add(this.labelReply);
			this.Controls.Add(this.butReply);
			this.Controls.Add(this.butNowFinished);
			this.Controls.Add(this.textDateTimeFinished);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textUser);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butNow);
			this.Controls.Add(this.textDateTimeEntry);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.panelObject);
			this.Controls.Add(this.listObjectType);
			this.Controls.Add(this.butCopy);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.splitContainerDescriptNote);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormTaskEdit";
			this.Text = "Task";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTaskEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormTaskListEdit_Load);
			this.panelRepeating.ResumeLayout(false);
			this.panelRepeating.PerformLayout();
			this.groupReminder.ResumeLayout(false);
			this.panelReminderFrequency.ResumeLayout(false);
			this.panelReminderFrequency.PerformLayout();
			this.panelReminderDays.ResumeLayout(false);
			this.panelObject.ResumeLayout(false);
			this.panelObject.PerformLayout();
			this.splitContainerDescriptNote.Panel1.ResumeLayout(false);
			this.splitContainerDescriptNote.Panel1.PerformLayout();
			this.splitContainerDescriptNote.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerDescriptNote)).EndInit();
			this.splitContainerDescriptNote.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelDateTask;
		private System.Windows.Forms.Label labelDateAdvice;
		private System.Windows.Forms.Label labelDateType;
		private OpenDental.ODtextBox textDescript;
		private OpenDental.ValidDate textDateTask;
		private OpenDental.UI.Button butChange;
		private OpenDental.UI.Button butGoto;
		private System.Windows.Forms.CheckBox checkFromNum;
		private System.Windows.Forms.Label labelObjectDesc;
		private System.Windows.Forms.TextBox textObjectDesc;
		private OpenDental.UI.ListBoxOD listObjectType;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Panel panelObject;
		private Label label5;
		private TextBox textDateTimeEntry;
		private OpenDental.UI.Button butNow;
		private OpenDental.UI.Button butDelete;
		private TextBox textUser;
		private Label label16;
		private OpenDental.UI.Button butNowFinished;
		private TextBox textDateTimeFinished;
		private Label label7;
		private TextBox textTaskNum;
		private Label labelTaskNum;
		private Label labelReply;
		private OpenDental.UI.Button butReply;
		private OpenDental.UI.Button butSend;
		private TextBox textTaskList;
		private Label label10;
		private ComboBox comboDateType;
		private UI.GridOD gridMain;
		private UI.Button butAddNote;
		private UI.Button butChangeUser;
		private CheckBox checkNew;
		private CheckBox checkDone;
		private Label labelDoneAffectsAll;
		private UI.Button butCopy;
		private TextBox textBox1;
		private UI.Button butRed;
		private UI.Button butBlue;
		private ComboBox comboTaskPriorities;
		private Label label8;
		private System.Windows.Forms.Button butColor;
		private UI.Button butAudit;
		private ComboBox comboJobs;
		private Label labelJobs;
		private ComboBox comboReminderRepeat;
		private Label labelReminderRepeat;
		private ValidNum textReminderRepeatFrequency;
		private Label label2;
		private GroupBox groupReminder;
		private Label labelRemindFrequency;
		private Label labelReminderRepeatDays;
		private CheckBox checkReminderRepeatSunday;
		private CheckBox checkReminderRepeatSaturday;
		private CheckBox checkReminderRepeatFriday;
		private CheckBox checkReminderRepeatThursday;
		private CheckBox checkReminderRepeatWednesday;
		private CheckBox checkReminderRepeatTuesday;
		private CheckBox checkReminderRepeatMonday;
		private Label labelReminderRepeatDayKey;
		private UI.PanelOD panelReminderDays;
		private UI.PanelOD panelRepeating;
		private UI.PanelOD panelReminderFrequency;
		private UI.Button butAutoNote;
		private UI.Button butCreateJob;
		private Label labelTaskChanged;
		private TextBox textBoxDateTimeCreated;
		private Label label3;
		private UI.Button butRefresh;
		private UI.Button butEditAutoNote;
		private TextBox textDescriptOverride;
		private Label label4;
		private SplitContainerNoFlicker splitContainerDescriptNote;
		private DateTimePicker datePickerReminder;
		private DateTimePicker timePickerReminder;
	}
}
