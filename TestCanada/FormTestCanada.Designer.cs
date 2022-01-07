namespace TestCanada {
	partial class FormTestCanada {
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
			this.butObjects = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.butNewDb = new System.Windows.Forms.Button();
			this.butClear = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.butScripts = new System.Windows.Forms.Button();
			this.textResults = new System.Windows.Forms.TextBox();
			this.checkEligibility = new System.Windows.Forms.CheckBox();
			this.checkClaims = new System.Windows.Forms.CheckBox();
			this.checkClaimReversals = new System.Windows.Forms.CheckBox();
			this.checkOutstanding = new System.Windows.Forms.CheckBox();
			this.checkPredeterm = new System.Windows.Forms.CheckBox();
			this.checkPayReconcil = new System.Windows.Forms.CheckBox();
			this.checkSumReconcil = new System.Windows.Forms.CheckBox();
			this.checkShowForms = new System.Windows.Forms.CheckBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textSingleScript = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.butShowEtrans = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.radioCompareInput = new System.Windows.Forms.RadioButton();
			this.radioCompareOutput = new System.Windows.Forms.RadioButton();
			this.textCompareFilePath = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.buttonCompFileBrowse = new System.Windows.Forms.Button();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.butCompare = new System.Windows.Forms.Button();
			this.richTextCompare = new System.Windows.Forms.RichTextBox();
			this.groupBox1.SuspendLayout();
			this.SuspendLayout();
			// 
			// butObjects
			// 
			this.butObjects.Location = new System.Drawing.Point(12,64);
			this.butObjects.Name = "butObjects";
			this.butObjects.Size = new System.Drawing.Size(87,23);
			this.butObjects.TabIndex = 10;
			this.butObjects.Text = "+ Objects";
			this.butObjects.UseVisualStyleBackColor = true;
			this.butObjects.Click += new System.EventHandler(this.butObjects_Click);
			// 
			// label2
			// 
			this.label2.Location = new System.Drawing.Point(106,14);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(505,18);
			this.label2.TabIndex = 9;
			this.label2.Text = "The scripts are all designed so that this will not normally be necessary except f" +
    "or new versions.";
			this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butNewDb
			// 
			this.butNewDb.Location = new System.Drawing.Point(12,12);
			this.butNewDb.Name = "butNewDb";
			this.butNewDb.Size = new System.Drawing.Size(87,23);
			this.butNewDb.TabIndex = 8;
			this.butNewDb.Text = "Fresh Db";
			this.butNewDb.UseVisualStyleBackColor = true;
			this.butNewDb.Click += new System.EventHandler(this.butNewDb_Click);
			// 
			// butClear
			// 
			this.butClear.Location = new System.Drawing.Point(12,38);
			this.butClear.Name = "butClear";
			this.butClear.Size = new System.Drawing.Size(87,23);
			this.butClear.TabIndex = 11;
			this.butClear.Text = "Clear";
			this.butClear.UseVisualStyleBackColor = true;
			this.butClear.Click += new System.EventHandler(this.butClear_Click);
			// 
			// label1
			// 
			this.label1.Location = new System.Drawing.Point(106,66);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(505,18);
			this.label1.TabIndex = 12;
			this.label1.Text = "Dentists, Carriers, Patients, InsPlans, Procedures, Claims";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butScripts
			// 
			this.butScripts.Location = new System.Drawing.Point(12,90);
			this.butScripts.Name = "butScripts";
			this.butScripts.Size = new System.Drawing.Size(87,23);
			this.butScripts.TabIndex = 15;
			this.butScripts.Text = "+ Script";
			this.butScripts.UseVisualStyleBackColor = true;
			this.butScripts.Click += new System.EventHandler(this.butScripts_Click);
			// 
			// textResults
			// 
			this.textResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textResults.Location = new System.Drawing.Point(12,211);
			this.textResults.Multiline = true;
			this.textResults.Name = "textResults";
			this.textResults.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textResults.Size = new System.Drawing.Size(759,207);
			this.textResults.TabIndex = 16;
			// 
			// checkEligibility
			// 
			this.checkEligibility.Checked = true;
			this.checkEligibility.CheckState = System.Windows.Forms.CheckState.Checked;
			this.checkEligibility.Location = new System.Drawing.Point(13,136);
			this.checkEligibility.Name = "checkEligibility";
			this.checkEligibility.Size = new System.Drawing.Size(161,18);
			this.checkEligibility.TabIndex = 17;
			this.checkEligibility.Text = "Eligibility 1-6";
			this.checkEligibility.UseVisualStyleBackColor = true;
			this.checkEligibility.Click += new System.EventHandler(this.checkEligibility_Click);
			// 
			// checkClaims
			// 
			this.checkClaims.Location = new System.Drawing.Point(13,154);
			this.checkClaims.Name = "checkClaims";
			this.checkClaims.Size = new System.Drawing.Size(161,18);
			this.checkClaims.TabIndex = 18;
			this.checkClaims.Text = "Claims 1-12";
			this.checkClaims.UseVisualStyleBackColor = true;
			this.checkClaims.Click += new System.EventHandler(this.checkClaims_Click);
			// 
			// checkClaimReversals
			// 
			this.checkClaimReversals.Location = new System.Drawing.Point(13,172);
			this.checkClaimReversals.Name = "checkClaimReversals";
			this.checkClaimReversals.Size = new System.Drawing.Size(161,18);
			this.checkClaimReversals.TabIndex = 19;
			this.checkClaimReversals.Text = "ClaimReversals 1-4";
			this.checkClaimReversals.UseVisualStyleBackColor = true;
			this.checkClaimReversals.Click += new System.EventHandler(this.checkClaimReversals_Click);
			// 
			// checkOutstanding
			// 
			this.checkOutstanding.Location = new System.Drawing.Point(13,190);
			this.checkOutstanding.Name = "checkOutstanding";
			this.checkOutstanding.Size = new System.Drawing.Size(182,18);
			this.checkOutstanding.TabIndex = 20;
			this.checkOutstanding.Text = "Outstanding Transactions 1-3";
			this.checkOutstanding.UseVisualStyleBackColor = true;
			this.checkOutstanding.Click += new System.EventHandler(this.checkOutstanding_Click);
			// 
			// checkPredeterm
			// 
			this.checkPredeterm.Location = new System.Drawing.Point(207,136);
			this.checkPredeterm.Name = "checkPredeterm";
			this.checkPredeterm.Size = new System.Drawing.Size(152,18);
			this.checkPredeterm.TabIndex = 21;
			this.checkPredeterm.Text = "Predeterminations 1-8";
			this.checkPredeterm.UseVisualStyleBackColor = true;
			this.checkPredeterm.Click += new System.EventHandler(this.checkPredeterm_Click);
			// 
			// checkPayReconcil
			// 
			this.checkPayReconcil.Location = new System.Drawing.Point(207,154);
			this.checkPayReconcil.Name = "checkPayReconcil";
			this.checkPayReconcil.Size = new System.Drawing.Size(215,18);
			this.checkPayReconcil.TabIndex = 22;
			this.checkPayReconcil.Text = "PaymentReconciliations 1-3";
			this.checkPayReconcil.UseVisualStyleBackColor = true;
			this.checkPayReconcil.Click += new System.EventHandler(this.checkPayReconcil_Click);
			// 
			// checkSumReconcil
			// 
			this.checkSumReconcil.Location = new System.Drawing.Point(207,172);
			this.checkSumReconcil.Name = "checkSumReconcil";
			this.checkSumReconcil.Size = new System.Drawing.Size(189,18);
			this.checkSumReconcil.TabIndex = 23;
			this.checkSumReconcil.Text = "Summary Reconciliations 1-3";
			this.checkSumReconcil.UseVisualStyleBackColor = true;
			this.checkSumReconcil.Click += new System.EventHandler(this.checkSumReconcil_Click);
			// 
			// checkShowForms
			// 
			this.checkShowForms.Location = new System.Drawing.Point(122,116);
			this.checkShowForms.Name = "checkShowForms";
			this.checkShowForms.Size = new System.Drawing.Size(185,18);
			this.checkShowForms.TabIndex = 24;
			this.checkShowForms.Text = "Show form on screen";
			this.checkShowForms.UseVisualStyleBackColor = true;
			// 
			// label3
			// 
			this.label3.Location = new System.Drawing.Point(9,115);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(56,18);
			this.label3.TabIndex = 26;
			this.label3.Text = "Script #";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSingleScript
			// 
			this.textSingleScript.Location = new System.Drawing.Point(67,115);
			this.textSingleScript.Name = "textSingleScript";
			this.textSingleScript.Size = new System.Drawing.Size(49,20);
			this.textSingleScript.TabIndex = 27;
			this.textSingleScript.Text = "2";
			// 
			// label4
			// 
			this.label4.Location = new System.Drawing.Point(106,92);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(648,18);
			this.label4.TabIndex = 28;
			this.label4.Text = "Their test environment is underpowered and can only handle one script at a time.";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// butShowEtrans
			// 
			this.butShowEtrans.Location = new System.Drawing.Point(684,90);
			this.butShowEtrans.Name = "butShowEtrans";
			this.butShowEtrans.Size = new System.Drawing.Size(87,23);
			this.butShowEtrans.TabIndex = 29;
			this.butShowEtrans.Text = "Show Etrans";
			this.butShowEtrans.UseVisualStyleBackColor = true;
			this.butShowEtrans.Click += new System.EventHandler(this.butShowEtrans_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.richTextCompare);
			this.groupBox1.Controls.Add(this.butCompare);
			this.groupBox1.Controls.Add(this.buttonCompFileBrowse);
			this.groupBox1.Controls.Add(this.label5);
			this.groupBox1.Controls.Add(this.textCompareFilePath);
			this.groupBox1.Controls.Add(this.radioCompareOutput);
			this.groupBox1.Controls.Add(this.radioCompareInput);
			this.groupBox1.Location = new System.Drawing.Point(13,424);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(758,431);
			this.groupBox1.TabIndex = 30;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Compare Output";
			// 
			// radioCompareInput
			// 
			this.radioCompareInput.AutoSize = true;
			this.radioCompareInput.Checked = true;
			this.radioCompareInput.Location = new System.Drawing.Point(36,19);
			this.radioCompareInput.Name = "radioCompareInput";
			this.radioCompareInput.Size = new System.Drawing.Size(166,17);
			this.radioCompareInput.TabIndex = 0;
			this.radioCompareInput.TabStop = true;
			this.radioCompareInput.Text = "Compare to C:\\iCA\\_nput.000";
			this.radioCompareInput.UseVisualStyleBackColor = true;
			this.radioCompareInput.Click += new System.EventHandler(this.radioCompareInput_Click);
			// 
			// radioCompareOutput
			// 
			this.radioCompareOutput.AutoSize = true;
			this.radioCompareOutput.Location = new System.Drawing.Point(218,19);
			this.radioCompareOutput.Name = "radioCompareOutput";
			this.radioCompareOutput.Size = new System.Drawing.Size(169,17);
			this.radioCompareOutput.TabIndex = 1;
			this.radioCompareOutput.Text = "Compare to C:\\iCA\\output.000";
			this.radioCompareOutput.UseVisualStyleBackColor = true;
			this.radioCompareOutput.Click += new System.EventHandler(this.radioCompareOutput_Click);
			// 
			// textCompareFilePath
			// 
			this.textCompareFilePath.Location = new System.Drawing.Point(20,74);
			this.textCompareFilePath.Name = "textCompareFilePath";
			this.textCompareFilePath.Size = new System.Drawing.Size(640,20);
			this.textCompareFilePath.TabIndex = 2;
			this.textCompareFilePath.Text = "\\\\serverfiles\\storage\\OPEN DENTAL\\Programmers Documents\\Standards (X12, ADA, etc)" +
    "\\Canada CDAnet\\Certification\\Test Transactions 2\\C_E1.NET";
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(17,58);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(153,13);
			this.label5.TabIndex = 3;
			this.label5.Text = "Path to .NET file to compare to";
			// 
			// buttonCompFileBrowse
			// 
			this.buttonCompFileBrowse.Location = new System.Drawing.Point(666,72);
			this.buttonCompFileBrowse.Name = "buttonCompFileBrowse";
			this.buttonCompFileBrowse.Size = new System.Drawing.Size(75,23);
			this.buttonCompFileBrowse.TabIndex = 4;
			this.buttonCompFileBrowse.Text = "Browse";
			this.buttonCompFileBrowse.UseVisualStyleBackColor = true;
			this.buttonCompFileBrowse.Click += new System.EventHandler(this.buttonCompFileBrowse_Click);
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// butCompare
			// 
			this.butCompare.Location = new System.Drawing.Point(334,119);
			this.butCompare.Name = "butCompare";
			this.butCompare.Size = new System.Drawing.Size(75,23);
			this.butCompare.TabIndex = 5;
			this.butCompare.Text = "Compare";
			this.butCompare.UseVisualStyleBackColor = true;
			this.butCompare.Click += new System.EventHandler(this.butCompare_Click);
			// 
			// richTextCompare
			// 
			this.richTextCompare.Font = new System.Drawing.Font("Courier New",8.25F,System.Drawing.FontStyle.Regular,System.Drawing.GraphicsUnit.Point,((byte)(0)));
			this.richTextCompare.HideSelection = false;
			this.richTextCompare.Location = new System.Drawing.Point(20,160);
			this.richTextCompare.Name = "richTextCompare";
			this.richTextCompare.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedHorizontal;
			this.richTextCompare.Size = new System.Drawing.Size(721,250);
			this.richTextCompare.TabIndex = 6;
			this.richTextCompare.Text = "";
			this.richTextCompare.WordWrap = false;
			// 
			// FormTestCanada
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F,13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(783,867);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.butShowEtrans);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.textSingleScript);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.checkShowForms);
			this.Controls.Add(this.checkSumReconcil);
			this.Controls.Add(this.checkPayReconcil);
			this.Controls.Add(this.checkPredeterm);
			this.Controls.Add(this.checkOutstanding);
			this.Controls.Add(this.checkClaimReversals);
			this.Controls.Add(this.checkClaims);
			this.Controls.Add(this.checkEligibility);
			this.Controls.Add(this.textResults);
			this.Controls.Add(this.butScripts);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.butClear);
			this.Controls.Add(this.butObjects);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.butNewDb);
			this.Name = "FormTestCanada";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "FormTestCanada";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Button butObjects;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button butNewDb;
		private System.Windows.Forms.Button butClear;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button butScripts;
		private System.Windows.Forms.TextBox textResults;
		private System.Windows.Forms.CheckBox checkEligibility;
		private System.Windows.Forms.CheckBox checkClaims;
		private System.Windows.Forms.CheckBox checkClaimReversals;
		private System.Windows.Forms.CheckBox checkOutstanding;
		private System.Windows.Forms.CheckBox checkPredeterm;
		private System.Windows.Forms.CheckBox checkPayReconcil;
		private System.Windows.Forms.CheckBox checkSumReconcil;
		private System.Windows.Forms.CheckBox checkShowForms;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textSingleScript;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button butShowEtrans;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.RadioButton radioCompareOutput;
		private System.Windows.Forms.RadioButton radioCompareInput;
		private System.Windows.Forms.Button buttonCompFileBrowse;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox textCompareFilePath;
		private System.Windows.Forms.OpenFileDialog openFileDialog1;
		private System.Windows.Forms.RichTextBox richTextCompare;
		private System.Windows.Forms.Button butCompare;
	}
}

