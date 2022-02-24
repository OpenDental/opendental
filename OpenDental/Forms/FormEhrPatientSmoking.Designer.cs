namespace OpenDental {
	partial class FormEhrPatientSmoking {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrPatientSmoking));
			this.label2 = new System.Windows.Forms.Label();
			this.comboSmokeStatus = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.comboAssessmentType = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.textDateAssessed = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.comboTobaccoStatus = new System.Windows.Forms.ComboBox();
			this.labelTobaccoStatus = new System.Windows.Forms.Label();
			this.groupTobaccoUse = new System.Windows.Forms.GroupBox();
			this.groupIntervention = new System.Windows.Forms.GroupBox();
			this.radioRecentInterventions = new System.Windows.Forms.RadioButton();
			this.checkPatientDeclined = new System.Windows.Forms.CheckBox();
			this.radioAllInterventions = new System.Windows.Forms.RadioButton();
			this.gridInterventions = new OpenDental.UI.GridOD();
			this.radioMedInterventions = new System.Windows.Forms.RadioButton();
			this.butAddIntervention = new OpenDental.UI.Button();
			this.radioCounselInterventions = new System.Windows.Forms.RadioButton();
			this.label7 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.textDateIntervention = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboInterventionCode = new System.Windows.Forms.ComboBox();
			this.groupAssessment = new System.Windows.Forms.GroupBox();
			this.radioRecentStatuses = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.gridAssessments = new OpenDental.UI.GridOD();
			this.radioAllStatuses = new System.Windows.Forms.RadioButton();
			this.radioNonUserStatuses = new System.Windows.Forms.RadioButton();
			this.radioUserStatuses = new System.Windows.Forms.RadioButton();
			this.butAddAssessment = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOK = new OpenDental.UI.Button();
			this.groupTobaccoUse.SuspendLayout();
			this.groupIntervention.SuspendLayout();
			this.groupAssessment.SuspendLayout();
			this.SuspendLayout();
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 26);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(135, 16);
			this.label2.TabIndex = 0;
			this.label2.Text = "Current Smoking Status";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboSmokeStatus
			// 
			this.comboSmokeStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboSmokeStatus.FormattingEnabled = true;
			this.comboSmokeStatus.Location = new System.Drawing.Point(153, 25);
			this.comboSmokeStatus.MaxDropDownItems = 30;
			this.comboSmokeStatus.Name = "comboSmokeStatus";
			this.comboSmokeStatus.Size = new System.Drawing.Size(225, 21);
			this.comboSmokeStatus.TabIndex = 1;
			this.comboSmokeStatus.SelectionChangeCommitted += new System.EventHandler(this.comboSmokeStatus_SelectionChangeCommitted);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(384, 26);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(383, 16);
			this.label1.TabIndex = 0;
			this.label1.Text = "Used for calculating MU measures.";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboAssessmentType
			// 
			this.comboAssessmentType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboAssessmentType.DropDownWidth = 350;
			this.comboAssessmentType.FormattingEnabled = true;
			this.comboAssessmentType.Location = new System.Drawing.Point(100, 50);
			this.comboAssessmentType.MaxDropDownItems = 30;
			this.comboAssessmentType.Name = "comboAssessmentType";
			this.comboAssessmentType.Size = new System.Drawing.Size(260, 21);
			this.comboAssessmentType.TabIndex = 2;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 51);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(93, 16);
			this.label5.TabIndex = 0;
			this.label5.Text = "Assessment Type";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateAssessed
			// 
			this.textDateAssessed.Location = new System.Drawing.Point(100, 24);
			this.textDateAssessed.Name = "textDateAssessed";
			this.textDateAssessed.ReadOnly = true;
			this.textDateAssessed.Size = new System.Drawing.Size(140, 20);
			this.textDateAssessed.TabIndex = 1;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(6, 25);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(93, 16);
			this.label6.TabIndex = 0;
			this.label6.Text = "Date Assessed";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTobaccoStatus
			// 
			this.comboTobaccoStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboTobaccoStatus.DropDownWidth = 325;
			this.comboTobaccoStatus.FormattingEnabled = true;
			this.comboTobaccoStatus.Location = new System.Drawing.Point(100, 99);
			this.comboTobaccoStatus.MaxDropDownItems = 30;
			this.comboTobaccoStatus.Name = "comboTobaccoStatus";
			this.comboTobaccoStatus.Size = new System.Drawing.Size(260, 21);
			this.comboTobaccoStatus.TabIndex = 7;
			this.comboTobaccoStatus.SelectionChangeCommitted += new System.EventHandler(this.comboTobaccoStatus_SelectionChangeCommitted);
			// 
			// labelTobaccoStatus
			// 
			this.labelTobaccoStatus.Location = new System.Drawing.Point(6, 100);
			this.labelTobaccoStatus.Name = "labelTobaccoStatus";
			this.labelTobaccoStatus.Size = new System.Drawing.Size(93, 16);
			this.labelTobaccoStatus.TabIndex = 0;
			this.labelTobaccoStatus.Text = "Tobacco Status";
			this.labelTobaccoStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupTobaccoUse
			// 
			this.groupTobaccoUse.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupTobaccoUse.Controls.Add(this.groupIntervention);
			this.groupTobaccoUse.Controls.Add(this.groupAssessment);
			this.groupTobaccoUse.Location = new System.Drawing.Point(12, 66);
			this.groupTobaccoUse.Name = "groupTobaccoUse";
			this.groupTobaccoUse.Size = new System.Drawing.Size(943, 416);
			this.groupTobaccoUse.TabIndex = 2;
			this.groupTobaccoUse.TabStop = false;
			this.groupTobaccoUse.Text = "Tobacco Use Screening and Cessation Intervention (CQM)";
			// 
			// groupIntervention
			// 
			this.groupIntervention.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupIntervention.Controls.Add(this.radioRecentInterventions);
			this.groupIntervention.Controls.Add(this.checkPatientDeclined);
			this.groupIntervention.Controls.Add(this.radioAllInterventions);
			this.groupIntervention.Controls.Add(this.gridInterventions);
			this.groupIntervention.Controls.Add(this.radioMedInterventions);
			this.groupIntervention.Controls.Add(this.butAddIntervention);
			this.groupIntervention.Controls.Add(this.radioCounselInterventions);
			this.groupIntervention.Controls.Add(this.label7);
			this.groupIntervention.Controls.Add(this.label4);
			this.groupIntervention.Controls.Add(this.textDateIntervention);
			this.groupIntervention.Controls.Add(this.label3);
			this.groupIntervention.Controls.Add(this.comboInterventionCode);
			this.groupIntervention.Location = new System.Drawing.Point(6, 220);
			this.groupIntervention.Name = "groupIntervention";
			this.groupIntervention.Size = new System.Drawing.Size(931, 190);
			this.groupIntervention.TabIndex = 2;
			this.groupIntervention.TabStop = false;
			this.groupIntervention.Text = "Cessation Intervention";
			// 
			// radioRecentInterventions
			// 
			this.radioRecentInterventions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioRecentInterventions.Location = new System.Drawing.Point(293, 50);
			this.radioRecentInterventions.Name = "radioRecentInterventions";
			this.radioRecentInterventions.Size = new System.Drawing.Size(67, 16);
			this.radioRecentInterventions.TabIndex = 5;
			this.radioRecentInterventions.TabStop = true;
			this.radioRecentInterventions.Text = "Frequent";
			this.radioRecentInterventions.CheckedChanged += new System.EventHandler(this.radioInterventions_CheckedChanged);
			// 
			// checkPatientDeclined
			// 
			this.checkPatientDeclined.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkPatientDeclined.Location = new System.Drawing.Point(100, 99);
			this.checkPatientDeclined.Name = "checkPatientDeclined";
			this.checkPatientDeclined.Size = new System.Drawing.Size(154, 18);
			this.checkPatientDeclined.TabIndex = 7;
			this.checkPatientDeclined.Text = "Patient Declined";
			this.checkPatientDeclined.UseVisualStyleBackColor = true;
			// 
			// radioAllInterventions
			// 
			this.radioAllInterventions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAllInterventions.Location = new System.Drawing.Point(100, 50);
			this.radioAllInterventions.Name = "radioAllInterventions";
			this.radioAllInterventions.Size = new System.Drawing.Size(47, 16);
			this.radioAllInterventions.TabIndex = 2;
			this.radioAllInterventions.TabStop = true;
			this.radioAllInterventions.Text = "All";
			this.radioAllInterventions.CheckedChanged += new System.EventHandler(this.radioInterventions_CheckedChanged);
			// 
			// gridInterventions
			// 
			this.gridInterventions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridInterventions.Location = new System.Drawing.Point(366, 24);
			this.gridInterventions.MinimumSize = new System.Drawing.Size(559, 160);
			this.gridInterventions.Name = "gridInterventions";
			this.gridInterventions.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridInterventions.Size = new System.Drawing.Size(559, 160);
			this.gridInterventions.TabIndex = 9;
			this.gridInterventions.Title = "Intervention History";
			this.gridInterventions.TranslationName = "TableIntervention";
			this.gridInterventions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInterventions_CellDoubleClick);
			// 
			// radioMedInterventions
			// 
			this.radioMedInterventions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioMedInterventions.Location = new System.Drawing.Point(153, 50);
			this.radioMedInterventions.Name = "radioMedInterventions";
			this.radioMedInterventions.Size = new System.Drawing.Size(55, 16);
			this.radioMedInterventions.TabIndex = 3;
			this.radioMedInterventions.TabStop = true;
			this.radioMedInterventions.Text = "Med";
			this.radioMedInterventions.CheckedChanged += new System.EventHandler(this.radioInterventions_CheckedChanged);
			// 
			// butAddIntervention
			// 
			this.butAddIntervention.Location = new System.Drawing.Point(100, 123);
			this.butAddIntervention.Name = "butAddIntervention";
			this.butAddIntervention.Size = new System.Drawing.Size(100, 23);
			this.butAddIntervention.TabIndex = 8;
			this.butAddIntervention.Text = "Add Intervention";
			this.butAddIntervention.Click += new System.EventHandler(this.butIntervention_Click);
			// 
			// radioCounselInterventions
			// 
			this.radioCounselInterventions.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioCounselInterventions.Location = new System.Drawing.Point(213, 50);
			this.radioCounselInterventions.Name = "radioCounselInterventions";
			this.radioCounselInterventions.Size = new System.Drawing.Size(73, 16);
			this.radioCounselInterventions.TabIndex = 4;
			this.radioCounselInterventions.TabStop = true;
			this.radioCounselInterventions.Text = "Counsel";
			this.radioCounselInterventions.CheckedChanged += new System.EventHandler(this.radioInterventions_CheckedChanged);
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(6, 50);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(93, 16);
			this.label7.TabIndex = 0;
			this.label7.Text = "Filter Codes By";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(6, 25);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(93, 16);
			this.label4.TabIndex = 0;
			this.label4.Text = "Date Intervened";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateIntervention
			// 
			this.textDateIntervention.Location = new System.Drawing.Point(100, 24);
			this.textDateIntervention.Name = "textDateIntervention";
			this.textDateIntervention.ReadOnly = true;
			this.textDateIntervention.Size = new System.Drawing.Size(140, 20);
			this.textDateIntervention.TabIndex = 1;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 73);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(93, 16);
			this.label3.TabIndex = 0;
			this.label3.Text = "Intervention Code";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboInterventionCode
			// 
			this.comboInterventionCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboInterventionCode.DropDownWidth = 340;
			this.comboInterventionCode.FormattingEnabled = true;
			this.comboInterventionCode.Location = new System.Drawing.Point(100, 72);
			this.comboInterventionCode.MaxDropDownItems = 30;
			this.comboInterventionCode.Name = "comboInterventionCode";
			this.comboInterventionCode.Size = new System.Drawing.Size(260, 21);
			this.comboInterventionCode.TabIndex = 6;
			this.comboInterventionCode.SelectionChangeCommitted += new System.EventHandler(this.comboInterventionCode_SelectionChangeCommitted);
			// 
			// groupAssessment
			// 
			this.groupAssessment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupAssessment.Controls.Add(this.radioRecentStatuses);
			this.groupAssessment.Controls.Add(this.label8);
			this.groupAssessment.Controls.Add(this.gridAssessments);
			this.groupAssessment.Controls.Add(this.radioAllStatuses);
			this.groupAssessment.Controls.Add(this.comboAssessmentType);
			this.groupAssessment.Controls.Add(this.radioNonUserStatuses);
			this.groupAssessment.Controls.Add(this.label5);
			this.groupAssessment.Controls.Add(this.radioUserStatuses);
			this.groupAssessment.Controls.Add(this.label6);
			this.groupAssessment.Controls.Add(this.textDateAssessed);
			this.groupAssessment.Controls.Add(this.butAddAssessment);
			this.groupAssessment.Controls.Add(this.labelTobaccoStatus);
			this.groupAssessment.Controls.Add(this.comboTobaccoStatus);
			this.groupAssessment.Location = new System.Drawing.Point(6, 20);
			this.groupAssessment.Name = "groupAssessment";
			this.groupAssessment.Size = new System.Drawing.Size(931, 190);
			this.groupAssessment.TabIndex = 1;
			this.groupAssessment.TabStop = false;
			this.groupAssessment.Text = "Tobacco Use Assessment";
			// 
			// radioRecentStatuses
			// 
			this.radioRecentStatuses.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioRecentStatuses.Location = new System.Drawing.Point(293, 78);
			this.radioRecentStatuses.Name = "radioRecentStatuses";
			this.radioRecentStatuses.Size = new System.Drawing.Size(67, 16);
			this.radioRecentStatuses.TabIndex = 6;
			this.radioRecentStatuses.TabStop = true;
			this.radioRecentStatuses.Text = "Frequent";
			this.radioRecentStatuses.CheckedChanged += new System.EventHandler(this.radioTobaccoStatuses_CheckedChanged);
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(6, 77);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(93, 16);
			this.label8.TabIndex = 0;
			this.label8.Text = "Filter Statuses By";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// gridAssessments
			// 
			this.gridAssessments.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridAssessments.Location = new System.Drawing.Point(366, 24);
			this.gridAssessments.MinimumSize = new System.Drawing.Size(559, 160);
			this.gridAssessments.Name = "gridAssessments";
			this.gridAssessments.SelectionMode = OpenDental.UI.GridSelectionMode.MultiExtended;
			this.gridAssessments.Size = new System.Drawing.Size(559, 160);
			this.gridAssessments.TabIndex = 9;
			this.gridAssessments.Title = "Assessment History";
			this.gridAssessments.TranslationName = "TableAssessment";
			this.gridAssessments.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridAssessments_CellDoubleClick);
			// 
			// radioAllStatuses
			// 
			this.radioAllStatuses.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioAllStatuses.Location = new System.Drawing.Point(100, 77);
			this.radioAllStatuses.Name = "radioAllStatuses";
			this.radioAllStatuses.Size = new System.Drawing.Size(47, 16);
			this.radioAllStatuses.TabIndex = 3;
			this.radioAllStatuses.TabStop = true;
			this.radioAllStatuses.Text = "All";
			this.radioAllStatuses.CheckedChanged += new System.EventHandler(this.radioTobaccoStatuses_CheckedChanged);
			// 
			// radioNonUserStatuses
			// 
			this.radioNonUserStatuses.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioNonUserStatuses.Location = new System.Drawing.Point(213, 77);
			this.radioNonUserStatuses.Name = "radioNonUserStatuses";
			this.radioNonUserStatuses.Size = new System.Drawing.Size(73, 16);
			this.radioNonUserStatuses.TabIndex = 5;
			this.radioNonUserStatuses.TabStop = true;
			this.radioNonUserStatuses.Text = "Non-user";
			this.radioNonUserStatuses.CheckedChanged += new System.EventHandler(this.radioTobaccoStatuses_CheckedChanged);
			// 
			// radioUserStatuses
			// 
			this.radioUserStatuses.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioUserStatuses.Location = new System.Drawing.Point(153, 77);
			this.radioUserStatuses.Name = "radioUserStatuses";
			this.radioUserStatuses.Size = new System.Drawing.Size(55, 16);
			this.radioUserStatuses.TabIndex = 4;
			this.radioUserStatuses.TabStop = true;
			this.radioUserStatuses.Text = "User";
			this.radioUserStatuses.CheckedChanged += new System.EventHandler(this.radioTobaccoStatuses_CheckedChanged);
			// 
			// butAddAssessment
			// 
			this.butAddAssessment.Location = new System.Drawing.Point(100, 126);
			this.butAddAssessment.Name = "butAddAssessment";
			this.butAddAssessment.Size = new System.Drawing.Size(100, 23);
			this.butAddAssessment.TabIndex = 8;
			this.butAddAssessment.Text = "Add Assessment";
			this.butAddAssessment.Click += new System.EventHandler(this.butAssessed_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.butCancel.Location = new System.Drawing.Point(879, 488);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(76, 24);
			this.butCancel.TabIndex = 4;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(797, 488);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(76, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormEhrPatientSmoking
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(967, 524);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.groupTobaccoUse);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.comboSmokeStatus);
			this.Controls.Add(this.label2);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrPatientSmoking";
			this.Text = "Tobacco Use";
			this.Load += new System.EventHandler(this.FormPatientSmoking_Load);
			this.groupTobaccoUse.ResumeLayout(false);
			this.groupIntervention.ResumeLayout(false);
			this.groupIntervention.PerformLayout();
			this.groupAssessment.ResumeLayout(false);
			this.groupAssessment.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboSmokeStatus;
		private OpenDental.UI.GridOD gridAssessments;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.ComboBox comboAssessmentType;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textDateAssessed;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.ComboBox comboTobaccoStatus;
		private System.Windows.Forms.Label labelTobaccoStatus;
		private System.Windows.Forms.GroupBox groupTobaccoUse;
		private UI.GridOD gridInterventions;
		private UI.Button butAddAssessment;
		private UI.Button butAddIntervention;
		private System.Windows.Forms.ComboBox comboInterventionCode;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textDateIntervention;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label7;
		private UI.Button butCancel;
		private UI.Button butOK;
		private System.Windows.Forms.RadioButton radioAllStatuses;
		private System.Windows.Forms.RadioButton radioNonUserStatuses;
		private System.Windows.Forms.RadioButton radioUserStatuses;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.RadioButton radioAllInterventions;
		private System.Windows.Forms.RadioButton radioMedInterventions;
		private System.Windows.Forms.RadioButton radioCounselInterventions;
		private System.Windows.Forms.GroupBox groupIntervention;
		private System.Windows.Forms.GroupBox groupAssessment;
		private System.Windows.Forms.CheckBox checkPatientDeclined;
		private System.Windows.Forms.RadioButton radioRecentInterventions;
		private System.Windows.Forms.RadioButton radioRecentStatuses;
	}
}