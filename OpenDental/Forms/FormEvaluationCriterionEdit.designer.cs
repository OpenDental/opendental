namespace OpenDental{
	partial class FormEvaluationCriterionEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEvaluationCriterionEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.label7 = new System.Windows.Forms.Label();
			this.textCriterionDescript = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textGradingScale = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textGradeShowingPercent = new System.Windows.Forms.TextBox();
			this.comboGradeShowing = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textGradeNumber = new System.Windows.Forms.TextBox();
			this.labelApptNote = new System.Windows.Forms.Label();
			this.textNote = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(315, 169);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 4;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(315, 199);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 5;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(12, 15);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(122, 17);
			this.label7.TabIndex = 150;
			this.label7.Text = "Description";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCriterionDescript
			// 
			this.textCriterionDescript.Location = new System.Drawing.Point(135, 14);
			this.textCriterionDescript.MaxLength = 255;
			this.textCriterionDescript.Name = "textCriterionDescript";
			this.textCriterionDescript.ReadOnly = true;
			this.textCriterionDescript.Size = new System.Drawing.Size(121, 20);
			this.textCriterionDescript.TabIndex = 149;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 41);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(122, 17);
			this.label5.TabIndex = 148;
			this.label5.Text = "Grading Scale";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradingScale
			// 
			this.textGradingScale.Location = new System.Drawing.Point(135, 40);
			this.textGradingScale.MaxLength = 255;
			this.textGradingScale.Name = "textGradingScale";
			this.textGradingScale.ReadOnly = true;
			this.textGradingScale.Size = new System.Drawing.Size(121, 20);
			this.textGradingScale.TabIndex = 145;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 94);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(122, 17);
			this.label6.TabIndex = 147;
			this.label6.Text = "Grade";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeShowingPercent
			// 
			this.textGradeShowingPercent.Location = new System.Drawing.Point(135, 93);
			this.textGradeShowingPercent.MaxLength = 255;
			this.textGradeShowingPercent.Name = "textGradeShowingPercent";
			this.textGradeShowingPercent.Size = new System.Drawing.Size(121, 20);
			this.textGradeShowingPercent.TabIndex = 2;
			// 
			// comboGradeShowing
			// 
			this.comboGradeShowing.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboGradeShowing.FormattingEnabled = true;
			this.comboGradeShowing.ItemHeight = 13;
			this.comboGradeShowing.Location = new System.Drawing.Point(135, 93);
			this.comboGradeShowing.Name = "comboGradeShowing";
			this.comboGradeShowing.Size = new System.Drawing.Size(133, 21);
			this.comboGradeShowing.TabIndex = 2;
			this.comboGradeShowing.SelectionChangeCommitted += new System.EventHandler(this.comboGradeNumber_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(15, 66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(118, 18);
			this.label1.TabIndex = 151;
			this.label1.Text = "Grade Number";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textGradeNumber
			// 
			this.textGradeNumber.Location = new System.Drawing.Point(135, 66);
			this.textGradeNumber.MaxLength = 255;
			this.textGradeNumber.Name = "textGradeNumber";
			this.textGradeNumber.Size = new System.Drawing.Size(121, 20);
			this.textGradeNumber.TabIndex = 1;
			this.textGradeNumber.Tag = "";
			// 
			// labelApptNote
			// 
			this.labelApptNote.Location = new System.Drawing.Point(7, 120);
			this.labelApptNote.Name = "labelApptNote";
			this.labelApptNote.Size = new System.Drawing.Size(126, 16);
			this.labelApptNote.TabIndex = 154;
			this.labelApptNote.Text = "Note";
			this.labelApptNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNote
			// 
			this.textNote.Location = new System.Drawing.Point(135, 119);
			this.textNote.MaxLength = 255;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.Size = new System.Drawing.Size(174, 105);
			this.textNote.TabIndex = 155;
			// 
			// FormEvaluationCriterionEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(402, 235);
			this.Controls.Add(this.textGradeShowingPercent);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.labelApptNote);
			this.Controls.Add(this.textGradeNumber);
			this.Controls.Add(this.comboGradeShowing);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.textCriterionDescript);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textGradingScale);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEvaluationCriterionEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Evaluation Criterion Edit";
			this.Load += new System.EventHandler(this.FormEvaluationCriterionEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.TextBox textCriterionDescript;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textGradingScale;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textGradeShowingPercent;
		private System.Windows.Forms.ComboBox comboGradeShowing;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textGradeNumber;
		private System.Windows.Forms.Label labelApptNote;
		private System.Windows.Forms.TextBox textNote;
	}
}