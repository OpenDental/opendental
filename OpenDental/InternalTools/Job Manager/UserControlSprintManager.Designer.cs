namespace OpenDental {
	partial class UserControlSprintManager {
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
			this.labelCompletionPercentDescription = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.gridSprintJobs = new OpenDental.UI.GridOD();
			this.panel2 = new System.Windows.Forms.Panel();
			this.textCompletionPercentage = new System.Windows.Forms.Label();
			this.progressBarCompletionPercent = new OpenDental.UI.OdProgressBar();
			this.groupBoxSprintDetails = new System.Windows.Forms.GroupBox();
			this.butDashboard = new OpenDental.UI.Button();
			this.checkProgressMode = new System.Windows.Forms.CheckBox();
			this.butSave = new OpenDental.UI.Button();
			this.textBreakHours = new OpenDental.ValidDouble();
			this.textAvgDevelopmentHours = new OpenDental.ValidDouble();
			this.textEngJobPercent = new OpenDental.ValidDouble();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textDateEndActual = new System.Windows.Forms.Label();
			this.labelDateEndActual = new System.Windows.Forms.Label();
			this.textAvgAllocatableHours = new System.Windows.Forms.Label();
			this.label17 = new System.Windows.Forms.Label();
			this.textDays = new System.Windows.Forms.Label();
			this.label19 = new System.Windows.Forms.Label();
			this.textRatioHours = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textAfterBreak = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textEngHours = new System.Windows.Forms.Label();
			this.label10 = new System.Windows.Forms.Label();
			this.textJobNumber = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.textDateEnd = new OpenDental.ValidDate();
			this.textDateStart = new OpenDental.ValidDate();
			this.textAllocatedHours = new System.Windows.Forms.Label();
			this.textMaxHours = new System.Windows.Forms.Label();
			this.labelAllocatedHoursDescription = new System.Windows.Forms.Label();
			this.labelMaxHoursDescription = new System.Windows.Forms.Label();
			this.progressBarAllocatedHours = new OpenDental.UI.OdProgressBar();
			this.panel1 = new System.Windows.Forms.Panel();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.tableLayoutPanel1.SuspendLayout();
			this.panel2.SuspendLayout();
			this.groupBoxSprintDetails.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// labelCompletionPercentDescription
			// 
			this.labelCompletionPercentDescription.AutoSize = true;
			this.labelCompletionPercentDescription.Location = new System.Drawing.Point(3, 22);
			this.labelCompletionPercentDescription.Name = "labelCompletionPercentDescription";
			this.labelCompletionPercentDescription.Size = new System.Drawing.Size(120, 13);
			this.labelCompletionPercentDescription.TabIndex = 10;
			this.labelCompletionPercentDescription.Text = "Completion Percentage:";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.ColumnCount = 1;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Controls.Add(this.gridSprintJobs, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.panel2, 0, 3);
			this.tableLayoutPanel1.Controls.Add(this.groupBoxSprintDetails, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.panel1, 0, 0);
			this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 4;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 45F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 205F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 70F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(791, 566);
			this.tableLayoutPanel1.TabIndex = 11;
			// 
			// gridSprintJobs
			// 
			this.gridSprintJobs.Dock = System.Windows.Forms.DockStyle.Fill;
			this.gridSprintJobs.HasAddButton = true;
			this.gridSprintJobs.Location = new System.Drawing.Point(3, 253);
			this.gridSprintJobs.Name = "gridSprintJobs";
			this.gridSprintJobs.ShowContextMenu = false;
			this.gridSprintJobs.Size = new System.Drawing.Size(785, 240);
			this.gridSprintJobs.TabIndex = 2;
			this.gridSprintJobs.Title = "Jobs";
			this.gridSprintJobs.TranslationName = "Jobs";
			this.gridSprintJobs.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridSprintJobs_CellDoubleClick);
			this.gridSprintJobs.TitleAddClick += new System.EventHandler(this.gridSprintJobs_TitleAddClick);
			this.gridSprintJobs.MouseClick += new System.Windows.Forms.MouseEventHandler(this.gridSprintJobs_MouseClick);
			// 
			// panel2
			// 
			this.panel2.Controls.Add(this.textCompletionPercentage);
			this.panel2.Controls.Add(this.progressBarCompletionPercent);
			this.panel2.Controls.Add(this.labelCompletionPercentDescription);
			this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel2.Location = new System.Drawing.Point(3, 499);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(785, 64);
			this.panel2.TabIndex = 1;
			// 
			// textCompletionPercentage
			// 
			this.textCompletionPercentage.AutoSize = true;
			this.textCompletionPercentage.Location = new System.Drawing.Point(123, 22);
			this.textCompletionPercentage.Name = "textCompletionPercentage";
			this.textCompletionPercentage.Size = new System.Drawing.Size(31, 13);
			this.textCompletionPercentage.TabIndex = 14;
			this.textCompletionPercentage.Text = "1000";
			// 
			// progressBarCompletionPercent
			// 
			this.progressBarCompletionPercent.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarCompletionPercent.BackColor = System.Drawing.Color.LightSkyBlue;
			this.progressBarCompletionPercent.ForeColor = System.Drawing.Color.SteelBlue;
			this.progressBarCompletionPercent.Location = new System.Drawing.Point(3, 38);
			this.progressBarCompletionPercent.MarqueeAnimationSpeed = 0;
			this.progressBarCompletionPercent.Name = "progressBarCompletionPercent";
			this.progressBarCompletionPercent.Size = new System.Drawing.Size(773, 23);
			this.progressBarCompletionPercent.TabIndex = 9;
			this.progressBarCompletionPercent.TargetColor = System.Drawing.Color.Red;
			this.progressBarCompletionPercent.TargetValue = 0;
			// 
			// groupBoxSprintDetails
			// 
			this.groupBoxSprintDetails.Controls.Add(this.butDashboard);
			this.groupBoxSprintDetails.Controls.Add(this.checkProgressMode);
			this.groupBoxSprintDetails.Controls.Add(this.butSave);
			this.groupBoxSprintDetails.Controls.Add(this.textBreakHours);
			this.groupBoxSprintDetails.Controls.Add(this.textAvgDevelopmentHours);
			this.groupBoxSprintDetails.Controls.Add(this.textEngJobPercent);
			this.groupBoxSprintDetails.Controls.Add(this.groupBox1);
			this.groupBoxSprintDetails.Controls.Add(this.textNote);
			this.groupBoxSprintDetails.Controls.Add(this.label5);
			this.groupBoxSprintDetails.Controls.Add(this.label7);
			this.groupBoxSprintDetails.Controls.Add(this.label3);
			this.groupBoxSprintDetails.Controls.Add(this.label4);
			this.groupBoxSprintDetails.Controls.Add(this.label2);
			this.groupBoxSprintDetails.Controls.Add(this.label1);
			this.groupBoxSprintDetails.Controls.Add(this.textDateEnd);
			this.groupBoxSprintDetails.Controls.Add(this.textDateStart);
			this.groupBoxSprintDetails.Controls.Add(this.textAllocatedHours);
			this.groupBoxSprintDetails.Controls.Add(this.textMaxHours);
			this.groupBoxSprintDetails.Controls.Add(this.labelAllocatedHoursDescription);
			this.groupBoxSprintDetails.Controls.Add(this.labelMaxHoursDescription);
			this.groupBoxSprintDetails.Controls.Add(this.progressBarAllocatedHours);
			this.groupBoxSprintDetails.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBoxSprintDetails.Location = new System.Drawing.Point(3, 48);
			this.groupBoxSprintDetails.Name = "groupBoxSprintDetails";
			this.groupBoxSprintDetails.Size = new System.Drawing.Size(785, 199);
			this.groupBoxSprintDetails.TabIndex = 0;
			this.groupBoxSprintDetails.TabStop = false;
			this.groupBoxSprintDetails.Text = "Sprint Settings";
			// 
			// butDashboard
			// 
			this.butDashboard.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDashboard.Location = new System.Drawing.Point(414, 145);
			this.butDashboard.Name = "butDashboard";
			this.butDashboard.Size = new System.Drawing.Size(95, 23);
			this.butDashboard.TabIndex = 33;
			this.butDashboard.Text = "Open Dashboard";
			this.butDashboard.UseVisualStyleBackColor = true;
			this.butDashboard.Click += new System.EventHandler(this.butDashboard_Click);
			// 
			// checkProgressMode
			// 
			this.checkProgressMode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkProgressMode.AutoSize = true;
			this.checkProgressMode.Location = new System.Drawing.Point(519, 149);
			this.checkProgressMode.Name = "checkProgressMode";
			this.checkProgressMode.Size = new System.Drawing.Size(97, 17);
			this.checkProgressMode.TabIndex = 32;
			this.checkProgressMode.Text = "Progress Mode";
			this.checkProgressMode.UseVisualStyleBackColor = true;
			this.checkProgressMode.CheckedChanged += new System.EventHandler(this.checkProgressMode_CheckedChanged);
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butSave.Location = new System.Drawing.Point(235, 145);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(125, 23);
			this.butSave.TabIndex = 29;
			this.butSave.Text = "Save Sprint Settings";
			this.butSave.UseVisualStyleBackColor = true;
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// textBreakHours
			// 
			this.textBreakHours.Location = new System.Drawing.Point(108, 108);
			this.textBreakHours.MaxVal = 100000000D;
			this.textBreakHours.MinVal = -100000000D;
			this.textBreakHours.Name = "textBreakHours";
			this.textBreakHours.Size = new System.Drawing.Size(100, 20);
			this.textBreakHours.TabIndex = 31;
			this.textBreakHours.TextChanged += new System.EventHandler(this.textBreakHours_TextChanged);
			// 
			// textAvgDevelopmentHours
			// 
			this.textAvgDevelopmentHours.Location = new System.Drawing.Point(108, 85);
			this.textAvgDevelopmentHours.MaxVal = 100000000D;
			this.textAvgDevelopmentHours.MinVal = -100000000D;
			this.textAvgDevelopmentHours.Name = "textAvgDevelopmentHours";
			this.textAvgDevelopmentHours.Size = new System.Drawing.Size(100, 20);
			this.textAvgDevelopmentHours.TabIndex = 30;
			this.textAvgDevelopmentHours.TextChanged += new System.EventHandler(this.textAvgJobHours_TextChanged);
			// 
			// textEngJobPercent
			// 
			this.textEngJobPercent.Location = new System.Drawing.Point(108, 63);
			this.textEngJobPercent.MaxVal = 100000000D;
			this.textEngJobPercent.MinVal = -100000000D;
			this.textEngJobPercent.Name = "textEngJobPercent";
			this.textEngJobPercent.Size = new System.Drawing.Size(100, 20);
			this.textEngJobPercent.TabIndex = 29;
			this.textEngJobPercent.TextChanged += new System.EventHandler(this.textEngJobPercent_TextChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.textDateEndActual);
			this.groupBox1.Controls.Add(this.labelDateEndActual);
			this.groupBox1.Controls.Add(this.textAvgAllocatableHours);
			this.groupBox1.Controls.Add(this.label17);
			this.groupBox1.Controls.Add(this.textDays);
			this.groupBox1.Controls.Add(this.label19);
			this.groupBox1.Controls.Add(this.textRatioHours);
			this.groupBox1.Controls.Add(this.label11);
			this.groupBox1.Controls.Add(this.textAfterBreak);
			this.groupBox1.Controls.Add(this.label9);
			this.groupBox1.Controls.Add(this.textEngHours);
			this.groupBox1.Controls.Add(this.label10);
			this.groupBox1.Controls.Add(this.textJobNumber);
			this.groupBox1.Controls.Add(this.label8);
			this.groupBox1.Location = new System.Drawing.Point(235, 13);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(272, 130);
			this.groupBox1.TabIndex = 28;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Info";
			// 
			// textDateEndActual
			// 
			this.textDateEndActual.AutoSize = true;
			this.textDateEndActual.Location = new System.Drawing.Point(131, 110);
			this.textDateEndActual.Name = "textDateEndActual";
			this.textDateEndActual.Size = new System.Drawing.Size(65, 13);
			this.textDateEndActual.TabIndex = 33;
			this.textDateEndActual.Text = "01/01/2019";
			// 
			// labelDateEndActual
			// 
			this.labelDateEndActual.AutoSize = true;
			this.labelDateEndActual.Location = new System.Drawing.Point(40, 110);
			this.labelDateEndActual.Name = "labelDateEndActual";
			this.labelDateEndActual.Size = new System.Drawing.Size(88, 13);
			this.labelDateEndActual.TabIndex = 32;
			this.labelDateEndActual.Text = "Actual End Date:";
			// 
			// textAvgAllocatableHours
			// 
			this.textAvgAllocatableHours.AutoSize = true;
			this.textAvgAllocatableHours.Location = new System.Drawing.Point(249, 36);
			this.textAvgAllocatableHours.Name = "textAvgAllocatableHours";
			this.textAvgAllocatableHours.Size = new System.Drawing.Size(13, 13);
			this.textAvgAllocatableHours.TabIndex = 31;
			this.textAvgAllocatableHours.Text = "0";
			// 
			// label17
			// 
			this.label17.AutoSize = true;
			this.label17.Location = new System.Drawing.Point(156, 36);
			this.label17.Name = "label17";
			this.label17.Size = new System.Drawing.Size(87, 13);
			this.label17.TabIndex = 30;
			this.label17.Text = "Avg. Hours/Day:";
			// 
			// textDays
			// 
			this.textDays.AutoSize = true;
			this.textDays.Location = new System.Drawing.Point(249, 13);
			this.textDays.Name = "textDays";
			this.textDays.Size = new System.Drawing.Size(13, 13);
			this.textDays.TabIndex = 29;
			this.textDays.Text = "0";
			// 
			// label19
			// 
			this.label19.AutoSize = true;
			this.label19.Location = new System.Drawing.Point(160, 13);
			this.label19.Name = "label19";
			this.label19.Size = new System.Drawing.Size(56, 13);
			this.label19.TabIndex = 28;
			this.label19.Text = "# of Days:";
			// 
			// textRatioHours
			// 
			this.textRatioHours.AutoSize = true;
			this.textRatioHours.Location = new System.Drawing.Point(112, 59);
			this.textRatioHours.Name = "textRatioHours";
			this.textRatioHours.Size = new System.Drawing.Size(13, 13);
			this.textRatioHours.TabIndex = 23;
			this.textRatioHours.Text = "0";
			// 
			// label11
			// 
			this.label11.AutoSize = true;
			this.label11.Location = new System.Drawing.Point(13, 58);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(93, 13);
			this.label11.TabIndex = 22;
			this.label11.Text = "Allocatable Hours:";
			// 
			// textAfterBreak
			// 
			this.textAfterBreak.AutoSize = true;
			this.textAfterBreak.Location = new System.Drawing.Point(112, 36);
			this.textAfterBreak.Name = "textAfterBreak";
			this.textAfterBreak.Size = new System.Drawing.Size(13, 13);
			this.textAfterBreak.TabIndex = 21;
			this.textAfterBreak.Text = "0";
			// 
			// label9
			// 
			this.label9.AutoSize = true;
			this.label9.Location = new System.Drawing.Point(16, 36);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(90, 13);
			this.label9.TabIndex = 20;
			this.label9.Text = "Est. Break Hours:";
			// 
			// textEngHours
			// 
			this.textEngHours.AutoSize = true;
			this.textEngHours.Location = new System.Drawing.Point(112, 13);
			this.textEngHours.Name = "textEngHours";
			this.textEngHours.Size = new System.Drawing.Size(13, 13);
			this.textEngHours.TabIndex = 19;
			this.textEngHours.Text = "0";
			// 
			// label10
			// 
			this.label10.AutoSize = true;
			this.label10.Location = new System.Drawing.Point(23, 13);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(83, 13);
			this.label10.TabIndex = 18;
			this.label10.Text = "Engineer Hours:";
			// 
			// textJobNumber
			// 
			this.textJobNumber.AutoSize = true;
			this.textJobNumber.Location = new System.Drawing.Point(112, 82);
			this.textJobNumber.Name = "textJobNumber";
			this.textJobNumber.Size = new System.Drawing.Size(13, 13);
			this.textJobNumber.TabIndex = 17;
			this.textJobNumber.Text = "0";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.Location = new System.Drawing.Point(52, 82);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(54, 13);
			this.label8.TabIndex = 16;
			this.label8.Text = "# of Jobs:";
			// 
			// textNote
			// 
			this.textNote.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNote.Location = new System.Drawing.Point(519, 36);
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(260, 107);
			this.textNote.TabIndex = 27;
			this.textNote.TextChanged += new System.EventHandler(this.textNote_TextChanged);
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(523, 17);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(30, 13);
			this.label5.TabIndex = 26;
			this.label5.Text = "Note";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.Location = new System.Drawing.Point(14, 111);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(88, 13);
			this.label7.TabIndex = 25;
			this.label7.Text = "Avg Break Hours";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(38, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(64, 13);
			this.label3.TabIndex = 21;
			this.label3.Text = "Job Percent";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(19, 88);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(83, 13);
			this.label4.TabIndex = 23;
			this.label4.Text = "Avg Dev. Hours";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(16, 44);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(86, 13);
			this.label2.TabIndex = 17;
			this.label2.Text = "Target End Date";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(47, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(55, 13);
			this.label1.TabIndex = 16;
			this.label1.Text = "Start Date";
			// 
			// textDateEnd
			// 
			this.textDateEnd.Location = new System.Drawing.Point(108, 41);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(100, 20);
			this.textDateEnd.TabIndex = 15;
			this.textDateEnd.TextChanged += new System.EventHandler(this.dateEnd_TextChanged);
			// 
			// textDateStart
			// 
			this.textDateStart.Location = new System.Drawing.Point(108, 19);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(100, 20);
			this.textDateStart.TabIndex = 14;
			this.textDateStart.TextChanged += new System.EventHandler(this.dateStart_TextChanged);
			// 
			// textAllocatedHours
			// 
			this.textAllocatedHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textAllocatedHours.AutoSize = true;
			this.textAllocatedHours.Location = new System.Drawing.Point(95, 150);
			this.textAllocatedHours.Name = "textAllocatedHours";
			this.textAllocatedHours.Size = new System.Drawing.Size(31, 13);
			this.textAllocatedHours.TabIndex = 13;
			this.textAllocatedHours.Text = "1000";
			// 
			// textMaxHours
			// 
			this.textMaxHours.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textMaxHours.AutoSize = true;
			this.textMaxHours.Location = new System.Drawing.Point(748, 150);
			this.textMaxHours.Name = "textMaxHours";
			this.textMaxHours.Size = new System.Drawing.Size(31, 13);
			this.textMaxHours.TabIndex = 12;
			this.textMaxHours.Text = "1000";
			// 
			// labelAllocatedHoursDescription
			// 
			this.labelAllocatedHoursDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.labelAllocatedHoursDescription.AutoSize = true;
			this.labelAllocatedHoursDescription.Location = new System.Drawing.Point(9, 150);
			this.labelAllocatedHoursDescription.Name = "labelAllocatedHoursDescription";
			this.labelAllocatedHoursDescription.Size = new System.Drawing.Size(85, 13);
			this.labelAllocatedHoursDescription.TabIndex = 11;
			this.labelAllocatedHoursDescription.Text = "Allocated Hours:";
			// 
			// labelMaxHoursDescription
			// 
			this.labelMaxHoursDescription.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelMaxHoursDescription.AutoSize = true;
			this.labelMaxHoursDescription.Location = new System.Drawing.Point(629, 150);
			this.labelMaxHoursDescription.Name = "labelMaxHoursDescription";
			this.labelMaxHoursDescription.Size = new System.Drawing.Size(113, 13);
			this.labelMaxHoursDescription.TabIndex = 10;
			this.labelMaxHoursDescription.Text = "Max allocatable hours:";
			// 
			// progressBarAllocatedHours
			// 
			this.progressBarAllocatedHours.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.progressBarAllocatedHours.BackColor = System.Drawing.Color.PaleGreen;
			this.progressBarAllocatedHours.ForeColor = System.Drawing.Color.ForestGreen;
			this.progressBarAllocatedHours.Location = new System.Drawing.Point(6, 169);
			this.progressBarAllocatedHours.MarqueeAnimationSpeed = 0;
			this.progressBarAllocatedHours.Name = "progressBarAllocatedHours";
			this.progressBarAllocatedHours.Size = new System.Drawing.Size(773, 23);
			this.progressBarAllocatedHours.TabIndex = 4;
			this.progressBarAllocatedHours.TargetColor = System.Drawing.Color.Red;
			this.progressBarAllocatedHours.TargetValue = 0;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.textTitle);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(3, 3);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(785, 39);
			this.panel1.TabIndex = 0;
			// 
			// textTitle
			// 
			this.textTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textTitle.Location = new System.Drawing.Point(0, 0);
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(785, 38);
			this.textTitle.TabIndex = 0;
			this.textTitle.Text = "Version";
			this.textTitle.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			this.textTitle.TextChanged += new System.EventHandler(this.textTitle_TextChanged);
			// 
			// UserControlSprintManager
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.tableLayoutPanel1);
			this.Name = "UserControlSprintManager";
			this.Size = new System.Drawing.Size(791, 566);
			this.tableLayoutPanel1.ResumeLayout(false);
			this.panel2.ResumeLayout(false);
			this.panel2.PerformLayout();
			this.groupBoxSprintDetails.ResumeLayout(false);
			this.groupBoxSprintDetails.PerformLayout();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.OdProgressBar progressBarAllocatedHours;
		private System.Windows.Forms.Label labelCompletionPercentDescription;
		private OpenDental.UI.OdProgressBar progressBarCompletionPercent;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.GroupBox groupBoxSprintDetails;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label labelAllocatedHoursDescription;
		private System.Windows.Forms.Label labelMaxHoursDescription;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private ValidDate textDateEnd;
		private ValidDate textDateStart;
		private System.Windows.Forms.Label textAllocatedHours;
		private System.Windows.Forms.Label textMaxHours;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.TextBox textNote;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label textAvgAllocatableHours;
		private System.Windows.Forms.Label label17;
		private System.Windows.Forms.Label textDays;
		private System.Windows.Forms.Label label19;
		private System.Windows.Forms.Label textRatioHours;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label textAfterBreak;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.Label textEngHours;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label textJobNumber;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Panel panel2;
		private UI.GridOD gridSprintJobs;
		private System.Windows.Forms.Label textCompletionPercentage;
		private UI.Button butSave;
		private ValidDouble textEngJobPercent;
		private ValidDouble textBreakHours;
		private ValidDouble textAvgDevelopmentHours;
		private System.Windows.Forms.Label textDateEndActual;
		private System.Windows.Forms.Label labelDateEndActual;
		private System.Windows.Forms.CheckBox checkProgressMode;
		private UI.Button butDashboard;
	}
}
