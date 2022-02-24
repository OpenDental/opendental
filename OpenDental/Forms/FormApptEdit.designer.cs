namespace OpenDental {
	partial class FormApptEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormApptEdit));
			this.comboConfirmed = new System.Windows.Forms.ComboBox();
			this.comboUnschedStatus = new System.Windows.Forms.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboStatus = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.labelStatus = new System.Windows.Forms.Label();
			this.label24 = new System.Windows.Forms.Label();
			this.checkIsHygiene = new System.Windows.Forms.CheckBox();
			this.comboClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboAssistant = new System.Windows.Forms.ComboBox();
			this.comboProvHyg = new OpenDental.UI.ComboBoxOD();
			this.comboProv = new OpenDental.UI.ComboBoxOD();
			this.label12 = new System.Windows.Forms.Label();
			this.checkIsNewPatient = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelApptNote = new System.Windows.Forms.Label();
			this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
			this.checkTimeLocked = new System.Windows.Forms.CheckBox();
			this.contextMenuTimeArrived = new System.Windows.Forms.ContextMenu();
			this.menuItemArrivedNow = new System.Windows.Forms.MenuItem();
			this.contextMenuTimeSeated = new System.Windows.Forms.ContextMenu();
			this.menuItemSeatedNow = new System.Windows.Forms.MenuItem();
			this.contextMenuTimeDismissed = new System.Windows.Forms.ContextMenu();
			this.menuItemDismissedNow = new System.Windows.Forms.MenuItem();
			this.textTimeAskedToArrive = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.listQuickAdd = new OpenDental.UI.ListBoxOD();
			this.labelQuickAdd = new System.Windows.Forms.Label();
			this.panel1 = new System.Windows.Forms.Panel();
			this.checkASAP = new System.Windows.Forms.CheckBox();
			this.comboApptType = new System.Windows.Forms.ComboBox();
			this.label10 = new System.Windows.Forms.Label();
			this.labelSyndromicObservations = new System.Windows.Forms.Label();
			this.butSyndromicObservations = new OpenDental.UI.Button();
			this.label9 = new System.Windows.Forms.Label();
			this.butColorClear = new OpenDental.UI.Button();
			this.butColor = new System.Windows.Forms.Button();
			this.textRequirement = new System.Windows.Forms.TextBox();
			this.butRequirement = new OpenDental.UI.Button();
			this.butInsPlan2 = new OpenDental.UI.Button();
			this.butInsPlan1 = new OpenDental.UI.Button();
			this.textInsPlan2 = new System.Windows.Forms.TextBox();
			this.labelInsPlan2 = new System.Windows.Forms.Label();
			this.textInsPlan1 = new System.Windows.Forms.TextBox();
			this.labelInsPlan1 = new System.Windows.Forms.Label();
			this.textTimeDismissed = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textTimeSeated = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textTimeArrived = new System.Windows.Forms.TextBox();
			this.labelTimeArrived = new System.Windows.Forms.Label();
			this.textLabCase = new System.Windows.Forms.TextBox();
			this.butLab = new OpenDental.UI.Button();
			this.butPickHyg = new OpenDental.UI.Button();
			this.butPickDentist = new OpenDental.UI.Button();
			this.gridFields = new OpenDental.UI.GridOD();
			this.gridPatient = new OpenDental.UI.GridOD();
			this.gridComm = new OpenDental.UI.GridOD();
			this.gridProc = new OpenDental.UI.GridOD();
			this.labelPlannedComplete = new System.Windows.Forms.Label();
			this.butPDF = new OpenDental.UI.Button();
			this.butComplete = new OpenDental.UI.Button();
			this.butDeleteProc = new OpenDental.UI.Button();
			this.butAdd = new OpenDental.UI.Button();
			this.textNote = new OpenDental.ODtextBox();
			this.butText = new OpenDental.UI.Button();
			this.butAddComm = new OpenDental.UI.Button();
			this.butAudit = new OpenDental.UI.Button();
			this.butTask = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butPin = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butAttachAll = new OpenDental.UI.Button();
			this.contrApptProvSlider = new OpenDental.UI.ControlApptProvSlider();
			this.label6 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.checkShowCommAuto = new System.Windows.Forms.CheckBox();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboConfirmed
			// 
			this.comboConfirmed.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboConfirmed.Location = new System.Drawing.Point(105, 65);
			this.comboConfirmed.MaxDropDownItems = 30;
			this.comboConfirmed.Name = "comboConfirmed";
			this.comboConfirmed.Size = new System.Drawing.Size(126, 21);
			this.comboConfirmed.TabIndex = 84;
			this.comboConfirmed.SelectionChangeCommitted += new System.EventHandler(this.comboConfirmed_SelectionChangeCommitted);
			// 
			// comboUnschedStatus
			// 
			this.comboUnschedStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboUnschedStatus.Location = new System.Drawing.Point(105, 44);
			this.comboUnschedStatus.MaxDropDownItems = 30;
			this.comboUnschedStatus.Name = "comboUnschedStatus";
			this.comboUnschedStatus.Size = new System.Drawing.Size(126, 21);
			this.comboUnschedStatus.TabIndex = 83;
			// 
			// label4
			// 
			this.label4.ForeColor = System.Drawing.SystemColors.ControlText;
			this.label4.Location = new System.Drawing.Point(-3, 46);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(106, 16);
			this.label4.TabIndex = 82;
			this.label4.Text = "Unscheduled Status";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboStatus
			// 
			this.comboStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboStatus.Location = new System.Drawing.Point(105, 4);
			this.comboStatus.MaxDropDownItems = 30;
			this.comboStatus.Name = "comboStatus";
			this.comboStatus.Size = new System.Drawing.Size(126, 21);
			this.comboStatus.TabIndex = 81;
			this.comboStatus.SelectionChangeCommitted += new System.EventHandler(this.comboStatus_SelectionChangeCommitted);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(-3, 67);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(106, 16);
			this.label5.TabIndex = 80;
			this.label5.Text = "Confirmed";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelStatus
			// 
			this.labelStatus.Location = new System.Drawing.Point(-3, 6);
			this.labelStatus.Name = "labelStatus";
			this.labelStatus.Size = new System.Drawing.Size(106, 16);
			this.labelStatus.TabIndex = 79;
			this.labelStatus.Text = "Status";
			this.labelStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label24
			// 
			this.label24.Location = new System.Drawing.Point(119, 170);
			this.label24.Name = "label24";
			this.label24.Size = new System.Drawing.Size(113, 16);
			this.label24.TabIndex = 138;
			this.label24.Text = "(use hyg color)";
			// 
			// checkIsHygiene
			// 
			this.checkIsHygiene.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHygiene.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsHygiene.Location = new System.Drawing.Point(-3, 170);
			this.checkIsHygiene.Name = "checkIsHygiene";
			this.checkIsHygiene.Size = new System.Drawing.Size(121, 16);
			this.checkIsHygiene.TabIndex = 137;
			this.checkIsHygiene.Text = "Is Hygiene";
			this.checkIsHygiene.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsHygiene.Click += new System.EventHandler(this.checkIsHygiene_Click);
			// 
			// comboClinic
			// 
			this.comboClinic.HqDescription = "None";
			this.comboClinic.IncludeUnassigned = true;
			this.comboClinic.Location = new System.Drawing.Point(68, 105);
			this.comboClinic.Name = "comboClinic";
			this.comboClinic.Size = new System.Drawing.Size(163, 21);
			this.comboClinic.TabIndex = 136;
			this.comboClinic.SelectionChangeCommitted += new System.EventHandler(this.ComboClinic_SelectionChangeCommitted);
			// 
			// comboAssistant
			// 
			this.comboAssistant.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAssistant.Location = new System.Drawing.Point(105, 187);
			this.comboAssistant.MaxDropDownItems = 30;
			this.comboAssistant.Name = "comboAssistant";
			this.comboAssistant.Size = new System.Drawing.Size(126, 21);
			this.comboAssistant.TabIndex = 133;
			// 
			// comboProvHyg
			// 
			this.comboProvHyg.Location = new System.Drawing.Point(105, 147);
			this.comboProvHyg.Name = "comboProvHyg";
			this.comboProvHyg.Size = new System.Drawing.Size(106, 21);
			this.comboProvHyg.TabIndex = 132;
			this.comboProvHyg.SelectionChangeCommitted += new System.EventHandler(this.comboProvHyg_SelectionChangeCommitted);
			// 
			// comboProv
			// 
			this.comboProv.Location = new System.Drawing.Point(105, 126);
			this.comboProv.Name = "comboProv";
			this.comboProv.Size = new System.Drawing.Size(106, 21);
			this.comboProv.TabIndex = 131;
			this.comboProv.SelectionChangeCommitted += new System.EventHandler(this.comboProv_SelectionChangeCommitted);
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(-3, 189);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(106, 16);
			this.label12.TabIndex = 129;
			this.label12.Text = "Assistant";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkIsNewPatient
			// 
			this.checkIsNewPatient.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkIsNewPatient.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsNewPatient.Location = new System.Drawing.Point(-3, 88);
			this.checkIsNewPatient.Name = "checkIsNewPatient";
			this.checkIsNewPatient.Size = new System.Drawing.Size(121, 16);
			this.checkIsNewPatient.TabIndex = 128;
			this.checkIsNewPatient.Text = "New Patient";
			this.checkIsNewPatient.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(-3, 149);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(106, 16);
			this.label3.TabIndex = 127;
			this.label3.Text = "Hygienist";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(-3, 128);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(106, 16);
			this.label2.TabIndex = 126;
			this.label2.Text = "Provider";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelApptNote
			// 
			this.labelApptNote.Location = new System.Drawing.Point(301, 568);
			this.labelApptNote.Name = "labelApptNote";
			this.labelApptNote.Size = new System.Drawing.Size(197, 16);
			this.labelApptNote.TabIndex = 141;
			this.labelApptNote.Text = "Appointment Note";
			this.labelApptNote.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// toolTip1
			// 
			this.toolTip1.AutomaticDelay = 5;
			this.toolTip1.AutoPopDelay = 5000000;
			this.toolTip1.InitialDelay = 5;
			this.toolTip1.ReshowDelay = 1;
			this.toolTip1.UseAnimation = false;
			this.toolTip1.UseFading = false;
			// 
			// checkTimeLocked
			// 
			this.checkTimeLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeLocked.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkTimeLocked.Location = new System.Drawing.Point(-3, 210);
			this.checkTimeLocked.Name = "checkTimeLocked";
			this.checkTimeLocked.Size = new System.Drawing.Size(121, 16);
			this.checkTimeLocked.TabIndex = 148;
			this.checkTimeLocked.Text = "Time Locked";
			this.checkTimeLocked.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkTimeLocked.Click += new System.EventHandler(this.checkTimeLocked_Click);
			// 
			// contextMenuTimeArrived
			// 
			this.contextMenuTimeArrived.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemArrivedNow});
			// 
			// menuItemArrivedNow
			// 
			this.menuItemArrivedNow.Index = 0;
			this.menuItemArrivedNow.Text = "Now";
			this.menuItemArrivedNow.Click += new System.EventHandler(this.menuItemArrivedNow_Click);
			// 
			// contextMenuTimeSeated
			// 
			this.contextMenuTimeSeated.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemSeatedNow});
			// 
			// menuItemSeatedNow
			// 
			this.menuItemSeatedNow.Index = 0;
			this.menuItemSeatedNow.Text = "Now";
			this.menuItemSeatedNow.Click += new System.EventHandler(this.menuItemSeatedNow_Click);
			// 
			// contextMenuTimeDismissed
			// 
			this.contextMenuTimeDismissed.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
            this.menuItemDismissedNow});
			// 
			// menuItemDismissedNow
			// 
			this.menuItemDismissedNow.Index = 0;
			this.menuItemDismissedNow.Text = "Now";
			this.menuItemDismissedNow.Click += new System.EventHandler(this.menuItemDismissedNow_Click);
			// 
			// textTimeAskedToArrive
			// 
			this.textTimeAskedToArrive.Location = new System.Drawing.Point(105, 271);
			this.textTimeAskedToArrive.Name = "textTimeAskedToArrive";
			this.textTimeAskedToArrive.Size = new System.Drawing.Size(126, 20);
			this.textTimeAskedToArrive.TabIndex = 146;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(-3, 273);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(106, 16);
			this.label8.TabIndex = 160;
			this.label8.Text = "Time Ask To Arrive";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// listQuickAdd
			// 
			this.listQuickAdd.IntegralHeight = false;
			this.listQuickAdd.Location = new System.Drawing.Point(539, 48);
			this.listQuickAdd.Name = "listQuickAdd";
			this.listQuickAdd.Size = new System.Drawing.Size(150, 355);
			this.listQuickAdd.TabIndex = 163;
			this.listQuickAdd.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listQuickAdd_MouseDown);
			// 
			// labelQuickAdd
			// 
			this.labelQuickAdd.Location = new System.Drawing.Point(540, 6);
			this.labelQuickAdd.Name = "labelQuickAdd";
			this.labelQuickAdd.Size = new System.Drawing.Size(143, 39);
			this.labelQuickAdd.TabIndex = 162;
			this.labelQuickAdd.Text = "Single click on items in the list below to add them to this appointment.";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.checkASAP);
			this.panel1.Controls.Add(this.comboApptType);
			this.panel1.Controls.Add(this.label10);
			this.panel1.Controls.Add(this.labelSyndromicObservations);
			this.panel1.Controls.Add(this.butSyndromicObservations);
			this.panel1.Controls.Add(this.label9);
			this.panel1.Controls.Add(this.butColorClear);
			this.panel1.Controls.Add(this.butColor);
			this.panel1.Controls.Add(this.textRequirement);
			this.panel1.Controls.Add(this.butRequirement);
			this.panel1.Controls.Add(this.butInsPlan2);
			this.panel1.Controls.Add(this.butInsPlan1);
			this.panel1.Controls.Add(this.textInsPlan2);
			this.panel1.Controls.Add(this.labelInsPlan2);
			this.panel1.Controls.Add(this.textInsPlan1);
			this.panel1.Controls.Add(this.labelInsPlan1);
			this.panel1.Controls.Add(this.textTimeDismissed);
			this.panel1.Controls.Add(this.label7);
			this.panel1.Controls.Add(this.textTimeSeated);
			this.panel1.Controls.Add(this.label1);
			this.panel1.Controls.Add(this.textTimeArrived);
			this.panel1.Controls.Add(this.labelTimeArrived);
			this.panel1.Controls.Add(this.textLabCase);
			this.panel1.Controls.Add(this.butLab);
			this.panel1.Controls.Add(this.comboStatus);
			this.panel1.Controls.Add(this.checkIsNewPatient);
			this.panel1.Controls.Add(this.comboConfirmed);
			this.panel1.Controls.Add(this.label24);
			this.panel1.Controls.Add(this.textTimeAskedToArrive);
			this.panel1.Controls.Add(this.comboUnschedStatus);
			this.panel1.Controls.Add(this.label8);
			this.panel1.Controls.Add(this.checkIsHygiene);
			this.panel1.Controls.Add(this.comboClinic);
			this.panel1.Controls.Add(this.label4);
			this.panel1.Controls.Add(this.comboAssistant);
			this.panel1.Controls.Add(this.butPickHyg);
			this.panel1.Controls.Add(this.comboProvHyg);
			this.panel1.Controls.Add(this.comboProv);
			this.panel1.Controls.Add(this.butPickDentist);
			this.panel1.Controls.Add(this.label5);
			this.panel1.Controls.Add(this.label12);
			this.panel1.Controls.Add(this.label3);
			this.panel1.Controls.Add(this.labelStatus);
			this.panel1.Controls.Add(this.label2);
			this.panel1.Controls.Add(this.checkTimeLocked);
			this.panel1.Location = new System.Drawing.Point(302, 2);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(234, 511);
			this.panel1.TabIndex = 164;
			// 
			// checkASAP
			// 
			this.checkASAP.BackColor = System.Drawing.SystemColors.Control;
			this.checkASAP.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkASAP.ForeColor = System.Drawing.SystemColors.ControlText;
			this.checkASAP.Location = new System.Drawing.Point(-3, 27);
			this.checkASAP.Name = "checkASAP";
			this.checkASAP.Size = new System.Drawing.Size(121, 16);
			this.checkASAP.TabIndex = 184;
			this.checkASAP.Text = "ASAP";
			this.checkASAP.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkASAP.UseVisualStyleBackColor = false;
			this.checkASAP.CheckedChanged += new System.EventHandler(this.checkASAP_CheckedChanged);
			// 
			// comboApptType
			// 
			this.comboApptType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboApptType.Location = new System.Drawing.Point(105, 248);
			this.comboApptType.MaxDropDownItems = 30;
			this.comboApptType.Name = "comboApptType";
			this.comboApptType.Size = new System.Drawing.Size(126, 21);
			this.comboApptType.TabIndex = 183;
			this.comboApptType.SelectionChangeCommitted += new System.EventHandler(this.comboApptType_SelectionChangeCommitted);
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(-3, 250);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(106, 16);
			this.label10.TabIndex = 182;
			this.label10.Text = "Appointment Type";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelSyndromicObservations
			// 
			this.labelSyndromicObservations.Location = new System.Drawing.Point(54, 486);
			this.labelSyndromicObservations.Name = "labelSyndromicObservations";
			this.labelSyndromicObservations.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.labelSyndromicObservations.Size = new System.Drawing.Size(144, 16);
			this.labelSyndromicObservations.TabIndex = 181;
			this.labelSyndromicObservations.Text = "(Syndromic Observations)";
			this.labelSyndromicObservations.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelSyndromicObservations.Visible = false;
			// 
			// butSyndromicObservations
			// 
			this.butSyndromicObservations.Location = new System.Drawing.Point(6, 484);
			this.butSyndromicObservations.Name = "butSyndromicObservations";
			this.butSyndromicObservations.Size = new System.Drawing.Size(46, 20);
			this.butSyndromicObservations.TabIndex = 180;
			this.butSyndromicObservations.Text = "Obs";
			this.butSyndromicObservations.Visible = false;
			this.butSyndromicObservations.Click += new System.EventHandler(this.butSyndromicObservations_Click);
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(-3, 229);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(106, 16);
			this.label9.TabIndex = 179;
			this.label9.Text = "Color";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butColorClear
			// 
			this.butColorClear.Location = new System.Drawing.Point(128, 226);
			this.butColorClear.Name = "butColorClear";
			this.butColorClear.Size = new System.Drawing.Size(39, 20);
			this.butColorClear.TabIndex = 178;
			this.butColorClear.Text = "none";
			this.butColorClear.Click += new System.EventHandler(this.butColorClear_Click);
			// 
			// butColor
			// 
			this.butColor.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
			this.butColor.Location = new System.Drawing.Point(105, 227);
			this.butColor.Name = "butColor";
			this.butColor.Size = new System.Drawing.Size(21, 19);
			this.butColor.TabIndex = 177;
			this.butColor.Click += new System.EventHandler(this.butColor_Click);
			// 
			// textRequirement
			// 
			this.textRequirement.Location = new System.Drawing.Point(54, 432);
			this.textRequirement.Multiline = true;
			this.textRequirement.Name = "textRequirement";
			this.textRequirement.ReadOnly = true;
			this.textRequirement.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textRequirement.Size = new System.Drawing.Size(177, 53);
			this.textRequirement.TabIndex = 164;
			// 
			// butRequirement
			// 
			this.butRequirement.Location = new System.Drawing.Point(6, 432);
			this.butRequirement.Name = "butRequirement";
			this.butRequirement.Size = new System.Drawing.Size(46, 20);
			this.butRequirement.TabIndex = 163;
			this.butRequirement.Text = "Req";
			this.butRequirement.Click += new System.EventHandler(this.butRequirement_Click);
			// 
			// butInsPlan2
			// 
			this.butInsPlan2.Location = new System.Drawing.Point(213, 411);
			this.butInsPlan2.Name = "butInsPlan2";
			this.butInsPlan2.Size = new System.Drawing.Size(18, 20);
			this.butInsPlan2.TabIndex = 176;
			this.butInsPlan2.Text = "...";
			this.butInsPlan2.Click += new System.EventHandler(this.butInsPlan2_Click);
			// 
			// butInsPlan1
			// 
			this.butInsPlan1.Location = new System.Drawing.Point(213, 390);
			this.butInsPlan1.Name = "butInsPlan1";
			this.butInsPlan1.Size = new System.Drawing.Size(18, 20);
			this.butInsPlan1.TabIndex = 175;
			this.butInsPlan1.Text = "...";
			this.butInsPlan1.Click += new System.EventHandler(this.butInsPlan1_Click);
			// 
			// textInsPlan2
			// 
			this.textInsPlan2.Location = new System.Drawing.Point(54, 411);
			this.textInsPlan2.Name = "textInsPlan2";
			this.textInsPlan2.ReadOnly = true;
			this.textInsPlan2.Size = new System.Drawing.Size(158, 20);
			this.textInsPlan2.TabIndex = 174;
			// 
			// labelInsPlan2
			// 
			this.labelInsPlan2.Location = new System.Drawing.Point(-3, 413);
			this.labelInsPlan2.Name = "labelInsPlan2";
			this.labelInsPlan2.Size = new System.Drawing.Size(55, 16);
			this.labelInsPlan2.TabIndex = 173;
			this.labelInsPlan2.Text = "InsPlan 2";
			this.labelInsPlan2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInsPlan1
			// 
			this.textInsPlan1.Location = new System.Drawing.Point(54, 390);
			this.textInsPlan1.Name = "textInsPlan1";
			this.textInsPlan1.ReadOnly = true;
			this.textInsPlan1.Size = new System.Drawing.Size(158, 20);
			this.textInsPlan1.TabIndex = 172;
			// 
			// labelInsPlan1
			// 
			this.labelInsPlan1.Location = new System.Drawing.Point(-3, 392);
			this.labelInsPlan1.Name = "labelInsPlan1";
			this.labelInsPlan1.Size = new System.Drawing.Size(55, 16);
			this.labelInsPlan1.TabIndex = 171;
			this.labelInsPlan1.Text = "InsPlan 1";
			this.labelInsPlan1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeDismissed
			// 
			this.textTimeDismissed.Location = new System.Drawing.Point(105, 331);
			this.textTimeDismissed.Name = "textTimeDismissed";
			this.textTimeDismissed.Size = new System.Drawing.Size(126, 20);
			this.textTimeDismissed.TabIndex = 170;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(-3, 333);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(106, 16);
			this.label7.TabIndex = 169;
			this.label7.Text = "Time Dismissed";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeSeated
			// 
			this.textTimeSeated.Location = new System.Drawing.Point(105, 311);
			this.textTimeSeated.Name = "textTimeSeated";
			this.textTimeSeated.Size = new System.Drawing.Size(126, 20);
			this.textTimeSeated.TabIndex = 168;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(-3, 313);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(106, 16);
			this.label1.TabIndex = 166;
			this.label1.Text = "Time Seated";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTimeArrived
			// 
			this.textTimeArrived.Location = new System.Drawing.Point(105, 291);
			this.textTimeArrived.Name = "textTimeArrived";
			this.textTimeArrived.Size = new System.Drawing.Size(126, 20);
			this.textTimeArrived.TabIndex = 167;
			// 
			// labelTimeArrived
			// 
			this.labelTimeArrived.Location = new System.Drawing.Point(-3, 293);
			this.labelTimeArrived.Name = "labelTimeArrived";
			this.labelTimeArrived.Size = new System.Drawing.Size(106, 16);
			this.labelTimeArrived.TabIndex = 165;
			this.labelTimeArrived.Text = "Time Arrived";
			this.labelTimeArrived.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLabCase
			// 
			this.textLabCase.AcceptsReturn = true;
			this.textLabCase.Location = new System.Drawing.Point(54, 354);
			this.textLabCase.Multiline = true;
			this.textLabCase.Name = "textLabCase";
			this.textLabCase.ReadOnly = true;
			this.textLabCase.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textLabCase.Size = new System.Drawing.Size(177, 34);
			this.textLabCase.TabIndex = 162;
			// 
			// butLab
			// 
			this.butLab.Location = new System.Drawing.Point(6, 354);
			this.butLab.Name = "butLab";
			this.butLab.Size = new System.Drawing.Size(46, 20);
			this.butLab.TabIndex = 161;
			this.butLab.Text = "Lab";
			this.butLab.Click += new System.EventHandler(this.butLab_Click);
			// 
			// butPickHyg
			// 
			this.butPickHyg.Location = new System.Drawing.Point(213, 148);
			this.butPickHyg.Name = "butPickHyg";
			this.butPickHyg.Size = new System.Drawing.Size(18, 20);
			this.butPickHyg.TabIndex = 158;
			this.butPickHyg.Text = "...";
			this.butPickHyg.Click += new System.EventHandler(this.butPickHyg_Click);
			// 
			// butPickDentist
			// 
			this.butPickDentist.Location = new System.Drawing.Point(213, 127);
			this.butPickDentist.Name = "butPickDentist";
			this.butPickDentist.Size = new System.Drawing.Size(18, 20);
			this.butPickDentist.TabIndex = 157;
			this.butPickDentist.Text = "...";
			this.butPickDentist.Click += new System.EventHandler(this.butPickDentist_Click);
			// 
			// gridFields
			// 
			this.gridFields.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridFields.Location = new System.Drawing.Point(48, 293);
			this.gridFields.Name = "gridFields";
			this.gridFields.Size = new System.Drawing.Size(252, 403);
			this.gridFields.TabIndex = 159;
			this.gridFields.Title = "Appt Fields";
			this.gridFields.TranslationName = "FormApptEdit";
			this.gridFields.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridFields_CellDoubleClick);
			// 
			// gridPatient
			// 
			this.gridPatient.Location = new System.Drawing.Point(48, 2);
			this.gridPatient.Name = "gridPatient";
			this.gridPatient.Size = new System.Drawing.Size(252, 289);
			this.gridPatient.TabIndex = 0;
			this.gridPatient.Title = "Patient Info";
			this.gridPatient.TranslationName = "TableApptPtInfo";
			this.gridPatient.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridPatient_CellClick);
			// 
			// gridComm
			// 
			this.gridComm.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridComm.Location = new System.Drawing.Point(691, 425);
			this.gridComm.Name = "gridComm";
			this.gridComm.Size = new System.Drawing.Size(443, 271);
			this.gridComm.TabIndex = 1;
			this.gridComm.Title = "Communications Log - Appointment Scheduling";
			this.gridComm.TranslationName = "TableCommLog";
			this.gridComm.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridComm_CellDoubleClick);
			// 
			// gridProc
			// 
			this.gridProc.AllowSelection = false;
			this.gridProc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridProc.Location = new System.Drawing.Point(691, 28);
			this.gridProc.Name = "gridProc";
			this.gridProc.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridProc.Size = new System.Drawing.Size(538, 375);
			this.gridProc.TabIndex = 139;
			this.gridProc.Title = "Procedures on this Appointment";
			this.gridProc.TranslationName = "TableApptProcs";
			this.gridProc.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProc_CellDoubleClick);
			this.gridProc.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridProc_CellClick);
			this.gridProc.MouseLeave += new System.EventHandler(this.gridProc_MouseLeave);
			// 
			// labelPlannedComplete
			// 
			this.labelPlannedComplete.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPlannedComplete.Location = new System.Drawing.Point(921, 1);
			this.labelPlannedComplete.Name = "labelPlannedComplete";
			this.labelPlannedComplete.Size = new System.Drawing.Size(227, 26);
			this.labelPlannedComplete.TabIndex = 184;
			this.labelPlannedComplete.Text = "This planned appointment is attached\r\nto a completed appointment.";
			this.labelPlannedComplete.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.labelPlannedComplete.Visible = false;
			// 
			// butPDF
			// 
			this.butPDF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPDF.Location = new System.Drawing.Point(1137, 478);
			this.butPDF.Name = "butPDF";
			this.butPDF.Size = new System.Drawing.Size(92, 24);
			this.butPDF.TabIndex = 161;
			this.butPDF.Text = "Notes PDF";
			this.butPDF.Visible = false;
			this.butPDF.Click += new System.EventHandler(this.butPDF_Click);
			// 
			// butComplete
			// 
			this.butComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butComplete.Location = new System.Drawing.Point(1137, 504);
			this.butComplete.Name = "butComplete";
			this.butComplete.Size = new System.Drawing.Size(92, 24);
			this.butComplete.TabIndex = 155;
			this.butComplete.Text = "Finish && Send";
			this.butComplete.Visible = false;
			this.butComplete.Click += new System.EventHandler(this.butComplete_Click);
			// 
			// butDeleteProc
			// 
			this.butDeleteProc.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDeleteProc.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDeleteProc.Location = new System.Drawing.Point(691, 2);
			this.butDeleteProc.Name = "butDeleteProc";
			this.butDeleteProc.Size = new System.Drawing.Size(75, 24);
			this.butDeleteProc.TabIndex = 154;
			this.butDeleteProc.Text = "Delete";
			this.butDeleteProc.Click += new System.EventHandler(this.butDeleteProc_Click);
			// 
			// butAdd
			// 
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(767, 2);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 152;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.textNote.BackColor = System.Drawing.SystemColors.Window;
			this.textNote.DetectLinksEnabled = false;
			this.textNote.DetectUrls = false;
			this.textNote.HasAutoNotes = true;
			this.textNote.Location = new System.Drawing.Point(302, 587);
			this.textNote.Name = "textNote";
			this.textNote.QuickPasteType = OpenDentBusiness.QuickPasteType.Appointment;
			this.textNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(387, 109);
			this.textNote.TabIndex = 142;
			this.textNote.Text = "";
			// 
			// butText
			// 
			this.butText.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butText.Icon = OpenDental.UI.EnumIcons.Text;
			this.butText.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butText.Location = new System.Drawing.Point(1137, 452);
			this.butText.Name = "butText";
			this.butText.Size = new System.Drawing.Size(92, 24);
			this.butText.TabIndex = 143;
			this.butText.Text = "Text";
			this.butText.Click += new System.EventHandler(this.butText_Click);
			// 
			// butAddComm
			// 
			this.butAddComm.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAddComm.Icon = OpenDental.UI.EnumIcons.CommLog;
			this.butAddComm.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAddComm.Location = new System.Drawing.Point(1137, 426);
			this.butAddComm.Name = "butAddComm";
			this.butAddComm.Size = new System.Drawing.Size(92, 24);
			this.butAddComm.TabIndex = 143;
			this.butAddComm.Text = "Comm";
			this.butAddComm.Click += new System.EventHandler(this.butAddComm_Click);
			// 
			// butAudit
			// 
			this.butAudit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAudit.Location = new System.Drawing.Point(1137, 530);
			this.butAudit.Name = "butAudit";
			this.butAudit.Size = new System.Drawing.Size(92, 24);
			this.butAudit.TabIndex = 125;
			this.butAudit.Text = "Audit Trail";
			this.butAudit.Click += new System.EventHandler(this.butAudit_Click);
			// 
			// butTask
			// 
			this.butTask.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butTask.Location = new System.Drawing.Point(1137, 556);
			this.butTask.Name = "butTask";
			this.butTask.Size = new System.Drawing.Size(92, 24);
			this.butTask.TabIndex = 124;
			this.butTask.Text = "To Task List";
			this.butTask.Click += new System.EventHandler(this.butTask_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(1137, 608);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(92, 24);
			this.butDelete.TabIndex = 123;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butPin
			// 
			this.butPin.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPin.Image = ((System.Drawing.Image)(resources.GetObject("butPin.Image")));
			this.butPin.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPin.Location = new System.Drawing.Point(1137, 582);
			this.butPin.Name = "butPin";
			this.butPin.Size = new System.Drawing.Size(92, 24);
			this.butPin.TabIndex = 122;
			this.butPin.Text = "Pinboard";
			this.butPin.Click += new System.EventHandler(this.butPin_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(1136, 640);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(92, 24);
			this.butOK.TabIndex = 1;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(1136, 666);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(92, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butAttachAll
			// 
			this.butAttachAll.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAttachAll.Location = new System.Drawing.Point(843, 2);
			this.butAttachAll.Name = "butAttachAll";
			this.butAttachAll.Size = new System.Drawing.Size(75, 24);
			this.butAttachAll.TabIndex = 185;
			this.butAttachAll.Text = "Attach All";
			this.butAttachAll.Click += new System.EventHandler(this.butAttachAll_Click);
			// 
			// contrApptProvSlider
			// 
			this.contrApptProvSlider.Location = new System.Drawing.Point(1, 16);
			this.contrApptProvSlider.Name = "contrApptProvSlider";
			this.contrApptProvSlider.Size = new System.Drawing.Size(45, 677);
			this.contrApptProvSlider.TabIndex = 187;
			this.contrApptProvSlider.Text = "contrApptProvSlider1";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(19, 3);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(12, 12);
			this.label6.TabIndex = 185;
			this.label6.Text = "2";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(7, 3);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(12, 12);
			this.label11.TabIndex = 188;
			this.label11.Text = "1";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// checkShowCommAuto
			// 
			this.checkShowCommAuto.Location = new System.Drawing.Point(692, 406);
			this.checkShowCommAuto.Margin = new System.Windows.Forms.Padding(0);
			this.checkShowCommAuto.Name = "checkShowCommAuto";
			this.checkShowCommAuto.Size = new System.Drawing.Size(158, 17);
			this.checkShowCommAuto.TabIndex = 189;
			this.checkShowCommAuto.Text = "Show Automated Commlogs";
			this.checkShowCommAuto.UseVisualStyleBackColor = true;
			this.checkShowCommAuto.Click += new System.EventHandler(this.checkShowCommAuto_Click);
			// 
			// FormApptEdit
			// 
			this.ClientSize = new System.Drawing.Size(1230, 698);
			this.Controls.Add(this.checkShowCommAuto);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.contrApptProvSlider);
			this.Controls.Add(this.butAttachAll);
			this.Controls.Add(this.labelPlannedComplete);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.listQuickAdd);
			this.Controls.Add(this.labelQuickAdd);
			this.Controls.Add(this.butPDF);
			this.Controls.Add(this.gridFields);
			this.Controls.Add(this.butComplete);
			this.Controls.Add(this.butDeleteProc);
			this.Controls.Add(this.butAdd);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelApptNote);
			this.Controls.Add(this.butText);
			this.Controls.Add(this.butAddComm);
			this.Controls.Add(this.gridPatient);
			this.Controls.Add(this.gridComm);
			this.Controls.Add(this.gridProc);
			this.Controls.Add(this.butAudit);
			this.Controls.Add(this.butTask);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butPin);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormApptEdit";
			this.ShowInTaskbar = false;
			this.Text = "Edit Appointment";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormApptEdit_FormClosing);
			this.Load += new System.EventHandler(this.FormApptEdit_Load);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.Button butOK;
		private OpenDental.UI.GridOD gridPatient;
		private OpenDental.UI.GridOD gridComm;
		private System.Windows.Forms.ComboBox comboConfirmed;
		private System.Windows.Forms.ComboBox comboUnschedStatus;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboStatus;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label labelStatus;
		private OpenDental.UI.Button butAudit;
		private OpenDental.UI.Button butTask;
		private OpenDental.UI.Button butDelete;
		private OpenDental.UI.Button butPin;
		private System.Windows.Forms.Label label24;
		private System.Windows.Forms.CheckBox checkIsHygiene;
		private UI.ComboBoxClinicPicker comboClinic;
		private System.Windows.Forms.ComboBox comboAssistant;
		private UI.ComboBoxOD comboProvHyg;
		private UI.ComboBoxOD comboProv;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.CheckBox checkIsNewPatient;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.GridOD gridProc;
		private ODtextBox textNote;
		private System.Windows.Forms.Label labelApptNote;
		private OpenDental.UI.Button butAddComm;
		private System.Windows.Forms.ToolTip toolTip1;
		private System.Windows.Forms.ContextMenu contextMenuTimeArrived;
		private System.Windows.Forms.MenuItem menuItemArrivedNow;
		private System.Windows.Forms.ContextMenu contextMenuTimeSeated;
		private System.Windows.Forms.MenuItem menuItemSeatedNow;
		private System.Windows.Forms.ContextMenu contextMenuTimeDismissed;
		private System.Windows.Forms.MenuItem menuItemDismissedNow;
		private OpenDental.UI.Button butAdd;
		private OpenDental.UI.Button butDeleteProc;
		private OpenDental.UI.Button butComplete;
		private System.Windows.Forms.CheckBox checkTimeLocked;
		private OpenDental.UI.Button butPickHyg;
		private OpenDental.UI.Button butPickDentist;
		private OpenDental.UI.GridOD gridFields;
		private System.Windows.Forms.TextBox textTimeAskedToArrive;
		private System.Windows.Forms.Label label8;
		private OpenDental.UI.Button butPDF;
		private OpenDental.UI.ListBoxOD listQuickAdd;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.TextBox textRequirement;
		private UI.Button butRequirement;
		private UI.Button butInsPlan2;
		private UI.Button butInsPlan1;
		private System.Windows.Forms.TextBox textInsPlan2;
		private System.Windows.Forms.Label labelInsPlan2;
		private System.Windows.Forms.TextBox textInsPlan1;
		private System.Windows.Forms.Label labelInsPlan1;
		private System.Windows.Forms.TextBox textTimeDismissed;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textTimeSeated;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textTimeArrived;
		private System.Windows.Forms.Label labelTimeArrived;
		private System.Windows.Forms.TextBox textLabCase;
		private UI.Button butLab;
		private System.Windows.Forms.Label label9;
		private UI.Button butColorClear;
		private System.Windows.Forms.Button butColor;
		private UI.Button butText;
		private System.Windows.Forms.Label labelQuickAdd;
		private UI.Button butSyndromicObservations;
		private System.Windows.Forms.Label labelSyndromicObservations;
		private System.Windows.Forms.CheckBox checkASAP;
		private System.Windows.Forms.ComboBox comboApptType;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label labelPlannedComplete;
		private UI.Button butAttachAll;
		private UI.ControlApptProvSlider contrApptProvSlider;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.CheckBox checkShowCommAuto;
	}
}
