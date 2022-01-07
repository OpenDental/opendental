
namespace OpenDental {
	partial class FormEClipboardSheetRules {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormEClipboardSheetRules));
			this.labelBehavior = new System.Windows.Forms.Label();
			this.labelMinAge = new System.Windows.Forms.Label();
			this.labelMaxAge = new System.Windows.Forms.Label();
			this.labelFrequency = new System.Windows.Forms.Label();
			this.comboBehavior = new OpenDental.UI.ComboBoxOD();
			this.checkMinAge = new System.Windows.Forms.CheckBox();
			this.checkMaxAge = new System.Windows.Forms.CheckBox();
			this.textMinAge = new System.Windows.Forms.TextBox();
			this.textMaxAge = new System.Windows.Forms.TextBox();
			this.labelFreqNote = new System.Windows.Forms.Label();
			this.textFrequency = new System.Windows.Forms.TextBox();
			this.labelSheet = new System.Windows.Forms.Label();
			this.textSheet = new System.Windows.Forms.TextBox();
			this.butOK = new OpenDental.UI.Button();
			this.butCancel = new OpenDental.UI.Button();
			this.SuspendLayout();
			// 
			// labelBehavior
			// 
			this.labelBehavior.Location = new System.Drawing.Point(12, 36);
			this.labelBehavior.Name = "labelBehavior";
			this.labelBehavior.Size = new System.Drawing.Size(120, 20);
			this.labelBehavior.TabIndex = 0;
			this.labelBehavior.Text = "Behavior";
			this.labelBehavior.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMinAge
			// 
			this.labelMinAge.Location = new System.Drawing.Point(12, 145);
			this.labelMinAge.Name = "labelMinAge";
			this.labelMinAge.Size = new System.Drawing.Size(120, 20);
			this.labelMinAge.TabIndex = 1;
			this.labelMinAge.Text = "Minimum Age";
			this.labelMinAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelMaxAge
			// 
			this.labelMaxAge.Location = new System.Drawing.Point(12, 196);
			this.labelMaxAge.Name = "labelMaxAge";
			this.labelMaxAge.Size = new System.Drawing.Size(120, 20);
			this.labelMaxAge.TabIndex = 2;
			this.labelMaxAge.Text = "Maximum Age";
			this.labelMaxAge.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// labelFrequency
			// 
			this.labelFrequency.Location = new System.Drawing.Point(12, 92);
			this.labelFrequency.Name = "labelFrequency";
			this.labelFrequency.Size = new System.Drawing.Size(120, 20);
			this.labelFrequency.TabIndex = 3;
			this.labelFrequency.Text = "Frequency (Days)";
			this.labelFrequency.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// comboBehavior
			// 
			this.comboBehavior.Location = new System.Drawing.Point(138, 36);
			this.comboBehavior.Name = "comboBehavior";
			this.comboBehavior.Size = new System.Drawing.Size(144, 21);
			this.comboBehavior.TabIndex = 4;
			this.comboBehavior.Text = "comboBehavior";
			this.comboBehavior.SelectionChangeCommitted += new System.EventHandler(this.comboBehavior_SelectionChangeCommitted);
			// 
			// checkMinAge
			// 
			this.checkMinAge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMinAge.Location = new System.Drawing.Point(138, 119);
			this.checkMinAge.Name = "checkMinAge";
			this.checkMinAge.Size = new System.Drawing.Size(215, 20);
			this.checkMinAge.TabIndex = 5;
			this.checkMinAge.Text = "Include minimum age requirement";
			this.checkMinAge.UseVisualStyleBackColor = true;
			this.checkMinAge.CheckedChanged += new System.EventHandler(this.checkMinAge_CheckedChanged);
			// 
			// checkMaxAge
			// 
			this.checkMaxAge.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.checkMaxAge.Location = new System.Drawing.Point(138, 171);
			this.checkMaxAge.Name = "checkMaxAge";
			this.checkMaxAge.Size = new System.Drawing.Size(215, 20);
			this.checkMaxAge.TabIndex = 6;
			this.checkMaxAge.Text = "Include maximum age requirement";
			this.checkMaxAge.UseVisualStyleBackColor = true;
			this.checkMaxAge.CheckedChanged += new System.EventHandler(this.checkMaxAge_CheckedChanged);
			// 
			// textMinAge
			// 
			this.textMinAge.Location = new System.Drawing.Point(138, 145);
			this.textMinAge.Name = "textMinAge";
			this.textMinAge.Size = new System.Drawing.Size(35, 20);
			this.textMinAge.TabIndex = 7;
			// 
			// textMaxAge
			// 
			this.textMaxAge.Location = new System.Drawing.Point(138, 197);
			this.textMaxAge.Name = "textMaxAge";
			this.textMaxAge.Size = new System.Drawing.Size(35, 20);
			this.textMaxAge.TabIndex = 8;
			// 
			// labelFreqNote
			// 
			this.labelFreqNote.Location = new System.Drawing.Point(12, 62);
			this.labelFreqNote.Name = "labelFreqNote";
			this.labelFreqNote.Size = new System.Drawing.Size(341, 30);
			this.labelFreqNote.TabIndex = 9;
			this.labelFreqNote.Text = "How often should the patient be asked to resubmit this form? (In days, where 0 in" +
    "dicates only submit once.)";
			// 
			// textFrequency
			// 
			this.textFrequency.Location = new System.Drawing.Point(138, 93);
			this.textFrequency.Name = "textFrequency";
			this.textFrequency.Size = new System.Drawing.Size(35, 20);
			this.textFrequency.TabIndex = 10;
			this.textFrequency.TextChanged += new System.EventHandler(this.textFrequency_TextChanged);
			// 
			// labelSheet
			// 
			this.labelSheet.Location = new System.Drawing.Point(32, 9);
			this.labelSheet.Name = "labelSheet";
			this.labelSheet.Size = new System.Drawing.Size(100, 20);
			this.labelSheet.TabIndex = 11;
			this.labelSheet.Text = "Sheet";
			this.labelSheet.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// textSheet
			// 
			this.textSheet.Enabled = false;
			this.textSheet.Location = new System.Drawing.Point(138, 10);
			this.textSheet.Name = "textSheet";
			this.textSheet.Size = new System.Drawing.Size(144, 20);
			this.textSheet.TabIndex = 12;
			// 
			// butOK
			// 
			this.butOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butOK.Location = new System.Drawing.Point(197, 228);
			this.butOK.Name = "butOK";
			this.butOK.Size = new System.Drawing.Size(75, 24);
			this.butOK.TabIndex = 13;
			this.butOK.Text = "OK";
			this.butOK.UseVisualStyleBackColor = true;
			this.butOK.Click += new System.EventHandler(this.butOK_Click);
			// 
			// butCancel
			// 
			this.butCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.butCancel.Location = new System.Drawing.Point(278, 228);
			this.butCancel.Name = "butCancel";
			this.butCancel.Size = new System.Drawing.Size(75, 24);
			this.butCancel.TabIndex = 14;
			this.butCancel.Text = "Cancel";
			this.butCancel.UseVisualStyleBackColor = true;
			this.butCancel.Click += new System.EventHandler(this.butCancel_Click);
			// 
			// FormEClipboardSheetRules
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(365, 260);
			this.Controls.Add(this.butCancel);
			this.Controls.Add(this.butOK);
			this.Controls.Add(this.textSheet);
			this.Controls.Add(this.labelSheet);
			this.Controls.Add(this.textFrequency);
			this.Controls.Add(this.labelFreqNote);
			this.Controls.Add(this.textMaxAge);
			this.Controls.Add(this.textMinAge);
			this.Controls.Add(this.checkMaxAge);
			this.Controls.Add(this.checkMinAge);
			this.Controls.Add(this.comboBehavior);
			this.Controls.Add(this.labelFrequency);
			this.Controls.Add(this.labelMaxAge);
			this.Controls.Add(this.labelMinAge);
			this.Controls.Add(this.labelBehavior);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "FormEClipboardSheetRules";
			this.Text = "eClipboard Sheet Rules";
			this.Load += new System.EventHandler(this.FormEClipboardSheetRules_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelBehavior;
		private System.Windows.Forms.Label labelMinAge;
		private System.Windows.Forms.Label labelMaxAge;
		private System.Windows.Forms.Label labelFrequency;
		private UI.ComboBoxOD comboBehavior;
		private System.Windows.Forms.CheckBox checkMinAge;
		private System.Windows.Forms.CheckBox checkMaxAge;
		private System.Windows.Forms.TextBox textMinAge;
		private System.Windows.Forms.TextBox textMaxAge;
		private System.Windows.Forms.Label labelFreqNote;
		private System.Windows.Forms.TextBox textFrequency;
		private System.Windows.Forms.Label labelSheet;
		private System.Windows.Forms.TextBox textSheet;
		private UI.Button butOK;
		private UI.Button butCancel;
	}
}