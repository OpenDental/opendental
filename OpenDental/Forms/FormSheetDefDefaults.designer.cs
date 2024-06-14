namespace OpenDental{
	partial class FormSheetDefDefaults {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormSheetDefDefaults));
			this.butSave = new OpenDental.UI.Button();
			this.comboClinicDefault = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboRx = new OpenDental.UI.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBoxChartLayout = new OpenDental.UI.ComboBox();
			this.labelChartLayout = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBox();
			this.labelTreatmentPlan = new System.Windows.Forms.Label();
			this.comboTreatmentPlan = new OpenDental.UI.ComboBox();
			this.groupBoxOD2 = new OpenDental.UI.GroupBox();
			this.comboReceipt = new OpenDental.UI.ComboBox();
			this.comboInvoice = new OpenDental.UI.ComboBox();
			this.comboLimited = new OpenDental.UI.ComboBox();
			this.comboStatements = new OpenDental.UI.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.comboLabel = new OpenDental.UI.ComboBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butSave
			// 
			this.butSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butSave.Location = new System.Drawing.Point(487, 438);
			this.butSave.Name = "butSave";
			this.butSave.Size = new System.Drawing.Size(75, 24);
			this.butSave.TabIndex = 3;
			this.butSave.Text = "&Save";
			this.butSave.Click += new System.EventHandler(this.butSave_Click);
			// 
			// comboClinicDefault
			// 
			this.comboClinicDefault.HqDescription = "Defaults";
			this.comboClinicDefault.IncludeUnassigned = true;
			this.comboClinicDefault.Location = new System.Drawing.Point(47, 287);
			this.comboClinicDefault.Name = "comboClinicDefault";
			this.comboClinicDefault.Size = new System.Drawing.Size(239, 21);
			this.comboClinicDefault.TabIndex = 5;
			this.comboClinicDefault.SelectionChangeCommitted += new System.EventHandler(this.comboClinicDefault_SelectionChangeCommitted);
			// 
			// comboRx
			// 
			this.comboRx.Location = new System.Drawing.Point(93, 11);
			this.comboRx.Name = "comboRx";
			this.comboRx.Size = new System.Drawing.Size(200, 21);
			this.comboRx.TabIndex = 17;
			// 
			// label7
			// 
			this.label7.Location = new System.Drawing.Point(27, 12);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(61, 19);
			this.label7.TabIndex = 18;
			this.label7.Text = "Rx";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBoxChartLayout
			// 
			this.comboBoxChartLayout.Location = new System.Drawing.Point(93, 38);
			this.comboBoxChartLayout.Name = "comboBoxChartLayout";
			this.comboBoxChartLayout.Size = new System.Drawing.Size(201, 21);
			this.comboBoxChartLayout.TabIndex = 49;
			// 
			// labelChartLayout
			// 
			this.labelChartLayout.Location = new System.Drawing.Point(8, 38);
			this.labelChartLayout.Name = "labelChartLayout";
			this.labelChartLayout.Size = new System.Drawing.Size(81, 19);
			this.labelChartLayout.TabIndex = 50;
			this.labelChartLayout.Text = "Chart Layout";
			this.labelChartLayout.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// groupBoxOD1
			// 
			this.groupBoxOD1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD1.Controls.Add(this.labelTreatmentPlan);
			this.groupBoxOD1.Controls.Add(this.comboTreatmentPlan);
			this.groupBoxOD1.Controls.Add(this.comboRx);
			this.groupBoxOD1.Controls.Add(this.labelChartLayout);
			this.groupBoxOD1.Controls.Add(this.label7);
			this.groupBoxOD1.Controls.Add(this.comboBoxChartLayout);
			this.groupBoxOD1.Location = new System.Drawing.Point(39, 314);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(323, 96);
			this.groupBoxOD1.TabIndex = 52;
			// 
			// labelTreatmentPlan
			// 
			this.labelTreatmentPlan.Location = new System.Drawing.Point(8, 65);
			this.labelTreatmentPlan.Name = "labelTreatmentPlan";
			this.labelTreatmentPlan.Size = new System.Drawing.Size(81, 19);
			this.labelTreatmentPlan.TabIndex = 52;
			this.labelTreatmentPlan.Text = "Treatment Plan";
			this.labelTreatmentPlan.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboTreatmentPlan
			// 
			this.comboTreatmentPlan.Location = new System.Drawing.Point(93, 65);
			this.comboTreatmentPlan.Name = "comboTreatmentPlan";
			this.comboTreatmentPlan.Size = new System.Drawing.Size(201, 21);
			this.comboTreatmentPlan.TabIndex = 51;
			// 
			// groupBoxOD2
			// 
			this.groupBoxOD2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(249)))), ((int)(((byte)(249)))));
			this.groupBoxOD2.Controls.Add(this.comboReceipt);
			this.groupBoxOD2.Controls.Add(this.comboInvoice);
			this.groupBoxOD2.Controls.Add(this.comboLimited);
			this.groupBoxOD2.Controls.Add(this.comboStatements);
			this.groupBoxOD2.Controls.Add(this.label3);
			this.groupBoxOD2.Controls.Add(this.label8);
			this.groupBoxOD2.Controls.Add(this.label2);
			this.groupBoxOD2.Controls.Add(this.label1);
			this.groupBoxOD2.Location = new System.Drawing.Point(39, 129);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(323, 138);
			this.groupBoxOD2.TabIndex = 53;
			this.groupBoxOD2.Text = "Statements";
			// 
			// comboReceipt
			// 
			this.comboReceipt.Location = new System.Drawing.Point(88, 103);
			this.comboReceipt.Name = "comboReceipt";
			this.comboReceipt.Size = new System.Drawing.Size(200, 21);
			this.comboReceipt.TabIndex = 63;
			// 
			// comboInvoice
			// 
			this.comboInvoice.Location = new System.Drawing.Point(88, 76);
			this.comboInvoice.Name = "comboInvoice";
			this.comboInvoice.Size = new System.Drawing.Size(200, 21);
			this.comboInvoice.TabIndex = 62;
			// 
			// comboLimited
			// 
			this.comboLimited.Location = new System.Drawing.Point(88, 49);
			this.comboLimited.Name = "comboLimited";
			this.comboLimited.Size = new System.Drawing.Size(200, 21);
			this.comboLimited.TabIndex = 52;
			// 
			// comboStatements
			// 
			this.comboStatements.Location = new System.Drawing.Point(88, 22);
			this.comboStatements.Name = "comboStatements";
			this.comboStatements.Size = new System.Drawing.Size(200, 21);
			this.comboStatements.TabIndex = 51;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(18, 104);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(67, 19);
			this.label3.TabIndex = 61;
			this.label3.Text = "Receipt";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label8
			// 
			this.label8.Location = new System.Drawing.Point(21, 23);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(64, 19);
			this.label8.TabIndex = 58;
			this.label8.Text = "Default";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(18, 77);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(67, 19);
			this.label2.TabIndex = 60;
			this.label2.Text = "Invoice";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(18, 50);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(67, 19);
			this.label1.TabIndex = 59;
			this.label1.Text = "Limited";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboLabel
			// 
			this.comboLabel.Location = new System.Drawing.Point(127, 91);
			this.comboLabel.Name = "comboLabel";
			this.comboLabel.Size = new System.Drawing.Size(200, 21);
			this.comboLabel.TabIndex = 54;
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(12, 31);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(557, 42);
			this.label4.TabIndex = 55;
			this.label4.Text = resources.GetString("label4.Text");
			// 
			// label5
			// 
			this.label5.Location = new System.Drawing.Point(13, 85);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(111, 32);
			this.label5.TabIndex = 64;
			this.label5.Text = "Label assigned to \r\npatient button";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// FormSheetDefDefaults
			// 
			this.ClientSize = new System.Drawing.Size(574, 474);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.comboLabel);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboClinicDefault);
			this.Controls.Add(this.groupBoxOD2);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.butSave);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDefDefaults";
			this.Text = "Sheet Def Defaults";
			this.Load += new System.EventHandler(this.FormSheetDefDefaults_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butSave;
		private OpenDental.UI.ComboBoxClinicPicker comboClinicDefault;
		private UI.ComboBox comboRx;
		private System.Windows.Forms.Label label7;
		private UI.ComboBox comboBoxChartLayout;
		private System.Windows.Forms.Label labelChartLayout;
		private OpenDental.UI.GroupBox groupBoxOD1;
		private OpenDental.UI.GroupBox groupBoxOD2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private UI.ComboBox comboLimited;
		private UI.ComboBox comboStatements;
		private UI.ComboBox comboReceipt;
		private UI.ComboBox comboInvoice;
		private System.Windows.Forms.Label labelTreatmentPlan;
		private UI.ComboBox comboTreatmentPlan;
		private UI.ComboBox comboLabel;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label5;
	}
}