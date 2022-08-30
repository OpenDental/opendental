namespace OpenDental{
	partial class FormEvaluationReport {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEvaluationReport));
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.checkAllCourses = new System.Windows.Forms.CheckBox();
			this.checkAllInstructors = new System.Windows.Forms.CheckBox();
			this.textDateStart = new ValidDate();
			this.textDateEnd = new ValidDate();
			this.gridCourses = new OpenDental.UI.GridOD();
			this.gridInstructors = new OpenDental.UI.GridOD();
			this.gridStudents = new OpenDental.UI.GridOD();
			this.butAllStudents = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.butClose = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label7.Location = new System.Drawing.Point(244, 20);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(83, 18);
			this.label7.TabIndex = 43;
			this.label7.Text = "Date Start:";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.label6.Location = new System.Drawing.Point(478, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(99, 18);
			this.label6.TabIndex = 42;
			this.label6.Text = "Date End:";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkAllCourses
			// 
			this.checkAllCourses.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllCourses.Location = new System.Drawing.Point(114, 49);
			this.checkAllCourses.Name = "checkAllCourses";
			this.checkAllCourses.Size = new System.Drawing.Size(134, 16);
			this.checkAllCourses.TabIndex = 55;
			this.checkAllCourses.Text = "All Courses";
			this.checkAllCourses.CheckedChanged += new System.EventHandler(this.checkAllCourses_CheckedChanged);
			// 
			// checkAllInstructors
			// 
			this.checkAllInstructors.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkAllInstructors.Location = new System.Drawing.Point(434, 49);
			this.checkAllInstructors.Name = "checkAllInstructors";
			this.checkAllInstructors.Size = new System.Drawing.Size(141, 16);
			this.checkAllInstructors.TabIndex = 56;
			this.checkAllInstructors.Text = "All Instructors";
			this.checkAllInstructors.CheckedChanged += new System.EventHandler(this.checkAllInstructors_CheckedChanged);
			// 
			// textDateStart
			// 
			this.textDateStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateStart.Location = new System.Drawing.Point(333, 20);
			this.textDateStart.Name = "textDateStart";
			this.textDateStart.Size = new System.Drawing.Size(98, 20);
			this.textDateStart.TabIndex = 40;
			// 
			// textDateEnd
			// 
			this.textDateEnd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.textDateEnd.Location = new System.Drawing.Point(583, 20);
			this.textDateEnd.Name = "textDateEnd";
			this.textDateEnd.Size = new System.Drawing.Size(98, 20);
			this.textDateEnd.TabIndex = 41;
			// 
			// gridCourses
			// 
			this.gridCourses.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridCourses.Location = new System.Drawing.Point(12, 68);
			this.gridCourses.Name = "gridCourses";
			this.gridCourses.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridCourses.Size = new System.Drawing.Size(308, 362);
			this.gridCourses.TabIndex = 16;
			this.gridCourses.Title = "Courses";
			this.gridCourses.TranslationName = "TableCourses";
			this.gridCourses.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCourses_CellClick);
			// 
			// gridInstructors
			// 
			this.gridInstructors.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridInstructors.Location = new System.Drawing.Point(333, 68);
			this.gridInstructors.Name = "gridInstructors";
			this.gridInstructors.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridInstructors.Size = new System.Drawing.Size(308, 362);
			this.gridInstructors.TabIndex = 15;
			this.gridInstructors.Title = "Instructors";
			this.gridInstructors.TranslationName = "TableInstructors";
			this.gridInstructors.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInstructors_CellClick);
			// 
			// gridStudents
			// 
			this.gridStudents.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridStudents.Location = new System.Drawing.Point(654, 68);
			this.gridStudents.Name = "gridStudents";
			this.gridStudents.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridStudents.Size = new System.Drawing.Size(308, 362);
			this.gridStudents.TabIndex = 14;
			this.gridStudents.Title = "Students";
			this.gridStudents.TranslationName = "TableStudents";
			// 
			// butAllStudents
			// 
			this.butAllStudents.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butAllStudents.Location = new System.Drawing.Point(769, 41);
			this.butAllStudents.Name = "butAllStudents";
			this.butAllStudents.Size = new System.Drawing.Size(78, 24);
			this.butAllStudents.TabIndex = 62;
			this.butAllStudents.Text = "All Students";
			this.butAllStudents.Click += new System.EventHandler(this.butAllStudents_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(887, 452);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 19;
			this.butOK.Text = "OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(887, 482);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 24);
			this.butClose.TabIndex = 2;
			this.butClose.Text = "Close";
			this.butClose.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEvaluationReport
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(974, 518);
			this.Controls.Add(this.butAllStudents);
			this.Controls.Add(this.checkAllInstructors);
			this.Controls.Add(this.checkAllCourses);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDateStart);
			this.Controls.Add(this.textDateEnd);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.gridCourses);
			this.Controls.Add(this.gridInstructors);
			this.Controls.Add(this.gridStudents);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEvaluationReport";
			this.Text = "Evaluation Report";
			this.Load += new System.EventHandler(this.FormEvaluationReport_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butClose;
		private UI.GridOD gridStudents;
		private UI.GridOD gridInstructors;
		private UI.GridOD gridCourses;
		private UI.Button butOK;
		private System.Windows.Forms.Label label7;
		private ValidDate textDateStart;
		private ValidDate textDateEnd;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.CheckBox checkAllCourses;
		private System.Windows.Forms.CheckBox checkAllInstructors;
		private UI.Button butAllStudents;
	}
}