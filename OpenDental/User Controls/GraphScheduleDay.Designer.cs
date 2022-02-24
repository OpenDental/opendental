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
			this.splitContainerMaster = new OpenDental.SplitContainerNoFlicker();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.numEndHour = new System.Windows.Forms.NumericUpDown();
			this.label3 = new System.Windows.Forms.Label();
			this.numStartHour = new System.Windows.Forms.NumericUpDown();
			this.label2 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.checkBoxEmployees = new System.Windows.Forms.CheckBox();
			this.checkBoxProviders = new System.Windows.Forms.CheckBox();
			this.checkBoxNotes = new System.Windows.Forms.CheckBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioStopTime = new System.Windows.Forms.RadioButton();
			this.radioStartTime = new System.Windows.Forms.RadioButton();
			this.radioName = new System.Windows.Forms.RadioButton();
			this.splitContainerBottom = new OpenDental.SplitContainerNoFlicker();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMaster)).BeginInit();
			this.splitContainerMaster.Panel1.SuspendLayout();
			this.splitContainerMaster.Panel2.SuspendLayout();
			this.splitContainerMaster.SuspendLayout();
			this.groupBox3.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.numEndHour)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.numStartHour)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).BeginInit();
			this.splitContainerBottom.SuspendLayout();
			this.SuspendLayout();
			// 
			// timerRefresh
			// 
			this.timerRefresh.Enabled = true;
			this.timerRefresh.Interval = 60000;
			this.timerRefresh.Tick += new System.EventHandler(this.timerRefresh_Tick);
			// 
			// splitContainerMaster
			// 
			this.splitContainerMaster.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerMaster.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
			this.splitContainerMaster.IsSplitterFixed = true;
			this.splitContainerMaster.Location = new System.Drawing.Point(0, 0);
			this.splitContainerMaster.Name = "splitContainerMaster";
			this.splitContainerMaster.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerMaster.Panel1
			// 
			this.splitContainerMaster.Panel1.Controls.Add(this.groupBox3);
			this.splitContainerMaster.Panel1.Controls.Add(this.groupBox2);
			this.splitContainerMaster.Panel1.Controls.Add(this.groupBox1);
			// 
			// splitContainerMaster.Panel2
			// 
			this.splitContainerMaster.Panel2.Controls.Add(this.splitContainerBottom);
			this.splitContainerMaster.Size = new System.Drawing.Size(738, 462);
			this.splitContainerMaster.SplitterDistance = 52;
			this.splitContainerMaster.TabIndex = 2;
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
			this.groupBox3.TabStop = false;
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
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.checkBoxEmployees);
			this.groupBox2.Controls.Add(this.checkBoxProviders);
			this.groupBox2.Controls.Add(this.checkBoxNotes);
			this.groupBox2.Location = new System.Drawing.Point(251, 3);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(232, 43);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Filter";
			// 
			// checkBoxEmployees
			// 
			this.checkBoxEmployees.BackColor = System.Drawing.SystemColors.Control;
			this.checkBoxEmployees.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxEmployees.Checked = true;
			this.checkBoxEmployees.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxEmployees.Location = new System.Drawing.Point(145, 18);
			this.checkBoxEmployees.Name = "checkBoxEmployees";
			this.checkBoxEmployees.Size = new System.Drawing.Size(77, 20);
			this.checkBoxEmployees.TabIndex = 4;
			this.checkBoxEmployees.Text = "Employees";
			this.checkBoxEmployees.UseVisualStyleBackColor = false;
			this.checkBoxEmployees.CheckedChanged += new System.EventHandler(this.checkBoxEmployees_CheckedChanged);
			// 
			// checkBoxProviders
			// 
			this.checkBoxProviders.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxProviders.Checked = true;
			this.checkBoxProviders.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxProviders.Location = new System.Drawing.Point(69, 18);
			this.checkBoxProviders.Name = "checkBoxProviders";
			this.checkBoxProviders.Size = new System.Drawing.Size(70, 20);
			this.checkBoxProviders.TabIndex = 3;
			this.checkBoxProviders.Text = "Providers";
			this.checkBoxProviders.UseVisualStyleBackColor = true;
			this.checkBoxProviders.CheckedChanged += new System.EventHandler(this.checkBoxProviders_CheckedChanged);
			// 
			// checkBoxNotes
			// 
			this.checkBoxNotes.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkBoxNotes.Checked = true;
			this.checkBoxNotes.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkBoxNotes.Location = new System.Drawing.Point(9, 18);
			this.checkBoxNotes.Name = "checkBoxNotes";
			this.checkBoxNotes.Size = new System.Drawing.Size(54, 20);
			this.checkBoxNotes.TabIndex = 2;
			this.checkBoxNotes.Text = "Notes";
			this.checkBoxNotes.UseVisualStyleBackColor = true;
			this.checkBoxNotes.CheckedChanged += new System.EventHandler(this.checkBoxNotes_CheckedChanged);
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
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Sort";
			// 
			// radioStopTime
			// 
			this.radioStopTime.AutoSize = true;
			this.radioStopTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStopTime.Checked = true;
			this.radioStopTime.Location = new System.Drawing.Point(95, 20);
			this.radioStopTime.Name = "radioStopTime";
			this.radioStopTime.Size = new System.Drawing.Size(73, 17);
			this.radioStopTime.TabIndex = 2;
			this.radioStopTime.TabStop = true;
			this.radioStopTime.Text = "Stop Time";
			this.radioStopTime.UseVisualStyleBackColor = true;
			this.radioStopTime.CheckedChanged += new System.EventHandler(this.radioStopTime_CheckedChanged);
			// 
			// radioStartTime
			// 
			this.radioStartTime.AutoSize = true;
			this.radioStartTime.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioStartTime.Location = new System.Drawing.Point(5, 20);
			this.radioStartTime.Name = "radioStartTime";
			this.radioStartTime.Size = new System.Drawing.Size(73, 17);
			this.radioStartTime.TabIndex = 1;
			this.radioStartTime.Text = "Start Time";
			this.radioStartTime.UseVisualStyleBackColor = true;
			this.radioStartTime.CheckedChanged += new System.EventHandler(this.radioStartTime_CheckedChanged);
			// 
			// radioName
			// 
			this.radioName.AutoSize = true;
			this.radioName.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.radioName.Location = new System.Drawing.Point(185, 20);
			this.radioName.Name = "radioName";
			this.radioName.Size = new System.Drawing.Size(53, 17);
			this.radioName.TabIndex = 0;
			this.radioName.Text = "Name";
			this.radioName.UseVisualStyleBackColor = true;
			this.radioName.CheckedChanged += new System.EventHandler(this.radioName_CheckedChanged);
			// 
			// splitContainerBottom
			// 
			this.splitContainerBottom.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitContainerBottom.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
			this.splitContainerBottom.IsSplitterFixed = true;
			this.splitContainerBottom.Location = new System.Drawing.Point(0, 0);
			this.splitContainerBottom.Name = "splitContainerBottom";
			this.splitContainerBottom.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitContainerBottom.Panel1
			// 
			this.splitContainerBottom.Panel1.AutoScroll = true;
			this.splitContainerBottom.Panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel1_Paint);
			// 
			// splitContainerBottom.Panel2
			// 
			this.splitContainerBottom.Panel2.Paint += new System.Windows.Forms.PaintEventHandler(this.splitContainer_Panel2_Paint);
			this.splitContainerBottom.Size = new System.Drawing.Size(738, 406);
			this.splitContainerBottom.SplitterDistance = 377;
			this.splitContainerBottom.SplitterWidth = 1;
			this.splitContainerBottom.TabIndex = 0;
			// 
			// GraphScheduleDay
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainerMaster);
			this.DoubleBuffered = true;
			this.Name = "GraphScheduleDay";
			this.Size = new System.Drawing.Size(738, 462);
			this.SizeChanged += new System.EventHandler(this.GraphScheduleDay_SizeChanged);
			this.splitContainerMaster.Panel1.ResumeLayout(false);
			this.splitContainerMaster.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitContainerMaster)).EndInit();
			this.splitContainerMaster.ResumeLayout(false);
			this.groupBox3.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.numEndHour)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.numStartHour)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitContainerBottom)).EndInit();
			this.splitContainerBottom.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private SplitContainerNoFlicker splitContainerBottom;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioStopTime;
		private System.Windows.Forms.RadioButton radioStartTime;
		private System.Windows.Forms.RadioButton radioName;
		private SplitContainerNoFlicker splitContainerMaster;
		private System.Windows.Forms.CheckBox checkBoxEmployees;
		private System.Windows.Forms.CheckBox checkBoxProviders;
		private System.Windows.Forms.CheckBox checkBoxNotes;
		private System.Windows.Forms.Timer timerRefresh;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.NumericUpDown numStartHour;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.NumericUpDown numEndHour;
		private System.Windows.Forms.Label label3;
	}
}
