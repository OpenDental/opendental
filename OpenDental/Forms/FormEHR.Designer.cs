namespace OpenDental {
	partial class FormEHR {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEHR));
			this.butClose = new System.Windows.Forms.Button();
			this.butMeasures = new System.Windows.Forms.Button();
			this.butHash = new System.Windows.Forms.Button();
			this.butEncryption = new System.Windows.Forms.Button();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.but2014CQM = new System.Windows.Forms.Button();
			this.butEhrNotPerformed = new System.Windows.Forms.Button();
			this.groupBox5 = new System.Windows.Forms.GroupBox();
			this.butVaccines = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.labelProvPat = new System.Windows.Forms.Label();
			this.labelProvUser = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.butAmendments = new System.Windows.Forms.Button();
			this.butEncounters = new System.Windows.Forms.Button();
			this.butInterventions = new System.Windows.Forms.Button();
			this.butCarePlans = new System.Windows.Forms.Button();
			this.but2011Labs = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.butMeasureEvent = new System.Windows.Forms.Button();
			this.gridMu = new OpenDental.UI.GridOD();
			this.butClinicalSummary = new System.Windows.Forms.Button();
			this.butPatList = new System.Windows.Forms.Button();
			this.groupBox4.SuspendLayout();
			this.groupBox5.SuspendLayout();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butClose
			// 
			this.butClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butClose.Location = new System.Drawing.Point(713, 663);
			this.butClose.Name = "butClose";
			this.butClose.Size = new System.Drawing.Size(86, 23);
			this.butClose.TabIndex = 9;
			this.butClose.Text = "Close";
			this.butClose.UseVisualStyleBackColor = true;
			this.butClose.Click += new System.EventHandler(this.butClose_Click);
			// 
			// butMeasures
			// 
			this.butMeasures.Location = new System.Drawing.Point(10, 19);
			this.butMeasures.Name = "butMeasures";
			this.butMeasures.Size = new System.Drawing.Size(84, 23);
			this.butMeasures.TabIndex = 11;
			this.butMeasures.Text = "Measure Calc";
			this.butMeasures.UseVisualStyleBackColor = true;
			this.butMeasures.Click += new System.EventHandler(this.butMeasures_Click);
			// 
			// butHash
			// 
			this.butHash.Location = new System.Drawing.Point(10, 19);
			this.butHash.Name = "butHash";
			this.butHash.Size = new System.Drawing.Size(84, 23);
			this.butHash.TabIndex = 13;
			this.butHash.Text = "Hash";
			this.butHash.UseVisualStyleBackColor = true;
			this.butHash.Click += new System.EventHandler(this.butHash_Click);
			// 
			// butEncryption
			// 
			this.butEncryption.Location = new System.Drawing.Point(10, 48);
			this.butEncryption.Name = "butEncryption";
			this.butEncryption.Size = new System.Drawing.Size(84, 23);
			this.butEncryption.TabIndex = 17;
			this.butEncryption.Text = "Encryption";
			this.butEncryption.UseVisualStyleBackColor = true;
			this.butEncryption.Click += new System.EventHandler(this.butEncryption_Click);
			// 
			// groupBox4
			// 
			this.groupBox4.Controls.Add(this.but2014CQM);
			this.groupBox4.Controls.Add(this.butMeasures);
			this.groupBox4.Location = new System.Drawing.Point(702, 81);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.Size = new System.Drawing.Size(104, 83);
			this.groupBox4.TabIndex = 25;
			this.groupBox4.TabStop = false;
			this.groupBox4.Text = "For All Patients";
			// 
			// but2014CQM
			// 
			this.but2014CQM.Location = new System.Drawing.Point(10, 48);
			this.but2014CQM.Name = "but2014CQM";
			this.but2014CQM.Size = new System.Drawing.Size(84, 23);
			this.but2014CQM.TabIndex = 21;
			this.but2014CQM.Text = "Quality Meas";
			this.but2014CQM.UseVisualStyleBackColor = true;
			this.but2014CQM.Click += new System.EventHandler(this.but2014CQM_Click);
			// 
			// butEhrNotPerformed
			// 
			this.butEhrNotPerformed.Location = new System.Drawing.Point(712, 317);
			this.butEhrNotPerformed.Name = "butEhrNotPerformed";
			this.butEhrNotPerformed.Size = new System.Drawing.Size(84, 23);
			this.butEhrNotPerformed.TabIndex = 38;
			this.butEhrNotPerformed.Text = "Not Performed";
			this.butEhrNotPerformed.UseVisualStyleBackColor = true;
			this.butEhrNotPerformed.Click += new System.EventHandler(this.butEhrNotPerformed_Click);
			// 
			// groupBox5
			// 
			this.groupBox5.Controls.Add(this.butHash);
			this.groupBox5.Controls.Add(this.butEncryption);
			this.groupBox5.Location = new System.Drawing.Point(702, 170);
			this.groupBox5.Name = "groupBox5";
			this.groupBox5.Size = new System.Drawing.Size(104, 83);
			this.groupBox5.TabIndex = 26;
			this.groupBox5.TabStop = false;
			this.groupBox5.Text = "Tools";
			// 
			// butVaccines
			// 
			this.butVaccines.Location = new System.Drawing.Point(713, 259);
			this.butVaccines.Name = "butVaccines";
			this.butVaccines.Size = new System.Drawing.Size(84, 23);
			this.butVaccines.TabIndex = 27;
			this.butVaccines.Text = "Vaccines";
			this.butVaccines.UseVisualStyleBackColor = true;
			this.butVaccines.Click += new System.EventHandler(this.butVaccines_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(128, 18);
			this.label1.TabIndex = 28;
			this.label1.Text = "Provider for this patient:";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(12, 30);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(128, 18);
			this.label2.TabIndex = 29;
			this.label2.Text = "Provider logged on:";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvPat
			// 
			this.labelProvPat.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProvPat.ForeColor = System.Drawing.Color.DarkRed;
			this.labelProvPat.Location = new System.Drawing.Point(135, 8);
			this.labelProvPat.Name = "labelProvPat";
			this.labelProvPat.Size = new System.Drawing.Size(426, 18);
			this.labelProvPat.TabIndex = 30;
			this.labelProvPat.Text = "Abbr - ProvLName, ProvFName";
			this.labelProvPat.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelProvUser
			// 
			this.labelProvUser.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProvUser.ForeColor = System.Drawing.Color.DarkRed;
			this.labelProvUser.Location = new System.Drawing.Point(135, 29);
			this.labelProvUser.Name = "labelProvUser";
			this.labelProvUser.Size = new System.Drawing.Size(426, 18);
			this.labelProvUser.TabIndex = 31;
			this.labelProvUser.Text = "Abbr - ProvLName, ProvFName";
			this.labelProvUser.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(6, 53);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(688, 32);
			this.label3.TabIndex = 33;
			this.label3.Text = resources.GetString("label3.Text");
			// 
			// butAmendments
			// 
			this.butAmendments.Location = new System.Drawing.Point(712, 288);
			this.butAmendments.Name = "butAmendments";
			this.butAmendments.Size = new System.Drawing.Size(84, 23);
			this.butAmendments.TabIndex = 36;
			this.butAmendments.Text = "Amendments";
			this.butAmendments.UseVisualStyleBackColor = true;
			this.butAmendments.Click += new System.EventHandler(this.butAmendments_Click);
			// 
			// butEncounters
			// 
			this.butEncounters.Location = new System.Drawing.Point(712, 346);
			this.butEncounters.Name = "butEncounters";
			this.butEncounters.Size = new System.Drawing.Size(84, 23);
			this.butEncounters.TabIndex = 39;
			this.butEncounters.Text = "Encounters";
			this.butEncounters.UseVisualStyleBackColor = true;
			this.butEncounters.Click += new System.EventHandler(this.butEncounters_Click);
			// 
			// butInterventions
			// 
			this.butInterventions.Location = new System.Drawing.Point(712, 375);
			this.butInterventions.Name = "butInterventions";
			this.butInterventions.Size = new System.Drawing.Size(84, 23);
			this.butInterventions.TabIndex = 40;
			this.butInterventions.Text = "Interventions";
			this.butInterventions.UseVisualStyleBackColor = true;
			this.butInterventions.Click += new System.EventHandler(this.butInterventions_Click);
			// 
			// butCarePlans
			// 
			this.butCarePlans.Location = new System.Drawing.Point(712, 404);
			this.butCarePlans.Name = "butCarePlans";
			this.butCarePlans.Size = new System.Drawing.Size(84, 23);
			this.butCarePlans.TabIndex = 42;
			this.butCarePlans.Text = "Care Plans";
			this.butCarePlans.UseVisualStyleBackColor = true;
			this.butCarePlans.Click += new System.EventHandler(this.butCarePlans_Click);
			// 
			// but2011Labs
			// 
			this.but2011Labs.Location = new System.Drawing.Point(10, 19);
			this.but2011Labs.Name = "but2011Labs";
			this.but2011Labs.Size = new System.Drawing.Size(84, 23);
			this.but2011Labs.TabIndex = 43;
			this.but2011Labs.Text = "Labs/Rads";
			this.but2011Labs.UseVisualStyleBackColor = true;
			this.but2011Labs.Click += new System.EventHandler(this.but2011Labs_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.but2011Labs);
			this.groupBox1.Location = new System.Drawing.Point(702, 495);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(104, 53);
			this.groupBox1.TabIndex = 26;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "2011 Ed.";
			// 
			// butMeasureEvent
			// 
			this.butMeasureEvent.Location = new System.Drawing.Point(712, 554);
			this.butMeasureEvent.Name = "butMeasureEvent";
			this.butMeasureEvent.Size = new System.Drawing.Size(84, 23);
			this.butMeasureEvent.TabIndex = 43;
			this.butMeasureEvent.Text = "Edit Events";
			this.butMeasureEvent.UseVisualStyleBackColor = true;
			this.butMeasureEvent.Click += new System.EventHandler(this.butMeasureEvent_Click);
			// 
			// gridMu
			// 
			this.gridMu.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
			this.gridMu.Location = new System.Drawing.Point(6, 88);
			this.gridMu.Name = "gridMu";
			this.gridMu.SelectionMode = OpenDental.UI.GridSelectionMode.None;
			this.gridMu.Size = new System.Drawing.Size(688, 598);
			this.gridMu.TabIndex = 24;
			this.gridMu.Title = "Stage 1 Meaningful Use for this patient";
			this.gridMu.TranslationName = "TableMeaningfulUse";
			this.gridMu.CellClick += new OpenDental.UI.ODGridClickEventHandler(this.gridMu_CellClick);
			// 
			// butClinicalSummary
			// 
			this.butClinicalSummary.Location = new System.Drawing.Point(712, 433);
			this.butClinicalSummary.Name = "butClinicalSummary";
			this.butClinicalSummary.Size = new System.Drawing.Size(84, 23);
			this.butClinicalSummary.TabIndex = 44;
			this.butClinicalSummary.Text = "Clinical Summ.";
			this.butClinicalSummary.UseVisualStyleBackColor = true;
			this.butClinicalSummary.Click += new System.EventHandler(this.butClinicalSummary_Click);
			// 
			// butPatList
			// 
			this.butPatList.Location = new System.Drawing.Point(712, 462);
			this.butPatList.Name = "butPatList";
			this.butPatList.Size = new System.Drawing.Size(84, 23);
			this.butPatList.TabIndex = 45;
			this.butPatList.Text = "Patient List";
			this.butPatList.UseVisualStyleBackColor = true;
			this.butPatList.Click += new System.EventHandler(this.butPatList_Click);
			// 
			// FormEHR
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(817, 696);
			this.Controls.Add(this.butPatList);
			this.Controls.Add(this.butClinicalSummary);
			this.Controls.Add(this.butMeasureEvent);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butCarePlans);
			this.Controls.Add(this.butInterventions);
			this.Controls.Add(this.butEncounters);
			this.Controls.Add(this.butEhrNotPerformed);
			this.Controls.Add(this.butAmendments);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.labelProvUser);
			this.Controls.Add(this.labelProvPat);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butVaccines);
			this.Controls.Add(this.groupBox5);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.gridMu);
			this.Controls.Add(this.butClose);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "FormEHR";
			this.Text = "EHR";
			this.Load += new System.EventHandler(this.FormEHR_Load);
			this.Shown += new System.EventHandler(this.FormEHR_Shown);
			this.groupBox4.ResumeLayout(false);
			this.groupBox5.ResumeLayout(false);
			this.groupBox1.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button butClose;
		private System.Windows.Forms.Button butMeasures;
		private System.Windows.Forms.Button butHash;
		private System.Windows.Forms.Button butEncryption;
		private OpenDental.UI.GridOD gridMu;
		private System.Windows.Forms.GroupBox groupBox4;
		private System.Windows.Forms.GroupBox groupBox5;
		private System.Windows.Forms.Button butVaccines;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label labelProvPat;
		private System.Windows.Forms.Label labelProvUser;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button butAmendments;
		private System.Windows.Forms.Button butEhrNotPerformed;
		private System.Windows.Forms.Button but2014CQM;
		private System.Windows.Forms.Button butEncounters;
		private System.Windows.Forms.Button butInterventions;
		private System.Windows.Forms.Button butCarePlans;
		private System.Windows.Forms.Button but2011Labs;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Button butMeasureEvent;
		private System.Windows.Forms.Button butClinicalSummary;
		private System.Windows.Forms.Button butPatList;
	}
}