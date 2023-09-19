namespace OpenDental {
	partial class FormPayPlanTemplateEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormPayPlanTemplateEdit));
			this.butSave = new OpenDental.UI.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textTemplateName = new System.Windows.Forms.TextBox();
			this.groupTreatmentPlanned = new OpenDental.UI.GroupBox();
			this.radioTpTreatAsComplete = new System.Windows.Forms.RadioButton();
			this.radioTpAwaitComplete = new System.Windows.Forms.RadioButton();
			this.labelInterestDelay2 = new System.Windows.Forms.Label();
			this.labelInterestDelay1 = new System.Windows.Forms.Label();
			this.textInterestDelay = new OpenDental.ValidDouble();
			this.label16 = new System.Windows.Forms.Label();
			this.label7 = new System.Windows.Forms.Label();
			this.textPaymentCount = new OpenDental.ValidNum();
			this.textPeriodPayment = new OpenDental.ValidDouble();
			this.groupBoxFrequency = new OpenDental.UI.GroupBox();
			this.radioQuarterly = new System.Windows.Forms.RadioButton();
			this.radioMonthly = new System.Windows.Forms.RadioButton();
			this.radioOrdinalWeekday = new System.Windows.Forms.RadioButton();
			this.radioEveryOtherWeek = new System.Windows.Forms.RadioButton();
			this.radioWeekly = new System.Windows.Forms.RadioButton();
			this.label8 = new System.Windows.Forms.Label();
			this.textAPR = new OpenDental.ValidDouble();
			this.textDownPayment = new OpenDental.ValidDouble();
			this.label11 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.comboBoxClinic = new OpenDental.UI.ComboBoxClinicPicker();
			this.checkHidden = new OpenDental.UI.CheckBox();
			this.groupTreatmentPlanned.SuspendLayout();
			this.groupBoxFrequency.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(416, 318);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(60, 33);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(114, 17);
			this.label1.TabIndex = 206;
			this.label1.Text = "Template Name";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textTemplateName
			// 
			this.textTemplateName.Location = new System.Drawing.Point(174, 32);
			this.textTemplateName.Name = "textTemplateName";
			this.textTemplateName.Size = new System.Drawing.Size(245, 20);
			this.textTemplateName.TabIndex = 204;
			// 
			// groupTreatmentPlanned
			// 
			this.groupTreatmentPlanned.Controls.Add(this.radioTpTreatAsComplete);
			this.groupTreatmentPlanned.Controls.Add(this.radioTpAwaitComplete);
			this.groupTreatmentPlanned.Location = new System.Drawing.Point(225, 186);
			this.groupTreatmentPlanned.Name = "groupTreatmentPlanned";
			this.groupTreatmentPlanned.Size = new System.Drawing.Size(180, 115);
			this.groupTreatmentPlanned.TabIndex = 202;
			this.groupTreatmentPlanned.Text = "Handle Treatment Planned";
			// 
			// radioTpTreatAsComplete
			// 
			this.radioTpTreatAsComplete.Location = new System.Drawing.Point(6, 54);
			this.radioTpTreatAsComplete.Name = "radioTpTreatAsComplete";
			this.radioTpTreatAsComplete.Size = new System.Drawing.Size(148, 32);
			this.radioTpTreatAsComplete.TabIndex = 1;
			this.radioTpTreatAsComplete.TabStop = true;
			this.radioTpTreatAsComplete.Text = "Procedure as complete";
			this.radioTpTreatAsComplete.UseVisualStyleBackColor = true;
			// 
			// radioTpAwaitComplete
			// 
			this.radioTpAwaitComplete.Location = new System.Drawing.Point(7, 20);
			this.radioTpAwaitComplete.Name = "radioTpAwaitComplete";
			this.radioTpAwaitComplete.Size = new System.Drawing.Size(147, 32);
			this.radioTpAwaitComplete.TabIndex = 0;
			this.radioTpAwaitComplete.TabStop = true;
			this.radioTpAwaitComplete.Text = "Await procedure completion";
			this.radioTpAwaitComplete.UseVisualStyleBackColor = true;
			// 
			// labelInterestDelay2
			// 
			this.labelInterestDelay2.Location = new System.Drawing.Point(222, 122);
			this.labelInterestDelay2.Name = "labelInterestDelay2";
			this.labelInterestDelay2.Size = new System.Drawing.Size(138, 17);
			this.labelInterestDelay2.TabIndex = 198;
			this.labelInterestDelay2.Text = "payments";
			this.labelInterestDelay2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelInterestDelay1
			// 
			this.labelInterestDelay1.Location = new System.Drawing.Point(34, 122);
			this.labelInterestDelay1.Name = "labelInterestDelay1";
			this.labelInterestDelay1.Size = new System.Drawing.Size(138, 17);
			this.labelInterestDelay1.TabIndex = 197;
			this.labelInterestDelay1.Text = "No interest for the first";
			this.labelInterestDelay1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textInterestDelay
			// 
			this.textInterestDelay.Location = new System.Drawing.Point(174, 120);
			this.textInterestDelay.MaxVal = 100000000D;
			this.textInterestDelay.MinVal = 0D;
			this.textInterestDelay.Name = "textInterestDelay";
			this.textInterestDelay.Size = new System.Drawing.Size(47, 20);
			this.textInterestDelay.TabIndex = 5;
			// 
			// label16
			// 
			this.label16.Location = new System.Drawing.Point(222, 166);
			this.label16.Name = "label16";
			this.label16.Size = new System.Drawing.Size(124, 17);
			this.label16.TabIndex = 195;
			this.label16.Text = "(sets payment amount)";
			this.label16.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(50, 143);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(122, 17);
			this.label7.TabIndex = 0;
			this.label7.Text = "Payment Amount";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPaymentCount
			// 
			this.textPaymentCount.Location = new System.Drawing.Point(174, 164);
			this.textPaymentCount.MinVal = 1;
			this.textPaymentCount.Name = "textPaymentCount";
			this.textPaymentCount.ShowZero = false;
			this.textPaymentCount.Size = new System.Drawing.Size(47, 20);
			this.textPaymentCount.TabIndex = 8;
			this.textPaymentCount.TextChanged += new System.EventHandler(this.textPaymentCount_TextChanged);
			// 
			// textPeriodPayment
			// 
			this.textPeriodPayment.Location = new System.Drawing.Point(174, 142);
			this.textPeriodPayment.MaxVal = 100000000D;
			this.textPeriodPayment.MinVal = 0.01D;
			this.textPeriodPayment.Name = "textPeriodPayment";
			this.textPeriodPayment.Size = new System.Drawing.Size(85, 20);
			this.textPeriodPayment.TabIndex = 7;
			// 
			// groupBoxFrequency
			// 
			this.groupBoxFrequency.Controls.Add(this.radioQuarterly);
			this.groupBoxFrequency.Controls.Add(this.radioMonthly);
			this.groupBoxFrequency.Controls.Add(this.radioOrdinalWeekday);
			this.groupBoxFrequency.Controls.Add(this.radioEveryOtherWeek);
			this.groupBoxFrequency.Controls.Add(this.radioWeekly);
			this.groupBoxFrequency.Location = new System.Drawing.Point(40, 186);
			this.groupBoxFrequency.Name = "groupBoxFrequency";
			this.groupBoxFrequency.Size = new System.Drawing.Size(181, 115);
			this.groupBoxFrequency.TabIndex = 9;
			this.groupBoxFrequency.Text = "Charge Frequency";
			// 
			// radioQuarterly
			// 
			this.radioQuarterly.Location = new System.Drawing.Point(5, 91);
			this.radioQuarterly.Name = "radioQuarterly";
			this.radioQuarterly.Size = new System.Drawing.Size(104, 17);
			this.radioQuarterly.TabIndex = 4;
			this.radioQuarterly.TabStop = true;
			this.radioQuarterly.Text = "Quarterly";
			this.radioQuarterly.UseVisualStyleBackColor = true;
			// 
			// radioMonthly
			// 
			this.radioMonthly.Checked = true;
			this.radioMonthly.Location = new System.Drawing.Point(5, 73);
			this.radioMonthly.Name = "radioMonthly";
			this.radioMonthly.Size = new System.Drawing.Size(104, 17);
			this.radioMonthly.TabIndex = 3;
			this.radioMonthly.TabStop = true;
			this.radioMonthly.Text = "Monthly";
			this.radioMonthly.UseVisualStyleBackColor = true;
			// 
			// radioOrdinalWeekday
			// 
			this.radioOrdinalWeekday.Location = new System.Drawing.Point(5, 55);
			this.radioOrdinalWeekday.Name = "radioOrdinalWeekday";
			this.radioOrdinalWeekday.Size = new System.Drawing.Size(174, 17);
			this.radioOrdinalWeekday.TabIndex = 2;
			this.radioOrdinalWeekday.TabStop = true;
			this.radioOrdinalWeekday.Text = "Specific day of month";
			this.radioOrdinalWeekday.UseVisualStyleBackColor = true;
			// 
			// radioEveryOtherWeek
			// 
			this.radioEveryOtherWeek.Location = new System.Drawing.Point(5, 37);
			this.radioEveryOtherWeek.Name = "radioEveryOtherWeek";
			this.radioEveryOtherWeek.Size = new System.Drawing.Size(156, 17);
			this.radioEveryOtherWeek.TabIndex = 1;
			this.radioEveryOtherWeek.TabStop = true;
			this.radioEveryOtherWeek.Text = "Every other week";
			this.radioEveryOtherWeek.UseVisualStyleBackColor = true;
			// 
			// radioWeekly
			// 
			this.radioWeekly.Location = new System.Drawing.Point(5, 19);
			this.radioWeekly.Name = "radioWeekly";
			this.radioWeekly.Size = new System.Drawing.Size(104, 17);
			this.radioWeekly.TabIndex = 0;
			this.radioWeekly.TabStop = true;
			this.radioWeekly.Text = "Weekly";
			this.radioWeekly.UseVisualStyleBackColor = true;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(48, 165);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(124, 17);
			this.label8.TabIndex = 0;
			this.label8.Text = "Number of Payments";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textAPR
			// 
			this.textAPR.Location = new System.Drawing.Point(174, 98);
			this.textAPR.MaxVal = 100000000D;
			this.textAPR.MinVal = 0D;
			this.textAPR.Name = "textAPR";
			this.textAPR.Size = new System.Drawing.Size(47, 20);
			this.textAPR.TabIndex = 4;
			this.textAPR.TextChanged += new System.EventHandler(this.textAPR_TextChanged);
			// 
			// textDownPayment
			// 
			this.textDownPayment.Location = new System.Drawing.Point(174, 76);
			this.textDownPayment.MaxVal = 100000000D;
			this.textDownPayment.MinVal = 0D;
			this.textDownPayment.Name = "textDownPayment";
			this.textDownPayment.Size = new System.Drawing.Size(85, 20);
			this.textDownPayment.TabIndex = 3;
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(40, 79);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(134, 17);
			this.label11.TabIndex = 0;
			this.label11.Text = "Down Payment";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label6
			// 
			this.label6.Location = new System.Drawing.Point(35, 100);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(138, 17);
			this.label6.TabIndex = 0;
			this.label6.Text = "APR (for example 0 or 18)";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxClinic
			// 
			this.comboBoxClinic.IncludeUnassigned = true;
			this.comboBoxClinic.Location = new System.Drawing.Point(137, 54);
			this.comboBoxClinic.Name = "comboBoxClinic";
			this.comboBoxClinic.Size = new System.Drawing.Size(201, 21);
			this.comboBoxClinic.TabIndex = 207;
			// 
			// checkHidden
			// 
			this.checkHidden.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.checkHidden.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkHidden.Location = new System.Drawing.Point(403, 12);
			this.checkHidden.Name = "checkHidden";
			this.checkHidden.Size = new System.Drawing.Size(88, 17);
			this.checkHidden.TabIndex = 208;
			this.checkHidden.Text = "Hidden";
			// 
			// FormPayPlanTemplateEdit
			// 
			this.ClientSize = new System.Drawing.Size(503, 354);
			this.Controls.Add(this.checkHidden);
			this.Controls.Add(this.comboBoxClinic);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.textTemplateName);
			this.Controls.Add(this.butSave);
			this.Controls.Add(this.groupTreatmentPlanned);
			this.Controls.Add(this.labelInterestDelay2);
			this.Controls.Add(this.labelInterestDelay1);
			this.Controls.Add(this.textInterestDelay);
			this.Controls.Add(this.label16);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.label7);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textPaymentCount);
			this.Controls.Add(this.textDownPayment);
			this.Controls.Add(this.textPeriodPayment);
			this.Controls.Add(this.textAPR);
			this.Controls.Add(this.groupBoxFrequency);
			this.Controls.Add(this.label8);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormPayPlanTemplateEdit";
			this.Text = "Edit Pay Plan Template";
			this.CloseXClicked += new System.ComponentModel.CancelEventHandler(this.FormPayPlanTemplateEdit_CloseXClicked);
			this.Load += new System.EventHandler(this.FormPayPlanTemplateEdit_Load);
			this.groupTreatmentPlanned.ResumeLayout(false);
			this.groupBoxFrequency.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private UI.GroupBox groupTreatmentPlanned;
		private System.Windows.Forms.RadioButton radioTpTreatAsComplete;
		private System.Windows.Forms.RadioButton radioTpAwaitComplete;
		private System.Windows.Forms.Label labelInterestDelay2;
		private System.Windows.Forms.Label labelInterestDelay1;
		private ValidDouble textInterestDelay;
		private System.Windows.Forms.Label label16;
		private System.Windows.Forms.Label label7;
		private ValidNum textPaymentCount;
		private ValidDouble textPeriodPayment;
		private UI.GroupBox groupBoxFrequency;
		public System.Windows.Forms.RadioButton radioQuarterly;
		public System.Windows.Forms.RadioButton radioMonthly;
		public System.Windows.Forms.RadioButton radioOrdinalWeekday;
		public System.Windows.Forms.RadioButton radioEveryOtherWeek;
		public System.Windows.Forms.RadioButton radioWeekly;
		private System.Windows.Forms.Label label8;
		private ValidDouble textAPR;
		private ValidDouble textDownPayment;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textTemplateName;
		private System.Windows.Forms.Label label1;
		private UI.ComboBoxClinicPicker comboBoxClinic;
		private UI.CheckBox checkHidden;
	}
}