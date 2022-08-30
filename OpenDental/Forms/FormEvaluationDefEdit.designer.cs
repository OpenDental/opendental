namespace OpenDental{
	partial class FormEvaluationDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEvaluationDefEdit));
			this.textGradeScaleName = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textTitle = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textCourse = new System.Windows.Forms.TextBox();
			this.butCoursePicker = new OpenDental.UI.Button();
			this.butDelete = new OpenDental.UI.Button();
			this.butCriterionAdd = new OpenDental.UI.Button();
			this.butGradingScale = new OpenDental.UI.Button();
			this.butDown = new OpenDental.UI.Button();
			this.butUp = new OpenDental.UI.Button();
			this.gridMain = new OpenDental.UI.GridOD();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.labelTotalPoint = new System.Windows.Forms.Label();
			this.textTotalPoints = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// textGradeScaleName
			// 
			this.textGradeScaleName.Location = new System.Drawing.Point(130, 38);
			this.textGradeScaleName.MaxLength = 255;
			this.textGradeScaleName.Name = "textGradeScaleName";
			this.textGradeScaleName.ReadOnly = true;
			this.textGradeScaleName.Size = new System.Drawing.Size(145, 20);
			this.textGradeScaleName.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(16, 39);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(112, 17);
			this.label1.TabIndex = 114;
			this.label1.Text = "Grading Scale";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(16, 13);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(112, 17);
			this.label2.TabIndex = 125;
			this.label2.Text = "Title";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTitle
			// 
			this.textTitle.Location = new System.Drawing.Point(130, 12);
			this.textTitle.MaxLength = 255;
			this.textTitle.Name = "textTitle";
			this.textTitle.Size = new System.Drawing.Size(145, 20);
			this.textTitle.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(16, 65);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(112, 17);
			this.label3.TabIndex = 128;
			this.label3.Text = "Course";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCourse
			// 
			this.textCourse.Location = new System.Drawing.Point(130, 64);
			this.textCourse.MaxLength = 255;
			this.textCourse.Name = "textCourse";
			this.textCourse.ReadOnly = true;
			this.textCourse.Size = new System.Drawing.Size(145, 20);
			this.textCourse.TabIndex = 4;
			// 
			// butCoursePicker
			// 
			this.butCoursePicker.Location = new System.Drawing.Point(280, 62);
			this.butCoursePicker.Name = "butCoursePicker";
			this.butCoursePicker.Size = new System.Drawing.Size(24, 24);
			this.butCoursePicker.TabIndex = 5;
			this.butCoursePicker.Text = "...";
			this.butCoursePicker.Click += new System.EventHandler(this.butCoursePicker_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 411);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 8;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCriterionAdd
			// 
			this.butCriterionAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butCriterionAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butCriterionAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butCriterionAdd.Location = new System.Drawing.Point(365, 93);
			this.butCriterionAdd.Name = "butCriterionAdd";
			this.butCriterionAdd.Size = new System.Drawing.Size(75, 24);
			this.butCriterionAdd.TabIndex = 5;
			this.butCriterionAdd.Text = "Add";
			this.butCriterionAdd.Click += new System.EventHandler(this.butCriterionAdd_Click);
			// 
			// butGradingScale
			// 
			this.butGradingScale.Location = new System.Drawing.Point(280, 36);
			this.butGradingScale.Name = "butGradingScale";
			this.butGradingScale.Size = new System.Drawing.Size(24, 24);
			this.butGradingScale.TabIndex = 3;
			this.butGradingScale.Text = "...";
			this.butGradingScale.Click += new System.EventHandler(this.butGradingScale_Click);
			// 
			// butDown
			// 
			this.butDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butDown.Image = global::OpenDental.Properties.Resources.down;
			this.butDown.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDown.Location = new System.Drawing.Point(365, 239);
			this.butDown.Name = "butDown";
			this.butDown.Size = new System.Drawing.Size(75, 24);
			this.butDown.TabIndex = 7;
			this.butDown.Text = "&Down";
			this.butDown.Click += new System.EventHandler(this.butDown_Click);
			// 
			// butUp
			// 
			this.butUp.AdjustImageLocation = new System.Drawing.Point(0, 1);
			this.butUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.butUp.Image = global::OpenDental.Properties.Resources.up;
			this.butUp.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butUp.Location = new System.Drawing.Point(365, 209);
			this.butUp.Name = "butUp";
			this.butUp.Size = new System.Drawing.Size(75, 24);
			this.butUp.TabIndex = 6;
			this.butUp.Text = "&Up";
			this.butUp.Click += new System.EventHandler(this.butUp_Click);
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 93);
			this.gridMain.Name = "gridMain";
			this.gridMain.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridMain.Size = new System.Drawing.Size(347, 288);
			this.gridMain.TabIndex = 60;
			this.gridMain.Title = "Criteria Used";
			this.gridMain.TranslationName = "FormEvaluationDefEdit";
			this.gridMain.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMain_CellDoubleClick);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(365, 381);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 10;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(365, 411);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 11;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// labelTotalPoint
			// 
			this.labelTotalPoint.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.labelTotalPoint.Location = new System.Drawing.Point(162, 388);
			this.labelTotalPoint.Name = "labelTotalPoint";
			this.labelTotalPoint.Size = new System.Drawing.Size(112, 17);
			this.labelTotalPoint.TabIndex = 130;
			this.labelTotalPoint.Text = "Total Points";
			this.labelTotalPoint.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTotalPoints
			// 
			this.textTotalPoints.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.textTotalPoints.Location = new System.Drawing.Point(276, 387);
			this.textTotalPoints.MaxLength = 255;
			this.textTotalPoints.Name = "textTotalPoints";
			this.textTotalPoints.ReadOnly = true;
			this.textTotalPoints.Size = new System.Drawing.Size(64, 20);
			this.textTotalPoints.TabIndex = 129;
			// 
			// FormEvaluationDefEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(452, 447);
			this.Controls.Add(this.labelTotalPoint);
			this.Controls.Add(this.textTotalPoints);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.textCourse);
			this.Controls.Add(this.butCoursePicker);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textTitle);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butCriterionAdd);
			this.Controls.Add(this.textGradeScaleName);
			this.Controls.Add(this.butGradingScale);
			this.Controls.Add(this.butDown);
			this.Controls.Add(this.butUp);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEvaluationDefEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Evaluation Definition Edit";
			this.Load += new System.EventHandler(this.FormEvaluationDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private UI.Button butDown;
		private UI.Button butUp;
		private UI.GridOD gridMain;
		private UI.Button butGradingScale;
		private System.Windows.Forms.TextBox textGradeScaleName;
		private UI.Button butCriterionAdd;
		private System.Windows.Forms.Label label1;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textTitle;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textCourse;
		private UI.Button butCoursePicker;
		private System.Windows.Forms.Label labelTotalPoint;
		private System.Windows.Forms.TextBox textTotalPoints;
	}
}