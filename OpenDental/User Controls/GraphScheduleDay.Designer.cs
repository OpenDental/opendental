namespace OpenDental {
	partial class GraphScheduleDay {
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
			this.components = new System.ComponentModel.Container();
			this.timerRefresh = new System.Windows.Forms.Timer(this.components);
			this.panel2 = new OpenDental.UI.PanelOD();
			this.groupBox3 = new OpenDental.UI.GroupBox();
			this.numEndHour = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.numStartHour = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.panel1 = new OpenDental.UI.PanelOD();
			this.groupBox2 = new OpenDental.UI.GroupBox();
			this.checkBoxNotes = new OpenDental.UI.CheckBox();
			this.checkBoxProviders = new OpenDental.UI.CheckBox();
			this.checkBoxEmployees = new OpenDental.UI.CheckBox();
			this.groupBox1 = new OpenDental.UI.GroupBox();
			this.radioStopTime = new System.Windows.Forms.RadioButton();
			this.radioStartTime = new System.Windows.Forms.RadioButton();
			this.radioName = new System.Windows.Forms.RadioButton();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numEndHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numStartHour)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerRefresh
			// 
			this.timerRefresh.Enabled = true;
			this.timerRefresh.Interval = 60000;
			this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
			// 
			// panel2
			// 
			this.panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel2.Location = new System.Drawing.Point(0, 436);
			this.panel2.Name = "panel2";
			this.panel2.Size = new System.Drawing.Size(738, 26);
			this.panel2.TabIndex = 1;
			this.panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.panel2_Paint);
			// 
			// groupBox3
			// 
			this.groupBox3.Controls.Add(this.numEndHour);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Controls.Add(this.numStartHour);
			this.groupBox3.Controls.Add(this.label2);
			this.groupBox3.Location = new System.Drawing.Point(489, 3);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(243, 43);
			this.groupBox3.TabIndex = 3;
			this.groupBox3.Text = "Scale (0-24)";
			// 
			// numEndHour
			// 
			this.numEndHour.Location = new System.Drawing.Point(183, 16);
			this.numEndHour.Margin = new System.Windows.Forms.Padding(2);
			this.numEndHour.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.numEndHour.Name = "numEndHour";
			this.numEndHour.Size = new System.Drawing.Size(44, 20);
			this.numEndHour.TabIndex = 8;
			this.numEndHour.ValueChanged += new System.EventHandler(this.numEndHour_ValueChanged);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(122, 17);
			this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56, 19);
			this.label3.TabIndex = 9;
			this.label3.Text = "End Hour";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// numStartHour
			// 
			this.numStartHour.Location = new System.Drawing.Point(74, 16);
			this.numStartHour.Margin = new System.Windows.Forms.Padding(2);
			this.numStartHour.Maximum = new decimal(new int[] {
            24,
            0,
            0,
            0});
			this.numStartHour.Name = "numStartHour";
			this.numStartHour.Size = new System.Drawing.Size(44, 20);
			this.numStartHour.TabIndex = 6;
			this.numStartHour.ValueChanged += new System.EventHandler(this.numStartHour_ValueChanged);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(5, 17);
			this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(64, 19);
			this.label2.TabIndex = 7;
			this.label2.Text = "Start Hour";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// panel1
			// 
			this.panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.panel1.AutoScroll = true;
			this.panel1.Location = new System.Drawing.Point(0, 52);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(738, 382);
			this.panel1.TabIndex = 0;
			this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxNotes);
			this.groupBox2.Controls.Add(this.checkBoxProviders);
			this.groupBox2.Controls.Add(this.checkBoxEmployees);
			this.groupBox2.Location = new System.Drawing.Point(251, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(232, 43);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.Text = "Filter";
			// 
			// checkBoxNotes
			// 
			this.checkBoxNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxNotes.Checked = true;
			this.checkBoxNotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxNotes.Location = new System.Drawing.Point(11, 18);
			this.checkBoxNotes.Name = "checkBoxNotes";
			this.checkBoxNotes.Size = new System.Drawing.Size(52, 20);
			this.checkBoxNotes.TabIndex = 2;
			this.checkBoxNotes.Text = "Notes";
			this.checkBoxNotes.CheckedChanged += new System.EventHandler(this.checkBoxNotes_CheckedChanged);
			// 
			// checkBoxProviders
			// 
			this.checkBoxProviders.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxProviders.Checked = true;
			this.checkBoxProviders.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxProviders.Location = new System.Drawing.Point(67, 18);
			this.checkBoxProviders.Name = "checkBoxProviders";
			this.checkBoxProviders.Size = new System.Drawing.Size(72, 20);
			this.checkBoxProviders.TabIndex = 3;
			this.checkBoxProviders.Text = "Providers";
			this.checkBoxProviders.CheckedChanged += new System.EventHandler(this.checkBoxProviders_CheckedChanged);
			// 
			// checkBoxEmployees
			// 
			this.checkBoxEmployees.BackColor = System.Drawing.SystemColors.Control;
			this.checkBoxEmployees.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxEmployees.Checked = true;
			this.checkBoxEmployees.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxEmployees.Location = new System.Drawing.Point(142, 18);
			this.checkBoxEmployees.Name = "checkBoxEmployees";
			this.checkBoxEmployees.Size = new System.Drawing.Size(80, 20);
			this.checkBoxEmployees.TabIndex = 4;
			this.checkBoxEmployees.Text = "Employees";
			this.checkBoxEmployees.CheckedChanged += new System.EventHandler(this.checkBoxEmployees_CheckedChanged);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.radioStopTime);
			this.groupBox1.Controls.Add(this.radioStartTime);
			this.groupBox1.Controls.Add(this.radioName);
			this.groupBox1.Location = new System.Drawing.Point(0, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(245, 43);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.Text = "Sort";
			// 
			// radioStopTime
			// 
			this.radioStopTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStopTime.Checked = true;
			this.radioStopTime.Location = new System.Drawing.Point(89, 20);
			this.radioStopTime.Name = "radioStopTime";
			this.radioStopTime.Size = new System.Drawing.Size(79, 17);
			this.radioStopTime.TabIndex = 2;
			this.radioStopTime.TabStop = true;
			this.radioStopTime.Text = "Stop Time";
			this.radioStopTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStopTime.UseVisualStyleBackColor = true;
			this.radioStopTime.CheckedChanged += new System.EventHandler(this.radioStopTime_CheckedChanged);
			// 
			// radioStartTime
			// 
			this.radioStartTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStartTime.Location = new System.Drawing.Point(-2, 20);
			this.radioStartTime.Name = "radioStartTime";
			this.radioStartTime.Size = new System.Drawing.Size(80, 17);
			this.radioStartTime.TabIndex = 1;
			this.radioStartTime.Text = "Start Time";
			this.radioStartTime.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStartTime.UseVisualStyleBackColor = true;
			this.radioStartTime.CheckedChanged += new System.EventHandler(this.radioStartTime_CheckedChanged);
			// 
			// radioName
			// 
			this.radioName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioName.Location = new System.Drawing.Point(178, 20);
			this.radioName.Name = "radioName";
			this.radioName.Size = new System.Drawing.Size(60, 17);
			this.radioName.TabIndex = 0;
			this.radioName.Text = "Name";
			this.radioName.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioName.UseVisualStyleBackColor = true;
			this.radioName.CheckedChanged += new System.EventHandler(this.radioName_CheckedChanged);
			// 
			// GraphScheduleDay
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.Controls.Add(this.panel2);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.DoubleBuffered = true;
			this.Name = "GraphScheduleDay";
			this.Size = new System.Drawing.Size(738, 462);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numEndHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numStartHour)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion
		private OpenDental.UI.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioStopTime;
		private System.Windows.Forms.RadioButton radioStartTime;
		private System.Windows.Forms.RadioButton radioName;
		private OpenDental.UI.CheckBox checkBoxEmployees;
		private OpenDental.UI.CheckBox checkBoxProviders;
		private OpenDental.UI.CheckBox checkBoxNotes;
		private System.Windows.Forms.Timer timerRefresh;
		private OpenDental.UI.GroupBox groupBox2;
		private OpenDental.UI.GroupBox groupBox3;
		private System.Windows.Forms.NumericUpDown numStartHour;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numEndHour;
		private System.Windows.Forms.Label label3;
		private UI.PanelOD panel1;
		private UI.PanelOD panel2;
	}
}
