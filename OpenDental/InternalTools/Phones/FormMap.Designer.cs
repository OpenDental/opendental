namespace OpenDental.InternalTools.Phones{
	partial class FormMap {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if(disposing) {
				components?.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMap));
			this.menuMain = new OpenDental.UI.MenuOD();
			this.labelTriageCoordinator = new System.Windows.Forms.Label();
			this.labelCurrentTime = new System.Windows.Forms.Label();
			this.comboRoom = new OpenDental.UI.ComboBox();
			this.groupPhoneMetrics = new OpenDental.UI.GroupBox();
			this.mapNumberTriageTime = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberTriageRedTime = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberChatTime = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberTriageCalls = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberVoicemailTime = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberChatCount = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberVoicemailCalls = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberTriageRedCalls = new OpenDental.InternalTools.Phones.MapNumber();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.tabMain = new OpenDental.UI.TabControl();
			this.tabPage1 = new OpenDental.UI.TabPage();
			this.tabPage2 = new OpenDental.UI.TabPage();
			this.tabPage3 = new OpenDental.UI.TabPage();
			this.tabPage4 = new OpenDental.UI.TabPage();
			this.escalationView = new OpenDental.InternalTools.Phones.EscalationView();
			this.escalationViewOfficesDown = new OpenDental.InternalTools.Phones.EscalationView();
			this.labelTirageOpLocal = new System.Windows.Forms.Label();
			this.label14 = new System.Windows.Forms.Label();
			this.labelTirageOpTotal = new System.Windows.Forms.Label();
			this.mapNumberTriageOpsLocal = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberTriageOpsTotal = new OpenDental.InternalTools.Phones.MapNumber();
			this.userControlMapDetails1 = new OpenDental.InternalTools.Phones.UserControlMapDetails();
			this.label3 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.mapNumberCustDownCount = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapNumberCustDownTime = new OpenDental.InternalTools.Phones.MapNumber();
			this.mapPanel = new OpenDental.InternalTools.Phones.MapPanel();
			this.label8 = new System.Windows.Forms.Label();
			this.butZoomFit = new OpenDental.UI.Button();
			this.groupPhoneMetrics.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.SuspendLayout();
			// 
			// menuMain
			// 
			this.menuMain.Location = new System.Drawing.Point(0, 0);
			this.menuMain.Name = "menuMain";
			this.menuMain.Size = new System.Drawing.Size(400, 24);
			this.menuMain.TabIndex = 72;
			// 
			// labelTriageCoordinator
			// 
			this.labelTriageCoordinator.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTriageCoordinator.Font = new System.Drawing.Font("Calibri", 32F);
			this.labelTriageCoordinator.Location = new System.Drawing.Point(788, 3);
			this.labelTriageCoordinator.Name = "labelTriageCoordinator";
			this.labelTriageCoordinator.Size = new System.Drawing.Size(898, 60);
			this.labelTriageCoordinator.TabIndex = 74;
			this.labelTriageCoordinator.Text = "Triage Coord - Jim Smith";
			this.labelTriageCoordinator.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCurrentTime
			// 
			this.labelCurrentTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelCurrentTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelCurrentTime.Location = new System.Drawing.Point(1692, 5);
			this.labelCurrentTime.Name = "labelCurrentTime";
			this.labelCurrentTime.Size = new System.Drawing.Size(182, 61);
			this.labelCurrentTime.TabIndex = 75;
			this.labelCurrentTime.Text = "12:45 PM";
			this.labelCurrentTime.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// comboRoom
			// 
			this.comboRoom.Font = new System.Drawing.Font("Microsoft Sans Serif", 24F);
			this.comboRoom.Location = new System.Drawing.Point(0, 27);
			this.comboRoom.Name = "comboRoom";
			this.comboRoom.Size = new System.Drawing.Size(309, 40);
			this.comboRoom.TabIndex = 76;
			this.comboRoom.Text = "comboBox1";
			this.comboRoom.SelectionChangeCommitted += new System.EventHandler(this.comboRoom_SelectionChangeCommitted);
			// 
			// groupPhoneMetrics
			// 
			this.groupPhoneMetrics.Controls.Add(this.mapNumberTriageTime);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberTriageRedTime);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberChatTime);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberTriageCalls);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberVoicemailTime);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberChatCount);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberVoicemailCalls);
			this.groupPhoneMetrics.Controls.Add(this.mapNumberTriageRedCalls);
			this.groupPhoneMetrics.Controls.Add(this.label2);
			this.groupPhoneMetrics.Controls.Add(this.label1);
			this.groupPhoneMetrics.Controls.Add(this.label4);
			this.groupPhoneMetrics.Controls.Add(this.label11);
			this.groupPhoneMetrics.Controls.Add(this.label10);
			this.groupPhoneMetrics.Controls.Add(this.label6);
			this.groupPhoneMetrics.Location = new System.Drawing.Point(4, 72);
			this.groupPhoneMetrics.Name = "groupPhoneMetrics";
			this.groupPhoneMetrics.Size = new System.Drawing.Size(357, 251);
			this.groupPhoneMetrics.TabIndex = 83;
			// 
			// mapNumberTriageTime
			// 
			this.mapNumberTriageTime.Font = new System.Drawing.Font("Calibri", 56F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberTriageTime.Location = new System.Drawing.Point(206, 187);
			this.mapNumberTriageTime.Name = "mapNumberTriageTime";
			this.mapNumberTriageTime.Size = new System.Drawing.Size(120, 58);
			this.mapNumberTriageTime.TabIndex = 85;
			this.mapNumberTriageTime.Text = "0";
			// 
			// mapNumberTriageRedTime
			// 
			this.mapNumberTriageRedTime.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberTriageRedTime.Location = new System.Drawing.Point(206, 48);
			this.mapNumberTriageRedTime.Name = "mapNumberTriageRedTime";
			this.mapNumberTriageRedTime.Size = new System.Drawing.Size(120, 44);
			this.mapNumberTriageRedTime.TabIndex = 41;
			this.mapNumberTriageRedTime.Text = "00:00";
			// 
			// mapNumberChatTime
			// 
			this.mapNumberChatTime.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberChatTime.Location = new System.Drawing.Point(206, 140);
			this.mapNumberChatTime.Name = "mapNumberChatTime";
			this.mapNumberChatTime.Size = new System.Drawing.Size(120, 44);
			this.mapNumberChatTime.TabIndex = 84;
			this.mapNumberChatTime.Text = "00:00";
			// 
			// mapNumberTriageCalls
			// 
			this.mapNumberTriageCalls.Font = new System.Drawing.Font("Calibri", 56F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberTriageCalls.Location = new System.Drawing.Point(105, 187);
			this.mapNumberTriageCalls.Name = "mapNumberTriageCalls";
			this.mapNumberTriageCalls.Size = new System.Drawing.Size(81, 58);
			this.mapNumberTriageCalls.TabIndex = 40;
			this.mapNumberTriageCalls.Text = "0";
			// 
			// mapNumberVoicemailTime
			// 
			this.mapNumberVoicemailTime.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberVoicemailTime.Location = new System.Drawing.Point(206, 94);
			this.mapNumberVoicemailTime.Name = "mapNumberVoicemailTime";
			this.mapNumberVoicemailTime.Size = new System.Drawing.Size(120, 44);
			this.mapNumberVoicemailTime.TabIndex = 42;
			this.mapNumberVoicemailTime.Text = "00:00";
			// 
			// mapNumberChatCount
			// 
			this.mapNumberChatCount.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberChatCount.Location = new System.Drawing.Point(105, 140);
			this.mapNumberChatCount.Name = "mapNumberChatCount";
			this.mapNumberChatCount.Size = new System.Drawing.Size(81, 44);
			this.mapNumberChatCount.TabIndex = 39;
			this.mapNumberChatCount.Text = "0";
			// 
			// mapNumberVoicemailCalls
			// 
			this.mapNumberVoicemailCalls.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberVoicemailCalls.Location = new System.Drawing.Point(105, 94);
			this.mapNumberVoicemailCalls.Name = "mapNumberVoicemailCalls";
			this.mapNumberVoicemailCalls.Size = new System.Drawing.Size(81, 44);
			this.mapNumberVoicemailCalls.TabIndex = 38;
			this.mapNumberVoicemailCalls.Text = "0";
			// 
			// mapNumberTriageRedCalls
			// 
			this.mapNumberTriageRedCalls.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberTriageRedCalls.Location = new System.Drawing.Point(105, 48);
			this.mapNumberTriageRedCalls.Name = "mapNumberTriageRedCalls";
			this.mapNumberTriageRedCalls.Size = new System.Drawing.Size(81, 44);
			this.mapNumberTriageRedCalls.TabIndex = 37;
			this.mapNumberTriageRedCalls.Text = "0";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Calibri", 22F);
			this.label2.Location = new System.Drawing.Point(6, 141);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 37);
			this.label2.TabIndex = 34;
			this.label2.Text = "Chat";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Calibri", 22F);
			this.label1.Location = new System.Drawing.Point(6, 48);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(63, 37);
			this.label1.TabIndex = 6;
			this.label1.Text = "Red";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Font = new System.Drawing.Font("Calibri", 22F);
			this.label4.Location = new System.Drawing.Point(6, 94);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(60, 37);
			this.label4.TabIndex = 8;
			this.label4.Text = "VM";
			// 
			// label11
			// 
			this.label11.Font = new System.Drawing.Font("Calibri", 24F);
			this.label11.Location = new System.Drawing.Point(224, 9);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(97, 36);
			this.label11.TabIndex = 16;
			this.label11.Text = "Time";
			// 
			// label10
			// 
			this.label10.Font = new System.Drawing.Font("Calibri", 24F);
			this.label10.Location = new System.Drawing.Point(99, 9);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(105, 36);
			this.label10.TabIndex = 15;
			this.label10.Text = "# Calls";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Font = new System.Drawing.Font("Calibri", 22F);
			this.label6.Location = new System.Drawing.Point(6, 187);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(90, 37);
			this.label6.TabIndex = 10;
			this.label6.Text = "Triage";
			// 
			// tabMain
			// 
			this.tabMain.Controls.Add(this.tabPage1);
			this.tabMain.Controls.Add(this.tabPage2);
			this.tabMain.Controls.Add(this.tabPage3);
			this.tabMain.Controls.Add(this.tabPage4);
			this.tabMain.Font = new System.Drawing.Font("Microsoft Sans Serif", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.tabMain.Location = new System.Drawing.Point(0, 331);
			this.tabMain.Name = "tabMain";
			this.tabMain.Size = new System.Drawing.Size(57, 282);
			this.tabMain.SizeTabsLeft = new System.Drawing.Size(57, 28);
			this.tabMain.TabAlignment = OpenDental.UI.EnumTabAlignment.LeftHorizontal;
			this.tabMain.TabIndex = 90;
			this.tabMain.SelectedIndexChanged += new System.EventHandler(this.tabMain_SelectedIndexChanged);
			// 
			// tabPage1
			// 
			this.tabPage1.Location = new System.Drawing.Point(58, 2);
			this.tabPage1.Name = "tabPage1";
			this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage1.Size = new System.Drawing.Size(0, 278);
			this.tabPage1.TabIndex = 0;
			this.tabPage1.Text = "Avail";
			// 
			// tabPage2
			// 
			this.tabPage2.Location = new System.Drawing.Point(58, 2);
			this.tabPage2.Name = "tabPage2";
			this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
			this.tabPage2.Size = new System.Drawing.Size(0, 278);
			this.tabPage2.TabIndex = 1;
			this.tabPage2.Text = "Escal";
			// 
			// tabPage3
			// 
			this.tabPage3.Location = new System.Drawing.Point(58, 2);
			this.tabPage3.Name = "tabPage3";
			this.tabPage3.Size = new System.Drawing.Size(0, 278);
			this.tabPage3.TabIndex = 2;
			this.tabPage3.Text = "Eserv";
			// 
			// tabPage4
			// 
			this.tabPage4.Location = new System.Drawing.Point(58, 2);
			this.tabPage4.Name = "tabPage4";
			this.tabPage4.Size = new System.Drawing.Size(0, 278);
			this.tabPage4.TabIndex = 3;
			this.tabPage4.Text = "etc...";
			// 
			// escalationView
			// 
			this.escalationView.Font = new System.Drawing.Font("Calibri", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.escalationView.Location = new System.Drawing.Point(58, 331);
			this.escalationView.Name = "escalationView";
			this.escalationView.Size = new System.Drawing.Size(304, 282);
			this.escalationView.TabIndex = 91;
			this.escalationView.Text = "escalationView1";
			// 
			// escalationViewOfficesDown
			// 
			this.escalationViewOfficesDown.Font = new System.Drawing.Font("Calibri", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.escalationViewOfficesDown.Location = new System.Drawing.Point(4, 789);
			this.escalationViewOfficesDown.Name = "escalationViewOfficesDown";
			this.escalationViewOfficesDown.Size = new System.Drawing.Size(358, 127);
			this.escalationViewOfficesDown.TabIndex = 92;
			this.escalationViewOfficesDown.Text = "escalationView1";
			// 
			// labelTirageOpLocal
			// 
			this.labelTirageOpLocal.Font = new System.Drawing.Font("Calibri", 16F);
			this.labelTirageOpLocal.Location = new System.Drawing.Point(123, 622);
			this.labelTirageOpLocal.Name = "labelTirageOpLocal";
			this.labelTirageOpLocal.Size = new System.Drawing.Size(66, 32);
			this.labelTirageOpLocal.TabIndex = 94;
			this.labelTirageOpLocal.Text = "Local";
			this.labelTirageOpLocal.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label14
			// 
			this.label14.Font = new System.Drawing.Font("Calibri", 16F);
			this.label14.Location = new System.Drawing.Point(1, 622);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(116, 32);
			this.label14.TabIndex = 93;
			this.label14.Text = "Triage Op#";
			// 
			// labelTirageOpTotal
			// 
			this.labelTirageOpTotal.Font = new System.Drawing.Font("Calibri", 16F);
			this.labelTirageOpTotal.Location = new System.Drawing.Point(253, 622);
			this.labelTirageOpTotal.Name = "labelTirageOpTotal";
			this.labelTirageOpTotal.Size = new System.Drawing.Size(56, 28);
			this.labelTirageOpTotal.TabIndex = 95;
			this.labelTirageOpTotal.Text = "All";
			this.labelTirageOpTotal.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// mapNumberTriageOpsLocal
			// 
			this.mapNumberTriageOpsLocal.BackColor = System.Drawing.Color.LightCyan;
			this.mapNumberTriageOpsLocal.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberTriageOpsLocal.Location = new System.Drawing.Point(184, 615);
			this.mapNumberTriageOpsLocal.Name = "mapNumberTriageOpsLocal";
			this.mapNumberTriageOpsLocal.Size = new System.Drawing.Size(57, 39);
			this.mapNumberTriageOpsLocal.TabIndex = 86;
			this.mapNumberTriageOpsLocal.Text = "0";
			// 
			// mapNumberTriageOpsTotal
			// 
			this.mapNumberTriageOpsTotal.BackColor = System.Drawing.Color.LightCyan;
			this.mapNumberTriageOpsTotal.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberTriageOpsTotal.Location = new System.Drawing.Point(305, 615);
			this.mapNumberTriageOpsTotal.Name = "mapNumberTriageOpsTotal";
			this.mapNumberTriageOpsTotal.Size = new System.Drawing.Size(57, 39);
			this.mapNumberTriageOpsTotal.TabIndex = 96;
			this.mapNumberTriageOpsTotal.Text = "0";
			// 
			// userControlMapDetails1
			// 
			this.userControlMapDetails1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.userControlMapDetails1.Location = new System.Drawing.Point(4, 656);
			this.userControlMapDetails1.Name = "userControlMapDetails1";
			this.userControlMapDetails1.Size = new System.Drawing.Size(358, 130);
			this.userControlMapDetails1.TabIndex = 97;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("Calibri", 16F);
			this.label3.Location = new System.Drawing.Point(1, 924);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(116, 30);
			this.label3.TabIndex = 98;
			this.label3.Text = "Cust Down";
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Calibri", 16F);
			this.label5.Location = new System.Drawing.Point(112, 924);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(77, 30);
			this.label5.TabIndex = 101;
			this.label5.Text = "Count";
			this.label5.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Calibri", 16F);
			this.label7.Location = new System.Drawing.Point(247, 924);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(62, 30);
			this.label7.TabIndex = 102;
			this.label7.Text = "Time";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// mapNumberCustDownCount
			// 
			this.mapNumberCustDownCount.BackColor = System.Drawing.Color.LightCyan;
			this.mapNumberCustDownCount.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberCustDownCount.Location = new System.Drawing.Point(184, 918);
			this.mapNumberCustDownCount.Name = "mapNumberCustDownCount";
			this.mapNumberCustDownCount.Size = new System.Drawing.Size(57, 39);
			this.mapNumberCustDownCount.TabIndex = 104;
			this.mapNumberCustDownCount.Text = "0";
			// 
			// mapNumberCustDownTime
			// 
			this.mapNumberCustDownTime.BackColor = System.Drawing.Color.LightCyan;
			this.mapNumberCustDownTime.Font = new System.Drawing.Font("Calibri", 40F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.mapNumberCustDownTime.Location = new System.Drawing.Point(305, 918);
			this.mapNumberCustDownTime.Name = "mapNumberCustDownTime";
			this.mapNumberCustDownTime.Size = new System.Drawing.Size(57, 39);
			this.mapNumberCustDownTime.TabIndex = 105;
			this.mapNumberCustDownTime.Text = "0";
			// 
			// mapPanel
			// 
			this.mapPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.mapPanel.Location = new System.Drawing.Point(365, 69);
			this.mapPanel.Name = "mapPanel";
			this.mapPanel.Size = new System.Drawing.Size(1516, 970);
			this.mapPanel.TabIndex = 106;
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Calibri", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(411, 30);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(251, 36);
			this.label8.TabIndex = 107;
			this.label8.Text = "Use mouse to pan and zoom,\r\nbut it will not yet stick very well\r\n";
			// 
			// butZoomFit
			// 
			this.butZoomFit.Location = new System.Drawing.Point(340, 39);
			this.butZoomFit.Name = "butZoomFit";
			this.butZoomFit.Size = new System.Drawing.Size(60, 24);
			this.butZoomFit.TabIndex = 109;
			this.butZoomFit.Text = "Zoom Fit";
			this.butZoomFit.Click += new System.EventHandler(this.butZoomFit_Click);
			// 
			// FormMap
			// 
			this.ClientSize = new System.Drawing.Size(1884, 1042);
			this.Controls.Add(this.butZoomFit);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.mapPanel);
			this.Controls.Add(this.mapNumberCustDownTime);
			this.Controls.Add(this.mapNumberCustDownCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.userControlMapDetails1);
			this.Controls.Add(this.mapNumberTriageOpsTotal);
			this.Controls.Add(this.mapNumberTriageOpsLocal);
			this.Controls.Add(this.labelTirageOpLocal);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.labelTirageOpTotal);
			this.Controls.Add(this.escalationViewOfficesDown);
			this.Controls.Add(this.escalationView);
			this.Controls.Add(this.tabMain);
			this.Controls.Add(this.groupPhoneMetrics);
			this.Controls.Add(this.comboRoom);
			this.Controls.Add(this.labelCurrentTime);
			this.Controls.Add(this.labelTriageCoordinator);
			this.Controls.Add(this.menuMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormMap";
			this.Text = "Call Center Status Map";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FormMapHQ2_FormClosed);
			this.Load += new System.EventHandler(this.FormMap_Load);
			this.groupPhoneMetrics.ResumeLayout(false);
			this.groupPhoneMetrics.PerformLayout();
			this.tabMain.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private UI.MenuOD menuMain;
		private System.Windows.Forms.Label labelTriageCoordinator;
		private System.Windows.Forms.Label labelCurrentTime;
		private UI.ComboBox comboRoom;
		private UI.GroupBox groupPhoneMetrics;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label6;
		private MapNumber mapNumberTriageRedCalls;
		private MapNumber mapNumberVoicemailCalls;
		private MapNumber mapNumberTriageCalls;
		private MapNumber mapNumberChatCount;
		private MapNumber mapNumberTriageTime;
		private MapNumber mapNumberTriageRedTime;
		private MapNumber mapNumberChatTime;
		private MapNumber mapNumberVoicemailTime;
		private UI.TabControl tabMain;
		private UI.TabPage tabPage1;
		private UI.TabPage tabPage2;
		private UI.TabPage tabPage3;
		private UI.TabPage tabPage4;
		private EscalationView escalationView;
		private EscalationView escalationViewOfficesDown;
		private System.Windows.Forms.Label labelTirageOpLocal;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label labelTirageOpTotal;
		private MapNumber mapNumberTriageOpsLocal;
		private MapNumber mapNumberTriageOpsTotal;
		private OpenDental.InternalTools.Phones.UserControlMapDetails userControlMapDetails1;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private MapNumber mapNumberCustDownCount;
		private MapNumber mapNumberCustDownTime;
		private OpenDental.InternalTools.Phones.MapPanel mapPanel;
		private System.Windows.Forms.Label label8;
		private UI.Button butZoomFit;
	}
}