namespace OpenDental{
	partial class FormEhrSettings {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrSettings));
			this.comboMU2 = new System.Windows.Forms.ComboBox();
			this.groupEncounter = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.butEncounterTool = new OpenDental.UI.Button();
			this.textEncCodeDescript = new System.Windows.Forms.TextBox();
			this.butEncCpt = new OpenDental.UI.Button();
			this.comboEncCodes = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.labelEncWarning = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.textEncCodeValue = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.butEncHcpcs = new OpenDental.UI.Button();
			this.label3 = new System.Windows.Forms.Label();
			this.butEncSnomed = new OpenDental.UI.Button();
			this.butEncCdt = new OpenDental.UI.Button();
			this.groupPregnancy = new System.Windows.Forms.GroupBox();
			this.textPregCodeDescript = new System.Windows.Forms.TextBox();
			this.comboPregCodes = new System.Windows.Forms.ComboBox();
			this.labelPregWarning = new System.Windows.Forms.Label();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textPregCodeValue = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.butPregIcd9 = new OpenDental.UI.Button();
			this.butPregSnomed = new OpenDental.UI.Button();
			this.butPregIcd10 = new OpenDental.UI.Button();
			this.label11 = new System.Windows.Forms.Label();
			this.label12 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.groupGlobalSettings = new System.Windows.Forms.GroupBox();
			this.checkAutoWebmails = new System.Windows.Forms.CheckBox();
			this.checkAlertHighSeverity = new System.Windows.Forms.CheckBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.groupEncounter.SuspendLayout();
			this.groupPregnancy.SuspendLayout();
			this.groupGlobalSettings.SuspendLayout();
			this.SuspendLayout();
			// 
			// comboMU2
			// 
			this.comboMU2.Location = new System.Drawing.Point(282, 14);
			this.comboMU2.Name = "comboMU2";
			this.comboMU2.RightToLeft = System.Windows.Forms.RightToLeft.No;
			this.comboMU2.Size = new System.Drawing.Size(162, 21);
			this.comboMU2.TabIndex = 5;
			this.comboMU2.SelectionChangeCommitted += new System.EventHandler(this.checkMU2_SelectionChangeCommitted);
			// 
			// groupEncounter
			// 
			this.groupEncounter.Controls.Add(this.label7);
			this.groupEncounter.Controls.Add(this.butEncounterTool);
			this.groupEncounter.Controls.Add(this.textEncCodeDescript);
			this.groupEncounter.Controls.Add(this.butEncCpt);
			this.groupEncounter.Controls.Add(this.comboEncCodes);
			this.groupEncounter.Controls.Add(this.label5);
			this.groupEncounter.Controls.Add(this.labelEncWarning);
			this.groupEncounter.Controls.Add(this.label6);
			this.groupEncounter.Controls.Add(this.textEncCodeValue);
			this.groupEncounter.Controls.Add(this.label1);
			this.groupEncounter.Controls.Add(this.label2);
			this.groupEncounter.Controls.Add(this.label4);
			this.groupEncounter.Controls.Add(this.butEncHcpcs);
			this.groupEncounter.Controls.Add(this.label3);
			this.groupEncounter.Controls.Add(this.butEncSnomed);
			this.groupEncounter.Controls.Add(this.butEncCdt);
			this.groupEncounter.Location = new System.Drawing.Point(12, 70);
			this.groupEncounter.Name = "groupEncounter";
			this.groupEncounter.Size = new System.Drawing.Size(453, 298);
			this.groupEncounter.TabIndex = 119;
			this.groupEncounter.TabStop = false;
			this.groupEncounter.Text = "Default Encounter Code";
			// 
			// label7
			// 
			this.label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label7.Location = new System.Drawing.Point(90, 272);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(357, 17);
			this.label7.TabIndex = 131;
			this.label7.Text = "Insert encounters for a specified code for a specified date range.";
			this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butEncounterTool
			// 
			this.butEncounterTool.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butEncounterTool.Location = new System.Drawing.Point(9, 268);
			this.butEncounterTool.Name = "butEncounterTool";
			this.butEncounterTool.Size = new System.Drawing.Size(75, 24);
			this.butEncounterTool.TabIndex = 122;
			this.butEncounterTool.Text = "Insert Encs";
			this.butEncounterTool.Click += new System.EventHandler(this.butEncounterTool_Click);
			// 
			// textEncCodeDescript
			// 
			this.textEncCodeDescript.AcceptsTab = true;
			this.textEncCodeDescript.Location = new System.Drawing.Point(124, 102);
			this.textEncCodeDescript.MaxLength = 2147483647;
			this.textEncCodeDescript.Multiline = true;
			this.textEncCodeDescript.Name = "textEncCodeDescript";
			this.textEncCodeDescript.ReadOnly = true;
			this.textEncCodeDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textEncCodeDescript.Size = new System.Drawing.Size(323, 46);
			this.textEncCodeDescript.TabIndex = 108;
			// 
			// butEncCpt
			// 
			this.butEncCpt.Location = new System.Drawing.Point(288, 181);
			this.butEncCpt.Name = "butEncCpt";
			this.butEncCpt.Size = new System.Drawing.Size(75, 24);
			this.butEncCpt.TabIndex = 130;
			this.butEncCpt.Text = "CPT";
			this.butEncCpt.Click += new System.EventHandler(this.butEncCpt_Click);
			// 
			// comboEncCodes
			// 
			this.comboEncCodes.FormattingEnabled = true;
			this.comboEncCodes.Location = new System.Drawing.Point(124, 77);
			this.comboEncCodes.Name = "comboEncCodes";
			this.comboEncCodes.Size = new System.Drawing.Size(158, 21);
			this.comboEncCodes.TabIndex = 122;
			this.comboEncCodes.SelectionChangeCommitted += new System.EventHandler(this.comboEncCodes_SelectionChangeCommitted);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label5.Location = new System.Drawing.Point(6, 210);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(115, 17);
			this.label5.TabIndex = 127;
			this.label5.Text = "Code";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelEncWarning
			// 
			this.labelEncWarning.ForeColor = System.Drawing.Color.Red;
			this.labelEncWarning.Location = new System.Drawing.Point(29, 232);
			this.labelEncWarning.Name = "labelEncWarning";
			this.labelEncWarning.Size = new System.Drawing.Size(403, 26);
			this.labelEncWarning.TabIndex = 129;
			this.labelEncWarning.Text = "Warning: In order for patients to be considered for CQM calculations, you will ha" +
    "ve to manually create encounters with a qualified code specific to each measure." +
    "";
			// 
			// label6
			// 
			this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label6.Location = new System.Drawing.Point(285, 78);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(162, 17);
			this.label6.TabIndex = 128;
			this.label6.Text = "\'none\' disables auto encounters";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textEncCodeValue
			// 
			this.textEncCodeValue.Location = new System.Drawing.Point(124, 209);
			this.textEncCodeValue.Name = "textEncCodeValue";
			this.textEncCodeValue.ReadOnly = true;
			this.textEncCodeValue.Size = new System.Drawing.Size(158, 20);
			this.textEncCodeValue.TabIndex = 126;
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(6, 16);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(441, 55);
			this.label1.TabIndex = 4;
			this.label1.Text = resources.GetString("label1.Text");
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(6, 102);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(115, 17);
			this.label2.TabIndex = 109;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label4.Location = new System.Drawing.Point(6, 78);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(115, 17);
			this.label4.TabIndex = 110;
			this.label4.Text = "Recommended Codes";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butEncHcpcs
			// 
			this.butEncHcpcs.Location = new System.Drawing.Point(369, 181);
			this.butEncHcpcs.Name = "butEncHcpcs";
			this.butEncHcpcs.Size = new System.Drawing.Size(75, 24);
			this.butEncHcpcs.TabIndex = 124;
			this.butEncHcpcs.Text = "HCPCS";
			this.butEncHcpcs.Click += new System.EventHandler(this.butEncHcpcs_Click);
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(124, 152);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(276, 26);
			this.label3.TabIndex = 113;
			this.label3.Text = "Choosing a code not in the recommended list might make it more difficult to incre" +
    "ase your CQM percentages.";
			// 
			// butEncSnomed
			// 
			this.butEncSnomed.Location = new System.Drawing.Point(124, 181);
			this.butEncSnomed.Name = "butEncSnomed";
			this.butEncSnomed.Size = new System.Drawing.Size(77, 24);
			this.butEncSnomed.TabIndex = 125;
			this.butEncSnomed.Text = "SNOMED CT";
			this.butEncSnomed.Click += new System.EventHandler(this.butEncSnomed_Click);
			// 
			// butEncCdt
			// 
			this.butEncCdt.Location = new System.Drawing.Point(207, 181);
			this.butEncCdt.Name = "butEncCdt";
			this.butEncCdt.Size = new System.Drawing.Size(75, 24);
			this.butEncCdt.TabIndex = 123;
			this.butEncCdt.Text = "CDT";
			this.butEncCdt.Click += new System.EventHandler(this.butEncCdt_Click);
			// 
			// groupPregnancy
			// 
			this.groupPregnancy.Controls.Add(this.textPregCodeDescript);
			this.groupPregnancy.Controls.Add(this.comboPregCodes);
			this.groupPregnancy.Controls.Add(this.labelPregWarning);
			this.groupPregnancy.Controls.Add(this.label8);
			this.groupPregnancy.Controls.Add(this.label9);
			this.groupPregnancy.Controls.Add(this.textPregCodeValue);
			this.groupPregnancy.Controls.Add(this.label10);
			this.groupPregnancy.Controls.Add(this.butPregIcd9);
			this.groupPregnancy.Controls.Add(this.butPregSnomed);
			this.groupPregnancy.Controls.Add(this.butPregIcd10);
			this.groupPregnancy.Controls.Add(this.label11);
			this.groupPregnancy.Controls.Add(this.label12);
			this.groupPregnancy.Controls.Add(this.label13);
			this.groupPregnancy.Location = new System.Drawing.Point(12, 374);
			this.groupPregnancy.Name = "groupPregnancy";
			this.groupPregnancy.Size = new System.Drawing.Size(453, 265);
			this.groupPregnancy.TabIndex = 120;
			this.groupPregnancy.TabStop = false;
			this.groupPregnancy.Text = "Default Pregnancy Diagnosis Code";
			// 
			// textPregCodeDescript
			// 
			this.textPregCodeDescript.AcceptsTab = true;
			this.textPregCodeDescript.Location = new System.Drawing.Point(124, 102);
			this.textPregCodeDescript.MaxLength = 2147483647;
			this.textPregCodeDescript.Multiline = true;
			this.textPregCodeDescript.Name = "textPregCodeDescript";
			this.textPregCodeDescript.ReadOnly = true;
			this.textPregCodeDescript.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textPregCodeDescript.Size = new System.Drawing.Size(323, 46);
			this.textPregCodeDescript.TabIndex = 131;
			// 
			// comboPregCodes
			// 
			this.comboPregCodes.FormattingEnabled = true;
			this.comboPregCodes.Location = new System.Drawing.Point(124, 77);
			this.comboPregCodes.Name = "comboPregCodes";
			this.comboPregCodes.Size = new System.Drawing.Size(158, 21);
			this.comboPregCodes.TabIndex = 130;
			this.comboPregCodes.SelectionChangeCommitted += new System.EventHandler(this.comboPregCodes_SelectionChangeCommitted);
			// 
			// labelPregWarning
			// 
			this.labelPregWarning.ForeColor = System.Drawing.Color.Red;
			this.labelPregWarning.Location = new System.Drawing.Point(48, 232);
			this.labelPregWarning.Name = "labelPregWarning";
			this.labelPregWarning.Size = new System.Drawing.Size(366, 26);
			this.labelPregWarning.TabIndex = 129;
			this.labelPregWarning.Text = "Warning: In order for patients to be excluded from certain CQM calculations, you " +
    "will have to manually enter a pregnancy Dx with a qualified code.";
			// 
			// label8
			// 
			this.label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label8.Location = new System.Drawing.Point(285, 78);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(162, 17);
			this.label8.TabIndex = 128;
			this.label8.Text = "\'none\' disables auto preg Dx\'s";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label9
			// 
			this.label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label9.Location = new System.Drawing.Point(6, 210);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(115, 17);
			this.label9.TabIndex = 127;
			this.label9.Text = "Code";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textPregCodeValue
			// 
			this.textPregCodeValue.Location = new System.Drawing.Point(124, 209);
			this.textPregCodeValue.Name = "textPregCodeValue";
			this.textPregCodeValue.ReadOnly = true;
			this.textPregCodeValue.Size = new System.Drawing.Size(158, 20);
			this.textPregCodeValue.TabIndex = 126;
			// 
			// label10
			// 
			this.label10.Location = new System.Drawing.Point(6, 102);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(115, 17);
			this.label10.TabIndex = 109;
			this.label10.Text = "Description";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// butPregIcd9
			// 
			this.butPregIcd9.Location = new System.Drawing.Point(207, 181);
			this.butPregIcd9.Name = "butPregIcd9";
			this.butPregIcd9.Size = new System.Drawing.Size(75, 24);
			this.butPregIcd9.TabIndex = 124;
			this.butPregIcd9.Text = "ICD9CM";
			this.butPregIcd9.Click += new System.EventHandler(this.butPregIcd9_Click);
			// 
			// butPregSnomed
			// 
			this.butPregSnomed.Location = new System.Drawing.Point(124, 181);
			this.butPregSnomed.Name = "butPregSnomed";
			this.butPregSnomed.Size = new System.Drawing.Size(77, 24);
			this.butPregSnomed.TabIndex = 125;
			this.butPregSnomed.Text = "SNOMED CT";
			this.butPregSnomed.Click += new System.EventHandler(this.butPregSnomed_Click);
			// 
			// butPregIcd10
			// 
			this.butPregIcd10.Location = new System.Drawing.Point(288, 181);
			this.butPregIcd10.Name = "butPregIcd10";
			this.butPregIcd10.Size = new System.Drawing.Size(75, 24);
			this.butPregIcd10.TabIndex = 123;
			this.butPregIcd10.Text = "ICD10CM";
			this.butPregIcd10.Click += new System.EventHandler(this.butPregIcd10_Click);
			// 
			// label11
			// 
			this.label11.Location = new System.Drawing.Point(6, 16);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(441, 55);
			this.label11.TabIndex = 4;
			this.label11.Text = resources.GetString("label11.Text");
			// 
			// label12
			// 
			this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.label12.Location = new System.Drawing.Point(6, 78);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(115, 17);
			this.label12.TabIndex = 110;
			this.label12.Text = "Recommended Codes";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Location = new System.Drawing.Point(124, 152);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(323, 26);
			this.label13.TabIndex = 113;
			this.label13.Text = "Choosing a code not in the recommended list will generate a Dx that might not exc" +
    "lude the patient from certain measures.";
			// 
			// groupGlobalSettings
			// 
			this.groupGlobalSettings.Controls.Add(this.checkAutoWebmails);
			this.groupGlobalSettings.Controls.Add(this.checkAlertHighSeverity);
			this.groupGlobalSettings.Controls.Add(this.comboMU2);
			this.groupGlobalSettings.Location = new System.Drawing.Point(12, 4);
			this.groupGlobalSettings.Name = "groupGlobalSettings";
			this.groupGlobalSettings.Size = new System.Drawing.Size(453, 60);
			this.groupGlobalSettings.TabIndex = 121;
			this.groupGlobalSettings.TabStop = false;
			this.groupGlobalSettings.Text = "Global Settings";
			// 
			// checkAutoWebmails
			// 
			this.checkAutoWebmails.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoWebmails.Location = new System.Drawing.Point(6, 38);
			this.checkAutoWebmails.Name = "checkAutoWebmails";
			this.checkAutoWebmails.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkAutoWebmails.Size = new System.Drawing.Size(337, 20);
			this.checkAutoWebmails.TabIndex = 7;
			this.checkAutoWebmails.Text = "Automatically send Summary of Care webmails";
			this.checkAutoWebmails.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAutoWebmails.UseVisualStyleBackColor = true;
			// 
			// checkAlertHighSeverity
			// 
			this.checkAlertHighSeverity.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAlertHighSeverity.Location = new System.Drawing.Point(6, 14);
			this.checkAlertHighSeverity.Name = "checkAlertHighSeverity";
			this.checkAlertHighSeverity.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
			this.checkAlertHighSeverity.Size = new System.Drawing.Size(270, 20);
			this.checkAlertHighSeverity.TabIndex = 6;
			this.checkAlertHighSeverity.Text = "Only show high significance Rx alerts";
			this.checkAlertHighSeverity.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			this.checkAlertHighSeverity.UseVisualStyleBackColor = true;
			this.checkAlertHighSeverity.Click += new System.EventHandler(this.checkAlertHighSeverity_Click);
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(309, 649);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 3;
			this.butOK.Text = "&OK";
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(390, 649);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 2;
			this.butCancel.Text = "&Cancel";
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEhrSettings
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(477, 685);
			this.Controls.Add(this.groupGlobalSettings);
			this.Controls.Add(this.groupPregnancy);
			this.Controls.Add(this.groupEncounter);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.butCancel);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrSettings";
			this.Text = "EHR Settings";
			this.Load += new System.EventHandler(this.FormEhrSettings_Load);
			this.groupEncounter.ResumeLayout(false);
			this.groupEncounter.PerformLayout();
			this.groupPregnancy.ResumeLayout(false);
			this.groupPregnancy.PerformLayout();
			this.groupGlobalSettings.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private OpenDental.UI.Button butOK;
		private OpenDental.UI.Button butCancel;
		private System.Windows.Forms.ComboBox comboMU2;
		private System.Windows.Forms.GroupBox groupEncounter;
		private System.Windows.Forms.Label labelEncWarning;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textEncCodeValue;
		private System.Windows.Forms.Label label2;
		private UI.Button butEncHcpcs;
		private UI.Button butEncSnomed;
		private UI.Button butEncCdt;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupPregnancy;
		private System.Windows.Forms.Label labelPregWarning;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textPregCodeValue;
		private System.Windows.Forms.Label label10;
		private UI.Button butPregIcd9;
		private UI.Button butPregSnomed;
		private UI.Button butPregIcd10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.GroupBox groupGlobalSettings;
		private System.Windows.Forms.ComboBox comboEncCodes;
		private System.Windows.Forms.ComboBox comboPregCodes;
		private UI.Button butEncCpt;
		private System.Windows.Forms.CheckBox checkAlertHighSeverity;
		private System.Windows.Forms.TextBox textEncCodeDescript;
		private System.Windows.Forms.TextBox textPregCodeDescript;
		private UI.Button butEncounterTool;
		private System.Windows.Forms.Label label7;
		private System.Windows.Forms.CheckBox checkAutoWebmails;
	}
}