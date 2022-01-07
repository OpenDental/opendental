namespace OpenDental {
	partial class FormEduResourceEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEduResourceEdit));
			this.butProblemSelect = new System.Windows.Forms.Button();
			this.textProblem = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.textMedication = new System.Windows.Forms.TextBox();
			this.butMedicationSelect = new System.Windows.Forms.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.textLabResultsID = new System.Windows.Forms.TextBox();
			this.butCancel = new System.Windows.Forms.Button();
			this.butOk = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.label4 = new System.Windows.Forms.Label();
			this.textUrl = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textLabTestName = new System.Windows.Forms.TextBox();
			this.groupLabResults = new System.Windows.Forms.GroupBox();
			this.label6 = new System.Windows.Forms.Label();
			this.labelCriterionValue = new System.Windows.Forms.Label();
			this.textCompareValue = new System.Windows.Forms.TextBox();
			this.label7 = new System.Windows.Forms.Label();
			this.textICD9 = new System.Windows.Forms.TextBox();
			this.textSnomed = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.groupProblems = new System.Windows.Forms.GroupBox();
			this.label9 = new System.Windows.Forms.Label();
			this.textTobaccoAssessment = new System.Windows.Forms.TextBox();
			this.butTobaccoCodeSelect = new System.Windows.Forms.Button();
			this.groupLabResults.SuspendLayout();
			this.groupProblems.SuspendLayout();
			this.SuspendLayout();
			// 
			// butProblemSelect
			// 
			this.butProblemSelect.Location = new System.Drawing.Point(558, 18);
			this.butProblemSelect.Name = "butProblemSelect";
			this.butProblemSelect.Size = new System.Drawing.Size(29, 23);
			this.butProblemSelect.TabIndex = 1;
			this.butProblemSelect.Text = "...";
			this.butProblemSelect.UseVisualStyleBackColor = true;
			this.butProblemSelect.Click += new System.EventHandler(this.butProblemSelect_Click);
			// 
			// textProblem
			// 
			this.textProblem.Location = new System.Drawing.Point(160, 20);
			this.textProblem.Name = "textProblem";
			this.textProblem.ReadOnly = true;
			this.textProblem.Size = new System.Drawing.Size(392, 20);
			this.textProblem.TabIndex = 0;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(148, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Problem ";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(15, 119);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(154, 17);
			this.label2.TabIndex = 5;
			this.label2.Text = "Medication";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textMedication
			// 
			this.textMedication.Location = new System.Drawing.Point(175, 118);
			this.textMedication.Name = "textMedication";
			this.textMedication.ReadOnly = true;
			this.textMedication.Size = new System.Drawing.Size(392, 20);
			this.textMedication.TabIndex = 4;
			// 
			// butMedicationSelect
			// 
			this.butMedicationSelect.Location = new System.Drawing.Point(573, 116);
			this.butMedicationSelect.Name = "butMedicationSelect";
			this.butMedicationSelect.Size = new System.Drawing.Size(29, 23);
			this.butMedicationSelect.TabIndex = 5;
			this.butMedicationSelect.Text = "...";
			this.butMedicationSelect.UseVisualStyleBackColor = true;
			this.butMedicationSelect.Click += new System.EventHandler(this.butMedicationSelect_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(15, 17);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(100, 17);
			this.label3.TabIndex = 8;
			this.label3.Text = "Test Id";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLabResultsID
			// 
			this.textLabResultsID.Location = new System.Drawing.Point(116, 16);
			this.textLabResultsID.Name = "textLabResultsID";
			this.textLabResultsID.Size = new System.Drawing.Size(111, 20);
			this.textLabResultsID.TabIndex = 0;
			this.textLabResultsID.TabStop = false;
			this.textLabResultsID.Click += new System.EventHandler(this.textLabResults_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(536, 348);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 8;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(455, 348);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 23);
			this.butOk.TabIndex = 7;
			this.butOk.Text = "Ok";
			this.butOk.UseVisualStyleBackColor = true;
			this.butOk.Click += new System.EventHandler(this.butOk_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 348);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 9;
			this.butDelete.Text = "Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(15, 289);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(593, 17);
			this.label4.TabIndex = 13;
			this.label4.Text = "Resource URL.  Must be a full URL. Example: http://webmd.com/diabetesoverview";
			this.label4.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textUrl
			// 
			this.textUrl.Location = new System.Drawing.Point(15, 309);
			this.textUrl.Name = "textUrl";
			this.textUrl.Size = new System.Drawing.Size(593, 20);
			this.textUrl.TabIndex = 6;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(15, 43);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(100, 17);
			this.label5.TabIndex = 15;
			this.label5.Text = "Test Name";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textLabTestName
			// 
			this.textLabTestName.Location = new System.Drawing.Point(116, 42);
			this.textLabTestName.Name = "textLabTestName";
			this.textLabTestName.Size = new System.Drawing.Size(357, 20);
			this.textLabTestName.TabIndex = 1;
			this.textLabTestName.TabStop = false;
			this.textLabTestName.Click += new System.EventHandler(this.textLabResults_Click);
			// 
			// groupLabResults
			// 
			this.groupLabResults.Controls.Add(this.label6);
			this.groupLabResults.Controls.Add(this.labelCriterionValue);
			this.groupLabResults.Controls.Add(this.textCompareValue);
			this.groupLabResults.Controls.Add(this.textLabResultsID);
			this.groupLabResults.Controls.Add(this.label5);
			this.groupLabResults.Controls.Add(this.label3);
			this.groupLabResults.Controls.Add(this.textLabTestName);
			this.groupLabResults.Location = new System.Drawing.Point(15, 172);
			this.groupLabResults.Name = "groupLabResults";
			this.groupLabResults.Size = new System.Drawing.Size(593, 100);
			this.groupLabResults.TabIndex = 10;
			this.groupLabResults.TabStop = false;
			this.groupLabResults.Text = "Lab Results";
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(218, 69);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(107, 17);
			this.label6.TabIndex = 18;
			this.label6.Text = "For example, >120";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCriterionValue
			// 
			this.labelCriterionValue.Location = new System.Drawing.Point(7, 69);
			this.labelCriterionValue.Name = "labelCriterionValue";
			this.labelCriterionValue.Size = new System.Drawing.Size(107, 17);
			this.labelCriterionValue.TabIndex = 17;
			this.labelCriterionValue.Text = "Compare Value";
			this.labelCriterionValue.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCompareValue
			// 
			this.textCompareValue.Location = new System.Drawing.Point(116, 68);
			this.textCompareValue.Name = "textCompareValue";
			this.textCompareValue.Size = new System.Drawing.Size(100, 20);
			this.textCompareValue.TabIndex = 2;
			this.textCompareValue.TabStop = false;
			this.textCompareValue.Click += new System.EventHandler(this.textLabResults_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 47);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(148, 17);
			this.label7.TabIndex = 19;
			this.label7.Text = "ICD9";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textICD9
			// 
			this.textICD9.Location = new System.Drawing.Point(160, 46);
			this.textICD9.Name = "textICD9";
			this.textICD9.ReadOnly = true;
			this.textICD9.Size = new System.Drawing.Size(392, 20);
			this.textICD9.TabIndex = 2;
			// 
			// textSnomed
			// 
			this.textSnomed.Location = new System.Drawing.Point(160, 72);
			this.textSnomed.Name = "textSnomed";
			this.textSnomed.ReadOnly = true;
			this.textSnomed.Size = new System.Drawing.Size(392, 20);
			this.textSnomed.TabIndex = 2;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 73);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(148, 17);
			this.label8.TabIndex = 19;
			this.label8.Text = "SNOMED CT";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupProblems
			// 
			this.groupProblems.Controls.Add(this.textProblem);
			this.groupProblems.Controls.Add(this.label8);
			this.groupProblems.Controls.Add(this.butProblemSelect);
			this.groupProblems.Controls.Add(this.label7);
			this.groupProblems.Controls.Add(this.label1);
			this.groupProblems.Controls.Add(this.textSnomed);
			this.groupProblems.Controls.Add(this.textICD9);
			this.groupProblems.Location = new System.Drawing.Point(15, 11);
			this.groupProblems.Name = "groupProblems";
			this.groupProblems.Size = new System.Drawing.Size(593, 100);
			this.groupProblems.TabIndex = 20;
			this.groupProblems.TabStop = false;
			this.groupProblems.Text = "Problems";
			// 
			// label9
			// 
			this.label9.Location = new System.Drawing.Point(15, 146);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(154, 17);
			this.label9.TabIndex = 22;
			this.label9.Text = "Tobacco Use Assessment";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTobaccoAssessment
			// 
			this.textTobaccoAssessment.Location = new System.Drawing.Point(175, 145);
			this.textTobaccoAssessment.Name = "textTobaccoAssessment";
			this.textTobaccoAssessment.ReadOnly = true;
			this.textTobaccoAssessment.Size = new System.Drawing.Size(392, 20);
			this.textTobaccoAssessment.TabIndex = 21;
			// 
			// butTobaccoCodeSelect
			// 
			this.butTobaccoCodeSelect.Location = new System.Drawing.Point(573, 143);
			this.butTobaccoCodeSelect.Name = "butTobaccoCodeSelect";
			this.butTobaccoCodeSelect.Size = new System.Drawing.Size(29, 23);
			this.butTobaccoCodeSelect.TabIndex = 23;
			this.butTobaccoCodeSelect.Text = "...";
			this.butTobaccoCodeSelect.UseVisualStyleBackColor = true;
			this.butTobaccoCodeSelect.Click += new System.EventHandler(this.butTobaccoCodeSelect_Click);
			// 
			// FormEduResourceEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(623, 383);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textTobaccoAssessment);
			this.Controls.Add(this.butTobaccoCodeSelect);
			this.Controls.Add(this.groupProblems);
			this.Controls.Add(this.groupLabResults);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textUrl);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textMedication);
			this.Controls.Add(this.butMedicationSelect);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEduResourceEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Edit Education Resource";
			this.Load += new System.EventHandler(this.FormEduResourceEdit_Load);
			this.groupLabResults.ResumeLayout(false);
			this.groupLabResults.PerformLayout();
			this.groupProblems.ResumeLayout(false);
			this.groupProblems.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butProblemSelect;
		private System.Windows.Forms.TextBox textProblem;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textMedication;
		private System.Windows.Forms.Button butMedicationSelect;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textLabResultsID;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butOk;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox textUrl;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textLabTestName;
		private System.Windows.Forms.GroupBox groupLabResults;
		private System.Windows.Forms.Label labelCriterionValue;
		private System.Windows.Forms.TextBox textCompareValue;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textICD9;
		private System.Windows.Forms.TextBox textSnomed;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.GroupBox groupProblems;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textTobaccoAssessment;
		private System.Windows.Forms.Button butTobaccoCodeSelect;
	}
}