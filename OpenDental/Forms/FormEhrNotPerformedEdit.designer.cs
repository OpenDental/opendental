namespace OpenDental{
	partial class FormEhrNotPerformedEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrNotPerformedEdit));
			this.textDate = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.groupItem = new System.Windows.Forms.GroupBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.labelBPsExamCode = new System.Windows.Forms.Label();
			this.textCodeSystem = new System.Windows.Forms.TextBox();
			this.labelBMIExamCode = new System.Windows.Forms.Label();
			this.textCode = new System.Windows.Forms.TextBox();
			this.groupReason = new System.Windows.Forms.GroupBox();
			this.radioMedReason = new System.Windows.Forms.RadioButton();
			this.label1 = new System.Windows.Forms.Label();
			this.radioSysReason = new System.Windows.Forms.RadioButton();
			this.label2 = new System.Windows.Forms.Label();
			this.radioPatReason = new System.Windows.Forms.RadioButton();
			this.label4 = new System.Windows.Forms.Label();
			this.comboCodeReason = new System.Windows.Forms.ComboBox();
			this.textCodeSystemReason = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.butOK = new System.Windows.Forms.Button();
			this.butCancel = new System.Windows.Forms.Button();
			this.butDelete = new System.Windows.Forms.Button();
			this.textDescriptionReason = new System.Windows.Forms.TextBox();
			this.textNote = new System.Windows.Forms.TextBox();
			this.groupItem.SuspendLayout();
			this.groupReason.SuspendLayout();
			this.SuspendLayout();
			// 
			// textDate
			// 
			this.textDate.Location = new System.Drawing.Point(110, 18);
			this.textDate.Name = "textDate";
			this.textDate.Size = new System.Drawing.Size(80, 20);
			this.textDate.TabIndex = 143;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(12, 20);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(95, 17);
			this.label5.TabIndex = 144;
			this.label5.Text = "Date";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupItem
			// 
			this.groupItem.Controls.Add(this.textDescription);
			this.groupItem.Controls.Add(this.label3);
			this.groupItem.Controls.Add(this.labelBPsExamCode);
			this.groupItem.Controls.Add(this.textCodeSystem);
			this.groupItem.Controls.Add(this.labelBMIExamCode);
			this.groupItem.Controls.Add(this.textCode);
			this.groupItem.Location = new System.Drawing.Point(12, 42);
			this.groupItem.Name = "groupItem";
			this.groupItem.Size = new System.Drawing.Size(353, 125);
			this.groupItem.TabIndex = 145;
			this.groupItem.TabStop = false;
			this.groupItem.Text = "Item Not Performed";
			// 
			// textDescription
			// 
			this.textDescription.AcceptsTab = true;
			this.textDescription.Location = new System.Drawing.Point(98, 65);
			this.textDescription.MaxLength = 2147483647;
			this.textDescription.Multiline = true;
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textDescription.Size = new System.Drawing.Size(245, 51);
			this.textDescription.TabIndex = 141;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(12, 68);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(83, 17);
			this.label3.TabIndex = 142;
			this.label3.Text = "Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBPsExamCode
			// 
			this.labelBPsExamCode.Location = new System.Drawing.Point(12, 44);
			this.labelBPsExamCode.Name = "labelBPsExamCode";
			this.labelBPsExamCode.Size = new System.Drawing.Size(83, 17);
			this.labelBPsExamCode.TabIndex = 140;
			this.labelBPsExamCode.Text = "Code System";
			this.labelBPsExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCodeSystem
			// 
			this.textCodeSystem.Location = new System.Drawing.Point(98, 41);
			this.textCodeSystem.Name = "textCodeSystem";
			this.textCodeSystem.ReadOnly = true;
			this.textCodeSystem.Size = new System.Drawing.Size(100, 20);
			this.textCodeSystem.TabIndex = 139;
			// 
			// labelBMIExamCode
			// 
			this.labelBMIExamCode.Location = new System.Drawing.Point(12, 20);
			this.labelBMIExamCode.Name = "labelBMIExamCode";
			this.labelBMIExamCode.Size = new System.Drawing.Size(83, 17);
			this.labelBMIExamCode.TabIndex = 138;
			this.labelBMIExamCode.Text = "Code";
			this.labelBMIExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textCode
			// 
			this.textCode.Location = new System.Drawing.Point(98, 17);
			this.textCode.Name = "textCode";
			this.textCode.ReadOnly = true;
			this.textCode.Size = new System.Drawing.Size(100, 20);
			this.textCode.TabIndex = 137;
			// 
			// groupReason
			// 
			this.groupReason.Controls.Add(this.textDescriptionReason);
			this.groupReason.Controls.Add(this.radioMedReason);
			this.groupReason.Controls.Add(this.label1);
			this.groupReason.Controls.Add(this.radioSysReason);
			this.groupReason.Controls.Add(this.label2);
			this.groupReason.Controls.Add(this.radioPatReason);
			this.groupReason.Controls.Add(this.label4);
			this.groupReason.Controls.Add(this.comboCodeReason);
			this.groupReason.Controls.Add(this.textCodeSystemReason);
			this.groupReason.Location = new System.Drawing.Point(12, 173);
			this.groupReason.Name = "groupReason";
			this.groupReason.Size = new System.Drawing.Size(353, 125);
			this.groupReason.TabIndex = 146;
			this.groupReason.TabStop = false;
			this.groupReason.Text = "Reason Not Performed";
			// 
			// radioMedReason
			// 
			this.radioMedReason.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioMedReason.Location = new System.Drawing.Point(19, 16);
			this.radioMedReason.Name = "radioMedReason";
			this.radioMedReason.Size = new System.Drawing.Size(100, 16);
			this.radioMedReason.TabIndex = 156;
			this.radioMedReason.Text = "Medical Reason";
			this.radioMedReason.Visible = false;
			this.radioMedReason.Click += new System.EventHandler(this.radioReasonMed_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 84);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(83, 17);
			this.label1.TabIndex = 151;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioSysReason
			// 
			this.radioSysReason.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioSysReason.Location = new System.Drawing.Point(231, 16);
			this.radioSysReason.Name = "radioSysReason";
			this.radioSysReason.Size = new System.Drawing.Size(100, 16);
			this.radioSysReason.TabIndex = 158;
			this.radioSysReason.Text = "System Reason";
			this.radioSysReason.Visible = false;
			this.radioSysReason.Click += new System.EventHandler(this.radioReasonSys_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 62);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(83, 17);
			this.label2.TabIndex = 150;
			this.label2.Text = "Code System";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// radioPatReason
			// 
			this.radioPatReason.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.radioPatReason.Location = new System.Drawing.Point(125, 16);
			this.radioPatReason.Name = "radioPatReason";
			this.radioPatReason.Size = new System.Drawing.Size(100, 16);
			this.radioPatReason.TabIndex = 157;
			this.radioPatReason.Text = "Patient Reason";
			this.radioPatReason.Visible = false;
			this.radioPatReason.Click += new System.EventHandler(this.radioReasonPat_Click);
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 37);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(83, 17);
			this.label4.TabIndex = 149;
			this.label4.Text = "Code";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboCodeReason
			// 
			this.comboCodeReason.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboCodeReason.DropDownWidth = 385;
			this.comboCodeReason.Location = new System.Drawing.Point(98, 35);
			this.comboCodeReason.MaxDropDownItems = 30;
			this.comboCodeReason.Name = "comboCodeReason";
			this.comboCodeReason.Size = new System.Drawing.Size(100, 21);
			this.comboCodeReason.TabIndex = 148;
			this.comboCodeReason.DropDown += new System.EventHandler(this.comboCodeReason_DropDown);
			this.comboCodeReason.SelectionChangeCommitted += new System.EventHandler(this.comboReasonCode_SelectionChangeCommitted);
			this.comboCodeReason.DropDownClosed += new System.EventHandler(this.comboCodeReason_DropDownClosed);
			// 
			// textCodeSystemReason
			// 
			this.textCodeSystemReason.Location = new System.Drawing.Point(98, 60);
			this.textCodeSystemReason.Name = "textCodeSystemReason";
			this.textCodeSystemReason.ReadOnly = true;
			this.textCodeSystemReason.Size = new System.Drawing.Size(100, 20);
			this.textCodeSystemReason.TabIndex = 146;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(12, 307);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(95, 17);
			this.label6.TabIndex = 152;
			this.label6.Text = "Note";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(209, 379);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 23);
			this.butOK.TabIndex = 153;
			this.butOK.Text = "&OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(290, 379);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 23);
			this.butCancel.TabIndex = 154;
			this.butCancel.Text = "&Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Location = new System.Drawing.Point(12, 379);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 23);
			this.butDelete.TabIndex = 155;
			this.butDelete.TabStop = false;
			this.butDelete.Text = "&Delete";
			this.butDelete.UseVisualStyleBackColor = true;
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// textDescriptionReason
			// 
			this.textDescriptionReason.AcceptsTab = true;
			this.textDescriptionReason.Location = new System.Drawing.Point(98, 82);
			this.textDescriptionReason.MaxLength = 2147483647;
			this.textDescriptionReason.Multiline = true;
			this.textDescriptionReason.Name = "textDescriptionReason";
			this.textDescriptionReason.ReadOnly = true;
			this.textDescriptionReason.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textDescriptionReason.Size = new System.Drawing.Size(245, 34);
			this.textDescriptionReason.TabIndex = 147;
			// 
			// textNote
			// 
			this.textNote.AcceptsTab = true;
			this.textNote.Location = new System.Drawing.Point(110, 305);
			this.textNote.MaxLength = 2147483647;
			this.textNote.Multiline = true;
			this.textNote.Name = "textNote";
			this.textNote.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textNote.Size = new System.Drawing.Size(245, 60);
			this.textNote.TabIndex = 148;
			// 
			// FormEhrNotPerformedEdit
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(377, 414);
			this.Controls.Add(this.textNote);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.groupReason);
			this.Controls.Add(this.groupItem);
			this.Controls.Add(this.textDate);
			this.Controls.Add(this.label5);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrNotPerformedEdit";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Not Performed Item with Reason";
			this.Load += new System.EventHandler(this.FormEhrNotPerformedEdit_Load);
			this.groupItem.ResumeLayout(false);
			this.groupItem.PerformLayout();
			this.groupReason.ResumeLayout(false);
			this.groupReason.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textDate;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.GroupBox groupItem;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label labelBPsExamCode;
		private System.Windows.Forms.TextBox textCodeSystem;
		private System.Windows.Forms.Label labelBMIExamCode;
		private System.Windows.Forms.TextBox textCode;
		private System.Windows.Forms.GroupBox groupReason;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboCodeReason;
		private System.Windows.Forms.TextBox textCodeSystemReason;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Button butOK;
		private System.Windows.Forms.Button butCancel;
		private System.Windows.Forms.Button butDelete;
		private System.Windows.Forms.RadioButton radioMedReason;
		private System.Windows.Forms.RadioButton radioSysReason;
		private System.Windows.Forms.RadioButton radioPatReason;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.TextBox textDescriptionReason;
		private System.Windows.Forms.TextBox textNote;
	}
}