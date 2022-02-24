namespace OpenDental{
	partial class FormDentalSchoolSetup {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDentalSchoolSetup));
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textStudents = new System.Windows.Forms.TextBox();
			this.textInstructors = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butStudentPicker = new OpenDental.UI.Button();
			this.butInstructorPicker = new OpenDental.UI.Button();
			this.butGradingScales = new OpenDental.UI.Button();
			this.butEvaluation = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 65);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(85, 15);
			this.label1.TabIndex = 104;
			this.label1.Text = "Students";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 91);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(85, 15);
			this.label2.TabIndex = 107;
			this.label2.Text = "Instructors";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textStudents
			// 
			this.textStudents.Location = new System.Drawing.Point(103, 63);
			this.textStudents.MaxLength = 255;
			this.textStudents.Name = "textStudents";
			this.textStudents.ReadOnly = true;
			this.textStudents.Size = new System.Drawing.Size(121, 20);
			this.textStudents.TabIndex = 1;
			// 
			// textInstructors
			// 
			this.textInstructors.Location = new System.Drawing.Point(103, 89);
			this.textInstructors.MaxLength = 255;
			this.textInstructors.Name = "textInstructors";
			this.textInstructors.ReadOnly = true;
			this.textInstructors.Size = new System.Drawing.Size(121, 20);
			this.textInstructors.TabIndex = 3;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 18);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(299, 40);
			this.label3.TabIndex = 112;
			this.label3.Text = "Selecting a new user group gives you the opportunity to update all current studen" +
    "ts or instructors to the new group.";
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Controls.Add(this.butStudentPicker);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label3);
			this.groupBox1.Controls.Add(this.butInstructorPicker);
			this.groupBox1.Controls.Add(this.textInstructors);
			this.groupBox1.Controls.Add(this.textStudents);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(311, 129);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Default User Groups";
			// 
			// butStudentPicker
			// 
			this.butStudentPicker.Location = new System.Drawing.Point(230, 61);
			this.butStudentPicker.Name = "butStudentPicker";
			this.butStudentPicker.Size = new System.Drawing.Size(22, 22);
			this.butStudentPicker.TabIndex = 2;
			this.butStudentPicker.Text = "...";
			this.butStudentPicker.Click += new System.EventHandler(this.butStudentPicker_Click);
			// 
			// butInstructorPicker
			// 
			this.butInstructorPicker.Location = new System.Drawing.Point(230, 87);
			this.butInstructorPicker.Name = "butInstructorPicker";
			this.butInstructorPicker.Size = new System.Drawing.Size(22, 22);
			this.butInstructorPicker.TabIndex = 4;
			this.butInstructorPicker.Text = "...";
			this.butInstructorPicker.Click += new System.EventHandler(this.butInstructorPicker_Click);
			// 
			// butGradingScales
			// 
			this.butGradingScales.Location = new System.Drawing.Point(131, 147);
			this.butGradingScales.Name = "butGradingScales";
			this.butGradingScales.Size = new System.Drawing.Size(105, 24);
			this.butGradingScales.TabIndex = 3;
			this.butGradingScales.Text = "Grading Scales";
			this.butGradingScales.Click += new System.EventHandler(this.butGradingScales_Click);
			// 
			// butEvaluation
			// 
			this.butEvaluation.Location = new System.Drawing.Point(131, 177);
			this.butEvaluation.Name = "butEvaluation";
			this.butEvaluation.Size = new System.Drawing.Size(105, 24);
			this.butEvaluation.TabIndex = 4;
			this.butEvaluation.Text = "Evaluations";
			this.butEvaluation.Click += new System.EventHandler(this.butEvaluation_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(322, 177);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "Close";
			this.butCancel.Click += new System.EventHandler(this.butClose_Click);
			// 
			// FormDentalSchoolSetup
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(409, 213);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butGradingScales);
			this.Controls.Add(this.butEvaluation);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDentalSchoolSetup";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Dental School Setup";
			this.Load += new System.EventHandler(this.FormDentalSchoolSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label1;
		private UI.Button butStudentPicker;
		private System.Windows.Forms.Label label2;
		private UI.Button butInstructorPicker;
		private System.Windows.Forms.TextBox textStudents;
		private System.Windows.Forms.TextBox textInstructors;
		private System.Windows.Forms.Label label3;
		private UI.Button butEvaluation;
		private UI.Button butGradingScales;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}