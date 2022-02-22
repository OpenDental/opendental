namespace OpenDental{
	partial class FormEvaluationCriterionDefEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEvaluationCriterionDefEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.checkIsCategoryName = new System.Windows.Forms.CheckBox();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textGradeScaleName = new System.Windows.Forms.TextBox();
			this.butGradingScale = new OpenDental.UI.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butDelete = new OpenDental.UI.Button();
			this.labelPoints = new System.Windows.Forms.Label();
			this.textPoints = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(233, 138);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 6;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(314, 138);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 7;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// checkIsCategoryName
			// 
			this.checkIsCategoryName.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkIsCategoryName.Location = new System.Drawing.Point(153, 103);
			this.checkIsCategoryName.Name = "checkIsCategoryName";
			this.checkIsCategoryName.Size = new System.Drawing.Size(190, 17);
			this.checkIsCategoryName.TabIndex = 4;
			this.checkIsCategoryName.Text = "This will not show a grade";
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(153, 25);
			this.textDescript.MaxLength = 255;
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(155, 20);
			this.textDescript.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(36, 52);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(116, 17);
			this.label1.TabIndex = 119;
			this.label1.Text = "Grading Scale";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeScaleName
			// 
			this.textGradeScaleName.Location = new System.Drawing.Point(153, 51);
			this.textGradeScaleName.MaxLength = 255;
			this.textGradeScaleName.Name = "textGradeScaleName";
			this.textGradeScaleName.ReadOnly = true;
			this.textGradeScaleName.Size = new System.Drawing.Size(155, 20);
			this.textGradeScaleName.TabIndex = 2;
			// 
			// butGradingScale
			// 
			this.butGradingScale.Location = new System.Drawing.Point(314, 48);
			this.butGradingScale.Name = "butGradingScale";
			this.butGradingScale.Size = new System.Drawing.Size(24, 24);
			this.butGradingScale.TabIndex = 3;
			this.butGradingScale.Text = "...";
			this.butGradingScale.Click += new System.EventHandler(this.butGradingScale_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(36, 25);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(116, 17);
			this.label2.TabIndex = 120;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(39, 102);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(113, 17);
			this.label3.TabIndex = 121;
			this.label3.Text = "Is Category Name";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(12, 138);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 5;
			this.butDelete.Text = "&Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// labelPoints
			// 
			this.labelPoints.Location = new System.Drawing.Point(36, 77);
			this.labelPoints.Name = "labelPoints";
			this.labelPoints.Size = new System.Drawing.Size(116, 17);
			this.labelPoints.TabIndex = 123;
			this.labelPoints.Text = "Points";
			this.labelPoints.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelPoints.Visible = false;
			// 
			// textPoints
			// 
			this.textPoints.Location = new System.Drawing.Point(153, 77);
			this.textPoints.MaxLength = 255;
			this.textPoints.Name = "textPoints";
			this.textPoints.Size = new System.Drawing.Size(54, 20);
			this.textPoints.TabIndex = 122;
			this.textPoints.Visible = false;
			// 
			// FormEvaluationCriterionDefEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(401, 174);
			this.Controls.Add(this.labelPoints);
			this.Controls.Add(this.textPoints);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textGradeScaleName);
			this.Controls.Add(this.butGradingScale);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.checkIsCategoryName);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEvaluationCriterionDefEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Evaluation Criterion Def Edit";
			this.Load += new System.EventHandler(this.FormEvaluationCriterionDefEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.CheckBox checkIsCategoryName;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textGradeScaleName;
		private UI.Button butGradingScale;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private UI.Button butDelete;
		private System.Windows.Forms.Label labelPoints;
		private System.Windows.Forms.TextBox textPoints;
	}
}