using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Forms;
using OpenDental.UI;
using OpenDentBusiness;
using CodeBase;

namespace OpenDental{
	/// <summary>
	/// Summary description for FormBasicTemplate.
	/// </summary>
	public class FormTimeCard:FormODBase {
		private System.Windows.Forms.Label labelRegularTime;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.ComponentModel.IContainer components;
		private OpenDental.UI.Button butClose;
		private System.Windows.Forms.TextBox textTotal;
		private System.Windows.Forms.Timer timerUpdateBreak;
		///<summary>True to default to viewing breaks. False for regular hours.</summary>
		public bool IsBreaks;
		///<summary>Server time minus local computer time, usually +/- 1 or 2 minutes</summary>
		private TimeSpan TimeDelta;
		private OpenDental.UI.GridOD gridMain;
		private GroupBox groupBox1;
		private OpenDental.UI.Button butRight;
		private OpenDental.UI.Button butLeft;
		private Label label4;
		private TextBox textDateStart;
		private TextBox textDatePaycheck;
		private TextBox textDateStop;
		private List<ClockEvent> ClockEventList;
		private OpenDental.UI.Button butAdj;
		public int SelectedPayPeriod;
		private Label labelOvertime;
		private TextBox textOvertime;
		private OpenDental.UI.Button butCalcWeekOT;
		private OpenDental.UI.Button butPrint;
		private List<TimeAdjust> TimeAdjustList;
		private int linesPrinted;
		private GroupBox groupBox2;
		private RadioButton radioBreaks;
		private RadioButton radioTimeCard;
		//private OpenDental.UI.PrintPreview printPreview;
		///<summary>An array list of ojects representing the rows in the table. Can be either clockEvents or timeAdjusts.</summary>
		private ArrayList mergedAL;
		///<summary>The running weekly total, whether it gets displayed or not.</summary>
		private TimeSpan[] WeeklyTotals;
		private TextBox textOvertime2;
		private TextBox textTotal2;
		public Employee EmployeeCur;
		private OpenDental.UI.Button butCalcDaily;
		private TextBox textRateTwo2;
		private Label labelRateTwo;
		private TextBox textRateTwo;
		private GroupBox groupEmployee;
		private UI.Button butPrevEmp;
		private UI.Button butNextEmp;
		///<summary>Used to determine the order to advance through employee timecards in this window.</summary>
		public bool IsByLastName;
		///<summary>Cached list of employees sorted based on IsByLastName</summary>
		private List<Employee> _listEmp=new List<Employee>();
		///<summary>Filled when FillMain is called and fromDB=true.  If fromDB is false, we used this stored value from before instead to reduce calls to DB.  Because fillgrid does math on weekspan, this is a temporary cache of the last time we calculated it from the database.</summary>
		private TimeSpan storedWeekSpan;
		private Label labelNote;
		private TextBox textNote;
		private TimeAdjust _timeAdjustNote;
		private Label labelPTO;
		private TextBox textPTO;
		private TextBox textPTO2;
		private TextBox textUnpaidProtectedLeave;
		private TextBox textUnpaidProtectedLeave2;
		private Label labelUnpaidProtectedLeave;
		private List<PayPeriod> _listPayPeriods;

		///<summary>If true, the current employee cannot edit their own time card.</summary>
		private bool _cannotEditOwnTimecard {
			get {
				return _isTimeCardSecurityApplicable &&
					PrefC.GetBool(PrefName.TimecardUsersDontEditOwnCard);
			}
		}

		///<summary>If true, Time Card Security is enabled and should be considered for the current user.</summary>
		private bool _isTimeCardSecurityApplicable {
			get{
				return Security.CurUser!=null &&
				Security.CurUser.EmployeeNum==EmployeeCur.EmployeeNum &&
				PrefC.GetBool(PrefName.TimecardSecurityEnabled);
			} 
		}

		/// <summary>If true, the current employee can only edit their timecard for the current pay period</summary>
		private bool _cannotEditSelectedPayPeriod {
			get {
				return _isTimeCardSecurityApplicable &&
					PrefC.GetBool(PrefName.TimecardUsersCantEditPastPayPeriods) &&
					SelectedPayPeriod!=PayPeriods.GetForDate(DateTime.Today);
			}
		}

		///<summary></summary>
		public FormTimeCard(List<Employee> listEmployees)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			InitializeLayoutManager();
			Lan.F(this);
			_listEmp=listEmployees;
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormTimeCard));
			this.textTotal = new System.Windows.Forms.TextBox();
			this.labelRegularTime = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.timerUpdateBreak = new System.Windows.Forms.Timer(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.labelNote = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.textDatePaycheck = new System.Windows.Forms.TextBox();
			this.textDateStop = new System.Windows.Forms.TextBox();
			this.textDateStart = new System.Windows.Forms.TextBox();
			this.butRight = new OpenDental.UI.Button();
			this.butLeft = new OpenDental.UI.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.labelOvertime = new System.Windows.Forms.Label();
			this.textOvertime = new System.Windows.Forms.TextBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioBreaks = new System.Windows.Forms.RadioButton();
			this.radioTimeCard = new System.Windows.Forms.RadioButton();
			this.textOvertime2 = new System.Windows.Forms.TextBox();
			this.textTotal2 = new System.Windows.Forms.TextBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textRateTwo2 = new System.Windows.Forms.TextBox();
			this.labelRateTwo = new System.Windows.Forms.Label();
			this.textRateTwo = new System.Windows.Forms.TextBox();
			this.butCalcDaily = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.butCalcWeekOT = new OpenDental.UI.Button();
			this.butAdj = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.groupEmployee = new System.Windows.Forms.GroupBox();
			this.butPrevEmp = new OpenDental.UI.Button();
			this.butNextEmp = new OpenDental.UI.Button();
			this.labelPTO = new System.Windows.Forms.Label();
			this.textPTO = new System.Windows.Forms.TextBox();
			this.textPTO2 = new System.Windows.Forms.TextBox();
			this.textUnpaidProtectedLeave = new System.Windows.Forms.TextBox();
			this.textUnpaidProtectedLeave2 = new System.Windows.Forms.TextBox();
			this.labelUnpaidProtectedLeave = new System.Windows.Forms.Label();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupEmployee.SuspendLayout();
			this.SuspendLayout();
			// 
			// textTotal
			// 
			this.textTotal.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTotal.Location = new System.Drawing.Point(491, 623);
			this.textTotal.Name = "textTotal";
			this.textTotal.Size = new System.Drawing.Size(66, 20);
			this.textTotal.TabIndex = 3;
			this.textTotal.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelRegularTime
			// 
			this.labelRegularTime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRegularTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRegularTime.Location = new System.Drawing.Point(385, 624);
			this.labelRegularTime.Name = "labelRegularTime";
			this.labelRegularTime.Size = new System.Drawing.Size(100, 17);
			this.labelRegularTime.TabIndex = 4;
			this.labelRegularTime.Text = "Regular Time";
			this.labelRegularTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(107, 8);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(73, 18);
			this.label2.TabIndex = 6;
			this.label2.Text = "Start Date";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(110, 28);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(70, 18);
			this.label3.TabIndex = 8;
			this.label3.Text = "End Date";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// timerUpdateBreak
			// 
			this.timerUpdateBreak.Enabled = true;
			this.timerUpdateBreak.Interval = 1000;
			this.timerUpdateBreak.Tick += new System.EventHandler(this.timerUpdateBreak_Tick);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.labelNote);
			this.groupBox1.Controls.Add(this.textNote);
			this.groupBox1.Controls.Add(this.textDatePaycheck);
			this.groupBox1.Controls.Add(this.textDateStop);
			this.groupBox1.Controls.Add(this.textDateStart);
			this.groupBox1.Controls.Add(this.butRight);
			this.groupBox1.Controls.Add(this.butLeft);
			this.groupBox1.Controls.Add(this.label4);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Location = new System.Drawing.Point(18, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(611, 71);
			this.groupBox1.TabIndex = 14;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pay Period";
			// 
			// labelNote
			// 
			this.labelNote.Location = new System.Drawing.Point(284, 28);
			this.labelNote.Name = "labelNote";
			this.labelNote.Size = new System.Drawing.Size(46, 18);
			this.labelNote.TabIndex = 16;
			this.labelNote.Text = "Note";
			this.labelNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(331, 28);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(274, 40);
			this.textNote.TabIndex = 15;
			// 
			// textDatePaycheck
			// 
			this.textDatePaycheck.Location = new System.Drawing.Point(505, 8);
			this.textDatePaycheck.Name = "textDatePaycheck";
			this.textDatePaycheck.ReadOnly = true;
			this.textDatePaycheck.Size = new System.Drawing.Size(100, 20);
			this.textDatePaycheck.TabIndex = 14;
			// 
			// textDateStop
			// 
			this.textDateStop.Location = new System.Drawing.Point(181, 28);
			this.textDateStop.Name = "textDateStop";
			this.textDateStop.ReadOnly = true;
			this.textDateStop.Size = new System.Drawing.Size(100, 20);
			this.textDateStop.TabIndex = 13;
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(181, 8);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.ReadOnly = true;
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 12;
			// 
			// butRight
			// 
			this.butRight.Image = global::OpenDental.Properties.Resources.Right;
			this.butRight.Location = new System.Drawing.Point(63, 18);
			this.butRight.Name = "butRight";
			this.butRight.Size = new System.Drawing.Size(39, 24);
			this.butRight.TabIndex = 11;
			this.butRight.Click += new System.EventHandler(this.butRight_Click);
			// 
			// butLeft
			// 
			this.butLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.butLeft.Location = new System.Drawing.Point(13, 18);
			this.butLeft.Name = "butLeft";
			this.butLeft.Size = new System.Drawing.Size(39, 24);
			this.butLeft.TabIndex = 10;
			this.butLeft.Click += new System.EventHandler(this.butLeft_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(387, 8);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(117, 18);
			this.label4.TabIndex = 9;
			this.label4.Text = "Paycheck Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelOvertime
			// 
			this.labelOvertime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelOvertime.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelOvertime.Location = new System.Drawing.Point(385, 644);
			this.labelOvertime.Name = "labelOvertime";
			this.labelOvertime.Size = new System.Drawing.Size(100, 17);
			this.labelOvertime.TabIndex = 17;
			this.labelOvertime.Text = "Overtime";
			this.labelOvertime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textOvertime
			// 
			this.textOvertime.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textOvertime.Location = new System.Drawing.Point(491, 643);
			this.textOvertime.Name = "textOvertime";
			this.textOvertime.Size = new System.Drawing.Size(66, 20);
			this.textOvertime.TabIndex = 16;
			this.textOvertime.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.radioBreaks);
			this.groupBox2.Controls.Add(this.radioTimeCard);
			this.groupBox2.Location = new System.Drawing.Point(747, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(122, 71);
			this.groupBox2.TabIndex = 20;
			this.groupBox2.TabStop = false;
			// 
			// radioBreaks
			// 
			this.radioBreaks.Location = new System.Drawing.Point(14, 27);
			this.radioBreaks.Name = "radioBreaks";
			this.radioBreaks.Size = new System.Drawing.Size(97, 19);
			this.radioBreaks.TabIndex = 1;
			this.radioBreaks.Text = "Breaks";
			this.radioBreaks.UseVisualStyleBackColor = true;
			this.radioBreaks.Click += new System.EventHandler(this.radioBreaks_Click);
			// 
			// radioTimeCard
			// 
			this.radioTimeCard.Checked = true;
			this.radioTimeCard.Location = new System.Drawing.Point(14, 10);
			this.radioTimeCard.Name = "radioTimeCard";
			this.radioTimeCard.Size = new System.Drawing.Size(97, 19);
			this.radioTimeCard.TabIndex = 0;
			this.radioTimeCard.TabStop = true;
			this.radioTimeCard.Text = "Time Card";
			this.radioTimeCard.UseVisualStyleBackColor = true;
			this.radioTimeCard.Click += new System.EventHandler(this.radioTimeCard_Click);
			// 
			// textOvertime2
			// 
			this.textOvertime2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textOvertime2.Location = new System.Drawing.Point(563, 643);
			this.textOvertime2.Name = "textOvertime2";
			this.textOvertime2.Size = new System.Drawing.Size(66, 20);
			this.textOvertime2.TabIndex = 22;
			this.textOvertime2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textTotal2
			// 
			this.textTotal2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textTotal2.Location = new System.Drawing.Point(563, 623);
			this.textTotal2.Name = "textTotal2";
			this.textTotal2.Size = new System.Drawing.Size(66, 20);
			this.textTotal2.TabIndex = 21;
			this.textTotal2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(18, 80);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(851, 540);
			this.gridMain.TabIndex = 13;
			this.gridMain.Title = "Time Card";
			this.gridMain.TranslationName = "TableTimeCard";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// textRateTwo2
			// 
			this.textRateTwo2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textRateTwo2.Location = new System.Drawing.Point(563, 663);
			this.textRateTwo2.Name = "textRateTwo2";
			this.textRateTwo2.Size = new System.Drawing.Size(66, 20);
			this.textRateTwo2.TabIndex = 26;
			this.textRateTwo2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelRateTwo
			// 
			this.labelRateTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelRateTwo.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelRateTwo.Location = new System.Drawing.Point(385, 664);
			this.labelRateTwo.Name = "labelRateTwo";
			this.labelRateTwo.Size = new System.Drawing.Size(100, 17);
			this.labelRateTwo.TabIndex = 25;
			this.labelRateTwo.Text = "Rate2";
			this.labelRateTwo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textRateTwo
			// 
			this.textRateTwo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textRateTwo.Location = new System.Drawing.Point(491, 663);
			this.textRateTwo.Name = "textRateTwo";
			this.textRateTwo.Size = new System.Drawing.Size(66, 20);
			this.textRateTwo.TabIndex = 24;
			this.textRateTwo.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// butCalcDaily
			// 
			this.butCalcDaily.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCalcDaily.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCalcDaily.Location = new System.Drawing.Point(139, 681);
			this.butCalcDaily.Name = "butCalcDaily";
			this.butCalcDaily.Size = new System.Drawing.Size(78, 24);
			this.butCalcDaily.TabIndex = 23;
			this.butCalcDaily.Text = "Calc Daily";
			this.butCalcDaily.Click += new System.EventHandler(this.butCalcDaily_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(691, 681);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(86, 24);
			this.butPrint.TabIndex = 19;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// butCalcWeekOT
			// 
			this.butCalcWeekOT.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butCalcWeekOT.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCalcWeekOT.Location = new System.Drawing.Point(223, 681);
			this.butCalcWeekOT.Name = "butCalcWeekOT";
			this.butCalcWeekOT.Size = new System.Drawing.Size(90, 24);
			this.butCalcWeekOT.TabIndex = 18;
			this.butCalcWeekOT.Text = "Calc Week OT";
			this.butCalcWeekOT.Click += new System.EventHandler(this.butCalcWeekOT_Click);
			// 
			// butAdj
			// 
			this.butAdj.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butAdj.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdj.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdj.Location = new System.Drawing.Point(18, 681);
			this.butAdj.Name = "butAdj";
			this.butAdj.Size = new System.Drawing.Size(115, 24);
			this.butAdj.TabIndex = 15;
			this.butAdj.Text = "Add Adjustment";
			this.butAdj.Click += new System.EventHandler(this.butAdj_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butClose.Location = new System.Drawing.Point(794, 681);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 0;
			this.butClose.Text = "&Close";
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// groupEmployee
			// 
			this.groupEmployee.Controls.Add(this.butPrevEmp);
			this.groupEmployee.Controls.Add(this.butNextEmp);
			this.groupEmployee.Location = new System.Drawing.Point(635, 3);
			this.groupEmployee.Name = "groupEmployee";
			this.groupEmployee.Size = new System.Drawing.Size(106, 71);
			this.groupEmployee.TabIndex = 121;
			this.groupEmployee.TabStop = false;
			this.groupEmployee.Text = "Employee";
			this.groupEmployee.Visible = false;
			// 
			// butPrevEmp
			// 
			this.butPrevEmp.Image = global::OpenDental.Properties.Resources.Left;
			this.butPrevEmp.Location = new System.Drawing.Point(8, 18);
			this.butPrevEmp.Name = "butPrevEmp";
			this.butPrevEmp.Size = new System.Drawing.Size(39, 24);
			this.butPrevEmp.TabIndex = 127;
			this.butPrevEmp.Click += new System.EventHandler(this.butBrowseEmp_Click);
			// 
			// butNextEmp
			// 
			this.butNextEmp.Image = global::OpenDental.Properties.Resources.Right;
			this.butNextEmp.Location = new System.Drawing.Point(59, 18);
			this.butNextEmp.Name = "butNextEmp";
			this.butNextEmp.Size = new System.Drawing.Size(39, 24);
			this.butNextEmp.TabIndex = 128;
			this.butNextEmp.Click += new System.EventHandler(this.butBrowseEmp_Click);
			// 
			// labelPTO
			// 
			this.labelPTO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelPTO.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelPTO.Location = new System.Drawing.Point(385, 684);
			this.labelPTO.Name = "labelPTO";
			this.labelPTO.Size = new System.Drawing.Size(100, 17);
			this.labelPTO.TabIndex = 122;
			this.labelPTO.Text = "PTO";
			this.labelPTO.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPTO
			// 
			this.textPTO.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textPTO.Location = new System.Drawing.Point(491, 683);
			this.textPTO.Name = "textPTO";
			this.textPTO.Size = new System.Drawing.Size(66, 20);
			this.textPTO.TabIndex = 123;
			this.textPTO.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textPTO2
			// 
			this.textPTO2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textPTO2.Location = new System.Drawing.Point(563, 683);
			this.textPTO2.Name = "textPTO2";
			this.textPTO2.Size = new System.Drawing.Size(66, 20);
			this.textPTO2.TabIndex = 124;
			this.textPTO2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textUnpaidProtectedLeave2
			// 
			this.textUnpaidProtectedLeave2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textUnpaidProtectedLeave2.Location = new System.Drawing.Point(563, 703);
			this.textUnpaidProtectedLeave2.Name = "textProtectedLeave2";
			this.textUnpaidProtectedLeave2.Size = new System.Drawing.Size(66, 20);
			this.textUnpaidProtectedLeave2.TabIndex = 127;
			this.textUnpaidProtectedLeave2.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// textUnpaidProtectedLeave
			// 
			this.textUnpaidProtectedLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textUnpaidProtectedLeave.Location = new System.Drawing.Point(491, 703);
			this.textUnpaidProtectedLeave.Name = "textProtectedLeave";
			this.textUnpaidProtectedLeave.Size = new System.Drawing.Size(66, 20);
			this.textUnpaidProtectedLeave.TabIndex = 126;
			this.textUnpaidProtectedLeave.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
			// 
			// labelUnpaidProtectedLeave
			// 
			this.labelUnpaidProtectedLeave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelUnpaidProtectedLeave.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelUnpaidProtectedLeave.Location = new System.Drawing.Point(334, 704);
			this.labelUnpaidProtectedLeave.Name = "labelProtectedLeave";
			this.labelUnpaidProtectedLeave.Size = new System.Drawing.Size(151, 17);
			this.labelUnpaidProtectedLeave.TabIndex = 125;
			this.labelUnpaidProtectedLeave.Text = "Protected Leave";
			this.labelUnpaidProtectedLeave.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormTimeCard
			// 
			this.CancelButton = this.butClose;
			this.ClientSize = new System.Drawing.Size(891, 723);
			this.Controls.Add(this.textUnpaidProtectedLeave2);
			this.Controls.Add(this.textUnpaidProtectedLeave);
			this.Controls.Add(this.labelUnpaidProtectedLeave);
			this.Controls.Add(this.textPTO2);
			this.Controls.Add(this.textPTO);
			this.Controls.Add(this.labelPTO);
			this.Controls.Add(this.groupEmployee);
			this.Controls.Add(this.textRateTwo2);
			this.Controls.Add(this.labelRateTwo);
			this.Controls.Add(this.textRateTwo);
			this.Controls.Add(this.butCalcDaily);
			this.Controls.Add(this.textOvertime2);
			this.Controls.Add(this.textTotal2);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.butCalcWeekOT);
			this.Controls.Add(this.labelOvertime);
			this.Controls.Add(this.textOvertime);
			this.Controls.Add(this.butAdj);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.labelRegularTime);
			this.Controls.Add(this.textTotal);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormTimeCard";
			this.ShowInTaskbar = false;
			this.Text = "Time Card";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FormTimeCard_FormClosing);
			this.Load += new System.EventHandler(this.FormTimeCard_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupEmployee.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}
		#endregion

		private void FormTimeCard_Load(object sender, System.EventArgs e){
			Initialize(DateTime.Today);
			SortEmployeeList();
			if(Security.IsAuthorized(Permissions.TimecardsEditAll,true)) {
				groupEmployee.Visible=true;
			}
		}

		public void SortEmployeeList() {
			if(IsByLastName) {
				_listEmp.Sort(Employees.SortByLastName);
			}
			else {
				_listEmp.Sort(Employees.SortByFirstName);
			}
		}

		/// <summary>Returns title of FormTimeCard based on employee name and any restrictions that may apply to editing.</summary>
		private string GetTitle() {
			string textString=Lan.g(this,"Time Card for")+" "+EmployeeCur.FName+" "+EmployeeCur.LName;
			if(_cannotEditOwnTimecard) {
				textString+=Lan.g(this," - You cannot modify your time card");
			}
			else if(_cannotEditSelectedPayPeriod) {
				int currentPayPeriod=PayPeriods.GetForDate(DateTime.Today);
				DateTime startDate=currentPayPeriod>-1 ? _listPayPeriods[currentPayPeriod].DateStart : DateTime.Today;
				textString+=Lan.g(this," - You can only modify your time card for the pay period starting on ")+startDate.ToShortDateString()+Lan.g(this,".");
			}
			return textString;
		}

		public void Initialize(DateTime dateInitial){
			TimeDelta=MiscData.GetNowDateTime()-DateTime.Now;
			if(SelectedPayPeriod==0) {
				SelectedPayPeriod=PayPeriods.GetForDate(dateInitial);
			}
			if(!PrefC.GetBool(PrefName.ClockEventAllowBreak)) {//Breaks turned off, Lunch is now "Break", but maintains Lunch functionality.
				IsBreaks=false;
				groupBox2.Visible=false;
			}
			if(IsBreaks){
				textOvertime.Visible=false;
				labelOvertime.Visible=false;
				butCalcWeekOT.Visible=false;//butCompute.Visible=false;
				butAdj.Visible=false;
				labelRateTwo.Visible=false;
				textRateTwo.Visible=false;
				textRateTwo2.Visible=false;
			}
			radioTimeCard.Checked=!IsBreaks;
			radioBreaks.Checked=IsBreaks;
			_listPayPeriods=PayPeriods.GetDeepCopy();
			FillUi();
		}

		private void butLeft_Click(object sender,EventArgs e) {
			if(SelectedPayPeriod==0){
				return;
			}
			SaveNoteToDb();
			SelectedPayPeriod--;
			FillUi();
		}

		private void butRight_Click(object sender,EventArgs e) {
			if(SelectedPayPeriod==_listPayPeriods.Count-1) {
				return;
			}
			SaveNoteToDb();
			SelectedPayPeriod++;
			FillUi();
		}

		private void FillUi() {
			//Check to see if the employee currently logged in can edit this time-card.
			Text=GetTitle();
			FillPayPeriod();
			FillMain(true);
		}

		///<summary>SelectedPayPeriod should already be set.  This simply fills the screen with that data.</summary>
		private void FillPayPeriod(){
			textDateStart.Text=_listPayPeriods[SelectedPayPeriod].DateStart.ToShortDateString();
			textDateStop.Text=_listPayPeriods[SelectedPayPeriod].DateStop.ToShortDateString();
			if(_listPayPeriods[SelectedPayPeriod].DatePaycheck.Year<1880){
				textDatePaycheck.Text="";
			}
			else{
				textDatePaycheck.Text=_listPayPeriods[SelectedPayPeriod].DatePaycheck.ToShortDateString();
			}
			//fill the note for the pay period
			_timeAdjustNote=GetOrCreatePayPeriodNote();
			textNote.Text=_timeAdjustNote.Note;
		}

		///<summary>Gets the pay period note from the timeadjust row on midnight of the first day in the pay period from the database,
		///otherwise creates a new TimeAdjust object in memory to be inserted later.</summary>
		private TimeAdjust GetOrCreatePayPeriodNote() {
			DateTime date=_listPayPeriods[SelectedPayPeriod].DateStart.Date;
			DateTime midnightFirstDay=new DateTime(date.Year,date.Month,date.Day,0,0,0);
			TimeAdjust noteRow=TimeAdjusts.GetPayPeriodNote(EmployeeCur.EmployeeNum,midnightFirstDay);
			if(noteRow==null) {
				noteRow=new TimeAdjust {
					EmployeeNum=EmployeeCur.EmployeeNum,
					TimeEntry=midnightFirstDay,
					Note="",
					IsAuto=false
				};
			}
			return noteRow;
		}

		private void radioTimeCard_Click(object sender,EventArgs e) {
			IsBreaks=false;
			textOvertime.Visible=true;
			labelOvertime.Visible=true;
			butCalcDaily.Visible=true;//butDaily.Visible=true;
			butCalcWeekOT.Visible=true;//butCompute.Visible=true;
			butAdj.Visible=true;
			labelRateTwo.Visible=true;
			textRateTwo.Visible=true;
			textRateTwo2.Visible=true;
			labelPTO.Visible=true;
			textPTO.Visible=true;
			textPTO2.Visible=true;
			FillMain(true);
		}

		private void radioBreaks_Click(object sender,EventArgs e) {
			IsBreaks=true;
			textOvertime.Visible=false;
			labelOvertime.Visible=false;
			butCalcDaily.Visible=false;//butDaily.Visible=false;
			butCalcWeekOT.Visible=false;//butCompute.Visible=false;
			butAdj.Visible=false;
			labelRateTwo.Visible=false;
			textRateTwo.Visible=false;
			textRateTwo2.Visible=false;
			labelPTO.Visible=false;
			textPTO.Visible=false;
			textPTO2.Visible=false;
			//butDaily.Visible=false;
			FillMain(true);
		}

		private DateTime GetDateForRow(int i){
			if(mergedAL[i].GetType()==typeof(ClockEvent)){
				return ((ClockEvent)mergedAL[i]).TimeDisplayed1.Date;
			}
			else if(mergedAL[i].GetType()==typeof(TimeAdjust)){
				return ((TimeAdjust)mergedAL[i]).TimeEntry.Date;
			}
			return DateTime.MinValue;
		}

		///<summary>fromDB is set to false when it is refreshing every second so that there will be no extra network traffic.</summary>
		private void FillMain(bool fromDB){
			if(fromDB){
				ClockEventList=ClockEvents.Refresh(EmployeeCur.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text),IsBreaks);
				if(IsBreaks){
					TimeAdjustList=new List<TimeAdjust>();
				}
				else{
					TimeAdjustList=TimeAdjusts.Refresh(EmployeeCur.EmployeeNum,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
				}
			}
			TimeAdjustList.RemoveAll(x => x.TimeAdjustNum==_timeAdjustNote.TimeAdjustNum);//Do not show the note row in the grid.
			mergedAL=new ArrayList();
			for(int i=0;i<ClockEventList.Count;i++) {
				mergedAL.Add(ClockEventList[i]);
			}
			for(int i=0;i<TimeAdjustList.Count;i++) {
				mergedAL.Add(TimeAdjustList[i]);
			}
			IComparer myComparer=new ObjectDateComparer();
			mergedAL.Sort(myComparer);
			gridMain.BeginUpdate();
			gridMain.ListGridColumns.Clear();
			GridColumn col=new GridColumn(Lan.g(this,"Date"),70);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),45);
			gridMain.ListGridColumns.Add(col);
			//col=new ODGridColumn(Lan.g(this,"Altered"),50,HorizontalAlignment.Center);//use red now instead of separate col
			//gridMain.Columns.Add(col);
			if(IsBreaks){
				col=new GridColumn(Lan.g(this,"Out"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"In"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
			}
			else{
				col=new GridColumn(Lan.g(this,"In"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
				col=new GridColumn(Lan.g(this,"Out"),64,HorizontalAlignment.Right);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Total"),50,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Adjust"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Rate2"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PTO"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"OT"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"PL"),45,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Day"),50,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			col=new GridColumn(Lan.g(this,"Week"),50,HorizontalAlignment.Right);
			gridMain.ListGridColumns.Add(col);
			if(PrefC.HasClinicsEnabled) {
				col=new GridColumn(Lan.g(this,"Clinic"),100);
				gridMain.ListGridColumns.Add(col);
			}
			col=new GridColumn(Lan.g(this,"Note"),100){ IsWidthDynamic=true };
			gridMain.ListGridColumns.Add(col);
			gridMain.ListGridRows.Clear();
			GridRow row;
			WeeklyTotals=new TimeSpan[mergedAL.Count];
			TimeSpan alteredSpan=new TimeSpan(0);//used to display altered times
			TimeSpan oneSpan=new TimeSpan(0);//used to sum one pair of clock-in/clock-out
			TimeSpan oneAdj;
			TimeSpan oneOT;
			TimeSpan daySpan=new TimeSpan(0);//used for daily totals.
			TimeSpan weekSpan=new TimeSpan(0);//used for weekly totals.
			TimeSpan ptoSpan=new TimeSpan(0);//used for PTO totals.
			TimeSpan unpaidProtectedLeaveSpan=new TimeSpan(0);//used for Unpaid Protected Leave totals.
			if(mergedAL.Count>0) {  //Have to check fromDB here because we dont want to call DB every timer tick
				if(fromDB) {
					weekSpan=ClockEvents.GetWeekTotal(EmployeeCur.EmployeeNum,GetDateForRow(0));
					storedWeekSpan=weekSpan;
				}
				else {
					weekSpan=storedWeekSpan;
				}
			}
			TimeSpan periodSpan=new TimeSpan(0);//used to add up totals for entire page.
			TimeSpan otspan=new TimeSpan(0);//overtime for the entire period
			TimeSpan rate2span=new TimeSpan(0);//rate2 hours total
      Calendar cal=CultureInfo.CurrentCulture.Calendar;
			CalendarWeekRule rule=CalendarWeekRule.FirstFullWeek;//CultureInfo.CurrentCulture.DateTimeFormat.CalendarWeekRule;
			DateTime curDate=DateTime.MinValue;
			DateTime previousDate=DateTime.MinValue;
			Type type;
			ClockEvent clock;
			TimeAdjust adjust;
			for(int i=0;i<mergedAL.Count;i++){
				row=new GridRow();
				type=mergedAL[i].GetType();
				row.Tag=mergedAL[i];
				previousDate=curDate;
				//clock event row---------------------------------------------------------------------------------------------
				if(type==typeof(ClockEvent)){
					clock=(ClockEvent)mergedAL[i];
					curDate=clock.TimeDisplayed1.Date;
					if(curDate==previousDate){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(curDate.ToShortDateString());
						row.Cells.Add(curDate.ToString("ddd"));//Abbreviated name of day
					}
					//in------------------------------------------
					if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {
						row.Cells.Add(clock.TimeDisplayed1.ToLongTimeString());
					}
					else {
						row.Cells.Add(clock.TimeDisplayed1.ToShortTimeString());
					}
					if (clock.TimeEntered1!=clock.TimeDisplayed1){
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//out-----------------------------
					if(clock.TimeDisplayed2.Year<1880){
						row.Cells.Add("");//not clocked out yet
					}
					else{
						if(PrefC.GetBool(PrefName.TimeCardShowSeconds)) {
							row.Cells.Add(clock.TimeDisplayed2.ToLongTimeString());
						}
						else {
							row.Cells.Add(clock.TimeDisplayed2.ToShortTimeString());
						}
						if (clock.TimeEntered2!=clock.TimeDisplayed2){
							row.Cells[row.Cells.Count-1].ColorText = Color.Red;
						}
					}
					//total-------------------------------
					if(IsBreaks){ //breaks
						if(clock.TimeDisplayed2.Year<1880){
							row.Cells.Add("");
						}
						else{
							oneSpan=clock.TimeDisplayed2-clock.TimeDisplayed1;
							row.Cells.Add(ClockEvents.Format(oneSpan));
							daySpan+=oneSpan;
							periodSpan+=oneSpan;
						}
					}
					else{//regular hours
						if(clock.TimeDisplayed2.Year<1880){
							row.Cells.Add("");
						}
						else{
							oneSpan=clock.TimeDisplayed2-clock.TimeDisplayed1;
							row.Cells.Add(ClockEvents.Format(oneSpan));
							daySpan+=oneSpan;
							weekSpan+=oneSpan;
							periodSpan+=oneSpan;
						}
					}
					//Adjust---------------------------------
					oneAdj=TimeSpan.Zero;
					if(clock.AdjustIsOverridden) {
						oneAdj+=clock.Adjust;
					}
					else {
						oneAdj+=clock.AdjustAuto;//typically zero
					}
					daySpan+=oneAdj;
					weekSpan+=oneAdj;
					periodSpan+=oneAdj;
					row.Cells.Add(ClockEvents.Format(oneAdj));
					if(clock.AdjustIsOverridden) {
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//Rate2---------------------------------
					if(clock.Rate2Hours!=TimeSpan.FromHours(-1)) {
						rate2span+=clock.Rate2Hours;
						row.Cells.Add(ClockEvents.Format(clock.Rate2Hours));
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					else {
						rate2span+=clock.Rate2Auto;
						row.Cells.Add(ClockEvents.Format(clock.Rate2Auto));
					}
					//PTO------------------------------
					row.Cells.Add("");//No PTO should exist, leave blank
					//Overtime------------------------------
					oneOT=TimeSpan.Zero;
					if(clock.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						oneOT=clock.OTimeHours;
					}
					else {
						oneOT=clock.OTimeAuto;//typically zero
					}
					otspan+=oneOT;
					daySpan-=oneOT;
					weekSpan-=oneOT;
					periodSpan-=oneOT;
					row.Cells.Add(ClockEvents.Format(oneOT));
					if(clock.OTimeHours!=TimeSpan.FromHours(-1)) {//overridden
						row.Cells[row.Cells.Count-1].ColorText = Color.Red;
					}
					//Unpaid Protected Leave (PL)-------------------------------------------------
					row.Cells.Add("");//No PL should exist, leave blank
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==mergedAL.Count-1//if this is the last row
						|| GetDateForRow(i+1) != curDate)//or the next row is a different date
					{
						if(IsBreaks){
							if(clock.TimeDisplayed2.Year<1880){//if they have not clocked back in yet from break
								//display the timespan of oneSpan using current time as the other number.
								oneSpan=DateTime.Now-clock.TimeDisplayed1+TimeDelta;
								row.Cells.Add(oneSpan.ToStringHmmss());
								daySpan+=oneSpan;
							}
							else{
								row.Cells.Add(ClockEvents.Format(daySpan));
							}
						}
						else{
							row.Cells.Add(ClockEvents.Format(daySpan));
						}
						daySpan=new TimeSpan(0);
					}
					else{//not the last entry for the day
						row.Cells.Add("");
					}
					//Weekly-------------------------------------
					WeeklyTotals[i]=weekSpan;
					if(IsBreaks){
						row.Cells.Add("");
					}
					//if this is the last entry for a given week
					else if(i==mergedAL.Count-1//if this is the last row 
						|| cal.GetWeekOfYear(GetDateForRow(i+1),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= cal.GetWeekOfYear(clock.TimeDisplayed1.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						row.Cells.Add(ClockEvents.Format(weekSpan));
						weekSpan=new TimeSpan(0);
					}
					else {
						//row.Cells.Add(ClockEvents.Format(weekSpan));
						row.Cells.Add("");
					}
					//Clinic-----------------------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(clock.ClinicNum));
					}
					//Note-----------------------------------------
					row.Cells.Add(clock.Note);
				}
				//adjustment row--------------------------------------------------------------------------------------
				else if(type==typeof(TimeAdjust)){
					adjust=(TimeAdjust)mergedAL[i];
					curDate=adjust.TimeEntry.Date;
					if(curDate==previousDate){
						row.Cells.Add("");
						row.Cells.Add("");
					}
					else{
						row.Cells.Add(curDate.ToShortDateString());
						row.Cells.Add(curDate.ToString("ddd"));//Abbreviated name of day
					}
					//altered--------------------------------------
					//row.Cells.Add(Lan.g(this,"Adjust"));//2
					//row.ColorText=Color.Red;
					//status--------------------------------------
					//row.Cells.Add("");//3
					//in/out------------------------------------------
					row.Cells.Add("");//4
					//time-----------------------------
					if(adjust.PtoDefNum==0) {
						row.Cells.Add("(Adjust)");//5 Out column
					}
					else { 
						row.Cells.Add(Defs.GetDef(DefCat.TimeCardAdjTypes,adjust.PtoDefNum).ItemName);//5
					}
					row.Cells[row.Cells.Count-1].ColorText=Color.Red;
					//total-------------------------------
					row.Cells.Add("");//6
					//Adjust------------------------------
					if(adjust.IsUnpaidProtectedLeave) {
						row.Cells.Add("");//7
					}
					else if(adjust.PtoDefNum==0) {
						daySpan+=adjust.RegHours;//might be negative
						weekSpan+=adjust.RegHours;
						periodSpan+=adjust.RegHours;
						row.Cells.Add(ClockEvents.Format(adjust.RegHours));//7
					} 
					else {
						ptoSpan+=adjust.PtoHours;
						row.Cells.Add("");//7
					}
					//Rate2-------------------------------
					row.Cells.Add("");//8
					//PTO------------------------------
					row.Cells.Add(ClockEvents.Format(adjust.PtoHours));//9
					//Overtime------------------------------
					otspan+=adjust.OTimeHours;
					row.Cells.Add(ClockEvents.Format(adjust.OTimeHours));//10
					//Unpaid Protected Leave (PL)------------------------------------
					if(adjust.IsUnpaidProtectedLeave) {
						row.Cells.Add(ClockEvents.Format(adjust.RegHours));
						unpaidProtectedLeaveSpan+=adjust.RegHours;
					}
					else {
						row.Cells.Add("");
					}
					//Daily-----------------------------------
					//if this is the last entry for a given date
					if(i==mergedAL.Count-1//if this is the last row
						|| GetDateForRow(i+1) != curDate)//or the next row is a different date
					{
						row.Cells.Add(ClockEvents.Format(daySpan));//
						daySpan=new TimeSpan(0);
					}
					else{
						row.Cells.Add("");
					}
					//Weekly-------------------------------------
					WeeklyTotals[i]=weekSpan;
					if(IsBreaks){
						row.Cells.Add("");
					}
					//if this is the last entry for a given week
					else if(i==mergedAL.Count-1//if this is the last row 
						|| cal.GetWeekOfYear(GetDateForRow(i+1),rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek))//or the next row has a
						!= cal.GetWeekOfYear(adjust.TimeEntry.Date,rule,(DayOfWeek)PrefC.GetInt(PrefName.TimeCardOvertimeFirstDayOfWeek)))//different week of year
					{
						GridCell cell=new GridCell(ClockEvents.Format(weekSpan));
						cell.ColorText=Color.Black;
						row.Cells.Add(cell);
						weekSpan=new TimeSpan(0);
					}
					else {
						row.Cells.Add("");
					}
					//Clinic-----------------------------------------
					if(PrefC.HasClinicsEnabled) {
						row.Cells.Add(Clinics.GetAbbr(adjust.ClinicNum));
					}
					//Note-----------------------------------------
					row.Cells.Add(adjust.Note);
				}
				gridMain.ListGridRows.Add(row);
			}
			gridMain.EndUpdate();
			if(IsBreaks){
				labelRegularTime.Visible=false;
				labelOvertime.Visible=false;
				labelRateTwo.Visible=false;
				labelPTO.Visible=false;
				textTotal.Visible=false;
				textTotal2.Visible=false;
				textOvertime.Visible=false;
				textOvertime2.Visible=false;
				textRateTwo.Visible=false;
				textRateTwo2.Visible=false;
				textPTO.Visible=false;
				textPTO2.Visible=false;
				labelUnpaidProtectedLeave.Visible=false;
				textUnpaidProtectedLeave.Visible=false;
				textUnpaidProtectedLeave2.Visible=false;
			}
			else {
				labelRegularTime.Visible=true;
				labelOvertime.Visible=true;
				labelRateTwo.Visible=true;
				textTotal.Visible=true;
				textTotal2.Visible=true;
				textOvertime.Visible=true;
				textOvertime2.Visible=true;
				textRateTwo.Visible=true;
				textRateTwo2.Visible=true;
				labelUnpaidProtectedLeave.Visible=true;
				textUnpaidProtectedLeave.Visible=true;
				textUnpaidProtectedLeave2.Visible=true;
				textTotal.Text=periodSpan.ToStringHmm();
				textOvertime.Text=otspan.ToStringHmm();
				textRateTwo.Text=rate2span.ToStringHmm();
				textPTO.Text=ptoSpan.ToStringHmm();
				textUnpaidProtectedLeave.Text=unpaidProtectedLeaveSpan.ToStringHmm();
				textTotal2.Text=periodSpan.TotalHours.ToString("n");
				textOvertime2.Text=otspan.TotalHours.ToString("n");
				textRateTwo2.Text=rate2span.TotalHours.ToString("n");
				textPTO2.Text=ptoSpan.TotalHours.ToString("n");
				textUnpaidProtectedLeave2.Text=unpaidProtectedLeaveSpan.TotalHours.ToString("n");
			}
		}

		private void gridMain_CellDoubleClick(object sender,ODGridClickEventArgs e) {
			if(_cannotEditOwnTimecard) {
				MsgBox.Show("You do not have permission to modify your time card.");
				return;
			}
			if(_cannotEditSelectedPayPeriod) {
				MsgBox.Show("You do not have permission to modify your past pay periods.");
				return;
			}
			if(Security.CurUser.EmployeeNum!=EmployeeCur.EmployeeNum && !Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				return;
			}
			timerUpdateBreak.Enabled=false;
			if(gridMain.ListGridRows[e.Row].Tag.GetType()==typeof(TimeAdjust)) {
				TimeAdjust adjust=(TimeAdjust)gridMain.ListGridRows[e.Row].Tag;
				//Only users with the ProtectedLeaveAdjustmentEdit permission can edit other user's UPL Time Card Adjustments.
				if(adjust.IsUnpaidProtectedLeave 
					&& Security.CurUser.EmployeeNum!=EmployeeCur.EmployeeNum
					&& !Security.IsAuthorized(Permissions.ProtectedLeaveAdjustmentEdit))
				{
					timerUpdateBreak.Enabled=true;
					return;
				}
				//HQ users without the TimecardsEditAll permission can only edit PTO and UPL Time Card Adjustments.
				if(PrefC.IsODHQ
					&& !adjust.IsUnpaidProtectedLeave
					&& adjust.PtoDefNum==0
					&& !Security.IsAuthorized(Permissions.TimecardsEditAll,suppressMessage:true))
				{
					MsgBox.Show(this,"HQ users without the Edit All Time Cards permission can only edit Time Card Adjustments for PTO or Protected Leave.");
					timerUpdateBreak.Enabled=true;
					return;
				}
				using FormTimeAdjustEdit FormT=new FormTimeAdjustEdit(adjust);
				FormT.ShowDialog();
			}
			else {
				ClockEvent ce=(ClockEvent)gridMain.ListGridRows[e.Row].Tag;
				using FormClockEventEdit FormCEE=new FormClockEventEdit(ce);
				FormCEE.ShowDialog();
			}
			FillMain(true);
			timerUpdateBreak.Enabled=true;
		}

		private void butAdj_Click(object sender,EventArgs e) {
			if(Security.CurUser.EmployeeNum!=EmployeeCur.EmployeeNum && !Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				return;
			}
			if(_cannotEditOwnTimecard) {
				MsgBox.Show(this,"You do not have permission to modify your time card.");
				return;
			}
			TimeAdjust adjust=new TimeAdjust();
			adjust.EmployeeNum=EmployeeCur.EmployeeNum;
			if(PrefC.HasClinicsEnabled) {
				adjust.ClinicNum=Clinics.ClinicNum;
			}
			DateTime dateStop=PIn.Date(textDateStop.Text);
			if(DateTime.Today<=dateStop && DateTime.Today>=PIn.Date(textDateStart.Text)) {
				adjust.TimeEntry=DateTime.Now;
			}
			else {
				adjust.TimeEntry=new DateTime(dateStop.Year,dateStop.Month,dateStop.Day,
					DateTime.Now.Hour,DateTime.Now.Minute,DateTime.Now.Second);
			}
			using FormTimeAdjustEdit FormT=new FormTimeAdjustEdit(adjust);
			FormT.IsNew=true;
			FormT.ShowDialog();
			if(FormT.DialogResult==DialogResult.Cancel) {
				return;
			}
			FillMain(true);
		}

		private void butCalcWeekOT_Click(object sender,EventArgs e) {
			if(!Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				return;
			}
			TimeCardRule timeCardRule=TimeCardRules.GetTimeCardRule(EmployeeCur);
			if(timeCardRule!=null && timeCardRule.IsOvertimeExempt) {
				MsgBox.Show(this,"This employee is marked as exempt from receiving overtime hours.");
				return;
			}
			try {
				TimeCardRules.CalculateWeeklyOvertime(EmployeeCur,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			}
			FillMain(true);
		}

		private void butCalcDaily_Click(object sender,EventArgs e) {
			//not even visible if viewing breaks.
			//suppress security warning because the main point of this check is to see if users can edit their own time cards
			if(_cannotEditOwnTimecard) { 
				MsgBox.Show("You do not have permission to modify your time card.");
				return;
			}
			if(_cannotEditSelectedPayPeriod) {
				MsgBox.Show("You do not have permission to modify your past pay periods.");
			}
			else if(!PrefC.GetBool(PrefName.TimecardSecurityEnabled) && !Security.IsAuthorized(Permissions.TimecardsEditAll)) {
				//Security.IsAuthorized() shows the error to the user already.
				return;
			}
			string errors=TimeCardRules.ValidateOvertimeRules(new List<long>{EmployeeCur.EmployeeNum});
			if(errors != "") {
				MessageBox.Show(this,"Please fix the following timecard rule errors first:\r\n"+errors);
				return;
			}
			errors=TimeCardRules.ValidatePayPeriod(EmployeeCur,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			if(errors != "") {
				MessageBox.Show(this,errors);
				return;
			}
			try {
				TimeCardRules.CalculateDailyOvertime(EmployeeCur,PIn.Date(textDateStart.Text),PIn.Date(textDateStop.Text));
			}
			catch(Exception ex) {
				MessageBox.Show(this,ex.Message);
			}
			FillMain(true);
		}

		private void butPrint_Click(object sender,EventArgs e) {
			SaveNoteToDb();
			linesPrinted=0;
			PrinterL.TryPrintOrDebugClassicPreview(pd_PrintPage,
				Lan.g(this,"Time card for")+" "+EmployeeCur.LName+","+EmployeeCur.FName+" "+Lan.g(this,"printed"),
				new Margins(0,0,0,0),
				printoutOrigin:PrintoutOrigin.AtMargin
			);
		}

		///<summary>raised for each page to be printed.</summary>
		private void pd_PrintPage(object sender,PrintPageEventArgs e) {
			Graphics g=e.Graphics;
			float yPos=75;
			float xPos=55;
			string str;
			Font font=new Font(FontFamily.GenericSansSerif,8);
			Font fontTitle=new Font(FontFamily.GenericSansSerif,11,FontStyle.Bold);
			Font fontHeader=new Font(FontFamily.GenericSansSerif,8,FontStyle.Bold);
			SolidBrush brush=new SolidBrush(Color.Black);
			Pen pen=new Pen(Color.Black);
			//Title
			str=EmployeeCur.FName+" "+EmployeeCur.LName;
			str+="\r\n"+Lan.g(this,"Note")+": "+_timeAdjustNote.Note.ToString();
			int threeLineHeight=(int)e.Graphics.MeasureString("1\r\n2\r\n3",fontTitle).Height;
			int marginBothSides=(int)xPos*2;//110
			int noteStringHeight=(int)e.Graphics.MeasureString(str,fontTitle,e.PageBounds.Width-marginBothSides).Height;
			int rectHeight=Math.Min(noteStringHeight,threeLineHeight);
			StringFormat noteStringFormat=new StringFormat{ Trimming=StringTrimming.Word };
			g.DrawString(str,fontTitle,brush,new RectangleF(xPos,yPos,e.PageBounds.Width-marginBothSides,rectHeight),noteStringFormat);
			yPos+=rectHeight+5;//+5 pixels for a small space between columns and title area.
			//define columns
			int[] colW=new int[13];
			if(PrefC.HasClinicsEnabled) {
				colW=new int[14];
			}
			colW[0]=70;//Date
			colW[1]=45;//Day: Column starts to wrap at 32 pixels, however added padding to 45 to allow room for language translations
			colW[2]=60;//In/Out
			colW[3]=60;//Out/In
			colW[4]=50;//Total
			colW[5]=45;//Adjust: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[6]=45;//Rate 2: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[7]=45;//PTO: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[8]=45;//OT: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[9]=45;//PL: Column starts to wrap at 41 pixels (Ex. -10.00), buffered to 45 for font variations on different operating systems
			colW[10]=50;//Day
			colW[11]=50;//Week
			colW[12]=165;//Note
			if(PrefC.HasClinicsEnabled) {
				colW[12]=50;//Clinic
				colW[13]=115;//Note: Reduce width when Clinic column is added so that we do not exceed the margin.
			}
			int[] colPos=new int[colW.Length+1];
			colPos[0]=45;
			for(int i=1;i<colPos.Length;i++) {
				colPos[i]=colPos[i-1]+colW[i-1];
			}
			string[] ColCaption=new string[13];
			if(PrefC.HasClinicsEnabled) {
				ColCaption=new string[14];
			}
			ColCaption[0]=Lan.g(this,"Date");
			ColCaption[1]=Lan.g(this,"Day");
			if(radioBreaks.Checked) {
				ColCaption[2]=Lan.g(this,"Out");
				ColCaption[3]=Lan.g(this,"In");
			}
			else {
				ColCaption[2]=Lan.g(this,"In");
				ColCaption[3]=Lan.g(this,"Out");
			}
			ColCaption[4]=Lan.g(this,"Total");
			ColCaption[5]=Lan.g(this,"Adjust");
			ColCaption[6]=Lan.g(this,"Rate 2");
			ColCaption[7]=Lan.g(this,"PTO");
			ColCaption[8]=Lan.g(this,"OT");
			ColCaption[9]=Lan.g(this,"PL");
			ColCaption[10]=Lan.g(this,"Day");
			ColCaption[11]=Lan.g(this,"Week");
			ColCaption[12]=Lan.g(this,"Note");
			if(PrefC.HasClinicsEnabled) {
				ColCaption[12]=Lan.g(this,"Clinic");
				ColCaption[13]=Lan.g(this,"Note");
			}
			//column headers-----------------------------------------------------------------------------------------
			e.Graphics.FillRectangle(Brushes.LightGray,colPos[0],yPos,colPos[colPos.Length-1]-colPos[0],18);
			e.Graphics.DrawRectangle(pen,colPos[0],yPos,colPos[colPos.Length-1]-colPos[0],18);
			for(int i=1;i<colPos.Length;i++) {
				e.Graphics.DrawLine(new Pen(Color.Black),colPos[i],yPos,colPos[i],yPos+18);
			}
			//Prints the Column Titles
			for(int i=0;i<ColCaption.Length;i++) {
				e.Graphics.DrawString(ColCaption[i],fontHeader,brush,colPos[i]+2,yPos+1);
			}
			yPos+=18;
			while(yPos < e.PageBounds.Height-75-50-32-16 && linesPrinted < gridMain.ListGridRows.Count) {
				for(int i=0;i<colPos.Length-1;i++) {
					if(gridMain.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Empty || gridMain.ListGridRows[linesPrinted].Cells[i].ColorText==Color.Black) {
						e.Graphics.DrawString(gridMain.ListGridRows[linesPrinted].Cells[i].Text,font,brush
							,new RectangleF(colPos[i]+2,yPos,colPos[i+1]-colPos[i]-5,font.GetHeight(e.Graphics)));
					}
					else { //The only other color currently supported is red.
						e.Graphics.DrawString(gridMain.ListGridRows[linesPrinted].Cells[i].Text,font,Brushes.Red
							,new RectangleF(colPos[i]+2,yPos,colPos[i+1]-colPos[i]-5,font.GetHeight(e.Graphics)));
					}
				}
				//Column lines		
				for(int i=0;i<colPos.Length;i++) {
					e.Graphics.DrawLine(Pens.Gray,colPos[i],yPos+16,colPos[i],yPos);
				}
				linesPrinted++;
				yPos+=16;
				e.Graphics.DrawLine(new Pen(Color.Gray),colPos[0],yPos,colPos[colPos.Length-1],yPos);
			}
			//bottom line
			//e.Graphics.DrawLine(new Pen(Color.Gray),colPos[0],yPos,colPos[colPos.Length-1],yPos);
			//totals will print on every page for simplicity
			yPos+=10;
			g.DrawString(Lan.g(this,"Regular Time")+": "+textTotal.Text+" ("+textTotal2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Overtime")+": "+textOvertime.Text+" ("+textOvertime2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Rate 2 Time")+": "+textRateTwo.Text+" ("+textRateTwo2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"PTO Time")+": "+textPTO.Text+" ("+textPTO2.Text+")",fontHeader,brush,xPos,yPos);
			yPos+=16;
			g.DrawString(Lan.g(this,"Protected Leave")+": "+textUnpaidProtectedLeave.Text+" ("+textUnpaidProtectedLeave2.Text+")",fontHeader,brush,xPos,yPos);
			if(linesPrinted==gridMain.ListGridRows.Count) {
				e.HasMorePages=false;
			}
			else {
				e.HasMorePages=true;
			}
		}

		private void butBrowseEmp_Click(object sender,EventArgs e) {
			SaveNoteToDb();
			int empIndex=0;
			for(int i=0;i<_listEmp.Count;i++) {
				//find current employee index by Employeenum
				if(EmployeeCur.EmployeeNum==_listEmp[i].EmployeeNum) {
					if(sender.Equals(butPrevEmp)) {
						empIndex=i-1;//go to previous employee in list
					}
					else {
						empIndex=i+1;//go to next employee in list
					}
					empIndex=(empIndex+_listEmp.Count)%(_listEmp.Count);//allows wrapping at end of employee list.
					break;
				}
			}
			EmployeeCur=_listEmp[empIndex];
			FillUi();
		}

		private void SaveNoteToDb() {
			_timeAdjustNote.Note=textNote.Text;
			if(_timeAdjustNote.TimeAdjustNum==0) {//adding a note for the first time.
				if(textNote.Text!="") {//Do not create a row if not needed.
					TimeAdjusts.Insert(_timeAdjustNote);
				}
			}
			else {
				TimeAdjusts.Update(_timeAdjustNote);
			}
		}

		private void butClose_Click(object sender, System.EventArgs e) {
			SaveNoteToDb();
			Close();
		}

		///<summary>This timer updates break times so that you can see your break timer counting.</summary>
		private void timerUpdateBreak_Tick(object sender, System.EventArgs e) {
			if(IsBreaks) {
				int idx = gridMain.GetSelectedIndex();
				FillMain(false);//deselects current index.
				if(idx>-1 && idx<gridMain.ListGridRows.Count) {
					gridMain.SetSelected(idx,true);
				}
			}
		}

		private void FormTimeCard_FormClosing(object sender,FormClosingEventArgs e) {
			timerUpdateBreak.Enabled=false;  //This timer was never being disabled, so it would just keep ticking after the form was closed.
		}

		

		

	

		

		

		

		

		

		

		

		

		


	}
}





















