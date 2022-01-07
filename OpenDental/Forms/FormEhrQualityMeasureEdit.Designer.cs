namespace OpenDental {
	partial class FormEhrQualityMeasureEdit {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEhrQualityMeasureEdit));
			this.butClose = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.textId = new System.Windows.Forms.TextBox();
			this.textDescription = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.textDenominator = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textNumerator = new System.Windows.Forms.TextBox();
			this.label6 = new System.Windows.Forms.Label();
			this.textExclusions = new System.Windows.Forms.TextBox();
			this.label8 = new System.Windows.Forms.Label();
			this.label9 = new System.Windows.Forms.Label();
			this.textNotMet = new System.Windows.Forms.TextBox();
			this.label10 = new System.Windows.Forms.Label();
			this.label11 = new System.Windows.Forms.Label();
			this.textPerformanceRate = new System.Windows.Forms.TextBox();
			this.label12 = new System.Windows.Forms.Label();
			this.textReportingRate = new System.Windows.Forms.TextBox();
			this.label14 = new System.Windows.Forms.Label();
			this.label13 = new System.Windows.Forms.Label();
			this.gridMain = new OpenDental.UI.GridOD();
			this.textNumeratorExplain = new System.Windows.Forms.TextBox();
			this.textDenominatorExplain = new System.Windows.Forms.TextBox();
			this.label15 = new System.Windows.Forms.Label();
			this.textExclusionsExplain = new System.Windows.Forms.TextBox();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(737, 635);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(75, 23);
			this.butClose.TabIndex = 1;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(29, 12);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(100, 17);
			this.label1.TabIndex = 2;
			this.label1.Text = "Id";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textId
			// 
			this.textId.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textId.Location = new System.Drawing.Point(132, 10);
			this.textId.Name = "textId";
			this.textId.ReadOnly = true;
			this.textId.Size = new System.Drawing.Size(100, 20);
			this.textId.TabIndex = 3;
			// 
			// textDescription
			// 
			this.textDescription.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textDescription.Location = new System.Drawing.Point(132, 36);
			this.textDescription.Name = "textDescription";
			this.textDescription.ReadOnly = true;
			this.textDescription.Size = new System.Drawing.Size(288, 20);
			this.textDescription.TabIndex = 5;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(29, 38);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(100, 17);
			this.label2.TabIndex = 4;
			this.label2.Text = "Description";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textDenominator
			// 
			this.textDenominator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textDenominator.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textDenominator.Location = new System.Drawing.Point(320, 396);
			this.textDenominator.Name = "textDenominator";
			this.textDenominator.ReadOnly = true;
			this.textDenominator.Size = new System.Drawing.Size(75, 20);
			this.textDenominator.TabIndex = 15;
			this.textDenominator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label3
			// 
			this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label3.Location = new System.Drawing.Point(95, 388);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(222, 34);
			this.label3.TabIndex = 14;
			this.label3.Text = "Denominator.  Eligible Instances.  Total number of rows in the grid above";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textNumerator
			// 
			this.textNumerator.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNumerator.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textNumerator.Location = new System.Drawing.Point(320, 453);
			this.textNumerator.Name = "textNumerator";
			this.textNumerator.ReadOnly = true;
			this.textNumerator.Size = new System.Drawing.Size(75, 20);
			this.textNumerator.TabIndex = 18;
			this.textNumerator.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label6
			// 
			this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label6.Location = new System.Drawing.Point(87, 455);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(230, 17);
			this.label6.TabIndex = 17;
			this.label6.Text = "Numerator.  Meets Performance";
			this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textExclusions
			// 
			this.textExclusions.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textExclusions.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textExclusions.Location = new System.Drawing.Point(320, 510);
			this.textExclusions.Name = "textExclusions";
			this.textExclusions.ReadOnly = true;
			this.textExclusions.Size = new System.Drawing.Size(75, 20);
			this.textExclusions.TabIndex = 21;
			this.textExclusions.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label8
			// 
			this.label8.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label8.Location = new System.Drawing.Point(176, 512);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(141, 17);
			this.label8.TabIndex = 20;
			this.label8.Text = "Exclusions";
			this.label8.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label9
			// 
			this.label9.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label9.Location = new System.Drawing.Point(397, 570);
			this.label9.Name = "label9";
			this.label9.Size = new System.Drawing.Size(362, 17);
			this.label9.TabIndex = 25;
			this.label9.Text = "Denominator - Exclusions - Numerator";
			this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textNotMet
			// 
			this.textNotMet.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textNotMet.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textNotMet.Location = new System.Drawing.Point(320, 568);
			this.textNotMet.Name = "textNotMet";
			this.textNotMet.ReadOnly = true;
			this.textNotMet.Size = new System.Drawing.Size(75, 20);
			this.textNotMet.TabIndex = 24;
			this.textNotMet.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label10
			// 
			this.label10.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label10.Location = new System.Drawing.Point(176, 570);
			this.label10.Name = "label10";
			this.label10.Size = new System.Drawing.Size(141, 17);
			this.label10.TabIndex = 23;
			this.label10.Text = "Performance Not Met";
			this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label11
			// 
			this.label11.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label11.Location = new System.Drawing.Point(397, 622);
			this.label11.Name = "label11";
			this.label11.Size = new System.Drawing.Size(309, 17);
			this.label11.TabIndex = 28;
			this.label11.Text = "Numerator / (Numerator + Performance Not Met)";
			this.label11.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// textPerformanceRate
			// 
			this.textPerformanceRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textPerformanceRate.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textPerformanceRate.Location = new System.Drawing.Point(320, 620);
			this.textPerformanceRate.Name = "textPerformanceRate";
			this.textPerformanceRate.ReadOnly = true;
			this.textPerformanceRate.Size = new System.Drawing.Size(75, 20);
			this.textPerformanceRate.TabIndex = 27;
			this.textPerformanceRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label12
			// 
			this.label12.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label12.Location = new System.Drawing.Point(176, 622);
			this.label12.Name = "label12";
			this.label12.Size = new System.Drawing.Size(141, 17);
			this.label12.TabIndex = 26;
			this.label12.Text = "Performance Rate";
			this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textReportingRate
			// 
			this.textReportingRate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.textReportingRate.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textReportingRate.Location = new System.Drawing.Point(320, 594);
			this.textReportingRate.Name = "textReportingRate";
			this.textReportingRate.ReadOnly = true;
			this.textReportingRate.Size = new System.Drawing.Size(75, 20);
			this.textReportingRate.TabIndex = 30;
			this.textReportingRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
			// 
			// label14
			// 
			this.label14.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label14.Location = new System.Drawing.Point(176, 596);
			this.label14.Name = "label14";
			this.label14.Size = new System.Drawing.Size(141, 17);
			this.label14.TabIndex = 29;
			this.label14.Text = "Reporting Rate";
			this.label14.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label13
			// 
			this.label13.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label13.Location = new System.Drawing.Point(397, 598);
			this.label13.Name = "label13";
			this.label13.Size = new System.Drawing.Size(415, 17);
			this.label13.TabIndex = 31;
			this.label13.Text = "(Numerator + Performance Not Met + Exclusions) / Denominator.  Always 100%\r\n";
			this.label13.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// gridMain
			// 
			this.gridMain.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.gridMain.Location = new System.Drawing.Point(12, 62);
			this.gridMain.Name = "gridMain";
			this.gridMain.Size = new System.Drawing.Size(800, 309);
			this.gridMain.TabIndex = 13;
			this.gridMain.Title = "Audit";
			this.gridMain.TranslationName = "TableAudit";
			// 
			// textNumeratorExplain
			// 
			this.textNumeratorExplain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textNumeratorExplain.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textNumeratorExplain.Location = new System.Drawing.Point(401, 453);
			this.textNumeratorExplain.Multiline = true;
			this.textNumeratorExplain.Name = "textNumeratorExplain";
			this.textNumeratorExplain.ReadOnly = true;
			this.textNumeratorExplain.Size = new System.Drawing.Size(411, 54);
			this.textNumeratorExplain.TabIndex = 34;
			this.textNumeratorExplain.Text = "Explanation for Numerator";
			// 
			// textDenominatorExplain
			// 
			this.textDenominatorExplain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textDenominatorExplain.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textDenominatorExplain.Location = new System.Drawing.Point(401, 396);
			this.textDenominatorExplain.Multiline = true;
			this.textDenominatorExplain.Name = "textDenominatorExplain";
			this.textDenominatorExplain.ReadOnly = true;
			this.textDenominatorExplain.Size = new System.Drawing.Size(411, 54);
			this.textDenominatorExplain.TabIndex = 33;
			this.textDenominatorExplain.Text = "Explanation for Denominator";
			// 
			// label15
			// 
			this.label15.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.label15.Location = new System.Drawing.Point(400, 379);
			this.label15.Name = "label15";
			this.label15.Size = new System.Drawing.Size(86, 13);
			this.label15.TabIndex = 32;
			this.label15.Text = "Explanations";
			this.label15.TextAlign = System.Drawing.ContentAlignment.BottomLeft;
			// 
			// textExclusionsExplain
			// 
			this.textExclusionsExplain.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textExclusionsExplain.BackColor = System.Drawing.SystemColors.ControlLightLight;
			this.textExclusionsExplain.Location = new System.Drawing.Point(401, 510);
			this.textExclusionsExplain.Multiline = true;
			this.textExclusionsExplain.Name = "textExclusionsExplain";
			this.textExclusionsExplain.ReadOnly = true;
			this.textExclusionsExplain.Size = new System.Drawing.Size(411, 54);
			this.textExclusionsExplain.TabIndex = 35;
			this.textExclusionsExplain.Text = "Explanation for Exclusions";
			// 
			// FormEhrQualityMeasureEdit
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(824, 665);
			this.Controls.Add(this.textExclusionsExplain);
			this.Controls.Add(this.textNumeratorExplain);
			this.Controls.Add(this.textDenominatorExplain);
			this.Controls.Add(this.label15);
			this.Controls.Add(this.label13);
			this.Controls.Add(this.textReportingRate);
			this.Controls.Add(this.label14);
			this.Controls.Add(this.label11);
			this.Controls.Add(this.textPerformanceRate);
			this.Controls.Add(this.label12);
			this.Controls.Add(this.label9);
			this.Controls.Add(this.textNotMet);
			this.Controls.Add(this.label10);
			this.Controls.Add(this.textExclusions);
			this.Controls.Add(this.label8);
			this.Controls.Add(this.textNumerator);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.textDenominator);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.gridMain);
			this.Controls.Add(this.textDescription);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.textId);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEhrQualityMeasureEdit";
			this.Text = "Edit Quality Measure";
			this.Load += new System.EventHandler(this.FormQualityEdit_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textId;
		private System.Windows.Forms.TextBox textDescription;
		private System.Windows.Forms.Label label2;
		private OpenDental.UI.GridOD gridMain;
		private System.Windows.Forms.TextBox textDenominator;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textNumerator;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.TextBox textExclusions;
		private System.Windows.Forms.Label label8;
		private System.Windows.Forms.Label label9;
		private System.Windows.Forms.TextBox textNotMet;
		private System.Windows.Forms.Label label10;
		private System.Windows.Forms.Label label11;
		private System.Windows.Forms.TextBox textPerformanceRate;
		private System.Windows.Forms.Label label12;
		private System.Windows.Forms.TextBox textReportingRate;
		private System.Windows.Forms.Label label14;
		private System.Windows.Forms.Label label13;
		private System.Windows.Forms.TextBox textNumeratorExplain;
		private System.Windows.Forms.TextBox textDenominatorExplain;
		private System.Windows.Forms.Label label15;
		private System.Windows.Forms.TextBox textExclusionsExplain;
	}
}