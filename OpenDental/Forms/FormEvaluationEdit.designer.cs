namespace OpenDental{
	partial class FormEvaluationEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEvaluationEdit));
			this.label3 = new System.Windows.Forms.Label();
			this.textCourse = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textGradeScaleName = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.textInstructor = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textStudent = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textGradeShowingOverride = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.textGradeNumberOverride = new System.Windows.Forms.TextBox();
			this.textGradeNumber = new System.Windows.Forms.TextBox();
			this.textGradeShowing = new System.Windows.Forms.TextBox();
			this.gridGrades = new OpenDental.UI.GridOD();
			this.textDate = new OpenDental.ValidDate();
			this.butStudentPicker = new OpenDental.UI.Button();
			this.gridCriterion = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(158, 62);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(104, 17);
			this.label3.TabIndex = 134;
			this.label3.Text = "Course";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCourse
			// 
			this.textCourse.Location = new System.Drawing.Point(263, 61);
			this.textCourse.MaxLength = 255;
			this.textCourse.Name = "textCourse";
			this.textCourse.ReadOnly = true;
			this.textCourse.Size = new System.Drawing.Size(121, 20);
			this.textCourse.TabIndex = 131;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(395, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(137, 17);
			this.label2.TabIndex = 133;
			this.label2.Text = "Title";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTitle
			// 
			this.textTitle.Location = new System.Drawing.Point(533, 61);
			this.textTitle.MaxLength = 255;
			this.textTitle.Name = "textTitle";
			this.textTitle.ReadOnly = true;
			this.textTitle.Size = new System.Drawing.Size(121, 20);
			this.textTitle.TabIndex = 129;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(158, 36);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(104, 17);
			this.label1.TabIndex = 132;
			this.label1.Text = "Scale";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeScaleName
			// 
			this.textGradeScaleName.Location = new System.Drawing.Point(263, 35);
			this.textGradeScaleName.MaxLength = 255;
			this.textGradeScaleName.Name = "textGradeScaleName";
			this.textGradeScaleName.ReadOnly = true;
			this.textGradeScaleName.Size = new System.Drawing.Size(121, 20);
			this.textGradeScaleName.TabIndex = 130;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(158, 9);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(104, 17);
			this.label4.TabIndex = 140;
			this.label4.Text = "Date";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(390, 36);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(137, 17);
			this.label5.TabIndex = 139;
			this.label5.Text = "Instructor";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInstructor
			// 
			this.textInstructor.Location = new System.Drawing.Point(533, 35);
			this.textInstructor.MaxLength = 255;
			this.textInstructor.Name = "textInstructor";
			this.textInstructor.ReadOnly = true;
			this.textInstructor.Size = new System.Drawing.Size(121, 20);
			this.textInstructor.TabIndex = 135;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(390, 10);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(137, 17);
			this.label6.TabIndex = 138;
			this.label6.Text = "Student";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStudent
			// 
			this.textStudent.Location = new System.Drawing.Point(533, 9);
			this.textStudent.MaxLength = 255;
			this.textStudent.Name = "textStudent";
			this.textStudent.ReadOnly = true;
			this.textStudent.Size = new System.Drawing.Size(121, 20);
			this.textStudent.TabIndex = 136;
			// 
			// label7
			// 
			this.label7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label7.Location = new System.Drawing.Point(125, 462);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(137, 17);
			this.label7.TabIndex = 142;
			this.label7.Text = "Overall Grade";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeShowingOverride
			// 
			this.textGradeShowingOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textGradeShowingOverride.Location = new System.Drawing.Point(324, 461);
			this.textGradeShowingOverride.MaxLength = 255;
			this.textGradeShowingOverride.Name = "textGradeShowingOverride";
			this.textGradeShowingOverride.Size = new System.Drawing.Size(60, 20);
			this.textGradeShowingOverride.TabIndex = 4;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(125, 436);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(137, 17);
			this.label8.TabIndex = 147;
			this.label8.Text = "Overall Grade Number";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeNumberOverride
			// 
			this.textGradeNumberOverride.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textGradeNumberOverride.Location = new System.Drawing.Point(324, 435);
			this.textGradeNumberOverride.MaxLength = 255;
			this.textGradeNumberOverride.Name = "textGradeNumberOverride";
			this.textGradeNumberOverride.Size = new System.Drawing.Size(60, 20);
			this.textGradeNumberOverride.TabIndex = 3;
			// 
			// textGradeNumber
			// 
			this.textGradeNumber.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textGradeNumber.Location = new System.Drawing.Point(263, 435);
			this.textGradeNumber.MaxLength = 255;
			this.textGradeNumber.Name = "textGradeNumber";
			this.textGradeNumber.ReadOnly = true;
			this.textGradeNumber.Size = new System.Drawing.Size(60, 20);
			this.textGradeNumber.TabIndex = 149;
			// 
			// textGradeShowing
			// 
			this.textGradeShowing.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textGradeShowing.Location = new System.Drawing.Point(263, 461);
			this.textGradeShowing.MaxLength = 255;
			this.textGradeShowing.Name = "textGradeShowing";
			this.textGradeShowing.ReadOnly = true;
			this.textGradeShowing.Size = new System.Drawing.Size(60, 20);
			this.textGradeShowing.TabIndex = 148;
			// 
			// gridGrades
			// 
			this.gridGrades.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridGrades.Location = new System.Drawing.Point(511, 87);
			this.gridGrades.Name = "gridGrades";
			this.gridGrades.Size = new System.Drawing.Size(314, 342);
			this.gridGrades.TabIndex = 150;
			this.gridGrades.Title = "Grading Scale";
			this.gridGrades.TranslationName = "FormEvaluationDefEdit";
			this.gridGrades.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridGrades_CellClick);
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(263, 9);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(121, 20);
			this.textDate.TabIndex = 1;
			// 
			// butStudentPicker
			// 
			this.butStudentPicker.Location = new System.Drawing.Point(660, 6);
			this.butStudentPicker.Name = "butStudentPicker";
			this.butStudentPicker.Size = new System.Drawing.Size(24, 24);
			this.butStudentPicker.TabIndex = 2;
			this.butStudentPicker.Text = "...";
			this.butStudentPicker.Click += new System.EventHandler(this.butStudentPicker_Click);
			// 
			// gridMain
			// 
			this.gridCriterion.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridCriterion.Location = new System.Drawing.Point(18, 87);
			this.gridCriterion.Name = "gridMain";
			this.gridCriterion.SelectionMode = OpenDental.UI.GridSelectionMode.OneCell;
			this.gridCriterion.Size = new System.Drawing.Size(487, 342);
			this.gridCriterion.TabIndex = 143;
			this.gridCriterion.Title = "Criterion";
			this.gridCriterion.TranslationName = "FormEvaluationDefEdit";
			this.gridCriterion.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridCriterion_CellClick);
			this.gridCriterion.CellLeave += new OpenDental.UI.ODGridClickEventHandler(this.gridCriterion_CellLeave);
			this.gridCriterion.CellEnter += new OpenDental.UI.ODGridClickEventHandler(this.gridCriterion_CellEnter);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(671, 458);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 5;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(752, 458);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 6;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(18, 458);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 151;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// FormEvaluationEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(842, 493);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.gridGrades);
			this.Controls.Add(this.textGradeNumber);
			this.Controls.Add(this.textGradeShowing);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textGradeNumberOverride);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.butStudentPicker);
			this.Controls.Add(this.gridCriterion);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textGradeShowingOverride);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textInstructor);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textStudent);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCourse);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTitle);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textGradeScaleName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEvaluationEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Evaluation Edit";
			this.Load += new System.EventHandler(this.FormEvaluationEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textCourse;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textGradeScaleName;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textInstructor;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textStudent;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textGradeShowingOverride;
		private UI.GridOD gridCriterion;
		private UI.Button butStudentPicker;
		private ValidDate textDate;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.TextBox textGradeNumberOverride;
		private System.Windows.Forms.TextBox textGradeNumber;
		private System.Windows.Forms.TextBox textGradeShowing;
		private UI.GridOD gridGrades;
		private UI.Button butDelete;
	}
}