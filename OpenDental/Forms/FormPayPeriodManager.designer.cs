using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OpenDental {
	public partial class FormPayPeriodManager {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing ) {
			if( disposing )	{
				if(components != null) {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPeriodManager));
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.radioPayBefore = new System.Windows.Forms.RadioButton();
			this.radioPayAfter = new System.Windows.Forms.RadioButton();
			this.checkExcludeWeekends = new System.Windows.Forms.CheckBox();
			this.comboDay = new System.Windows.Forms.ComboBox();
			this.textDaysAfterPayPeriod = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBoxSemiMonthly = new System.Windows.Forms.GroupBox();
			this.checkLast = new System.Windows.Forms.CheckBox();
			this.labelPeriod2Day = new System.Windows.Forms.Label();
			this.textDay2 = new OpenDental.ValidNum();
			this.labelPeriod1Day = new System.Windows.Forms.Label();
			this.textDay1 = new OpenDental.ValidNum();
			this.radioPayDate = new System.Windows.Forms.RadioButton();
			this.radioEndDate = new System.Windows.Forms.RadioButton();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.radioSemiMonthly = new System.Windows.Forms.RadioButton();
			this.radioWeekly = new System.Windows.Forms.RadioButton();
			this.textPayPeriods = new OpenDental.ValidNum();
			this.radioMonthly = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioBiWeekly = new System.Windows.Forms.RadioButton();
			this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butGenerate = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBoxSemiMonthly.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.radioPayBefore);
			this.groupBox3.Controls.Add(this.radioPayAfter);
			this.groupBox3.Controls.Add(this.checkExcludeWeekends);
			this.groupBox3.Controls.Add(this.comboDay);
			this.groupBox3.Controls.Add(this.textDaysAfterPayPeriod);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(40, 256);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(250, 160);
			this.groupBox3.TabIndex = 7;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Pay Day";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(78, 51);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(23, 13);
			this.label3.TabIndex = 26;
			this.label3.Text = "OR";
			// 
			// radioPayBefore
			// 
			this.radioPayBefore.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayBefore.Location = new System.Drawing.Point(81, 120);
			this.radioPayBefore.Name = "radioPayBefore";
			this.radioPayBefore.Size = new System.Drawing.Size(129, 17);
			this.radioPayBefore.TabIndex = 24;
			this.radioPayBefore.TabStop = true;
			this.radioPayBefore.Text = "Pay Before Weekend";
			this.radioPayBefore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayBefore.UseVisualStyleBackColor = true;
			// 
			// radioPayAfter
			// 
			this.radioPayAfter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayAfter.Location = new System.Drawing.Point(90, 138);
			this.radioPayAfter.Name = "radioPayAfter";
			this.radioPayAfter.Size = new System.Drawing.Size(120, 17);
			this.radioPayAfter.TabIndex = 25;
			this.radioPayAfter.TabStop = true;
			this.radioPayAfter.Text = "Pay After Weekend";
			this.radioPayAfter.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayAfter.UseVisualStyleBackColor = true;
			// 
			// checkExcludeWeekends
			// 
			this.checkExcludeWeekends.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeWeekends.Location = new System.Drawing.Point(92, 100);
			this.checkExcludeWeekends.Name = "checkExcludeWeekends";
			this.checkExcludeWeekends.Size = new System.Drawing.Size(119, 17);
			this.checkExcludeWeekends.TabIndex = 23;
			this.checkExcludeWeekends.Text = "Exclude Weekends";
			this.checkExcludeWeekends.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkExcludeWeekends.UseVisualStyleBackColor = true;
			this.checkExcludeWeekends.CheckedChanged += new System.EventHandler(this.checkExcludeWeekends_CheckedChanged);
			// 
			// comboDay
			// 
			this.comboDay.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboDay.FormattingEnabled = true;
			this.comboDay.Items.AddRange(new object[] {
            "None",
            "Sunday",
            "Monday",
            "Tuesday",
            "Wednesday",
            "Thursday",
            "Friday",
            "Saturday"});
			this.comboDay.Location = new System.Drawing.Point(91, 21);
			this.comboDay.Name = "comboDay";
			this.comboDay.Size = new System.Drawing.Size(94, 21);
			this.comboDay.TabIndex = 22;
			this.comboDay.SelectionChangeCommitted += new System.EventHandler(this.comboDay_SelectionChangeCommitted);
			// 
			// textDaysAfterPayPeriod
			// 
			this.textDaysAfterPayPeriod.Location = new System.Drawing.Point(170, 73);
			this.textDaysAfterPayPeriod.MaxVal = 200;
			this.textDaysAfterPayPeriod.Name = "textDaysAfterPayPeriod";
			this.textDaysAfterPayPeriod.Size = new System.Drawing.Size(40, 20);
			this.textDaysAfterPayPeriod.TabIndex = 21;
			this.textDaysAfterPayPeriod.TextChanged += new System.EventHandler(this.numDaysAfterPayPeriod_TextChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(44, 74);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(120, 17);
			this.label4.TabIndex = 2;
			this.label4.Text = "# Days After Pay Period";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(4, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(81, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Day of Week";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(3, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 20);
			this.label1.TabIndex = 6;
			this.label1.Text = "Start Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.groupBoxSemiMonthly);
			this.groupBox2.Controls.Add(this.label7);
			this.groupBox2.Controls.Add(this.label6);
			this.groupBox2.Controls.Add(this.radioSemiMonthly);
			this.groupBox2.Controls.Add(this.radioWeekly);
			this.groupBox2.Controls.Add(this.textPayPeriods);
			this.groupBox2.Controls.Add(this.radioMonthly);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.radioBiWeekly);
			this.groupBox2.Location = new System.Drawing.Point(40, 44);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(250, 209);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Interval";
			// 
			// groupBoxSemiMonthly
			// 
			this.groupBoxSemiMonthly.Controls.Add(this.checkLast);
			this.groupBoxSemiMonthly.Controls.Add(this.labelPeriod2Day);
			this.groupBoxSemiMonthly.Controls.Add(this.textDay2);
			this.groupBoxSemiMonthly.Controls.Add(this.labelPeriod1Day);
			this.groupBoxSemiMonthly.Controls.Add(this.textDay1);
			this.groupBoxSemiMonthly.Controls.Add(this.radioPayDate);
			this.groupBoxSemiMonthly.Controls.Add(this.radioEndDate);
			this.groupBoxSemiMonthly.Location = new System.Drawing.Point(7, 121);
			this.groupBoxSemiMonthly.Name = "groupBoxSemiMonthly";
			this.groupBoxSemiMonthly.Size = new System.Drawing.Size(233, 83);
			this.groupBoxSemiMonthly.TabIndex = 32;
			this.groupBoxSemiMonthly.TabStop = false;
			this.groupBoxSemiMonthly.Text = "Semi-Monthly Settings";
			// 
			// checkLast
			// 
			this.checkLast.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLast.Location = new System.Drawing.Point(27, 62);
			this.checkLast.Name = "checkLast";
			this.checkLast.Size = new System.Drawing.Size(82, 17);
			this.checkLast.TabIndex = 27;
			this.checkLast.Text = "Last Day";
			this.checkLast.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkLast.UseVisualStyleBackColor = true;
			this.checkLast.Click += new System.EventHandler(this.checkLast_Click);
			// 
			// labelPeriod2Day
			// 
			this.labelPeriod2Day.Location = new System.Drawing.Point(2, 38);
			this.labelPeriod2Day.Name = "labelPeriod2Day";
			this.labelPeriod2Day.Size = new System.Drawing.Size(80, 18);
			this.labelPeriod2Day.TabIndex = 40;
			this.labelPeriod2Day.Text = "Period 2 Day";
			this.labelPeriod2Day.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDay2
			// 
			this.textDay2.Location = new System.Drawing.Point(83, 37);
			this.textDay2.MaxVal = 28;
			this.textDay2.MinVal = 1;
			this.textDay2.Name = "textDay2";
			this.textDay2.ShowZero = false;
			this.textDay2.Size = new System.Drawing.Size(26, 20);
			this.textDay2.TabIndex = 39;
			this.textDay2.Text = "16";
			// 
			// labelPeriod1Day
			// 
			this.labelPeriod1Day.Location = new System.Drawing.Point(3, 16);
			this.labelPeriod1Day.Name = "labelPeriod1Day";
			this.labelPeriod1Day.Size = new System.Drawing.Size(79, 18);
			this.labelPeriod1Day.TabIndex = 38;
			this.labelPeriod1Day.Text = "Period 1 Day";
			this.labelPeriod1Day.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDay1
			// 
			this.textDay1.Location = new System.Drawing.Point(83, 15);
			this.textDay1.MaxVal = 28;
			this.textDay1.MinVal = 1;
			this.textDay1.Name = "textDay1";
			this.textDay1.Size = new System.Drawing.Size(26, 20);
			this.textDay1.TabIndex = 37;
			this.textDay1.Text = "1";
			// 
			// radioPayDate
			// 
			this.radioPayDate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayDate.Checked = true;
			this.radioPayDate.Location = new System.Drawing.Point(136, 37);
			this.radioPayDate.Name = "radioPayDate";
			this.radioPayDate.Size = new System.Drawing.Size(77, 17);
			this.radioPayDate.TabIndex = 36;
			this.radioPayDate.TabStop = true;
			this.radioPayDate.Text = "Pay Date";
			this.radioPayDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayDate.UseVisualStyleBackColor = true;
			// 
			// radioEndDate
			// 
			this.radioEndDate.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioEndDate.Location = new System.Drawing.Point(136, 17);
			this.radioEndDate.Name = "radioEndDate";
			this.radioEndDate.Size = new System.Drawing.Size(77, 17);
			this.radioEndDate.TabIndex = 35;
			this.radioEndDate.Text = "End Date";
			this.radioEndDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioEndDate.UseVisualStyleBackColor = true;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(128, 69);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(119, 17);
			this.label7.TabIndex = 31;
			this.label7.Text = "(twice a month)";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(128, 50);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(102, 17);
			this.label6.TabIndex = 30;
			this.label6.Text = "(every 2 weeks)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioSemiMonthly
			// 
			this.radioSemiMonthly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioSemiMonthly.Location = new System.Drawing.Point(22, 69);
			this.radioSemiMonthly.Name = "radioSemiMonthly";
			this.radioSemiMonthly.Size = new System.Drawing.Size(102, 17);
			this.radioSemiMonthly.TabIndex = 29;
			this.radioSemiMonthly.Text = "Semi-Monthly";
			this.radioSemiMonthly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioSemiMonthly.UseVisualStyleBackColor = true;
			this.radioSemiMonthly.Click += new System.EventHandler(this.butSemiMonthly_Click);
			// 
			// radioWeekly
			// 
			this.radioWeekly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioWeekly.Location = new System.Drawing.Point(51, 15);
			this.radioWeekly.Name = "radioWeekly";
			this.radioWeekly.Size = new System.Drawing.Size(73, 17);
			this.radioWeekly.TabIndex = 1;
			this.radioWeekly.Text = "Weekly";
			this.radioWeekly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioWeekly.UseVisualStyleBackColor = true;
			this.radioWeekly.Click += new System.EventHandler(this.radioWeekly_Click);
			// 
			// textPayPeriods
			// 
			this.textPayPeriods.Location = new System.Drawing.Point(134, 94);
			this.textPayPeriods.MaxVal = 200;
			this.textPayPeriods.Name = "textPayPeriods";
			this.textPayPeriods.Size = new System.Drawing.Size(51, 20);
			this.textPayPeriods.TabIndex = 22;
			// 
			// radioMonthly
			// 
			this.radioMonthly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioMonthly.Location = new System.Drawing.Point(43, 33);
			this.radioMonthly.Name = "radioMonthly";
			this.radioMonthly.Size = new System.Drawing.Size(81, 17);
			this.radioMonthly.TabIndex = 3;
			this.radioMonthly.Text = "Monthly";
			this.radioMonthly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioMonthly.UseVisualStyleBackColor = true;
			this.radioMonthly.Click += new System.EventHandler(this.radioMonthly_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(5, 95);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(140, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "# Pay Periods to Generate";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// radioBiWeekly
			// 
			this.radioBiWeekly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBiWeekly.Location = new System.Drawing.Point(30, 51);
			this.radioBiWeekly.Name = "radioBiWeekly";
			this.radioBiWeekly.Size = new System.Drawing.Size(94, 17);
			this.radioBiWeekly.TabIndex = 2;
			this.radioBiWeekly.Text = "Bi-Weekly";
			this.radioBiWeekly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBiWeekly.UseVisualStyleBackColor = true;
			this.radioBiWeekly.Click += new System.EventHandler(this.radioBiWeekly_Click);
			// 
			// dateTimeStart
			// 
			this.dateTimeStart.CustomFormat = "";
			this.dateTimeStart.Location = new System.Drawing.Point(90, 21);
			this.dateTimeStart.Name = "dateTimeStart";
			this.dateTimeStart.Size = new System.Drawing.Size(200, 20);
			this.dateTimeStart.TabIndex = 0;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMain.Location = new System.Drawing.Point(327, 20);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(291, 485);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = "Pay Periods";
			this.gridMain.TranslationName = "TablePayPeriods";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(482, 523);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 18;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butGenerate
			// 
			this.butGenerate.Icon = OpenDental.UI.EnumIcons.Add;
			this.butGenerate.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butGenerate.Location = new System.Drawing.Point(204, 422);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(86, 24);
			this.butGenerate.TabIndex = 10;
			this.butGenerate.Text = "&Generate";
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(563, 523);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormPayPeriodManager
			// 
			this.ClientSize = new System.Drawing.Size(650, 559);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.dateTimeStart);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayPeriodManager";
			this.ShowInTaskbar = false;
			this.Text = "Pay Period Manager";
			this.Load += new System.EventHandler(this.FormPayPeriodManager_Load);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBoxSemiMonthly.ResumeLayout(false);
			this.groupBoxSemiMonthly.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private UI.Button butCancel;
		private UI.Button butGenerate;
		private UI.GridOD gridMain;
		private UI.Button butOK;
		private DateTimePicker dateTimeStart;
		private Label label1;
		private RadioButton radioWeekly;
		private RadioButton radioMonthly;
		private RadioButton radioBiWeekly;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private Label label4;
		private Label label2;
		private Label label5;
		private ValidNum textDaysAfterPayPeriod;
		private ValidNum textPayPeriods;
		private ComboBox comboDay;
		private RadioButton radioPayBefore;
		private RadioButton radioPayAfter;
		private CheckBox checkExcludeWeekends;
		private Label label3;
		private RadioButton radioSemiMonthly;
		private Label label7;
		private Label label6;
		private GroupBox groupBoxSemiMonthly;
		private Label labelPeriod2Day;
		private ValidNum textDay2;
		private Label labelPeriod1Day;
		private ValidNum textDay1;
		private RadioButton radioPayDate;
		private RadioButton radioEndDate;
		private CheckBox checkLast;
	}
}
