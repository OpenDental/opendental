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
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.comboClinicDefault = new OpenDental.UI.ComboBoxClinicPicker();
			this.comboRx = new OpenDental.UI.ComboBoxOD();
			this.label7 = new System.Windows.Forms.Label();
			this.comboBoxChartLayout = new OpenDental.UI.ComboBoxOD();
			this.labelChartLayout = new System.Windows.Forms.Label();
			this.groupBoxOD1 = new OpenDental.UI.GroupBoxOD();
			this.labelTreatmentPlan = new System.Windows.Forms.Label();
			this.comboTreatmentPlan = new OpenDental.UI.ComboBoxOD();
			this.groupBoxOD2 = new OpenDental.UI.GroupBoxOD();
			this.comboReceipt = new OpenDental.UI.ComboBoxOD();
			this.comboInvoice = new OpenDental.UI.ComboBoxOD();
			this.comboLimited = new OpenDental.UI.ComboBoxOD();
			this.comboStatements = new OpenDental.UI.ComboBoxOD();
			this.label3 = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBoxOD1.SuspendLayout();
			this.groupBoxOD2.SuspendLayout();
			this.SuspendLayout();
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(198, 313);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(279, 313);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// comboClinicDefault
			// 
			this.comboClinicDefault.HqDescription = "Defaults";
			this.comboClinicDefault.IncludeUnassigned = true;
			this.comboClinicDefault.Location = new System.Drawing.Point(28, 173);
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
			this.groupBoxOD1.Location = new System.Drawing.Point(17, 200);
			this.groupBoxOD1.Name = "groupBoxOD1";
			this.groupBoxOD1.Size = new System.Drawing.Size(303, 96);
			this.groupBoxOD1.TabIndex = 52;
			this.groupBoxOD1.TabStop = false;
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
			this.groupBoxOD2.Location = new System.Drawing.Point(17, 20);
			this.groupBoxOD2.Name = "groupBoxOD2";
			this.groupBoxOD2.Size = new System.Drawing.Size(303, 138);
			this.groupBoxOD2.TabIndex = 53;
			this.groupBoxOD2.TabStop = false;
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
			// FormSheetDefDefaults
			// 
			this.ClientSize = new System.Drawing.Size(367, 349);
			this.Controls.Add(this.comboClinicDefault);
			this.Controls.Add(this.groupBoxOD2);
			this.Controls.Add(this.groupBoxOD1);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormSheetDefDefaults";
			this.Text = "Sheet Def Defaults";
			this.Load += new System.EventHandler(this.FormSheetDefDefaults_Load);
			this.groupBoxOD1.ResumeLayout(false);
			this.groupBoxOD2.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private OpenDental.UI.ComboBoxClinicPicker comboClinicDefault;
		private UI.ComboBoxOD comboRx;
		private System.Windows.Forms.Label label7;
		private UI.ComboBoxOD comboBoxChartLayout;
		private System.Windows.Forms.Label labelChartLayout;
		private UI.GroupBoxOD groupBoxOD1;
		private UI.GroupBoxOD groupBoxOD2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private UI.ComboBoxOD comboLimited;
		private UI.ComboBoxOD comboStatements;
		private UI.ComboBoxOD comboReceipt;
		private UI.ComboBoxOD comboInvoice;
		private System.Windows.Forms.Label labelTreatmentPlan;
		private UI.ComboBoxOD comboTreatmentPlan;
	}
}