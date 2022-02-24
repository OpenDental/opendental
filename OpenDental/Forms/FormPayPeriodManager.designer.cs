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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPeriodManager));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label3 = new System.Windows.Forms.Label();
			this.radioPayBefore = new System.Windows.Forms.RadioButton();
			this.radioPayAfter = new System.Windows.Forms.RadioButton();
			this.checkExcludeWeekends = new System.Windows.Forms.CheckBox();
			this.comboDay = new System.Windows.Forms.ComboBox();
			this.numDaysAfterPayPeriod = new OpenDental.ValidNum();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.radioWeekly = new System.Windows.Forms.RadioButton();
			this.numPayPeriods = new OpenDental.ValidNum();
			this.radioMonthly = new System.Windows.Forms.RadioButton();
			this.label5 = new System.Windows.Forms.Label();
			this.radioBiWeekly = new System.Windows.Forms.RadioButton();
			this.dateTimeStart = new System.Windows.Forms.DateTimePicker();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butGenerate = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.groupBox3);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.groupBox2);
			this.groupBox1.Controls.Add(this.dateTimeStart);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(309, 290);
			this.groupBox1.TabIndex = 20;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Pay Period Options";
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.radioPayBefore);
			this.groupBox3.Controls.Add(this.radioPayAfter);
			this.groupBox3.Controls.Add(this.checkExcludeWeekends);
			this.groupBox3.Controls.Add(this.comboDay);
			this.groupBox3.Controls.Add(this.numDaysAfterPayPeriod);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(31, 120);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(262, 160);
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
			this.radioPayBefore.Location = new System.Drawing.Point(51, 127);
			this.radioPayBefore.Name = "radioPayBefore";
			this.radioPayBefore.Size = new System.Drawing.Size(94, 17);
			this.radioPayBefore.TabIndex = 24;
			this.radioPayBefore.TabStop = true;
			this.radioPayBefore.Text = "Pay Before";
			this.radioPayBefore.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayBefore.UseVisualStyleBackColor = true;
			// 
			// radioPayAfter
			// 
			this.radioPayAfter.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioPayAfter.Location = new System.Drawing.Point(151, 127);
			this.radioPayAfter.Name = "radioPayAfter";
			this.radioPayAfter.Size = new System.Drawing.Size(73, 17);
			this.radioPayAfter.TabIndex = 25;
			this.radioPayAfter.TabStop = true;
			this.radioPayAfter.Text = "Pay After";
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
			// numDaysAfterPayPeriod
			// 
			this.numDaysAfterPayPeriod.Location = new System.Drawing.Point(132, 74);
			this.numDaysAfterPayPeriod.MaxVal = 200;
			this.numDaysAfterPayPeriod.MinVal = 0;
			this.numDaysAfterPayPeriod.Name = "numDaysAfterPayPeriod";
			this.numDaysAfterPayPeriod.Size = new System.Drawing.Size(79, 20);
			this.numDaysAfterPayPeriod.TabIndex = 21;
			this.numDaysAfterPayPeriod.TextChanged += new System.EventHandler(this.numDaysAfterPayPeriod_TextChanged);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 75);
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
			this.label1.Location = new System.Drawing.Point(6, 19);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(81, 20);
			this.label1.TabIndex = 6;
			this.label1.Text = "Start Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.radioWeekly);
			this.groupBox2.Controls.Add(this.numPayPeriods);
			this.groupBox2.Controls.Add(this.radioMonthly);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.radioBiWeekly);
			this.groupBox2.Location = new System.Drawing.Point(31, 42);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(262, 72);
			this.groupBox2.TabIndex = 5;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Interval";
			// 
			// radioWeekly
			// 
			this.radioWeekly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioWeekly.Location = new System.Drawing.Point(28, 19);
			this.radioWeekly.Name = "radioWeekly";
			this.radioWeekly.Size = new System.Drawing.Size(61, 17);
			this.radioWeekly.TabIndex = 1;
			this.radioWeekly.TabStop = true;
			this.radioWeekly.Text = "Weekly";
			this.radioWeekly.UseVisualStyleBackColor = true;
			this.radioWeekly.Click += new System.EventHandler(this.radioWeekly_Click);
			// 
			// numPayPeriods
			// 
			this.numPayPeriods.Location = new System.Drawing.Point(168, 42);
			this.numPayPeriods.MaxVal = 200;
			this.numPayPeriods.MinVal = 0;
			this.numPayPeriods.Name = "numPayPeriods";
			this.numPayPeriods.Size = new System.Drawing.Size(79, 20);
			this.numPayPeriods.TabIndex = 22;
			// 
			// radioMonthly
			// 
			this.radioMonthly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioMonthly.Location = new System.Drawing.Point(179, 19);
			this.radioMonthly.Name = "radioMonthly";
			this.radioMonthly.Size = new System.Drawing.Size(62, 17);
			this.radioMonthly.TabIndex = 3;
			this.radioMonthly.TabStop = true;
			this.radioMonthly.Text = "Monthly";
			this.radioMonthly.UseVisualStyleBackColor = true;
			this.radioMonthly.Click += new System.EventHandler(this.radioMonthly_Click);
			// 
			// label5
			// 
			this.label5.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label5.Location = new System.Drawing.Point(3, 43);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(162, 17);
			this.label5.TabIndex = 10;
			this.label5.Text = "# Pay Periods to Generate";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioBiWeekly
			// 
			this.radioBiWeekly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioBiWeekly.Location = new System.Drawing.Point(95, 19);
			this.radioBiWeekly.Name = "radioBiWeekly";
			this.radioBiWeekly.Size = new System.Drawing.Size(73, 17);
			this.radioBiWeekly.TabIndex = 2;
			this.radioBiWeekly.TabStop = true;
			this.radioBiWeekly.Text = "Bi-Weekly";
			this.radioBiWeekly.UseVisualStyleBackColor = true;
			this.radioBiWeekly.Click += new System.EventHandler(this.radioBiWeekly_Click);
			this.radioBiWeekly.MouseUp += new System.Windows.Forms.MouseEventHandler(this.radioBiWeekly_Click);
			// 
			// dateTimeStart
			// 
			this.dateTimeStart.CustomFormat = "";
			this.dateTimeStart.Location = new System.Drawing.Point(93, 19);
			this.dateTimeStart.Name = "dateTimeStart";
			this.dateTimeStart.Size = new System.Drawing.Size(200, 20);
			this.dateTimeStart.TabIndex = 0;
			// 
			// gridMain
			// 
			this.gridMain.Location = new System.Drawing.Point(327, 20);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(291, 282);
			this.gridMain.TabIndex = 11;
			this.gridMain.Title = "Pay Periods";
			this.gridMain.TranslationName = "TablePayPeriods";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(462, 310);
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
			this.butGenerate.Location = new System.Drawing.Point(235, 308);
			this.butGenerate.Name = "butGenerate";
			this.butGenerate.Size = new System.Drawing.Size(86, 24);
			this.butGenerate.TabIndex = 10;
			this.butGenerate.Text = "&Generate";
			this.butGenerate.Click += new System.EventHandler(this.butGenerate_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(543, 310);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 0;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormPayPeriodManager
			// 
			this.ClientSize = new System.Drawing.Size(630, 346);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butGenerate);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormPayPeriodManager";
			this.ShowInTaskbar = false;
			this.Text = "Pay Period Manager";
			this.Load += new System.EventHandler(this.FormPayPeriodManager_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}
		#endregion

		private UI.Button butCancel;
		private UI.Button butGenerate;
		private UI.GridOD gridMain;
		private UI.Button butOK;
		private DateTimePicker dateTimeStart;
		private GroupBox groupBox1;
		private Label label1;
		private RadioButton radioWeekly;
		private RadioButton radioMonthly;
		private RadioButton radioBiWeekly;
		private GroupBox groupBox2;
		private GroupBox groupBox3;
		private Label label4;
		private Label label2;
		private Label label5;
		private ValidNum numDaysAfterPayPeriod;
		private ValidNum numPayPeriods;
		private ComboBox comboDay;
		private RadioButton radioPayBefore;
		private RadioButton radioPayAfter;
		private CheckBox checkExcludeWeekends;
		private Label label3;
	}
}
