namespace OpenDental{
	partial class FormGraphEmployeeTime {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormGraphEmployeeTime));
			this.panelMain = new OpenDental.UI.PanelOD();
			this.labelDate = new System.Windows.Forms.Label();
			this.buttonLeft = new OpenDental.UI.Button();
			this.buttonRight = new OpenDental.UI.Button();
			this.butPrint = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butPrefs = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.toolTip = new System.Windows.Forms.ToolTip(this.components);
			this.label7 = new System.Windows.Forms.Label();
			this.textFilterName = new System.Windows.Forms.TextBox();
			this.butEditDefaults = new OpenDental.UI.Button();
			this.checkGraphed = new System.Windows.Forms.CheckBox();
			this.checkScheduled = new System.Windows.Forms.CheckBox();
			this.label4 = new System.Windows.Forms.Label();
			this.textTime = new System.Windows.Forms.TextBox();
			this.radioAM = new System.Windows.Forms.RadioButton();
			this.radioPM = new System.Windows.Forms.RadioButton();
			this.listOrder = new OpenDental.UI.ListBoxOD();
			this.label5 = new System.Windows.Forms.Label();
			this.butRefresh = new OpenDental.UI.Button();
			this.checkOffAtTop = new System.Windows.Forms.CheckBox();
			this.gridMain = new OpenDental.UI.GridOD();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.textAbsent = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.labelLimit = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textDailyLimitSoFar = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textDailyLimit = new System.Windows.Forms.TextBox();
			this.butEditDaily = new OpenDental.UI.Button();
			this.textNote = new System.Windows.Forms.TextBox();
			this.monthCalendar1 = new System.Windows.Forms.MonthCalendar();
			this.butToday = new OpenDental.UI.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.comboSupervisor = new OpenDental.UI.ComboBoxOD();
			this.butShifts = new OpenDental.UI.Button();
			this.groupBoxOD1.SuspendLayout();
			this.SuspendLayout();
			// 
			// panelMain
			// 
			this.panelMain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.panelMain.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.panelMain.Location = new System.Drawing.Point(7, 419);
			this.panelMain.Name = "panelMain";
			this.panelMain.Size = new System.Drawing.Size(706, 451);
			this.panelMain.TabIndex = 5;
			this.panelMain.Paint += new System.Windows.Forms.PaintEventHandler(this.PanelMain_Paint);
			// 
			// labelDate
			// 
			this.labelDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelDate.Location = new System.Drawing.Point(382, 12);
			this.labelDate.Name = "labelDate";
			this.labelDate.Size = new System.Drawing.Size(203, 23);
			this.labelDate.TabIndex = 6;
			this.labelDate.Text = "Monday, January 1st";
			this.labelDate.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonLeft
			// 
			this.buttonLeft.Image = global::OpenDental.Properties.Resources.Left;
			this.buttonLeft.Location = new System.Drawing.Point(419, 38);
			this.buttonLeft.Name = "buttonLeft";
			this.buttonLeft.Size = new System.Drawing.Size(22, 22);
			this.buttonLeft.TabIndex = 8;
			this.buttonLeft.UseVisualStyleBackColor = true;
			this.buttonLeft.Click += new System.EventHandler(this.buttonLeft_Click);
			// 
			// buttonRight
			// 
			this.buttonRight.Image = global::OpenDental.Properties.Resources.Right;
			this.buttonRight.Location = new System.Drawing.Point(514, 38);
			this.buttonRight.Name = "buttonRight";
			this.buttonRight.Size = new System.Drawing.Size(22, 22);
			this.buttonRight.TabIndex = 7;
			this.buttonRight.UseVisualStyleBackColor = true;
			this.buttonRight.Click += new System.EventHandler(this.buttonRight_Click);
			// 
			// butPrint
			// 
			this.butPrint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrint.Image = global::OpenDental.Properties.Resources.butPrint;
			this.butPrint.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrint.Location = new System.Drawing.Point(638, 877);
			this.butPrint.Name = "butPrint";
			this.butPrint.Size = new System.Drawing.Size(75, 24);
			this.butPrint.TabIndex = 9;
			this.butPrint.Text = "Print";
			this.butPrint.Click += new System.EventHandler(this.butPrint_Click);
			// 
			// label1
			// 
			this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label1.ForeColor = System.Drawing.Color.Red;
			this.label1.Location = new System.Drawing.Point(333, 883);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(126, 23);
			this.label1.TabIndex = 10;
			this.label1.Text = "# Minutes Behind";
			// 
			// butPrefs
			// 
			this.butPrefs.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butPrefs.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butPrefs.Location = new System.Drawing.Point(7, 877);
			this.butPrefs.Name = "butPrefs";
			this.butPrefs.Size = new System.Drawing.Size(75, 24);
			this.butPrefs.TabIndex = 11;
			this.butPrefs.Text = "Prefs";
			this.butPrefs.Click += new System.EventHandler(this.butPrefs_Click);
			// 
			// label2
			// 
			this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label2.ForeColor = System.Drawing.Color.Blue;
			this.label2.Location = new System.Drawing.Point(467, 883);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(140, 23);
			this.label2.TabIndex = 12;
			this.label2.Text = "# Techs Available";
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.ForeColor = System.Drawing.Color.Red;
			this.label3.Location = new System.Drawing.Point(151, 883);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(174, 23);
			this.label3.TabIndex = 13;
			this.label3.Text = "------- Expected Call Volume";
			// 
			// toolTip
			// 
			this.toolTip.AutoPopDelay = 30000;
			this.toolTip.InitialDelay = 50;
			this.toolTip.IsBalloon = true;
			this.toolTip.ReshowDelay = 50;
			this.toolTip.ToolTipTitle = "Employees";
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(492, 163);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(93, 20);
			this.label7.TabIndex = 1;
			this.label7.Text = "Filter by Name";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFilterName
			// 
			this.textFilterName.Location = new System.Drawing.Point(586, 163);
			this.textFilterName.Name = "textFilterName";
			this.textFilterName.Size = new System.Drawing.Size(100, 20);
			this.textFilterName.TabIndex = 0;
			this.textFilterName.TextChanged += new System.EventHandler(this.textFilterName_TextChanged);
			// 
			// butEditDefaults
			// 
			this.butEditDefaults.Location = new System.Drawing.Point(624, 73);
			this.butEditDefaults.Name = "butEditDefaults";
			this.butEditDefaults.Size = new System.Drawing.Size(81, 24);
			this.butEditDefaults.TabIndex = 55;
			this.butEditDefaults.Text = "Edit Defaults";
			this.butEditDefaults.UseVisualStyleBackColor = true;
			this.butEditDefaults.Click += new System.EventHandler(this.butEditDefaults_Click);
			// 
			// checkGraphed
			// 
			this.checkGraphed.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGraphed.Location = new System.Drawing.Point(397, 210);
			this.checkGraphed.Name = "checkGraphed";
			this.checkGraphed.Size = new System.Drawing.Size(203, 22);
			this.checkGraphed.TabIndex = 56;
			this.checkGraphed.Text = "Only show graphed by default";
			this.checkGraphed.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkGraphed.UseVisualStyleBackColor = true;
			this.checkGraphed.Click += new System.EventHandler(this.checkGraphed_Click);
			// 
			// checkScheduled
			// 
			this.checkScheduled.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkScheduled.Checked = true;
			this.checkScheduled.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkScheduled.Location = new System.Drawing.Point(397, 232);
			this.checkScheduled.Name = "checkScheduled";
			this.checkScheduled.Size = new System.Drawing.Size(203, 22);
			this.checkScheduled.TabIndex = 57;
			this.checkScheduled.Text = "Only show scheduled";
			this.checkScheduled.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkScheduled.UseVisualStyleBackColor = true;
			this.checkScheduled.Click += new System.EventHandler(this.checkScheduled_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(419, 258);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(165, 20);
			this.label4.TabIndex = 59;
			this.label4.Text = "Only show working at time";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTime
			// 
			this.textTime.Location = new System.Drawing.Point(586, 258);
			this.textTime.Name = "textTime";
			this.textTime.Size = new System.Drawing.Size(52, 20);
			this.textTime.TabIndex = 58;
			this.textTime.TextChanged += new System.EventHandler(this.textTime_TextChanged);
			// 
			// radioAM
			// 
			this.radioAM.Checked = true;
			this.radioAM.Location = new System.Drawing.Point(644, 251);
			this.radioAM.Name = "radioAM";
			this.radioAM.Size = new System.Drawing.Size(42, 18);
			this.radioAM.TabIndex = 60;
			this.radioAM.TabStop = true;
			this.radioAM.Text = "AM";
			this.radioAM.UseVisualStyleBackColor = true;
			this.radioAM.Click += new System.EventHandler(this.radioAM_Click);
			// 
			// radioPM
			// 
			this.radioPM.Location = new System.Drawing.Point(644, 268);
			this.radioPM.Name = "radioPM";
			this.radioPM.Size = new System.Drawing.Size(42, 18);
			this.radioPM.TabIndex = 61;
			this.radioPM.Text = "PM";
			this.radioPM.UseVisualStyleBackColor = true;
			this.radioPM.Click += new System.EventHandler(this.radioPM_Click);
			// 
			// listOrder
			// 
			this.listOrder.Items.AddRange(new object[] {
            "Name",
            "Start Time",
            "Stop Time"});
			this.listOrder.Location = new System.Drawing.Point(586, 300);
			this.listOrder.Name = "listOrder";
			this.listOrder.Size = new System.Drawing.Size(100, 43);
			this.listOrder.TabIndex = 63;
			this.listOrder.Click += new System.EventHandler(this.listOrder_Click);
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(484, 298);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 20);
			this.label5.TabIndex = 64;
			this.label5.Text = "Order by";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butRefresh
			// 
			this.butRefresh.Location = new System.Drawing.Point(586, 379);
			this.butRefresh.Name = "butRefresh";
			this.butRefresh.Size = new System.Drawing.Size(81, 24);
			this.butRefresh.TabIndex = 65;
			this.butRefresh.Text = "Refresh";
			this.butRefresh.UseVisualStyleBackColor = true;
			this.butRefresh.Click += new System.EventHandler(this.butRefresh_Click);
			// 
			// checkOffAtTop
			// 
			this.checkOffAtTop.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOffAtTop.Checked = true;
			this.checkOffAtTop.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkOffAtTop.Location = new System.Drawing.Point(397, 349);
			this.checkOffAtTop.Name = "checkOffAtTop";
			this.checkOffAtTop.Size = new System.Drawing.Size(203, 22);
			this.checkOffAtTop.TabIndex = 66;
			this.checkOffAtTop.Text = "Prescheduled off and Absent at top";
			this.checkOffAtTop.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkOffAtTop.UseVisualStyleBackColor = true;
			this.checkOffAtTop.Click += new System.EventHandler(this.checkOffAtTop_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(722, 12);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(759, 887);
			this.gridMain.TabIndex = 49;
			this.gridMain.Title = "Phone Graphs";
			this.gridMain.TranslationName = "TablePhoneGraphDate";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD1.Controls.Add(this.textAbsent);
			this.groupBoxOD1.Controls.Add(this.label12);
			this.groupBoxOD1.Controls.Add(this.label10);
			this.groupBoxOD1.Controls.Add(this.labelLimit);
			this.groupBoxOD1.Controls.Add(this.label9);
			this.groupBoxOD1.Controls.Add(this.textDailyLimitSoFar);
			this.groupBoxOD1.Controls.Add(this.label8);
			this.groupBoxOD1.Controls.Add(this.label6);
			this.groupBoxOD1.Controls.Add(this.textDailyLimit);
			this.groupBoxOD1.Controls.Add(this.butEditDaily);
			this.groupBoxOD1.Controls.Add(this.textNote);
			this.groupBoxOD1.Location = new System.Drawing.Point(12, 188);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(371, 215);
			this.groupBoxOD1.TabIndex = 67;
			this.groupBoxOD1.TabStop = false;
			this.groupBoxOD1.Text = "Daily";
			// 
			// textAbsent
			// 
			this.textAbsent.Location = new System.Drawing.Point(147, 156);
			this.textAbsent.Name = "textAbsent";
			this.textAbsent.ReadOnly = true;
			this.textAbsent.Size = new System.Drawing.Size(46, 20);
			this.textAbsent.TabIndex = 74;
			// 
			// label12
			// 
			this.label12.Location = new System.Drawing.Point(41, 155);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(100, 20);
			this.label12.TabIndex = 73;
			this.label12.Text = "Currently Absent";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(199, 112);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(91, 33);
			this.label10.TabIndex = 72;
			this.label10.Text = "limit only applies to call center";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelLimit
			// 
			this.labelLimit.Location = new System.Drawing.Point(10, 179);
			this.labelLimit.Name = "labelLimit";
			this.labelLimit.Size = new System.Drawing.Size(355, 30);
			this.labelLimit.TabIndex = 68;
			this.labelLimit.Text = "Daily limit has been reached for non-emergencies.  A supervisor with \'Schedule\' p" +
    "ermissions can increase the limit.";
			this.labelLimit.Visible = false;
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(2, 130);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(139, 20);
			this.label9.TabIndex = 71;
			this.label9.Text = "Currently Prescheduled Off";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDailyLimitSoFar
			// 
			this.textDailyLimitSoFar.Location = new System.Drawing.Point(147, 130);
			this.textDailyLimitSoFar.Name = "textDailyLimitSoFar";
			this.textDailyLimitSoFar.ReadOnly = true;
			this.textDailyLimitSoFar.Size = new System.Drawing.Size(46, 20);
			this.textDailyLimitSoFar.TabIndex = 70;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(21, 104);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(120, 20);
			this.label8.TabIndex = 69;
			this.label8.Text = "Max Prescheduled Off";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(7, 16);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(45, 20);
			this.label6.TabIndex = 69;
			this.label6.Text = "Note";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDailyLimit
			// 
			this.textDailyLimit.Location = new System.Drawing.Point(147, 104);
			this.textDailyLimit.Name = "textDailyLimit";
			this.textDailyLimit.ReadOnly = true;
			this.textDailyLimit.Size = new System.Drawing.Size(46, 20);
			this.textDailyLimit.TabIndex = 68;
			// 
			// butEditDaily
			// 
			this.butEditDaily.Location = new System.Drawing.Point(296, 107);
			this.butEditDaily.Name = "butEditDaily";
			this.butEditDaily.Size = new System.Drawing.Size(60, 24);
			this.butEditDaily.TabIndex = 68;
			this.butEditDaily.Text = "Edit";
			this.butEditDaily.UseVisualStyleBackColor = true;
			this.butEditDaily.Click += new System.EventHandler(this.butEditDaily_Click);
			// 
			// textNote
			// 
			this.textNote.AcceptsReturn = true;
			this.textNote.BackColor = System.Drawing.Color.White;
			this.textNote.Location = new System.Drawing.Point(52, 12);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ReadOnly = true;
			this.textNote.Size = new System.Drawing.Size(304, 86);
			this.textNote.TabIndex = 68;
			// 
			// monthCalendar1
			// 
			this.monthCalendar1.Location = new System.Drawing.Point(154, 12);
			this.monthCalendar1.MaxSelectionCount = 1;
			this.monthCalendar1.Name = "monthCalendar1";
			this.monthCalendar1.TabIndex = 69;
			this.monthCalendar1.DateChanged += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateChanged);
			this.monthCalendar1.DateSelected += new System.Windows.Forms.DateRangeEventHandler(this.monthCalendar1_DateSelected);
			// 
			// butToday
			// 
			this.butToday.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.butToday.Location = new System.Drawing.Point(447, 38);
			this.butToday.Name = "butToday";
			this.butToday.Size = new System.Drawing.Size(61, 22);
			this.butToday.TabIndex = 70;
			this.butToday.Text = "Today";
			this.butToday.UseVisualStyleBackColor = true;
			this.butToday.Click += new System.EventHandler(this.butToday_Click);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(460, 187);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(124, 20);
			this.label11.TabIndex = 71;
			this.label11.Text = "Reports To";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSupervisor
			// 
			this.comboSupervisor.Location = new System.Drawing.Point(586, 188);
			this.comboSupervisor.Name = "comboSupervisor";
			this.comboSupervisor.Size = new System.Drawing.Size(121, 21);
			this.comboSupervisor.TabIndex = 72;
			this.comboSupervisor.SelectionChangeCommitted += new System.EventHandler(this.comboSupervisor_SelectionChangeCommitted);
			// 
			// butShifts
			// 
			this.butShifts.Location = new System.Drawing.Point(586, 133);
			this.butShifts.Name = "butShifts";
			this.butShifts.Size = new System.Drawing.Size(81, 24);
			this.butShifts.TabIndex = 74;
			this.butShifts.Text = "My Schedule";
			this.butShifts.UseVisualStyleBackColor = true;
			this.butShifts.Click += new System.EventHandler(this.ButMySchedule_Click);
			// 
			// FormGraphEmployeeTime
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
			this.ClientSize = new System.Drawing.Size(1484, 911);
			this.Controls.Add(this.butShifts);
			this.Controls.Add(this.comboSupervisor);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.butToday);
			this.Controls.Add(this.monthCalendar1);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.checkOffAtTop);
			this.Controls.Add(this.butRefresh);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.listOrder);
			this.Controls.Add(this.radioPM);
			this.Controls.Add(this.radioAM);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textTime);
			this.Controls.Add(this.checkScheduled);
			this.Controls.Add(this.checkGraphed);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.butEditDefaults);
			this.Controls.Add(this.textFilterName);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butPrefs);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butPrint);
			this.Controls.Add(this.buttonLeft);
			this.Controls.Add(this.buttonRight);
			this.Controls.Add(this.labelDate);
			this.Controls.Add(this.panelMain);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormGraphEmployeeTime";
			this.Text = "Daily Graph";
			this.Load += new System.EventHandler(this.FormGraphEmployeeTime_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion
		private UI.PanelOD panelMain;
		private System.Windows.Forms.Label labelDate;
		private OpenDental.UI.Button buttonRight;
		private OpenDental.UI.Button buttonLeft;
		private OpenDental.UI.Button butPrint;
		private System.Windows.Forms.Label label1;
		private UI.Button butPrefs;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ToolTip toolTip;
		private UI.GridOD gridMain;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textFilterName;
		private UI.Button butEditDefaults;
		private System.Windows.Forms.CheckBox checkGraphed;
		private System.Windows.Forms.CheckBox checkScheduled;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textTime;
		private System.Windows.Forms.RadioButton radioAM;
		private System.Windows.Forms.RadioButton radioPM;
		private OpenDental.UI.ListBoxOD listOrder;
		private System.Windows.Forms.Label label5;
		private UI.Button butRefresh;
		private System.Windows.Forms.CheckBox checkOffAtTop;
		private UI.GroupBoxOD groupBoxOD1;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textDailyLimitSoFar;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textDailyLimit;
		private UI.Button butEditDaily;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label labelLimit;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.MonthCalendar monthCalendar1;
		private UI.Button butToday;
		private System.Windows.Forms.Label label11;
		private UI.ComboBoxOD comboSupervisor;
		private System.Windows.Forms.TextBox textAbsent;
		private System.Windows.Forms.Label label12;
		private UI.Button butShifts;
	}
}