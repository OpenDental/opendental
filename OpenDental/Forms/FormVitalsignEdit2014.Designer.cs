namespace OpenDental {
	partial class FormVitalsignEdit2014 {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormVitalsignEdit2014));
			this.label1 = new System.Windows.Forms.Label();
			this.labelBMI = new System.Windows.Forms.Label();
			this.labelBPs = new System.Windows.Forms.Label();
			this.labelWeight = new System.Windows.Forms.Label();
			this.labelHeight = new System.Windows.Forms.Label();
			this.textDateTaken = new System.Windows.Forms.TextBox();
			this.textBPd = new System.Windows.Forms.TextBox();
			this.textBPs = new System.Windows.Forms.TextBox();
			this.textWeight = new System.Windows.Forms.TextBox();
			this.textHeight = new System.Windows.Forms.TextBox();
			this.textBMI = new System.Windows.Forms.TextBox();
			this.labelWeightCode = new System.Windows.Forms.Label();
			this.comboHeightExamCode = new System.Windows.Forms.ComboBox();
			this.comboWeightExamCode = new System.Windows.Forms.ComboBox();
			this.labelHeightExamCode = new System.Windows.Forms.Label();
			this.labelWeightExamCode = new System.Windows.Forms.Label();
			this.labelBMIPercentileCode = new System.Windows.Forms.Label();
			this.labelBMIExamCode = new System.Windows.Forms.Label();
			this.textBMIExamCode = new System.Windows.Forms.TextBox();
			this.labelBPd = new System.Windows.Forms.Label();
			this.labelBPdExamCode = new System.Windows.Forms.Label();
			this.textBPdExamCode = new System.Windows.Forms.TextBox();
			this.labelBPsExamCode = new System.Windows.Forms.Label();
			this.textBPsExamCode = new System.Windows.Forms.TextBox();
			this.checkPregnant = new System.Windows.Forms.CheckBox();
			this.textPregCode = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.groupExclusion = new System.Windows.Forms.GroupBox();
			this.butChangeDefault = new OpenDental.UI.Button();
			this.textReasonDescript = new System.Windows.Forms.TextBox();
			this.textPregCodeDescript = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.labelNotPerf = new System.Windows.Forms.Label();
			this.textReasonCode = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.checkNotPerf = new System.Windows.Forms.CheckBox();
			this.labelPregNotice = new System.Windows.Forms.Label();
			this.groupInterventions = new System.Windows.Forms.GroupBox();
			this.butAdd = new OpenDental.UI.Button();
			this.gridInterventions = new OpenDental.UI.GridOD();
			this.textBMIPercentile = new System.Windows.Forms.TextBox();
			this.labelBMIPercentile = new System.Windows.Forms.Label();
			this.textBMIPercentileCode = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textPulse = new System.Windows.Forms.TextBox();
			this.butDelete = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.butOk = new OpenDental.UI.Button();
			this.groupExclusion.SuspendLayout();
			this.groupInterventions.SuspendLayout();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(26, 20);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(93, 17);
			this.label1.TabIndex = 0;
			this.label1.Text = "Date";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBMI
			// 
			this.labelBMI.Location = new System.Drawing.Point(26, 139);
			this.labelBMI.Name = "labelBMI";
			this.labelBMI.Size = new System.Drawing.Size(93, 17);
			this.labelBMI.TabIndex = 0;
			this.labelBMI.Text = "BMI";
			this.labelBMI.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBPs
			// 
			this.labelBPs.Location = new System.Drawing.Point(26, 44);
			this.labelBPs.Name = "labelBPs";
			this.labelBPs.Size = new System.Drawing.Size(93, 17);
			this.labelBPs.TabIndex = 0;
			this.labelBPs.Text = "Systolic BP";
			this.labelBPs.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWeight
			// 
			this.labelWeight.Location = new System.Drawing.Point(26, 116);
			this.labelWeight.Name = "labelWeight";
			this.labelWeight.Size = new System.Drawing.Size(93, 17);
			this.labelWeight.TabIndex = 0;
			this.labelWeight.Text = "Weight (.lbs)";
			this.labelWeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelHeight
			// 
			this.labelHeight.Location = new System.Drawing.Point(26, 92);
			this.labelHeight.Name = "labelHeight";
			this.labelHeight.Size = new System.Drawing.Size(93, 17);
			this.labelHeight.TabIndex = 0;
			this.labelHeight.Text = "Height (in.)";
			this.labelHeight.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDateTaken
			// 
			this.textDateTaken.Location = new System.Drawing.Point(122, 18);
			this.textDateTaken.Name = "textDateTaken";
			this.textDateTaken.Size = new System.Drawing.Size(80, 20);
			this.textDateTaken.TabIndex = 1;
			this.textDateTaken.Leave += new System.EventHandler(this.textDateTaken_Leave);
			// 
			// textBPd
			// 
			this.textBPd.Location = new System.Drawing.Point(122, 66);
			this.textBPd.Name = "textBPd";
			this.textBPd.Size = new System.Drawing.Size(39, 20);
			this.textBPd.TabIndex = 4;
			this.textBPd.TextChanged += new System.EventHandler(this.textBPd_TextChanged);
			// 
			// textBPs
			// 
			this.textBPs.Location = new System.Drawing.Point(122, 42);
			this.textBPs.Name = "textBPs";
			this.textBPs.Size = new System.Drawing.Size(40, 20);
			this.textBPs.TabIndex = 3;
			this.textBPs.TextChanged += new System.EventHandler(this.textBPs_TextChanged);
			// 
			// textWeight
			// 
			this.textWeight.Location = new System.Drawing.Point(122, 114);
			this.textWeight.Name = "textWeight";
			this.textWeight.Size = new System.Drawing.Size(56, 20);
			this.textWeight.TabIndex = 6;
			this.textWeight.TextChanged += new System.EventHandler(this.textWeight_TextChanged);
			// 
			// textHeight
			// 
			this.textHeight.Location = new System.Drawing.Point(122, 90);
			this.textHeight.Name = "textHeight";
			this.textHeight.Size = new System.Drawing.Size(56, 20);
			this.textHeight.TabIndex = 5;
			this.textHeight.TextChanged += new System.EventHandler(this.textHeight_TextChanged);
			// 
			// textBMI
			// 
			this.textBMI.Location = new System.Drawing.Point(122, 137);
			this.textBMI.Name = "textBMI";
			this.textBMI.ReadOnly = true;
			this.textBMI.Size = new System.Drawing.Size(40, 20);
			this.textBMI.TabIndex = 17;
			this.textBMI.TabStop = false;
			// 
			// labelWeightCode
			// 
			this.labelWeightCode.Location = new System.Drawing.Point(168, 139);
			this.labelWeightCode.Name = "labelWeightCode";
			this.labelWeightCode.Size = new System.Drawing.Size(82, 17);
			this.labelWeightCode.TabIndex = 24;
			this.labelWeightCode.Text = "Over/Under";
			this.labelWeightCode.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// comboHeightExamCode
			// 
			this.comboHeightExamCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboHeightExamCode.Location = new System.Drawing.Point(434, 89);
			this.comboHeightExamCode.MaxDropDownItems = 30;
			this.comboHeightExamCode.Name = "comboHeightExamCode";
			this.comboHeightExamCode.Size = new System.Drawing.Size(158, 21);
			this.comboHeightExamCode.TabIndex = 7;
			// 
			// comboWeightExamCode
			// 
			this.comboWeightExamCode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboWeightExamCode.Location = new System.Drawing.Point(434, 113);
			this.comboWeightExamCode.MaxDropDownItems = 30;
			this.comboWeightExamCode.Name = "comboWeightExamCode";
			this.comboWeightExamCode.Size = new System.Drawing.Size(158, 21);
			this.comboWeightExamCode.TabIndex = 8;
			// 
			// labelHeightExamCode
			// 
			this.labelHeightExamCode.Location = new System.Drawing.Point(301, 92);
			this.labelHeightExamCode.Name = "labelHeightExamCode";
			this.labelHeightExamCode.Size = new System.Drawing.Size(130, 17);
			this.labelHeightExamCode.TabIndex = 0;
			this.labelHeightExamCode.Text = "Height Code";
			this.labelHeightExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelWeightExamCode
			// 
			this.labelWeightExamCode.Location = new System.Drawing.Point(301, 116);
			this.labelWeightExamCode.Name = "labelWeightExamCode";
			this.labelWeightExamCode.Size = new System.Drawing.Size(130, 17);
			this.labelWeightExamCode.TabIndex = 0;
			this.labelWeightExamCode.Text = "Weight Code";
			this.labelWeightExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBMIPercentileCode
			// 
			this.labelBMIPercentileCode.Location = new System.Drawing.Point(301, 163);
			this.labelBMIPercentileCode.Name = "labelBMIPercentileCode";
			this.labelBMIPercentileCode.Size = new System.Drawing.Size(130, 17);
			this.labelBMIPercentileCode.TabIndex = 0;
			this.labelBMIPercentileCode.Text = "BMI Percentile Code";
			this.labelBMIPercentileCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelBMIPercentileCode.Visible = false;
			// 
			// labelBMIExamCode
			// 
			this.labelBMIExamCode.Location = new System.Drawing.Point(301, 139);
			this.labelBMIExamCode.Name = "labelBMIExamCode";
			this.labelBMIExamCode.Size = new System.Drawing.Size(130, 17);
			this.labelBMIExamCode.TabIndex = 0;
			this.labelBMIExamCode.Text = "BMI Code";
			this.labelBMIExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBMIExamCode
			// 
			this.textBMIExamCode.Location = new System.Drawing.Point(434, 137);
			this.textBMIExamCode.Name = "textBMIExamCode";
			this.textBMIExamCode.ReadOnly = true;
			this.textBMIExamCode.Size = new System.Drawing.Size(95, 20);
			this.textBMIExamCode.TabIndex = 31;
			// 
			// labelBPd
			// 
			this.labelBPd.Location = new System.Drawing.Point(26, 68);
			this.labelBPd.Name = "labelBPd";
			this.labelBPd.Size = new System.Drawing.Size(93, 17);
			this.labelBPd.TabIndex = 0;
			this.labelBPd.Text = "Diastolic BP";
			this.labelBPd.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelBPdExamCode
			// 
			this.labelBPdExamCode.Location = new System.Drawing.Point(301, 68);
			this.labelBPdExamCode.Name = "labelBPdExamCode";
			this.labelBPdExamCode.Size = new System.Drawing.Size(130, 17);
			this.labelBPdExamCode.TabIndex = 0;
			this.labelBPdExamCode.Text = "Diastolic BP Code";
			this.labelBPdExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBPdExamCode
			// 
			this.textBPdExamCode.Location = new System.Drawing.Point(434, 66);
			this.textBPdExamCode.Name = "textBPdExamCode";
			this.textBPdExamCode.ReadOnly = true;
			this.textBPdExamCode.Size = new System.Drawing.Size(95, 20);
			this.textBPdExamCode.TabIndex = 34;
			// 
			// labelBPsExamCode
			// 
			this.labelBPsExamCode.Location = new System.Drawing.Point(301, 44);
			this.labelBPsExamCode.Name = "labelBPsExamCode";
			this.labelBPsExamCode.Size = new System.Drawing.Size(130, 17);
			this.labelBPsExamCode.TabIndex = 0;
			this.labelBPsExamCode.Text = "Systolic BP Code";
			this.labelBPsExamCode.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textBPsExamCode
			// 
			this.textBPsExamCode.Location = new System.Drawing.Point(434, 42);
			this.textBPsExamCode.Name = "textBPsExamCode";
			this.textBPsExamCode.ReadOnly = true;
			this.textBPsExamCode.Size = new System.Drawing.Size(95, 20);
			this.textBPsExamCode.TabIndex = 36;
			// 
			// checkPregnant
			// 
			this.checkPregnant.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkPregnant.Location = new System.Drawing.Point(6, 16);
			this.checkPregnant.Name = "checkPregnant";
			this.checkPregnant.Size = new System.Drawing.Size(261, 44);
			this.checkPregnant.TabIndex = 1;
			this.checkPregnant.Text = "Height and Weight was not recorded because the patient is pregnant or has been pr" +
    "egnant any time during the measurement period.";
			this.checkPregnant.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkPregnant.UseVisualStyleBackColor = true;
			this.checkPregnant.Click += new System.EventHandler(this.checkPregnant_Click);
			// 
			// textPregCode
			// 
			this.textPregCode.Location = new System.Drawing.Point(377, 14);
			this.textPregCode.Name = "textPregCode";
			this.textPregCode.ReadOnly = true;
			this.textPregCode.Size = new System.Drawing.Size(100, 20);
			this.textPregCode.TabIndex = 0;
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(273, 15);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(101, 17);
			this.label6.TabIndex = 0;
			this.label6.Text = "Pregnancy Code";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupExclusion
			// 
			this.groupExclusion.Controls.Add(this.butChangeDefault);
			this.groupExclusion.Controls.Add(this.textReasonDescript);
			this.groupExclusion.Controls.Add(this.textPregCodeDescript);
			this.groupExclusion.Controls.Add(this.label4);
			this.groupExclusion.Controls.Add(this.labelNotPerf);
			this.groupExclusion.Controls.Add(this.textReasonCode);
			this.groupExclusion.Controls.Add(this.label3);
			this.groupExclusion.Controls.Add(this.checkNotPerf);
			this.groupExclusion.Controls.Add(this.labelPregNotice);
			this.groupExclusion.Controls.Add(this.label6);
			this.groupExclusion.Controls.Add(this.textPregCode);
			this.groupExclusion.Controls.Add(this.checkPregnant);
			this.groupExclusion.Location = new System.Drawing.Point(10, 186);
			this.groupExclusion.Name = "groupExclusion";
			this.groupExclusion.Size = new System.Drawing.Size(578, 180);
			this.groupExclusion.TabIndex = 9;
			this.groupExclusion.TabStop = false;
			this.groupExclusion.Text = "Exclusion From BMI Exam";
			// 
			// butChangeDefault
			// 
			this.butChangeDefault.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butChangeDefault.Location = new System.Drawing.Point(483, 12);
			this.butChangeDefault.Name = "butChangeDefault";
			this.butChangeDefault.Size = new System.Drawing.Size(89, 24);
			this.butChangeDefault.TabIndex = 3;
			this.butChangeDefault.Text = "Change Default";
			this.butChangeDefault.Click += new System.EventHandler(this.butChangeDefault_Click);
			// 
			// textReasonDescript
			// 
			this.textReasonDescript.AcceptsTab = true;
			this.textReasonDescript.Location = new System.Drawing.Point(377, 133);
			this.textReasonDescript.MaxLength = 2147483647;
			this.textReasonDescript.Multiline = true;
			this.textReasonDescript.Name = "textReasonDescript";
			this.textReasonDescript.ReadOnly = true;
			this.textReasonDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textReasonDescript.Size = new System.Drawing.Size(195, 40);
			this.textReasonDescript.TabIndex = 0;
			// 
			// textPregCodeDescript
			// 
			this.textPregCodeDescript.AcceptsTab = true;
			this.textPregCodeDescript.Location = new System.Drawing.Point(377, 40);
			this.textPregCodeDescript.MaxLength = 2147483647;
			this.textPregCodeDescript.Multiline = true;
			this.textPregCodeDescript.Name = "textPregCodeDescript";
			this.textPregCodeDescript.ReadOnly = true;
			this.textPregCodeDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textPregCodeDescript.Size = new System.Drawing.Size(195, 40);
			this.textPregCodeDescript.TabIndex = 0;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(273, 134);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(101, 17);
			this.label4.TabIndex = 0;
			this.label4.Text = "Description";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelNotPerf
			// 
			this.labelNotPerf.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelNotPerf.Location = new System.Drawing.Point(273, 108);
			this.labelNotPerf.Name = "labelNotPerf";
			this.labelNotPerf.Size = new System.Drawing.Size(101, 17);
			this.labelNotPerf.TabIndex = 0;
			this.labelNotPerf.Text = "Reason Code";
			this.labelNotPerf.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReasonCode
			// 
			this.textReasonCode.Location = new System.Drawing.Point(377, 107);
			this.textReasonCode.Name = "textReasonCode";
			this.textReasonCode.ReadOnly = true;
			this.textReasonCode.Size = new System.Drawing.Size(100, 20);
			this.textReasonCode.TabIndex = 0;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(273, 41);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(101, 17);
			this.label3.TabIndex = 0;
			this.label3.Text = "Description";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// checkNotPerf
			// 
			this.checkNotPerf.CheckAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkNotPerf.Location = new System.Drawing.Point(6, 108);
			this.checkNotPerf.Name = "checkNotPerf";
			this.checkNotPerf.Size = new System.Drawing.Size(261, 31);
			this.checkNotPerf.TabIndex = 2;
			this.checkNotPerf.Text = "Height and Weight was not recorded for a medical or other reason.";
			this.checkNotPerf.TextAlign = System.Drawing.ContentAlignment.TopLeft;
			this.checkNotPerf.UseVisualStyleBackColor = true;
			this.checkNotPerf.Click += new System.EventHandler(this.checkNotPerf_Click);
			// 
			// labelPregNotice
			// 
			this.labelPregNotice.ForeColor = System.Drawing.Color.Red;
			this.labelPregNotice.Location = new System.Drawing.Point(24, 61);
			this.labelPregNotice.Name = "labelPregNotice";
			this.labelPregNotice.Size = new System.Drawing.Size(243, 39);
			this.labelPregNotice.TabIndex = 0;
			this.labelPregNotice.Text = "A diagnosis of pregnancy with this code will be added to the patient\'s medical hi" +
    "story with a start date equal to the date of this exam.";
			this.labelPregNotice.Visible = false;
			// 
			// groupInterventions
			// 
			this.groupInterventions.Controls.Add(this.butAdd);
			this.groupInterventions.Controls.Add(this.gridInterventions);
			this.groupInterventions.Location = new System.Drawing.Point(10, 372);
			this.groupInterventions.Name = "groupInterventions";
			this.groupInterventions.Size = new System.Drawing.Size(578, 198);
			this.groupInterventions.TabIndex = 10;
			this.groupInterventions.TabStop = false;
			this.groupInterventions.Text = "Interventions";
			// 
			// butAdd
			// 
			this.butAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butAdd.Icon = OpenDental.UI.EnumIcons.Add;
			this.butAdd.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butAdd.Location = new System.Drawing.Point(497, 19);
			this.butAdd.Name = "butAdd";
			this.butAdd.Size = new System.Drawing.Size(75, 24);
			this.butAdd.TabIndex = 1;
			this.butAdd.Text = "Add";
			this.butAdd.Click += new System.EventHandler(this.butAdd_Click);
			// 
			// gridInterventions
			// 
			this.gridInterventions.Location = new System.Drawing.Point(6, 19);
			this.gridInterventions.Name = "gridInterventions";
			this.gridInterventions.Size = new System.Drawing.Size(485, 169);
			this.gridInterventions.TabIndex = 0;
			this.gridInterventions.Title = "Interventions and/or Medications";
			this.gridInterventions.TranslationName = "TableMedication";
			this.gridInterventions.CellDoubleClick += new OpenDental.UI.ODGridClickEventHandler(this.gridInterventions_CellDoubleClick);
			// 
			// textBMIPercentile
			// 
			this.textBMIPercentile.Location = new System.Drawing.Point(122, 161);
			this.textBMIPercentile.Name = "textBMIPercentile";
			this.textBMIPercentile.ReadOnly = true;
			this.textBMIPercentile.Size = new System.Drawing.Size(40, 20);
			this.textBMIPercentile.TabIndex = 163;
			this.textBMIPercentile.TabStop = false;
			this.textBMIPercentile.Visible = false;
			// 
			// labelBMIPercentile
			// 
			this.labelBMIPercentile.Location = new System.Drawing.Point(26, 163);
			this.labelBMIPercentile.Name = "labelBMIPercentile";
			this.labelBMIPercentile.Size = new System.Drawing.Size(93, 17);
			this.labelBMIPercentile.TabIndex = 0;
			this.labelBMIPercentile.Text = "BMI Percentile";
			this.labelBMIPercentile.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.labelBMIPercentile.Visible = false;
			// 
			// textBMIPercentileCode
			// 
			this.textBMIPercentileCode.Location = new System.Drawing.Point(434, 161);
			this.textBMIPercentileCode.Name = "textBMIPercentileCode";
			this.textBMIPercentileCode.ReadOnly = true;
			this.textBMIPercentileCode.Size = new System.Drawing.Size(95, 20);
			this.textBMIPercentileCode.TabIndex = 164;
			this.textBMIPercentileCode.Visible = false;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(316, 18);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(115, 17);
			this.label2.TabIndex = 0;
			this.label2.Text = "Pulse (bpm)";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPulse
			// 
			this.textPulse.Location = new System.Drawing.Point(434, 18);
			this.textPulse.Name = "textPulse";
			this.textPulse.Size = new System.Drawing.Size(39, 20);
			this.textPulse.TabIndex = 2;
			// 
			// butDelete
			// 
			this.butDelete.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.butDelete.Icon = OpenDental.UI.EnumIcons.DeleteX;
			this.butDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.butDelete.Location = new System.Drawing.Point(10, 579);
			this.butDelete.Name = "butDelete";
			this.butDelete.Size = new System.Drawing.Size(75, 24);
			this.butDelete.TabIndex = 13;
			this.butDelete.Text = "Delete";
			this.butDelete.Click += new System.EventHandler(this.butDelete_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(513, 579);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 12;
			this.butCancel.Text = "Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// butOk
			// 
			this.butOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOk.Location = new System.Drawing.Point(432, 579);
			this.butOk.Name = "butOk";
			this.butOk.Size = new System.Drawing.Size(75, 24);
			this.butOk.TabIndex = 11;
			this.butOk.Text = "&OK";
			this.butOk.Click += new System.EventHandler(this.butOK_Click);
			// 
			// FormVitalsignEdit2014
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(602, 614);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOk);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textPulse);
			this.Controls.Add(this.butDelete);
			this.Controls.Add(this.textBMIPercentileCode);
			this.Controls.Add(this.textBMIPercentile);
			this.Controls.Add(this.labelBMIPercentile);
			this.Controls.Add(this.groupInterventions);
			this.Controls.Add(this.labelBPsExamCode);
			this.Controls.Add(this.textBPsExamCode);
			this.Controls.Add(this.labelBPdExamCode);
			this.Controls.Add(this.textBPdExamCode);
			this.Controls.Add(this.labelBPd);
			this.Controls.Add(this.labelBMIExamCode);
			this.Controls.Add(this.textBMIExamCode);
			this.Controls.Add(this.labelBMIPercentileCode);
			this.Controls.Add(this.labelHeightExamCode);
			this.Controls.Add(this.labelWeightExamCode);
			this.Controls.Add(this.comboWeightExamCode);
			this.Controls.Add(this.labelWeightCode);
			this.Controls.Add(this.comboHeightExamCode);
			this.Controls.Add(this.groupExclusion);
			this.Controls.Add(this.textBMI);
			this.Controls.Add(this.textBPd);
			this.Controls.Add(this.textBPs);
			this.Controls.Add(this.textHeight);
			this.Controls.Add(this.textWeight);
			this.Controls.Add(this.textDateTaken);
			this.Controls.Add(this.labelHeight);
			this.Controls.Add(this.labelWeight);
			this.Controls.Add(this.labelBPs);
			this.Controls.Add(this.labelBMI);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormVitalsignEdit2014";
			this.Text = "Edit Vital Sign";
			this.Load += new System.EventHandler(this.FormVitalsignEdit2014_Load);
			this.groupExclusion.ResumeLayout(false);
			this.groupExclusion.PerformLayout();
			this.groupInterventions.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label labelBMI;
		private System.Windows.Forms.Label labelBPs;
		private System.Windows.Forms.Label labelWeight;
		private System.Windows.Forms.Label labelHeight;
		private System.Windows.Forms.TextBox textDateTaken;
		private System.Windows.Forms.TextBox textBPd;
		private System.Windows.Forms.TextBox textBPs;
		private System.Windows.Forms.TextBox textWeight;
		private System.Windows.Forms.TextBox textHeight;
		private System.Windows.Forms.TextBox textBMI;
		private System.Windows.Forms.Label labelWeightCode;
		private System.Windows.Forms.ComboBox comboHeightExamCode;
		private System.Windows.Forms.ComboBox comboWeightExamCode;
		private System.Windows.Forms.Label labelHeightExamCode;
		private System.Windows.Forms.Label labelWeightExamCode;
		private System.Windows.Forms.Label labelBMIPercentileCode;
		private System.Windows.Forms.Label labelBMIExamCode;
		private System.Windows.Forms.TextBox textBMIExamCode;
		private System.Windows.Forms.Label labelBPd;
		private System.Windows.Forms.Label labelBPdExamCode;
		private System.Windows.Forms.TextBox textBPdExamCode;
		private System.Windows.Forms.Label labelBPsExamCode;
		private System.Windows.Forms.TextBox textBPsExamCode;
		private System.Windows.Forms.CheckBox checkPregnant;
		private System.Windows.Forms.TextBox textPregCode;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.GroupBox groupExclusion;
		private System.Windows.Forms.CheckBox checkNotPerf;
		private System.Windows.Forms.Label labelPregNotice;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label labelNotPerf;
		private System.Windows.Forms.TextBox textReasonCode;
		private System.Windows.Forms.GroupBox groupInterventions;
		private OpenDental.UI.GridOD gridInterventions;
		private System.Windows.Forms.TextBox textBMIPercentile;
		private System.Windows.Forms.Label labelBMIPercentile;
		private System.Windows.Forms.TextBox textBMIPercentileCode;
		private System.Windows.Forms.TextBox textReasonDescript;
		private System.Windows.Forms.TextBox textPregCodeDescript;
		private UI.Button butChangeDefault;
		private UI.Button butAdd;
		private UI.Button butOk;
		private UI.Button butCancel;
		private UI.Button butDelete;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox textPulse;
	}
}