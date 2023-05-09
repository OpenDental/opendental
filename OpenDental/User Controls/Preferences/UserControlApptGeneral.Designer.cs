
namespace OpenDental {
	partial class UserControlApptGeneral {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.groupBoxProduction = new OpenDental.UI.GroupBox();
			this.checkApptModuleAdjInProd = new OpenDental.UI.CheckBox();
			this.checkApptModuleProductionUsesOps = new OpenDental.UI.CheckBox();
			this.groupBoxOD2 = new OpenDental.UI.GroupBox();
			this.checkAppointmentTimeIsLocked = new OpenDental.UI.CheckBox();
			this.checkApptsRequireProcs = new OpenDental.UI.CheckBox();
			this.checkApptAllowFutureComplete = new OpenDental.UI.CheckBox();
			this.textApptWithoutProcsDefaultLength = new OpenDental.ValidNum();
			this.checkUnscheduledListNoRecalls = new OpenDental.UI.CheckBox();
			this.checkApptsAllowOverlap = new OpenDental.UI.CheckBox();
			this.labelApptWithoutProcsDefaultLength = new System.Windows.Forms.Label();
			this.checkPreventChangesToComplAppts = new OpenDental.UI.CheckBox();
			this.checkApptAllowEmptyComplete = new OpenDental.UI.CheckBox();
			this.groupBoxCalendarBehavior = new OpenDental.UI.GroupBox();
			this.textApptAutoRefreshRange = new OpenDental.ValidNum();
			this.labelApptAutoRefreshRange = new System.Windows.Forms.Label();
			this.comboSearchBehavior = new OpenDental.UI.ComboBox();
			this.label13 = new System.Windows.Forms.Label();
			this.checkApptTimeReset = new OpenDental.UI.CheckBox();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.label37 = new System.Windows.Forms.Label();
			this.comboBrokenApptProc = new OpenDental.UI.ComboBox();
			this.checkBrokenApptCommLog = new OpenDental.UI.CheckBox();
			this.checkBrokenApptRequiredOnMove = new OpenDental.UI.CheckBox();
			this.checkBrokenApptAdjustment = new OpenDental.UI.CheckBox();
			this.comboBrokenApptAdjType = new OpenDental.UI.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.groupBoxProduction.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.groupBoxCalendarBehavior.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBoxProduction
			// 
			this.groupBoxProduction.Controls.Add(this.checkApptModuleAdjInProd);
			this.groupBoxProduction.Controls.Add(this.checkApptModuleProductionUsesOps);
			this.groupBoxProduction.Location = new System.Drawing.Point(20, 410);
			this.groupBoxProduction.Name = "groupBoxProduction";
			this.groupBoxProduction.Size = new System.Drawing.Size(450, 49);
			this.groupBoxProduction.TabIndex = 327;
			this.groupBoxProduction.Text = "Production";
			// 
			// checkApptModuleAdjInProd
			// 
			this.checkApptModuleAdjInProd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptModuleAdjInProd.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleAdjInProd.Location = new System.Drawing.Point(97, 10);
			this.checkApptModuleAdjInProd.Name = "checkApptModuleAdjInProd";
			this.checkApptModuleAdjInProd.Size = new System.Drawing.Size(343, 17);
			this.checkApptModuleAdjInProd.TabIndex = 307;
			this.checkApptModuleAdjInProd.Text = "Add daily adjustments to net production";
			// 
			// checkApptModuleProductionUsesOps
			// 
			this.checkApptModuleProductionUsesOps.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptModuleProductionUsesOps.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptModuleProductionUsesOps.Location = new System.Drawing.Point(34, 27);
			this.checkApptModuleProductionUsesOps.Name = "checkApptModuleProductionUsesOps";
			this.checkApptModuleProductionUsesOps.Size = new System.Drawing.Size(406, 17);
			this.checkApptModuleProductionUsesOps.TabIndex = 309;
			this.checkApptModuleProductionUsesOps.Text = "Appointments Module production uses operatories";
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.Controls.Add(this.checkAppointmentTimeIsLocked);
			this.groupBoxOD2.Controls.Add(this.checkApptsRequireProcs);
			this.groupBoxOD2.Controls.Add(this.checkApptAllowFutureComplete);
			this.groupBoxOD2.Controls.Add(this.textApptWithoutProcsDefaultLength);
			this.groupBoxOD2.Controls.Add(this.checkUnscheduledListNoRecalls);
			this.groupBoxOD2.Controls.Add(this.checkApptsAllowOverlap);
			this.groupBoxOD2.Controls.Add(this.labelApptWithoutProcsDefaultLength);
			this.groupBoxOD2.Controls.Add(this.checkPreventChangesToComplAppts);
			this.groupBoxOD2.Controls.Add(this.checkApptAllowEmptyComplete);
			this.groupBoxOD2.Location = new System.Drawing.Point(20, 239);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(450, 165);
			this.groupBoxOD2.TabIndex = 325;
			this.groupBoxOD2.Text = "Appointment Behavior";
			// 
			// checkAppointmentTimeIsLocked
			// 
			this.checkAppointmentTimeIsLocked.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkAppointmentTimeIsLocked.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAppointmentTimeIsLocked.Location = new System.Drawing.Point(129, 10);
			this.checkAppointmentTimeIsLocked.Name = "checkAppointmentTimeIsLocked";
			this.checkAppointmentTimeIsLocked.Size = new System.Drawing.Size(311, 17);
			this.checkAppointmentTimeIsLocked.TabIndex = 297;
			this.checkAppointmentTimeIsLocked.Text = "Appointment time locked by default";
			this.checkAppointmentTimeIsLocked.MouseUp += new System.Windows.Forms.MouseEventHandler(this.checkAppointmentTimeIsLocked_MouseUp);
			// 
			// checkApptsRequireProcs
			// 
			this.checkApptsRequireProcs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptsRequireProcs.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsRequireProcs.Location = new System.Drawing.Point(34, 27);
			this.checkApptsRequireProcs.Name = "checkApptsRequireProcs";
			this.checkApptsRequireProcs.Size = new System.Drawing.Size(406, 17);
			this.checkApptsRequireProcs.TabIndex = 310;
			this.checkApptsRequireProcs.Text = "Appointments require procedures";
			this.checkApptsRequireProcs.CheckedChanged += new System.EventHandler(this.checkApptsRequireProcs_CheckedChanged);
			// 
			// checkApptAllowFutureComplete
			// 
			this.checkApptAllowFutureComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptAllowFutureComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptAllowFutureComplete.Location = new System.Drawing.Point(34, 44);
			this.checkApptAllowFutureComplete.Name = "checkApptAllowFutureComplete";
			this.checkApptAllowFutureComplete.Size = new System.Drawing.Size(406, 17);
			this.checkApptAllowFutureComplete.TabIndex = 311;
			this.checkApptAllowFutureComplete.Text = "Allow setting future appointments complete";
			// 
			// textApptWithoutProcsDefaultLength
			// 
			this.textApptWithoutProcsDefaultLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textApptWithoutProcsDefaultLength.Location = new System.Drawing.Point(340, 65);
			this.textApptWithoutProcsDefaultLength.MaxVal = 600;
			this.textApptWithoutProcsDefaultLength.Name = "textApptWithoutProcsDefaultLength";
			this.textApptWithoutProcsDefaultLength.ShowZero = false;
			this.textApptWithoutProcsDefaultLength.Size = new System.Drawing.Size(100, 20);
			this.textApptWithoutProcsDefaultLength.TabIndex = 314;
			// 
			// checkUnscheduledListNoRecalls
			// 
			this.checkUnscheduledListNoRecalls.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkUnscheduledListNoRecalls.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkUnscheduledListNoRecalls.Location = new System.Drawing.Point(34, 140);
			this.checkUnscheduledListNoRecalls.Name = "checkUnscheduledListNoRecalls";
			this.checkUnscheduledListNoRecalls.Size = new System.Drawing.Size(406, 17);
			this.checkUnscheduledListNoRecalls.TabIndex = 318;
			this.checkUnscheduledListNoRecalls.Text = "Do not allow recall appointments on the Unscheduled List";
			// 
			// checkApptsAllowOverlap
			// 
			this.checkApptsAllowOverlap.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptsAllowOverlap.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptsAllowOverlap.Location = new System.Drawing.Point(34, 123);
			this.checkApptsAllowOverlap.Name = "checkApptsAllowOverlap";
			this.checkApptsAllowOverlap.Size = new System.Drawing.Size(406, 17);
			this.checkApptsAllowOverlap.TabIndex = 322;
			this.checkApptsAllowOverlap.Text = "Appointments allow overlap";
			// 
			// labelApptWithoutProcsDefaultLength
			// 
			this.labelApptWithoutProcsDefaultLength.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelApptWithoutProcsDefaultLength.Location = new System.Drawing.Point(18, 70);
			this.labelApptWithoutProcsDefaultLength.Name = "labelApptWithoutProcsDefaultLength";
			this.labelApptWithoutProcsDefaultLength.Size = new System.Drawing.Size(319, 17);
			this.labelApptWithoutProcsDefaultLength.TabIndex = 313;
			this.labelApptWithoutProcsDefaultLength.Text = "Appointment without procedures default length";
			this.labelApptWithoutProcsDefaultLength.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkPreventChangesToComplAppts
			// 
			this.checkPreventChangesToComplAppts.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkPreventChangesToComplAppts.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkPreventChangesToComplAppts.Location = new System.Drawing.Point(34, 106);
			this.checkPreventChangesToComplAppts.Name = "checkPreventChangesToComplAppts";
			this.checkPreventChangesToComplAppts.Size = new System.Drawing.Size(406, 17);
			this.checkPreventChangesToComplAppts.TabIndex = 321;
			this.checkPreventChangesToComplAppts.Text = "Prevent changes to completed appointments with completed procedures";
			// 
			// checkApptAllowEmptyComplete
			// 
			this.checkApptAllowEmptyComplete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptAllowEmptyComplete.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptAllowEmptyComplete.Location = new System.Drawing.Point(34, 89);
			this.checkApptAllowEmptyComplete.Name = "checkApptAllowEmptyComplete";
			this.checkApptAllowEmptyComplete.Size = new System.Drawing.Size(406, 17);
			this.checkApptAllowEmptyComplete.TabIndex = 312;
			this.checkApptAllowEmptyComplete.Text = "Allow setting appointments without procedures complete";
			// 
			// groupBoxCalendarBehavior
			// 
			this.groupBoxCalendarBehavior.Controls.Add(this.textApptAutoRefreshRange);
			this.groupBoxCalendarBehavior.Controls.Add(this.labelApptAutoRefreshRange);
			this.groupBoxCalendarBehavior.Controls.Add(this.comboSearchBehavior);
			this.groupBoxCalendarBehavior.Controls.Add(this.label13);
			this.groupBoxCalendarBehavior.Controls.Add(this.checkApptTimeReset);
			this.groupBoxCalendarBehavior.Location = new System.Drawing.Point(20, 146);
			this.groupBoxCalendarBehavior.Name = "groupBoxCalendarBehavior";
			this.groupBoxCalendarBehavior.Size = new System.Drawing.Size(450, 87);
			this.groupBoxCalendarBehavior.TabIndex = 324;
			this.groupBoxCalendarBehavior.Text = "Calendar Behavior";
			// 
			// textApptAutoRefreshRange
			// 
			this.textApptAutoRefreshRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textApptAutoRefreshRange.Location = new System.Drawing.Point(370, 39);
			this.textApptAutoRefreshRange.MaxVal = 600;
			this.textApptAutoRefreshRange.MinVal = -1;
			this.textApptAutoRefreshRange.Name = "textApptAutoRefreshRange";
			this.textApptAutoRefreshRange.ShowZero = false;
			this.textApptAutoRefreshRange.Size = new System.Drawing.Size(70, 20);
			this.textApptAutoRefreshRange.TabIndex = 320;
			// 
			// labelApptAutoRefreshRange
			// 
			this.labelApptAutoRefreshRange.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.labelApptAutoRefreshRange.Location = new System.Drawing.Point(34, 43);
			this.labelApptAutoRefreshRange.Name = "labelApptAutoRefreshRange";
			this.labelApptAutoRefreshRange.Size = new System.Drawing.Size(333, 18);
			this.labelApptAutoRefreshRange.TabIndex = 319;
			this.labelApptAutoRefreshRange.Text = "Number of days out to automatically refresh Appointments Module";
			this.labelApptAutoRefreshRange.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboSearchBehavior
			// 
			this.comboSearchBehavior.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboSearchBehavior.Location = new System.Drawing.Point(237, 10);
			this.comboSearchBehavior.Name = "comboSearchBehavior";
			this.comboSearchBehavior.Size = new System.Drawing.Size(203, 21);
			this.comboSearchBehavior.TabIndex = 298;
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label13.Location = new System.Drawing.Point(111, 13);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(123, 17);
			this.label13.TabIndex = 299;
			this.label13.Text = "Search Behavior";
			this.label13.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// checkApptTimeReset
			// 
			this.checkApptTimeReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkApptTimeReset.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkApptTimeReset.Location = new System.Drawing.Point(34, 65);
			this.checkApptTimeReset.Name = "checkApptTimeReset";
			this.checkApptTimeReset.Size = new System.Drawing.Size(406, 17);
			this.checkApptTimeReset.TabIndex = 306;
			this.checkApptTimeReset.Text = "Reset calendar to today on Clinic select";
			// 
			// groupBox2
			// 
			this.groupBox2.BackColor = System.Drawing.Color.White;
			this.groupBox2.Controls.Add(this.label37);
			this.groupBox2.Controls.Add(this.comboBrokenApptProc);
			this.groupBox2.Controls.Add(this.checkBrokenApptCommLog);
			this.groupBox2.Controls.Add(this.checkBrokenApptRequiredOnMove);
			this.groupBox2.Controls.Add(this.checkBrokenApptAdjustment);
			this.groupBox2.Controls.Add(this.comboBrokenApptAdjType);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Location = new System.Drawing.Point(20, 10);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(450, 130);
			this.groupBox2.TabIndex = 305;
			this.groupBox2.Text = "Broken Appointment Automation";
			// 
			// label37
			// 
			this.label37.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label37.Location = new System.Drawing.Point(35, 18);
			this.label37.Name = "label37";
			this.label37.Size = new System.Drawing.Size(240, 17);
			this.label37.TabIndex = 235;
			this.label37.Text = "Broken appointment procedure type";
			this.label37.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// comboBrokenApptProc
			// 
			this.comboBrokenApptProc.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBrokenApptProc.Location = new System.Drawing.Point(278, 15);
			this.comboBrokenApptProc.Name = "comboBrokenApptProc";
			this.comboBrokenApptProc.Size = new System.Drawing.Size(162, 21);
			this.comboBrokenApptProc.TabIndex = 234;
			// 
			// checkBrokenApptCommLog
			// 
			this.checkBrokenApptCommLog.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBrokenApptCommLog.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptCommLog.Location = new System.Drawing.Point(55, 40);
			this.checkBrokenApptCommLog.Name = "checkBrokenApptCommLog";
			this.checkBrokenApptCommLog.Size = new System.Drawing.Size(385, 17);
			this.checkBrokenApptCommLog.TabIndex = 61;
			this.checkBrokenApptCommLog.Text = "Make broken appointment commlog";
			// 
			// checkBrokenApptRequiredOnMove
			// 
			this.checkBrokenApptRequiredOnMove.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBrokenApptRequiredOnMove.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptRequiredOnMove.Location = new System.Drawing.Point(34, 103);
			this.checkBrokenApptRequiredOnMove.Name = "checkBrokenApptRequiredOnMove";
			this.checkBrokenApptRequiredOnMove.Size = new System.Drawing.Size(406, 17);
			this.checkBrokenApptRequiredOnMove.TabIndex = 323;
			this.checkBrokenApptRequiredOnMove.Text = "Force users to break scheduled appointments before rescheduling";
			// 
			// checkBrokenApptAdjustment
			// 
			this.checkBrokenApptAdjustment.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.checkBrokenApptAdjustment.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBrokenApptAdjustment.Location = new System.Drawing.Point(55, 57);
			this.checkBrokenApptAdjustment.Name = "checkBrokenApptAdjustment";
			this.checkBrokenApptAdjustment.Size = new System.Drawing.Size(385, 17);
			this.checkBrokenApptAdjustment.TabIndex = 217;
			this.checkBrokenApptAdjustment.Text = "Make broken appointment adjustment";
			// 
			// comboBrokenApptAdjType
			// 
			this.comboBrokenApptAdjType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.comboBrokenApptAdjType.Location = new System.Drawing.Point(237, 78);
			this.comboBrokenApptAdjType.Name = "comboBrokenApptAdjType";
			this.comboBrokenApptAdjType.Size = new System.Drawing.Size(203, 21);
			this.comboBrokenApptAdjType.TabIndex = 70;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(37, 81);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(197, 17);
			this.label7.TabIndex = 71;
			this.label7.Text = "Broken appt default adj type";
			this.label7.TextAlign = System.Drawing.ContentAlignment.TopRight;
			// 
			// UserControlApptGeneral
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.BackColor = System.Drawing.Color.White;
			this.Controls.Add(this.groupBoxProduction);
			this.Controls.Add(this.groupBoxOD2);
			this.Controls.Add(this.groupBoxCalendarBehavior);
			this.Controls.Add(this.groupBox2);
			this.Name = "UserControlApptGeneral";
			this.Size = new System.Drawing.Size(494, 624);
			this.groupBoxProduction.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.groupBoxOD2.PerformLayout();
			this.groupBoxCalendarBehavior.ResumeLayout(false);
			this.groupBoxCalendarBehavior.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.CheckBox checkBrokenApptRequiredOnMove;
		private OpenDental.UI.CheckBox checkApptsAllowOverlap;
		private OpenDental.UI.CheckBox checkPreventChangesToComplAppts;
		private ValidNum textApptAutoRefreshRange;
		private System.Windows.Forms.Label labelApptAutoRefreshRange;
		private OpenDental.UI.CheckBox checkUnscheduledListNoRecalls;
		private ValidNum textApptWithoutProcsDefaultLength;
		private System.Windows.Forms.Label labelApptWithoutProcsDefaultLength;
		private OpenDental.UI.CheckBox checkApptAllowEmptyComplete;
		private OpenDental.UI.CheckBox checkApptAllowFutureComplete;
		private OpenDental.UI.CheckBox checkApptsRequireProcs;
		private OpenDental.UI.CheckBox checkApptModuleProductionUsesOps;
		private OpenDental.UI.CheckBox checkApptModuleAdjInProd;
		private OpenDental.UI.CheckBox checkApptTimeReset;
		private UI.GroupBox groupBox2;
		private System.Windows.Forms.Label label37;
		private UI.ComboBox comboBrokenApptProc;
		private UI.CheckBox checkBrokenApptCommLog;
		private UI.CheckBox checkBrokenApptAdjustment;
		private UI.ComboBox comboBrokenApptAdjType;
		private System.Windows.Forms.Label label7;
		private OpenDental.UI.CheckBox checkAppointmentTimeIsLocked;
		private UI.ComboBox comboSearchBehavior;
		private System.Windows.Forms.Label label13;
		private UI.GroupBox groupBoxCalendarBehavior;
		private UI.GroupBox groupBoxOD2;
		private UI.GroupBox groupBoxProduction;
	}
}
