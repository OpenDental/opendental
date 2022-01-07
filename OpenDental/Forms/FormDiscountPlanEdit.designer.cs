namespace OpenDental{
	partial class FormDiscountPlanEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDiscountPlanEdit));
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.textDescript = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.textFeeSched = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.butFeeSched = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxAdjType = new System.Windows.Forms.ComboBox();
			this.checkHidden = new System.Windows.Forms.CheckBox();
			this.butListPatients = new OpenDental.UI.Button();
			this.textNumPatients = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboPatient = new System.Windows.Forms.ComboBox();
			this.textPlanNote = new OpenDental.ODtextBox();
			this.labelPlanNote = new System.Windows.Forms.Label();
			this.textPlanNum = new System.Windows.Forms.TextBox();
			this.labelPlanNum = new System.Windows.Forms.Label();
			this.groupBoxOD4 = new OpenDental.UI.GroupBoxOD();
			this.textDiscountPAFreq = new OpenDental.ValidNum();
			this.textDiscountXrayFreq = new OpenDental.ValidNum();
			this.textDiscountLimitedFreq = new OpenDental.ValidNum();
			this.textDiscountPerioFreq = new OpenDental.ValidNum();
			this.textDiscountFluorideFreq = new OpenDental.ValidNum();
			this.textDiscountProphyFreq = new OpenDental.ValidNum();
			this.textDiscountExamFreq = new OpenDental.ValidNum();
			this.labelDiscountPAFreq = new System.Windows.Forms.Label();
			this.labelDiscountXrayFreq = new System.Windows.Forms.Label();
			this.labelDiscountPerioFreq = new System.Windows.Forms.Label();
			this.labelDiscountLimitedFreq = new System.Windows.Forms.Label();
			this.labelDiscountProphyFreq = new System.Windows.Forms.Label();
			this.labelDiscountFluorideFreq = new System.Windows.Forms.Label();
			this.labelDiscountExamFreq = new System.Windows.Forms.Label();
			this.textAnnualMax = new OpenDental.ValidDouble();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBoxOD4.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(368, 464);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 14;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(451, 464);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 16;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// textDescript
			// 
			this.textDescript.Location = new System.Drawing.Point(126, 57);
			this.textDescript.Name = "textDescript";
			this.textDescript.Size = new System.Drawing.Size(267, 20);
			this.textDescript.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(13, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(113, 18);
			this.label1.TabIndex = 20;
			this.label1.Text = "Description";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textFeeSched
			// 
			this.textFeeSched.Location = new System.Drawing.Point(126, 83);
			this.textFeeSched.Name = "textFeeSched";
			this.textFeeSched.ReadOnly = true;
			this.textFeeSched.Size = new System.Drawing.Size(241, 20);
			this.textFeeSched.TabIndex = 244;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(13, 84);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(113, 18);
			this.label2.TabIndex = 17;
			this.label2.Text = "Fee Schedule";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butFeeSched
			// 
			this.butFeeSched.Location = new System.Drawing.Point(372, 83);
			this.butFeeSched.Name = "butFeeSched";
			this.butFeeSched.Size = new System.Drawing.Size(20, 20);
			this.butFeeSched.TabIndex = 2;
			this.butFeeSched.Text = "...";
			this.butFeeSched.Click += new System.EventHandler(this.butFeeSched_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(13, 110);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(113, 18);
			this.label3.TabIndex = 18;
			this.label3.Text = "Adjustment Type";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxAdjType
			// 
			this.comboBoxAdjType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxAdjType.FormattingEnabled = true;
			this.comboBoxAdjType.Location = new System.Drawing.Point(126, 109);
			this.comboBoxAdjType.Name = "comboBoxAdjType";
			this.comboBoxAdjType.Size = new System.Drawing.Size(267, 21);
			this.comboBoxAdjType.TabIndex = 3;
			// 
			// checkHidden
			// 
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Location = new System.Drawing.Point(36, 10);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(104, 18);
			this.checkHidden.TabIndex = 15;
			this.checkHidden.Text = "Hidden";
			this.checkHidden.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.UseVisualStyleBackColor = true;
			this.checkHidden.Click += new System.EventHandler(this.checkHidden_Click);
			// 
			// butListPatients
			// 
			this.butListPatients.Location = new System.Drawing.Point(399, 136);
			this.butListPatients.Name = "butListPatients";
			this.butListPatients.Size = new System.Drawing.Size(75, 21);
			this.butListPatients.TabIndex = 22;
			this.butListPatients.Text = "List Patients";
			this.butListPatients.Click += new System.EventHandler(this.butListPatients_Click);
			// 
			// textNumPatients
			// 
			this.textNumPatients.Location = new System.Drawing.Point(127, 137);
			this.textNumPatients.Name = "textNumPatients";
			this.textNumPatients.ReadOnly = true;
			this.textNumPatients.Size = new System.Drawing.Size(72, 20);
			this.textNumPatients.TabIndex = 96;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(13, 139);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(113, 18);
			this.label4.TabIndex = 20;
			this.label4.Text = "Patients";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboPatient
			// 
			this.comboPatient.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPatient.FormattingEnabled = true;
			this.comboPatient.Location = new System.Drawing.Point(205, 136);
			this.comboPatient.Name = "comboPatient";
			this.comboPatient.Size = new System.Drawing.Size(188, 21);
			this.comboPatient.TabIndex = 95;
			// 
			// textPlanNote
			// 
			this.textPlanNote.AcceptsTab = true;
			this.textPlanNote.BackColor = System.Drawing.SystemColors.Window;
			this.textPlanNote.DetectLinksEnabled = false;
			this.textPlanNote.DetectUrls = false;
			this.textPlanNote.Location = new System.Drawing.Point(127, 163);
			this.textPlanNote.Name = "textPlanNote";
			this.textPlanNote.QuickPasteType = OpenDentBusiness.QuickPasteType.InsPlan;
			this.textPlanNote.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.Vertical;
			this.textPlanNote.Size = new System.Drawing.Size(265, 60);
			this.textPlanNote.TabIndex = 6;
			this.textPlanNote.Text = "";
			// 
			// labelPlanNote
			// 
			this.labelPlanNote.Location = new System.Drawing.Point(12, 163);
			this.labelPlanNote.Name = "labelPlanNote";
			this.labelPlanNote.Size = new System.Drawing.Size(114, 20);
			this.labelPlanNote.TabIndex = 24;
			this.labelPlanNote.Text = "Plan Note";
			this.labelPlanNote.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPlanNum
			// 
			this.textPlanNum.AcceptsTab = true;
			this.textPlanNum.BackColor = System.Drawing.SystemColors.Control;
			this.textPlanNum.Location = new System.Drawing.Point(126, 31);
			this.textPlanNum.Name = "textPlanNum";
			this.textPlanNum.ReadOnly = true;
			this.textPlanNum.Size = new System.Drawing.Size(100, 20);
			this.textPlanNum.TabIndex = 25;
			// 
			// labelPlanNum
			// 
			this.labelPlanNum.Location = new System.Drawing.Point(16, 31);
			this.labelPlanNum.Name = "labelPlanNum";
			this.labelPlanNum.Size = new System.Drawing.Size(110, 20);
			this.labelPlanNum.TabIndex = 26;
			this.labelPlanNum.Text = "Discount Plan ID";
			this.labelPlanNum.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxOD4
			// 
			this.groupBoxOD4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD4.Controls.Add(this.textDiscountPAFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountXrayFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountLimitedFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountPerioFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountFluorideFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountProphyFreq);
			this.groupBoxOD4.Controls.Add(this.textDiscountExamFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountPAFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountXrayFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountPerioFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountLimitedFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountProphyFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountFluorideFreq);
			this.groupBoxOD4.Controls.Add(this.labelDiscountExamFreq);
			this.groupBoxOD4.Location = new System.Drawing.Point(127, 256);
			this.groupBoxOD4.Name = "groupBoxOD4";
			this.groupBoxOD4.Size = new System.Drawing.Size(226, 188);
			this.groupBoxOD4.TabIndex = 8;
			this.groupBoxOD4.Text = "Frequency Limitations";
			// 
			// textDiscountPAFreq
			// 
			this.textDiscountPAFreq.Location = new System.Drawing.Point(154, 162);
			this.textDiscountPAFreq.MinVal = -1;
			this.textDiscountPAFreq.Name = "textDiscountPAFreq";
			this.textDiscountPAFreq.ShowZero = false;
			this.textDiscountPAFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountPAFreq.TabIndex = 261;
			// 
			// textDiscountXrayFreq
			// 
			this.textDiscountXrayFreq.Location = new System.Drawing.Point(154, 139);
			this.textDiscountXrayFreq.MinVal = -1;
			this.textDiscountXrayFreq.Name = "textDiscountXrayFreq";
			this.textDiscountXrayFreq.ShowZero = false;
			this.textDiscountXrayFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountXrayFreq.TabIndex = 260;
			// 
			// textDiscountLimitedFreq
			// 
			this.textDiscountLimitedFreq.Location = new System.Drawing.Point(154, 116);
			this.textDiscountLimitedFreq.MinVal = -1;
			this.textDiscountLimitedFreq.Name = "textDiscountLimitedFreq";
			this.textDiscountLimitedFreq.ShowZero = false;
			this.textDiscountLimitedFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountLimitedFreq.TabIndex = 259;
			// 
			// textDiscountPerioFreq
			// 
			this.textDiscountPerioFreq.Location = new System.Drawing.Point(154, 93);
			this.textDiscountPerioFreq.MinVal = -1;
			this.textDiscountPerioFreq.Name = "textDiscountPerioFreq";
			this.textDiscountPerioFreq.ShowZero = false;
			this.textDiscountPerioFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountPerioFreq.TabIndex = 258;
			// 
			// textDiscountFluorideFreq
			// 
			this.textDiscountFluorideFreq.Location = new System.Drawing.Point(154, 70);
			this.textDiscountFluorideFreq.MinVal = -1;
			this.textDiscountFluorideFreq.Name = "textDiscountFluorideFreq";
			this.textDiscountFluorideFreq.ShowZero = false;
			this.textDiscountFluorideFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountFluorideFreq.TabIndex = 257;
			// 
			// textDiscountProphyFreq
			// 
			this.textDiscountProphyFreq.Location = new System.Drawing.Point(154, 47);
			this.textDiscountProphyFreq.MinVal = -1;
			this.textDiscountProphyFreq.Name = "textDiscountProphyFreq";
			this.textDiscountProphyFreq.ShowZero = false;
			this.textDiscountProphyFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountProphyFreq.TabIndex = 256;
			// 
			// textDiscountExamFreq
			// 
			this.textDiscountExamFreq.Location = new System.Drawing.Point(154, 24);
			this.textDiscountExamFreq.MinVal = -1;
			this.textDiscountExamFreq.Name = "textDiscountExamFreq";
			this.textDiscountExamFreq.ShowZero = false;
			this.textDiscountExamFreq.Size = new System.Drawing.Size(42, 20);
			this.textDiscountExamFreq.TabIndex = 255;
			// 
			// labelDiscountPAFreq
			// 
			this.labelDiscountPAFreq.Location = new System.Drawing.Point(6, 164);
			this.labelDiscountPAFreq.Name = "labelDiscountPAFreq";
			this.labelDiscountPAFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountPAFreq.TabIndex = 241;
			this.labelDiscountPAFreq.Text = "Periapical X-Ray";
			this.labelDiscountPAFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiscountXrayFreq
			// 
			this.labelDiscountXrayFreq.Location = new System.Drawing.Point(6, 141);
			this.labelDiscountXrayFreq.Name = "labelDiscountXrayFreq";
			this.labelDiscountXrayFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountXrayFreq.TabIndex = 239;
			this.labelDiscountXrayFreq.Text = "X-Ray";
			this.labelDiscountXrayFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiscountPerioFreq
			// 
			this.labelDiscountPerioFreq.Location = new System.Drawing.Point(6, 95);
			this.labelDiscountPerioFreq.Name = "labelDiscountPerioFreq";
			this.labelDiscountPerioFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountPerioFreq.TabIndex = 233;
			this.labelDiscountPerioFreq.Text = "Perio Maintenance";
			this.labelDiscountPerioFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiscountLimitedFreq
			// 
			this.labelDiscountLimitedFreq.Location = new System.Drawing.Point(6, 118);
			this.labelDiscountLimitedFreq.Name = "labelDiscountLimitedFreq";
			this.labelDiscountLimitedFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountLimitedFreq.TabIndex = 231;
			this.labelDiscountLimitedFreq.Text = "Limited Exam";
			this.labelDiscountLimitedFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiscountProphyFreq
			// 
			this.labelDiscountProphyFreq.Location = new System.Drawing.Point(6, 49);
			this.labelDiscountProphyFreq.Name = "labelDiscountProphyFreq";
			this.labelDiscountProphyFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountProphyFreq.TabIndex = 229;
			this.labelDiscountProphyFreq.Text = "Prophylaxis";
			this.labelDiscountProphyFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiscountFluorideFreq
			// 
			this.labelDiscountFluorideFreq.Location = new System.Drawing.Point(6, 72);
			this.labelDiscountFluorideFreq.Name = "labelDiscountFluorideFreq";
			this.labelDiscountFluorideFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountFluorideFreq.TabIndex = 228;
			this.labelDiscountFluorideFreq.Text = "Fluoride";
			this.labelDiscountFluorideFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelDiscountExamFreq
			// 
			this.labelDiscountExamFreq.Location = new System.Drawing.Point(6, 26);
			this.labelDiscountExamFreq.Name = "labelDiscountExamFreq";
			this.labelDiscountExamFreq.Size = new System.Drawing.Size(148, 17);
			this.labelDiscountExamFreq.TabIndex = 227;
			this.labelDiscountExamFreq.Text = "Exam";
			this.labelDiscountExamFreq.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAnnualMax
			// 
			this.textAnnualMax.Location = new System.Drawing.Point(126, 229);
			this.textAnnualMax.MaxVal = 100000000D;
			this.textAnnualMax.MinVal = -1D;
			this.textAnnualMax.Name = "textAnnualMax";
			this.textAnnualMax.Size = new System.Drawing.Size(100, 20);
			this.textAnnualMax.TabIndex = 7;
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(6, 228);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(114, 20);
			this.label5.TabIndex = 246;
			this.label5.Text = "Annual Max";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormDiscountPlanEdit
			// 
			this.ClientSize = new System.Drawing.Size(538, 500);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.textAnnualMax);
			this.Controls.Add(this.groupBoxOD4);
			this.Controls.Add(this.labelPlanNum);
			this.Controls.Add(this.textPlanNum);
			this.Controls.Add(this.labelPlanNote);
			this.Controls.Add(this.textPlanNote);
			this.Controls.Add(this.butListPatients);
			this.Controls.Add(this.textNumPatients);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboPatient);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.comboBoxAdjType);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.butFeeSched);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textFeeSched);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textDescript);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormDiscountPlanEdit";
			this.Text = "Discount Plan Edit";
			this.Load += new System.EventHandler(this.FormDiscountPlanEdit_Load);
			this.groupBoxOD4.ResumeLayout(false);
			this.groupBoxOD4.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.TextBox textDescript;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textFeeSched;
		private System.Windows.Forms.Label label2;
		private UI.Button butFeeSched;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxAdjType;
		private System.Windows.Forms.CheckBox checkHidden;
		private UI.Button butListPatients;
		private System.Windows.Forms.TextBox textNumPatients;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboPatient;
		private OpenDental.ODtextBox textPlanNote;
		private System.Windows.Forms.Label labelPlanNote;
		private System.Windows.Forms.TextBox textPlanNum;
		private System.Windows.Forms.Label labelPlanNum;
		private UI.GroupBoxOD groupBoxOD4;
		private System.Windows.Forms.Label labelDiscountPAFreq;
		private System.Windows.Forms.Label labelDiscountXrayFreq;
		private System.Windows.Forms.Label labelDiscountPerioFreq;
		private System.Windows.Forms.Label labelDiscountLimitedFreq;
		private System.Windows.Forms.Label labelDiscountProphyFreq;
		private System.Windows.Forms.Label labelDiscountFluorideFreq;
		private System.Windows.Forms.Label labelDiscountExamFreq;
		private ValidDouble textAnnualMax;
		private System.Windows.Forms.Label label5;
		private ValidNum textDiscountPAFreq;
		private ValidNum textDiscountXrayFreq;
		private ValidNum textDiscountLimitedFreq;
		private ValidNum textDiscountPerioFreq;
		private ValidNum textDiscountFluorideFreq;
		private ValidNum textDiscountProphyFreq;
		private ValidNum textDiscountExamFreq;
	}
}