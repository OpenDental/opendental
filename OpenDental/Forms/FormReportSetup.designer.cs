namespace OpenDental{
	partial class FormReportSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormReportSetup));
			this.tabControl1 = new System.Windows.Forms.TabControl();
			this.tabDisplaySettings = new System.Windows.Forms.TabPage();
			this.userControlReportSetup = new OpenDental.User_Controls.UserControlReportSetup();
			this.tabReportPermissions = new System.Windows.Forms.TabPage();
			this.tabReportServer = new System.Windows.Forms.TabPage();
			this.checkUseReportServer = new System.Windows.Forms.CheckBox();
			this.radioReportServerMiddleTier = new System.Windows.Forms.RadioButton();
			this.radioReportServerDirect = new System.Windows.Forms.RadioButton();
			this.groupConnectionSettings = new System.Windows.Forms.GroupBox();
			this.comboServerName = new System.Windows.Forms.ComboBox();
			this.textMysqlPass = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textMysqlUser = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboDatabase = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.groupMiddleTier = new System.Windows.Forms.GroupBox();
			this.textMiddleTierURI = new System.Windows.Forms.TextBox();
			this.label11 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.tabMiscOptions = new System.Windows.Forms.TabPage();
			this.butIncompleteProcsExcludeCodes = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.textIncompleteProcsExcludeCodes = new OpenDental.ODtextBox();
			this.checkReportDisplayUnearnedTP = new System.Windows.Forms.CheckBox();
			this.comboReportWriteoff = new OpenDental.UI.ComboBoxOD();
			this.checkOutstandingRpDateTab = new System.Windows.Forms.CheckBox();
			this.checkBenefitAssumeGeneral = new System.Windows.Forms.CheckBox();
			this.checkReportsIncompleteProcsUnsigned = new System.Windows.Forms.CheckBox();
			this.checkReportsIncompleteProcsNoNotes = new System.Windows.Forms.CheckBox();
			this.checkNetProdDetailUseSnapshotToday = new System.Windows.Forms.CheckBox();
			this.checkProviderPayrollAllowToday = new System.Windows.Forms.CheckBox();
			this.checkReportsShowHistory = new System.Windows.Forms.CheckBox();
			this.checkReportProdWO = new System.Windows.Forms.CheckBox();
			this.checkReportPIClinic = new System.Windows.Forms.CheckBox();
			this.checkReportsShowPatNum = new System.Windows.Forms.CheckBox();
			this.checkReportPIClinicInfo = new System.Windows.Forms.CheckBox();
			this.checkReportPrintWrapColumns = new System.Windows.Forms.CheckBox();
			this.label1 = new System.Windows.Forms.Label();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.tabControl1.SuspendLayout();
			this.tabDisplaySettings.SuspendLayout();
			this.tabReportServer.SuspendLayout();
			this.groupConnectionSettings.SuspendLayout();
			this.groupMiddleTier.SuspendLayout();
			this.tabMiscOptions.SuspendLayout();
			this.SuspendLayout();
			// 
			// tabControl1
			// 
			this.tabControl1.Controls.Add(this.tabDisplaySettings);
			this.tabControl1.Controls.Add(this.tabReportPermissions);
			this.tabControl1.Controls.Add(this.tabReportServer);
			this.tabControl1.Controls.Add(this.tabMiscOptions);
			this.tabControl1.Location = new System.Drawing.Point(6, 4);
			this.tabControl1.Name = "tabControl1";
			this.tabControl1.SelectedIndex = 0;
			this.tabControl1.Size = new System.Drawing.Size(545, 638);
			this.tabControl1.TabIndex = 216;
			this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
			// 
			// tabDisplaySettings
			// 
			this.tabDisplaySettings.BackColor = System.Drawing.Color.Transparent;
			this.tabDisplaySettings.Controls.Add(this.userControlReportSetup);
			this.tabDisplaySettings.Location = new System.Drawing.Point(4, 22);
			this.tabDisplaySettings.Name = "tabDisplaySettings";
			this.tabDisplaySettings.Padding = new System.Windows.Forms.Padding(3);
			this.tabDisplaySettings.Size = new System.Drawing.Size(537, 612);
			this.tabDisplaySettings.TabIndex = 1;
			this.tabDisplaySettings.Text = "Display Settings";
			// 
			// userControlReportSetup
			// 
			this.userControlReportSetup.Location = new System.Drawing.Point(2, 2);
			this.userControlReportSetup.Name = "userControlReportSetup";
			this.userControlReportSetup.Size = new System.Drawing.Size(525, 607);
			this.userControlReportSetup.TabIndex = 1;
			// 
			// tabReportPermissions
			// 
			this.tabReportPermissions.Location = new System.Drawing.Point(4, 22);
			this.tabReportPermissions.Name = "tabReportPermissions";
			this.tabReportPermissions.Size = new System.Drawing.Size(537, 612);
			this.tabReportPermissions.TabIndex = 3;
			this.tabReportPermissions.Text = "Security Permissions";
			// 
			// tabReportServer
			// 
			this.tabReportServer.Controls.Add(this.checkUseReportServer);
			this.tabReportServer.Controls.Add(this.radioReportServerMiddleTier);
			this.tabReportServer.Controls.Add(this.radioReportServerDirect);
			this.tabReportServer.Controls.Add(this.groupConnectionSettings);
			this.tabReportServer.Controls.Add(this.groupMiddleTier);
			this.tabReportServer.Controls.Add(this.label2);
			this.tabReportServer.Location = new System.Drawing.Point(4, 22);
			this.tabReportServer.Name = "tabReportServer";
			this.tabReportServer.Padding = new System.Windows.Forms.Padding(3);
			this.tabReportServer.Size = new System.Drawing.Size(537, 612);
			this.tabReportServer.TabIndex = 2;
			this.tabReportServer.Text = "Report Server";
			// 
			// checkUseReportServer
			// 
			this.checkUseReportServer.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkUseReportServer.Location = new System.Drawing.Point(14, 43);
			this.checkUseReportServer.Name = "checkUseReportServer";
			this.checkUseReportServer.Size = new System.Drawing.Size(505, 18);
			this.checkUseReportServer.TabIndex = 15;
			this.checkUseReportServer.Text = "Use separate reporting server";
			this.checkUseReportServer.UseVisualStyleBackColor = true;
			this.checkUseReportServer.CheckedChanged += new System.EventHandler(this.checkReportingServer_CheckChanged);
			// 
			// radioReportServerMiddleTier
			// 
			this.radioReportServerMiddleTier.Location = new System.Drawing.Point(38, 320);
			this.radioReportServerMiddleTier.Name = "radioReportServerMiddleTier";
			this.radioReportServerMiddleTier.Size = new System.Drawing.Size(219, 20);
			this.radioReportServerMiddleTier.TabIndex = 228;
			this.radioReportServerMiddleTier.TabStop = true;
			this.radioReportServerMiddleTier.Text = "Middle Tier";
			this.radioReportServerMiddleTier.UseVisualStyleBackColor = true;
			this.radioReportServerMiddleTier.CheckedChanged += new System.EventHandler(this.checkReportingServer_CheckChanged);
			// 
			// radioReportServerDirect
			// 
			this.radioReportServerDirect.Location = new System.Drawing.Point(38, 76);
			this.radioReportServerDirect.Name = "radioReportServerDirect";
			this.radioReportServerDirect.Size = new System.Drawing.Size(219, 20);
			this.radioReportServerDirect.TabIndex = 227;
			this.radioReportServerDirect.TabStop = true;
			this.radioReportServerDirect.Text = "Direct Connection";
			this.radioReportServerDirect.UseVisualStyleBackColor = true;
			this.radioReportServerDirect.CheckedChanged += new System.EventHandler(this.checkReportingServer_CheckChanged);
			// 
			// groupConnectionSettings
			// 
			this.groupConnectionSettings.Controls.Add(this.comboServerName);
			this.groupConnectionSettings.Controls.Add(this.textMysqlPass);
			this.groupConnectionSettings.Controls.Add(this.label6);
			this.groupConnectionSettings.Controls.Add(this.textMysqlUser);
			this.groupConnectionSettings.Controls.Add(this.label3);
			this.groupConnectionSettings.Controls.Add(this.comboDatabase);
			this.groupConnectionSettings.Controls.Add(this.label5);
			this.groupConnectionSettings.Controls.Add(this.label4);
			this.groupConnectionSettings.Location = new System.Drawing.Point(61, 95);
			this.groupConnectionSettings.Name = "groupConnectionSettings";
			this.groupConnectionSettings.Size = new System.Drawing.Size(405, 216);
			this.groupConnectionSettings.TabIndex = 226;
			this.groupConnectionSettings.Text = "Connection Settings";
			// 
			// comboServerName
			// 
			this.comboServerName.FormattingEnabled = true;
			this.comboServerName.Location = new System.Drawing.Point(6, 36);
			this.comboServerName.Name = "comboServerName";
			this.comboServerName.Size = new System.Drawing.Size(386, 21);
			this.comboServerName.TabIndex = 214;
			// 
			// textMysqlPass
			// 
			this.textMysqlPass.Location = new System.Drawing.Point(6, 177);
			this.textMysqlPass.Name = "textMysqlPass";
			this.textMysqlPass.Size = new System.Drawing.Size(386, 20);
			this.textMysqlPass.TabIndex = 223;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 161);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(386, 17);
			this.label6.TabIndex = 221;
			this.label6.Text = "MySQL Password:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textMysqlUser
			// 
			this.textMysqlUser.Location = new System.Drawing.Point(6, 129);
			this.textMysqlUser.Name = "textMysqlUser";
			this.textMysqlUser.Size = new System.Drawing.Size(386, 20);
			this.textMysqlUser.TabIndex = 222;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 20);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(386, 17);
			this.label3.TabIndex = 215;
			this.label3.Text = "Server Name:";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboDatabase
			// 
			this.comboDatabase.FormattingEnabled = true;
			this.comboDatabase.Location = new System.Drawing.Point(6, 81);
			this.comboDatabase.Name = "comboDatabase";
			this.comboDatabase.Size = new System.Drawing.Size(386, 21);
			this.comboDatabase.TabIndex = 216;
			this.comboDatabase.DropDown += new System.EventHandler(this.comboDatabase_DropDown);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 113);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(386, 17);
			this.label5.TabIndex = 219;
			this.label5.Text = "MySQL User:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 65);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(386, 17);
			this.label4.TabIndex = 217;
			this.label4.Text = "Database:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// groupMiddleTier
			// 
			this.groupMiddleTier.Controls.Add(this.textMiddleTierURI);
			this.groupMiddleTier.Controls.Add(this.label11);
			this.groupMiddleTier.Controls.Add(this.label9);
			this.groupMiddleTier.Location = new System.Drawing.Point(61, 340);
			this.groupMiddleTier.Name = "groupMiddleTier";
			this.groupMiddleTier.Size = new System.Drawing.Size(405, 90);
			this.groupMiddleTier.TabIndex = 224;
			this.groupMiddleTier.Text = "Middle Tier";
			// 
			// textMiddleTierURI
			// 
			this.textMiddleTierURI.Location = new System.Drawing.Point(6, 33);
			this.textMiddleTierURI.Name = "textMiddleTierURI";
			this.textMiddleTierURI.Size = new System.Drawing.Size(386, 20);
			this.textMiddleTierURI.TabIndex = 7;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 56);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(386, 26);
			this.label11.TabIndex = 14;
			this.label11.Text = "The currently logged in user\'s credentials will be used to when accessing the Mid" +
    "dle Tier database.";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(6, 18);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(203, 17);
			this.label9.TabIndex = 9;
			this.label9.Text = "URI";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(0, 3);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(534, 37);
			this.label2.TabIndex = 213;
			this.label2.Text = resources.GetString("label2.Text");
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// tabMiscOptions
			// 
			this.tabMiscOptions.BackColor = System.Drawing.Color.Transparent;
			this.tabMiscOptions.Controls.Add(this.butIncompleteProcsExcludeCodes);
			this.tabMiscOptions.Controls.Add(this.label7);
			this.tabMiscOptions.Controls.Add(this.textIncompleteProcsExcludeCodes);
			this.tabMiscOptions.Controls.Add(this.checkReportDisplayUnearnedTP);
			this.tabMiscOptions.Controls.Add(this.comboReportWriteoff);
			this.tabMiscOptions.Controls.Add(this.checkOutstandingRpDateTab);
			this.tabMiscOptions.Controls.Add(this.checkBenefitAssumeGeneral);
			this.tabMiscOptions.Controls.Add(this.checkReportsIncompleteProcsUnsigned);
			this.tabMiscOptions.Controls.Add(this.checkReportsIncompleteProcsNoNotes);
			this.tabMiscOptions.Controls.Add(this.checkNetProdDetailUseSnapshotToday);
			this.tabMiscOptions.Controls.Add(this.checkProviderPayrollAllowToday);
			this.tabMiscOptions.Controls.Add(this.checkReportsShowHistory);
			this.tabMiscOptions.Controls.Add(this.checkReportProdWO);
			this.tabMiscOptions.Controls.Add(this.checkReportPIClinic);
			this.tabMiscOptions.Controls.Add(this.checkReportsShowPatNum);
			this.tabMiscOptions.Controls.Add(this.checkReportPIClinicInfo);
			this.tabMiscOptions.Controls.Add(this.checkReportPrintWrapColumns);
			this.tabMiscOptions.Controls.Add(this.label1);
			this.tabMiscOptions.Location = new System.Drawing.Point(4, 22);
			this.tabMiscOptions.Name = "tabMiscOptions";
			this.tabMiscOptions.Padding = new System.Windows.Forms.Padding(3);
			this.tabMiscOptions.Size = new System.Drawing.Size(537, 612);
			this.tabMiscOptions.TabIndex = 0;
			this.tabMiscOptions.Text = "Misc Settings";
			// 
			// butIncompleteProcsExcludeCodes
			// 
			this.butIncompleteProcsExcludeCodes.Location = new System.Drawing.Point(501, 301);
			this.butIncompleteProcsExcludeCodes.Name = "butIncompleteProcsExcludeCodes";
			this.butIncompleteProcsExcludeCodes.Size = new System.Drawing.Size(21, 21);
			this.butIncompleteProcsExcludeCodes.TabIndex = 220;
			this.butIncompleteProcsExcludeCodes.Text = "...";
			this.butIncompleteProcsExcludeCodes.UseVisualStyleBackColor = true;
			this.butIncompleteProcsExcludeCodes.Click += new System.EventHandler(this.butCodePicker_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(17, 286);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(478, 13);
			this.label7.TabIndex = 219;
			this.label7.Text = "Excluded Codes for Incomplete Procedure Notes Report";
			// 
			// textIncompleteProcsExcludeCodes
			// 
			this.textIncompleteProcsExcludeCodes.AcceptsTab = true;
			this.textIncompleteProcsExcludeCodes.BackColor = System.Drawing.SystemColors.Window;
			this.textIncompleteProcsExcludeCodes.DetectLinksEnabled = false;
			this.textIncompleteProcsExcludeCodes.DetectUrls = false;
			this.textIncompleteProcsExcludeCodes.Location = new System.Drawing.Point(20, 302);
			this.textIncompleteProcsExcludeCodes.Multiline = false;
			this.textIncompleteProcsExcludeCodes.Name = "textIncompleteProcsExcludeCodes";
			this.textIncompleteProcsExcludeCodes.QuickPasteType = OpenDentBusiness.QuickPasteType.Adjustment;
			this.textIncompleteProcsExcludeCodes.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.None;
			this.textIncompleteProcsExcludeCodes.Size = new System.Drawing.Size(475, 21);
			this.textIncompleteProcsExcludeCodes.TabIndex = 218;
			this.textIncompleteProcsExcludeCodes.Text = "";
			// 
			// checkReportDisplayUnearnedTP
			// 
			this.checkReportDisplayUnearnedTP.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportDisplayUnearnedTP.Location = new System.Drawing.Point(20, 227);
			this.checkReportDisplayUnearnedTP.Name = "checkReportDisplayUnearnedTP";
			this.checkReportDisplayUnearnedTP.Size = new System.Drawing.Size(511, 17);
			this.checkReportDisplayUnearnedTP.TabIndex = 215;
			this.checkReportDisplayUnearnedTP.Text = "Default to including hidden treatment planned prepayments on the Payments report";
			this.checkReportDisplayUnearnedTP.UseVisualStyleBackColor = false;
			// 
			// comboReportWriteoff
			// 
			this.comboReportWriteoff.Location = new System.Drawing.Point(20, 262);
			this.comboReportWriteoff.Name = "comboReportWriteoff";
			this.comboReportWriteoff.Size = new System.Drawing.Size(171, 21);
			this.comboReportWriteoff.TabIndex = 216;
			// 
			// checkOutstandingRpDateTab
			// 
			this.checkOutstandingRpDateTab.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkOutstandingRpDateTab.Location = new System.Drawing.Point(20, 210);
			this.checkOutstandingRpDateTab.Name = "checkOutstandingRpDateTab";
			this.checkOutstandingRpDateTab.Size = new System.Drawing.Size(511, 17);
			this.checkOutstandingRpDateTab.TabIndex = 214;
			this.checkOutstandingRpDateTab.Text = "Default to \'Date Range\' tab in Outstanding Insurance Report";
			// 
			// checkBenefitAssumeGeneral
			// 
			this.checkBenefitAssumeGeneral.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkBenefitAssumeGeneral.Location = new System.Drawing.Point(20, 193);
			this.checkBenefitAssumeGeneral.Name = "checkBenefitAssumeGeneral";
			this.checkBenefitAssumeGeneral.Size = new System.Drawing.Size(511, 17);
			this.checkBenefitAssumeGeneral.TabIndex = 213;
			this.checkBenefitAssumeGeneral.Text = "Assume all procedures are in the General benefit category by default in Treatment" +
    " Finder report";
			// 
			// checkReportsIncompleteProcsUnsigned
			// 
			this.checkReportsIncompleteProcsUnsigned.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportsIncompleteProcsUnsigned.Location = new System.Drawing.Point(20, 176);
			this.checkReportsIncompleteProcsUnsigned.Name = "checkReportsIncompleteProcsUnsigned";
			this.checkReportsIncompleteProcsUnsigned.Size = new System.Drawing.Size(511, 17);
			this.checkReportsIncompleteProcsUnsigned.TabIndex = 212;
			this.checkReportsIncompleteProcsUnsigned.Text = "Include procedures with a note that is unsigned in the Incomplete Procedures Repo" +
    "rt";
			// 
			// checkReportsIncompleteProcsNoNotes
			// 
			this.checkReportsIncompleteProcsNoNotes.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportsIncompleteProcsNoNotes.Location = new System.Drawing.Point(20, 159);
			this.checkReportsIncompleteProcsNoNotes.Name = "checkReportsIncompleteProcsNoNotes";
			this.checkReportsIncompleteProcsNoNotes.Size = new System.Drawing.Size(511, 17);
			this.checkReportsIncompleteProcsNoNotes.TabIndex = 211;
			this.checkReportsIncompleteProcsNoNotes.Text = "Include procedures without a note in the Incomplete Procedures Report";
			// 
			// checkNetProdDetailUseSnapshotToday
			// 
			this.checkNetProdDetailUseSnapshotToday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkNetProdDetailUseSnapshotToday.Location = new System.Drawing.Point(20, 142);
			this.checkNetProdDetailUseSnapshotToday.Name = "checkNetProdDetailUseSnapshotToday";
			this.checkNetProdDetailUseSnapshotToday.Size = new System.Drawing.Size(511, 17);
			this.checkNetProdDetailUseSnapshotToday.TabIndex = 209;
			this.checkNetProdDetailUseSnapshotToday.Text = "Calculate write-offs by claim snapshot for today\'s date in Net Production Detail " +
    "report";
			// 
			// checkProviderPayrollAllowToday
			// 
			this.checkProviderPayrollAllowToday.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkProviderPayrollAllowToday.Location = new System.Drawing.Point(20, 125);
			this.checkProviderPayrollAllowToday.Name = "checkProviderPayrollAllowToday";
			this.checkProviderPayrollAllowToday.Size = new System.Drawing.Size(475, 17);
			this.checkProviderPayrollAllowToday.TabIndex = 208;
			this.checkProviderPayrollAllowToday.Text = "Allow using today\'s date in Provider Payroll report.";
			// 
			// checkReportsShowHistory
			// 
			this.checkReportsShowHistory.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportsShowHistory.Location = new System.Drawing.Point(20, 108);
			this.checkReportsShowHistory.Name = "checkReportsShowHistory";
			this.checkReportsShowHistory.Size = new System.Drawing.Size(475, 17);
			this.checkReportsShowHistory.TabIndex = 206;
			this.checkReportsShowHistory.Text = "Show a verbose history when previewing reports.";
			// 
			// checkReportProdWO
			// 
			this.checkReportProdWO.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportProdWO.Location = new System.Drawing.Point(20, 57);
			this.checkReportProdWO.Name = "checkReportProdWO";
			this.checkReportProdWO.Size = new System.Drawing.Size(345, 17);
			this.checkReportProdWO.TabIndex = 198;
			this.checkReportProdWO.Text = "Monthly P&&I scheduled production subtracts PPO write-offs";
			// 
			// checkReportPIClinic
			// 
			this.checkReportPIClinic.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportPIClinic.Location = new System.Drawing.Point(20, 91);
			this.checkReportPIClinic.Name = "checkReportPIClinic";
			this.checkReportPIClinic.Size = new System.Drawing.Size(345, 17);
			this.checkReportPIClinic.TabIndex = 202;
			this.checkReportPIClinic.Text = "Default to showing clinic breakdown on P&&I reports.";
			// 
			// checkReportsShowPatNum
			// 
			this.checkReportsShowPatNum.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportsShowPatNum.Location = new System.Drawing.Point(20, 40);
			this.checkReportsShowPatNum.Name = "checkReportsShowPatNum";
			this.checkReportsShowPatNum.Size = new System.Drawing.Size(345, 17);
			this.checkReportsShowPatNum.TabIndex = 196;
			this.checkReportsShowPatNum.Text = "Show PatNum: Aging, OutstandingIns, ProcsNotBilled";
			// 
			// checkReportPIClinicInfo
			// 
			this.checkReportPIClinicInfo.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportPIClinicInfo.Location = new System.Drawing.Point(20, 74);
			this.checkReportPIClinicInfo.Name = "checkReportPIClinicInfo";
			this.checkReportPIClinicInfo.Size = new System.Drawing.Size(345, 17);
			this.checkReportPIClinicInfo.TabIndex = 200;
			this.checkReportPIClinicInfo.Text = "Default to showing clinic info on Daily P&&I report.";
			// 
			// checkReportPrintWrapColumns
			// 
			this.checkReportPrintWrapColumns.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkReportPrintWrapColumns.Location = new System.Drawing.Point(20, 23);
			this.checkReportPrintWrapColumns.Name = "checkReportPrintWrapColumns";
			this.checkReportPrintWrapColumns.Size = new System.Drawing.Size(345, 17);
			this.checkReportPrintWrapColumns.TabIndex = 194;
			this.checkReportPrintWrapColumns.Text = "Wrap columns when printing";
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(17, 247);
			this.label1.Name = "label1";
			this.label1.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.label1.Size = new System.Drawing.Size(244, 18);
			this.label1.TabIndex = 217;
			this.label1.Text = "Default selected date for PPO write-offs";
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(388, 649);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(469, 649);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormReportSetup
			// 
			this.ClientSize = new System.Drawing.Size(556, 681);
			this.Controls.Add(this.tabControl1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormReportSetup";
			this.Text = "Report Setup";
			this.Load += new System.EventHandler(this.FormReportSetup_Load);
			this.tabControl1.ResumeLayout(false);
			this.tabDisplaySettings.ResumeLayout(false);
			this.tabReportServer.ResumeLayout(false);
			this.groupConnectionSettings.ResumeLayout(false);
			this.groupConnectionSettings.PerformLayout();
			this.groupMiddleTier.ResumeLayout(false);
			this.groupMiddleTier.PerformLayout();
			this.tabMiscOptions.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkReportProdWO;
		private System.Windows.Forms.CheckBox checkReportsShowPatNum;
		private System.Windows.Forms.CheckBox checkReportPIClinic;
		private System.Windows.Forms.CheckBox checkReportPrintWrapColumns;
    private System.Windows.Forms.CheckBox checkReportPIClinicInfo;
		private System.Windows.Forms.TabControl tabControl1;
		private System.Windows.Forms.TabPage tabMiscOptions;
		private System.Windows.Forms.TabPage tabDisplaySettings;
		private System.Windows.Forms.CheckBox checkReportsShowHistory;
		private System.Windows.Forms.CheckBox checkProviderPayrollAllowToday;
		private System.Windows.Forms.CheckBox checkNetProdDetailUseSnapshotToday;
		private System.Windows.Forms.CheckBox checkReportsIncompleteProcsUnsigned;
		private System.Windows.Forms.CheckBox checkReportsIncompleteProcsNoNotes;
    private System.Windows.Forms.TabPage tabReportServer;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox textMysqlPass;
    private System.Windows.Forms.TextBox textMysqlUser;
    private System.Windows.Forms.ComboBox comboDatabase;
    private System.Windows.Forms.ComboBox comboServerName;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.Label label6;
    private System.Windows.Forms.Label label5;
    private System.Windows.Forms.Label label4;
    private System.Windows.Forms.GroupBox groupConnectionSettings;
    private System.Windows.Forms.GroupBox groupMiddleTier;
    private System.Windows.Forms.TextBox textMiddleTierURI;
    private System.Windows.Forms.Label label11;
    private System.Windows.Forms.Label label9;
    private System.Windows.Forms.RadioButton radioReportServerMiddleTier;
    private System.Windows.Forms.RadioButton radioReportServerDirect;
    private System.Windows.Forms.CheckBox checkUseReportServer;
		private System.Windows.Forms.TabPage tabReportPermissions;
		private User_Controls.UserControlReportSetup userControlReportSetup;
		private System.Windows.Forms.CheckBox checkBenefitAssumeGeneral;
		private System.Windows.Forms.CheckBox checkOutstandingRpDateTab;
		private UI.ComboBoxOD comboReportWriteoff;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkReportDisplayUnearnedTP;
		private System.Windows.Forms.Label label7;
		private ODtextBox textIncompleteProcsExcludeCodes;
		private UI.Button butIncompleteProcsExcludeCodes;
	}
}